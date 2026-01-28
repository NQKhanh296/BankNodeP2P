using BankNodeP2P.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.UI
{
    public partial class MainForm : Form
    {
        private Logger? _logger;

        public MainForm()
        {
            InitializeComponent();
            lblStatus.Text = "Stopped";
        }

        public void SetLogger(Logger logger)
        {
            _logger = logger;
            _logger.OnLog += OnLog;
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
            btnStart.Location = new Point(65, 53);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(75, 23);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += button1_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(159, 53);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(75, 23);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(80, 97);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(51, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Stopped";
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Bottom;
            txtLog.Location = new Point(0, 126);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(432, 230);
            txtLog.TabIndex = 3;
            // 
            // MainForm
            // 
            ClientSize = new Size(432, 356);
            Controls.Add(txtLog);
            Controls.Add(lblStatus);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Name = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        private void OnLog(LogEntry entry)
        {
            UiThread.Post(this, () =>
            {
                txtLog.AppendText(
                    $"[{entry.Timestamp:HH:mm:ss}] {entry.Level} {entry.Event} {entry.Message}\r\n"
                );
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private Button btnStart;
        private Button btnStop;
        private Label lblStatus;
        private TextBox txtLog;
    }
}
