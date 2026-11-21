namespace AppTimeTracker
{
    public partial class MainForm : Form
    {
        private ApplicationTracker _tracker = new ApplicationTracker();
        private ApplicationListView _appListView;
        private Dictionary<string, ApplicationData> _appData = new Dictionary<string, ApplicationData>();
        private string _currentApp = "";
        private DateTime _lastSwitchTime;
        private System.Windows.Forms.Timer _timer;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _statusLabel;
        private ListView _listView;
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            SetUpTimer();
            _appListView = new ApplicationListView(_listView);
            _lastSwitchTime = DateTime.Now;
        }
        private void InitializeCustomComponents()
        {
            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true
            };
            _listView.Columns.Add("Application", 200, HorizontalAlignment.Left);
            _listView.Columns.Add("Проведенное время", 150, HorizontalAlignment.Left);
            _listView.ColumnWidthChanging += ListView_ColumnWidthChanging;
            Controls.Add(_listView);
            _statusStrip = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel { Text = "Ready" };
            _statusStrip.Items.Add(_statusLabel);
            Controls.Add(_statusStrip);
            Button refreshButton = new Button
            {
                Text = "Refresh",
                Dock = DockStyle.Bottom
            };
            refreshButton.Click += RefreshButton_Click;
            Controls.Add(refreshButton);
        }
        private void SetUpTimer()
        {
            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            string activeApp = _tracker.GetActiveWindowTitle();
            if (!string.IsNullOrEmpty(_currentApp))
            {
                if (!_appData.ContainsKey(_currentApp))
                {
                    _appData[_currentApp] = new ApplicationData(_currentApp) { TimeSpent = TimeSpan.Zero };
                }
                _appData[_currentApp].TimeSpent += DateTime.Now - _lastSwitchTime;
            }
            _lastSwitchTime = DateTime.Now;
            if (_currentApp != activeApp)
            {
                _currentApp = activeApp;
            }
            _appListView.Update(_appData.Values.ToList());
        }
        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            _appListView.Update(_appData.Values.ToList());
            _statusLabel.Text = "Список обновлен в " + DateTime.Now.ToString("T");
        }
        private void ListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}