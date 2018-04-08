namespace winArchive
{
    partial class FormArchive
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerReceiver = new System.Windows.Forms.Timer(this.components);
            this.labelLight = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timerReceiver
            // 
            this.timerReceiver.Interval = 200;
            this.timerReceiver.Tick += new System.EventHandler(this.timerReceiver_Tick);
            // 
            // labelLight
            // 
            this.labelLight.BackColor = System.Drawing.Color.Green;
            this.labelLight.Location = new System.Drawing.Point(11, 10);
            this.labelLight.Name = "labelLight";
            this.labelLight.Size = new System.Drawing.Size(12, 12);
            this.labelLight.TabIndex = 3;
            this.labelLight.DoubleClick += new System.EventHandler(this.FormArchive_DoubleClick);
            // 
            // FormArchive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(34, 30);
            this.ControlBox = false;
            this.Controls.Add(this.labelLight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormArchive";
            this.Text = " 归档";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormArchive_FormClosing);
            this.Load += new System.EventHandler(this.FormArchive_Load);
            this.DoubleClick += new System.EventHandler(this.FormArchive_DoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerReceiver;
        private System.Windows.Forms.Label labelLight;
    }
}

