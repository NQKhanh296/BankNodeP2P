using BankNodeP2P.Logging;

namespace BankNodeP2P.UI;

public partial class MainForm : Form
{
    private Logger? _logger;

    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;
    private TextBox txtLog;

    public MainForm()
    {
        InitializeComponent();

        lblStatus.Text = "Stopped";
        btnStop.Enabled = false;

        btnStart.Click += BtnStart_Click;
        btnStop.Click += BtnStop_Click;
    }

    public void SetLogger(Logger logger)
    {
        _logger = logger;
        _logger.OnLog += OnLog;
    }

    private void BtnStart_Click(object? sender, EventArgs e)
    {
        lblStatus.Text = "Running";
        btnStart.Enabled = false;
        btnStop.Enabled = true;

        _logger?.Info("UI", "Start clicked");
    }

    private void BtnStop_Click(object? sender, EventArgs e)
    {
        lblStatus.Text = "Stopped";
        btnStart.Enabled = true;
        btnStop.Enabled = false;

        _logger?.Info("UI", "Stop clicked");
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

        ClientSize = new Size(500, 350);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(lblStatus);
        Controls.Add(txtLog);

        Text = "BankNode P2P";

        ResumeLayout(false);
    }
}
