using BankNodeP2P.Logging;

namespace BankNodeP2P.UI;

public partial class MainForm : Form
{
    private Logger? _logger;
    private NodeController? _controller;

    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;
    private TextBox txtLog;

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
            MessageBox.Show("Server controller není připojen (zatím nenapojen Studentem A).", "Info");
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
            MessageBox.Show("Server controller není připojen (zatím nenapojen Studentem A).", "Info");
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
            // když stop selže, radši necháme running, ale vypíšeme chybu
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

        btnStart.Location = new Point(20, 20);
        btnStart.Size = new Size(80, 30);
        btnStart.Text = "Start";

        btnStop.Location = new Point(120, 20);
        btnStop.Size = new Size(80, 30);
        btnStop.Text = "Stop";

        lblStatus.Location = new Point(220, 26);
        lblStatus.Size = new Size(200, 20);
        lblStatus.Text = "Stopped";

        txtLog.Dock = DockStyle.Bottom;
        txtLog.Multiline = true;
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Height = 250;

        ClientSize = new Size(650, 350);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(lblStatus);
        Controls.Add(txtLog);

        Text = "BankNode P2P";

        ResumeLayout(false);
    }
}
