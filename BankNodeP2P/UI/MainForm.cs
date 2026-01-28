using BankNodeP2P.App;
using BankNodeP2P.Logging;
using BankNodeP2P.Networking;

namespace BankNodeP2P.UI;

public partial class MainForm : Form
{
    private Logger? _logger;
    private NodeController? _controller;

    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;
    private TextBox txtLog;

    private BankTcpServer? _server;
    private AppConfig? _config;

    public MainForm()
    {
        InitializeComponent();

        SetUiStopped();

        btnStart.Click += BtnStart_Click;
        btnStop.Click += BtnStop_Click;
    }

    public void SetLogger(Logger logger)
    {
        _logger = logger;
        _logger.OnLog += OnLog;
    }

    public void SetController(NodeController controller)
    {
        _controller = controller;
    }

    private async void BtnStart_Click(object? sender, EventArgs e)
    {
        if (_controller?.StartAsync == null)
        {
            _logger?.Warn("UI", "Start clicked, but controller not attached yet.");
            MessageBox.Show("Server controller is not connected.", "Info");
            return;
        }

        try
        {
            SetUiStarting();
            _logger?.Info("UI", "Start clicked");

            await _controller.StartAsync();

            SetUiRunning();
            _logger?.Info("UI", "Server started");
        }
        catch (Exception ex)
        {
            SetUiStopped();
            _logger?.Error("UI", $"Start failed: {ex.Message}");
            MessageBox.Show(ex.Message, "Start failed");
        }
    }

    private async void BtnStop_Click(object? sender, EventArgs e)
    {
        if (_controller?.StopAsync == null)
        {
            _logger?.Warn("UI", "Stop clicked, but controller not attached yet.");
            MessageBox.Show("Server controller is not connected..", "Info");
            return;
        }

        try
        {
            SetUiStopping();
            _logger?.Info("UI", "Stop clicked");

            await _controller.StopAsync();

            SetUiStopped();
            _logger?.Info("UI", "Server stopped");
        }
        catch (Exception ex)
        {
            SetUiRunning();
            _logger?.Error("UI", $"Stop failed: {ex.Message}");
            MessageBox.Show(ex.Message, "Stop failed");
        }
    }

    private void OnLog(LogEntry entry)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => OnLog(entry));
            return;
        }

        txtLog.AppendText(
            $"[{entry.Timestamp:HH:mm:ss}] {entry.Level} {entry.Event} {entry.Message}\r\n"
        );
    }

    private void SetUiStarting()
    {
        lblStatus.Text = "Starting...";
        btnStart.Enabled = false;
        btnStop.Enabled = false;
    }

    private void SetUiRunning()
    {
        lblStatus.Text = "Running";
        btnStart.Enabled = false;
        btnStop.Enabled = true;
    }

    private void SetUiStopping()
    {
        lblStatus.Text = "Stopping...";
        btnStart.Enabled = false;
        btnStop.Enabled = false;
    }

    private void SetUiStopped()
    {
        lblStatus.Text = "Stopped";
        btnStart.Enabled = true;
        btnStop.Enabled = false;
    }

    private void InitializeComponent()
    {
        btnStart = new Button();
        btnStop = new Button();
        lblStatus = new Label();
        txtLog = new TextBox();
        SuspendLayout();
        // 
        // btnStart
        // 
        btnStart.Location = new Point(20, 20);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(80, 30);
        btnStart.TabIndex = 0;
        btnStart.Text = "Start";
        btnStart.Click += btnStart_Click_1;
        // 
        // btnStop
        // 
        btnStop.Location = new Point(120, 20);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(80, 30);
        btnStop.TabIndex = 1;
        btnStop.Text = "Stop";
        // 
        // lblStatus
        // 
        lblStatus.Location = new Point(220, 26);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(200, 20);
        lblStatus.TabIndex = 2;
        lblStatus.Text = "Stopped";
        // 
        // txtLog
        // 
        txtLog.Dock = DockStyle.Bottom;
        txtLog.Location = new Point(0, 100);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(650, 250);
        txtLog.TabIndex = 3;
        // 
        // MainForm
        // 
        ClientSize = new Size(650, 350);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(lblStatus);
        Controls.Add(txtLog);
        Name = "MainForm";
        Text = "BankNode P2P";
        ResumeLayout(false);
        PerformLayout();
    }

    private void btnStart_Click_1(object sender, EventArgs e)
    {

    }
}
