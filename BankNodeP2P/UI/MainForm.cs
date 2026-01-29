using BankNodeP2P.App;
using BankNodeP2P.Logging;
using BankNodeP2P.Networking;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BankNodeP2P.UI;

public partial class MainForm : Form
{
    private Logger? _logger;
    private NodeController? _controller;

    private Button btnStart;
    private Button btnStop;
    private Label lblStatus;
    private TextBox txtLog;

    private Label lblIp;
    private TextBox txtIp;
    private Label lblPort;
    private NumericUpDown nudPort;
    private Label lblCmdTimeout;
    private NumericUpDown nudCmdTimeoutMs;
    private Label lblIdleTimeout;
    private NumericUpDown nudIdleTimeoutMs;

    public MainForm()
    {
        InitializeComponent();

        txtIp.Text = GetLocalIpv4OrLoopback();
        nudPort.Value = 65530;
        nudCmdTimeoutMs.Value = 5000;     
        nudIdleTimeoutMs.Value = 60000;  

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
            _logger?.Info("UI", $"Start clicked (ip={txtIp.Text.Trim()}, port={(int)nudPort.Value}, cmdTimeout={(int)nudCmdTimeoutMs.Value}ms, idleTimeout={(int)nudIdleTimeoutMs.Value}ms)");

            await _controller.StartAsync(txtIp.Text.Trim(), (int)nudPort.Value, (int)nudCmdTimeoutMs.Value, (int)nudIdleTimeoutMs.Value);

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

        nudPort.Enabled = false;
        nudCmdTimeoutMs.Enabled = false;
        nudIdleTimeoutMs.Enabled = false;
    }

    private void SetUiRunning()
    {
        lblStatus.Text = $"Running on {txtIp.Text}:{nudPort.Value}";
        btnStart.Enabled = false;
        btnStop.Enabled = true;

        nudPort.Enabled = false;
        nudCmdTimeoutMs.Enabled = false;
        nudIdleTimeoutMs.Enabled = false;
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

        nudPort.Enabled = true;
        nudCmdTimeoutMs.Enabled = true;
        nudIdleTimeoutMs.Enabled = true;

        txtIp.Text = GetLocalIpv4OrLoopback();
    }

    /// <summary>
    /// Returns the first non-loopback IPv4 address of an active network interface.
    /// Falls back to 127.0.0.1 if none is found.
    /// </summary>
    private static string GetLocalIpv4OrLoopback()
    {
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up)
                continue;

            var props = ni.GetIPProperties();
            foreach (var ua in props.UnicastAddresses)
            {
                if (ua.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                var ip = ua.Address.ToString();
                if (ip == "127.0.0.1") continue;
                if (ip.StartsWith("169.254.")) continue; 
                return ip;
            }
        }

        return "127.0.0.1";
    }

    private void InitializeComponent()
    {
        btnStart = new Button();
        btnStop = new Button();
        lblStatus = new Label();
        txtLog = new TextBox();
        lblIp = new Label();
        txtIp = new TextBox();
        lblPort = new Label();
        nudPort = new NumericUpDown();
        lblCmdTimeout = new Label();
        nudCmdTimeoutMs = new NumericUpDown();
        lblIdleTimeout = new Label();
        nudIdleTimeoutMs = new NumericUpDown();
        ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudCmdTimeoutMs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudIdleTimeoutMs).BeginInit();
        SuspendLayout();
        // 
        // btnStart
        // 
        btnStart.Location = new Point(20, 75);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(80, 30);
        btnStart.TabIndex = 0;
        btnStart.Text = "Start";
        // 
        // btnStop
        // 
        btnStop.Location = new Point(120, 75);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(80, 30);
        btnStop.TabIndex = 1;
        btnStop.Text = "Stop";
        // 
        // lblStatus
        // 
        lblStatus.Location = new Point(220, 82);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(430, 20);
        lblStatus.TabIndex = 2;
        lblStatus.Text = "Stopped";
        // 
        // txtLog
        // 
        txtLog.Dock = DockStyle.Bottom;
        txtLog.Location = new Point(0, 130);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(680, 260);
        txtLog.TabIndex = 3;
        // 
        // lblIp
        // 
        lblIp.Location = new Point(20, 15);
        lblIp.Name = "lblIp";
        lblIp.Size = new Size(60, 20);
        lblIp.TabIndex = 0;
        lblIp.Text = "IP:";
        // 
        // txtIp
        // 
        txtIp.Location = new Point(80, 12);
        txtIp.Name = "txtIp";
        txtIp.ReadOnly = true;
        txtIp.Size = new Size(140, 27);
        txtIp.TabIndex = 1;
        // 
        // lblPort
        // 
        lblPort.Location = new Point(240, 15);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(50, 20);
        lblPort.TabIndex = 2;
        lblPort.Text = "Port:";
        // 
        // nudPort
        // 
        nudPort.Location = new Point(290, 12);
        nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        nudPort.Minimum = new decimal(new int[] { 65525, 0, 0, 0 });
        nudPort.Name = "nudPort";
        nudPort.Size = new Size(90, 27);
        nudPort.TabIndex = 3;
        nudPort.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // lblCmdTimeout
        // 
        lblCmdTimeout.Location = new Point(20, 45);
        lblCmdTimeout.Name = "lblCmdTimeout";
        lblCmdTimeout.Size = new Size(130, 20);
        lblCmdTimeout.TabIndex = 4;
        lblCmdTimeout.Text = "Cmd timeout (ms):";
        // 
        // nudCmdTimeoutMs
        // 
        nudCmdTimeoutMs.Location = new Point(150, 42);
        nudCmdTimeoutMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
        nudCmdTimeoutMs.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        nudCmdTimeoutMs.Name = "nudCmdTimeoutMs";
        nudCmdTimeoutMs.Size = new Size(120, 27);
        nudCmdTimeoutMs.TabIndex = 5;
        nudCmdTimeoutMs.Value = new decimal(new int[] { 100, 0, 0, 0 });
        // 
        // lblIdleTimeout
        // 
        lblIdleTimeout.Location = new Point(290, 45);
        lblIdleTimeout.Name = "lblIdleTimeout";
        lblIdleTimeout.Size = new Size(130, 20);
        lblIdleTimeout.TabIndex = 6;
        lblIdleTimeout.Text = "Idle timeout (ms):";
        // 
        // nudIdleTimeoutMs
        // 
        nudIdleTimeoutMs.Location = new Point(426, 42);
        nudIdleTimeoutMs.Maximum = new decimal(new int[] { 600000, 0, 0, 0 });
        nudIdleTimeoutMs.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        nudIdleTimeoutMs.Name = "nudIdleTimeoutMs";
        nudIdleTimeoutMs.Size = new Size(120, 27);
        nudIdleTimeoutMs.TabIndex = 7;
        nudIdleTimeoutMs.Value = new decimal(new int[] { 100, 0, 0, 0 });
        // 
        // MainForm
        // 
        ClientSize = new Size(680, 390);
        Controls.Add(lblIp);
        Controls.Add(txtIp);
        Controls.Add(lblPort);
        Controls.Add(nudPort);
        Controls.Add(lblCmdTimeout);
        Controls.Add(nudCmdTimeoutMs);
        Controls.Add(lblIdleTimeout);
        Controls.Add(nudIdleTimeoutMs);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(lblStatus);
        Controls.Add(txtLog);
        Name = "MainForm";
        Text = "BankNode P2P";
        ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudCmdTimeoutMs).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudIdleTimeoutMs).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
