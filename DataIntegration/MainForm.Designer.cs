namespace DataIntegration
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressPPTN = new System.Windows.Forms.ProgressBar();
            this.labelPPTN = new System.Windows.Forms.Label();
            this.labelRIVER = new System.Windows.Forms.Label();
            this.progressRIVER = new System.Windows.Forms.ProgressBar();
            this.labelRSVR = new System.Windows.Forms.Label();
            this.progressRSVR = new System.Windows.Forms.ProgressBar();
            this.progressTIDE = new System.Windows.Forms.ProgressBar();
            this.labelTIDE = new System.Windows.Forms.Label();
            this.btnSourceSet = new System.Windows.Forms.Button();
            this.btnDestSet = new System.Windows.Forms.Button();
            this.btnSystemSet = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.cb_pptn = new System.Windows.Forms.CheckBox();
            this.cb_river = new System.Windows.Forms.CheckBox();
            this.cb_rvsr = new System.Windows.Forms.CheckBox();
            this.cb_tide = new System.Windows.Forms.CheckBox();
            this.btnBackupSet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn2000 = new System.Windows.Forms.Button();
            this.tx_beginYear = new System.Windows.Forms.TextBox();
            this.tx_endYear = new System.Windows.Forms.TextBox();
            this.lab_ytoy = new System.Windows.Forms.Label();
            this.cb_export = new System.Windows.Forms.CheckBox();
            this.btn_test = new System.Windows.Forms.Button();
            this.lbl_stbprp_trans = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressPPTN
            // 
            this.progressPPTN.ForeColor = System.Drawing.Color.Red;
            this.progressPPTN.Location = new System.Drawing.Point(44, 54);
            this.progressPPTN.Name = "progressPPTN";
            this.progressPPTN.Size = new System.Drawing.Size(414, 23);
            this.progressPPTN.TabIndex = 0;
            // 
            // labelPPTN
            // 
            this.labelPPTN.AutoSize = true;
            this.labelPPTN.Location = new System.Drawing.Point(69, 29);
            this.labelPPTN.Name = "labelPPTN";
            this.labelPPTN.Size = new System.Drawing.Size(89, 12);
            this.labelPPTN.TabIndex = 1;
            this.labelPPTN.Text = "降雨量数据同步";
            // 
            // labelRIVER
            // 
            this.labelRIVER.AutoSize = true;
            this.labelRIVER.Location = new System.Drawing.Point(69, 99);
            this.labelRIVER.Name = "labelRIVER";
            this.labelRIVER.Size = new System.Drawing.Size(101, 12);
            this.labelRIVER.TabIndex = 3;
            this.labelRIVER.Text = "河道水情数据同步";
            // 
            // progressRIVER
            // 
            this.progressRIVER.Location = new System.Drawing.Point(44, 124);
            this.progressRIVER.Name = "progressRIVER";
            this.progressRIVER.Size = new System.Drawing.Size(414, 23);
            this.progressRIVER.TabIndex = 2;
            // 
            // labelRSVR
            // 
            this.labelRSVR.AutoSize = true;
            this.labelRSVR.Location = new System.Drawing.Point(69, 176);
            this.labelRSVR.Name = "labelRSVR";
            this.labelRSVR.Size = new System.Drawing.Size(101, 12);
            this.labelRSVR.TabIndex = 5;
            this.labelRSVR.Text = "水库水情数据同步";
            // 
            // progressRSVR
            // 
            this.progressRSVR.Location = new System.Drawing.Point(44, 201);
            this.progressRSVR.Name = "progressRSVR";
            this.progressRSVR.Size = new System.Drawing.Size(414, 23);
            this.progressRSVR.TabIndex = 4;
            // 
            // progressTIDE
            // 
            this.progressTIDE.Location = new System.Drawing.Point(44, 278);
            this.progressTIDE.Name = "progressTIDE";
            this.progressTIDE.Size = new System.Drawing.Size(414, 23);
            this.progressTIDE.TabIndex = 4;
            // 
            // labelTIDE
            // 
            this.labelTIDE.AutoSize = true;
            this.labelTIDE.Location = new System.Drawing.Point(69, 253);
            this.labelTIDE.Name = "labelTIDE";
            this.labelTIDE.Size = new System.Drawing.Size(77, 12);
            this.labelTIDE.TabIndex = 5;
            this.labelTIDE.Text = "潮位数据同步";
            // 
            // btnSourceSet
            // 
            this.btnSourceSet.Location = new System.Drawing.Point(499, 78);
            this.btnSourceSet.Name = "btnSourceSet";
            this.btnSourceSet.Size = new System.Drawing.Size(115, 30);
            this.btnSourceSet.TabIndex = 7;
            this.btnSourceSet.Text = "源数据库设置";
            this.btnSourceSet.UseVisualStyleBackColor = true;
            this.btnSourceSet.Click += new System.EventHandler(this.btnSourceSet_Click);
            // 
            // btnDestSet
            // 
            this.btnDestSet.Location = new System.Drawing.Point(620, 78);
            this.btnDestSet.Name = "btnDestSet";
            this.btnDestSet.Size = new System.Drawing.Size(115, 30);
            this.btnDestSet.TabIndex = 7;
            this.btnDestSet.Text = "目标数据库设置";
            this.btnDestSet.UseVisualStyleBackColor = true;
            this.btnDestSet.Click += new System.EventHandler(this.btnDestSet_Click);
            // 
            // btnSystemSet
            // 
            this.btnSystemSet.Location = new System.Drawing.Point(499, 114);
            this.btnSystemSet.Name = "btnSystemSet";
            this.btnSystemSet.Size = new System.Drawing.Size(115, 30);
            this.btnSystemSet.TabIndex = 8;
            this.btnSystemSet.Text = "系统参数设置";
            this.btnSystemSet.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(44, 328);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 27);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(124, 328);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 27);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "暂停";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(205, 328);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 27);
            this.btnAbort.TabIndex = 9;
            this.btnAbort.Text = "停止";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // cb_pptn
            // 
            this.cb_pptn.AutoSize = true;
            this.cb_pptn.Location = new System.Drawing.Point(44, 29);
            this.cb_pptn.Name = "cb_pptn";
            this.cb_pptn.Size = new System.Drawing.Size(15, 14);
            this.cb_pptn.TabIndex = 12;
            this.cb_pptn.UseVisualStyleBackColor = true;
            // 
            // cb_river
            // 
            this.cb_river.AutoSize = true;
            this.cb_river.Location = new System.Drawing.Point(44, 99);
            this.cb_river.Name = "cb_river";
            this.cb_river.Size = new System.Drawing.Size(15, 14);
            this.cb_river.TabIndex = 13;
            this.cb_river.UseVisualStyleBackColor = true;
            // 
            // cb_rvsr
            // 
            this.cb_rvsr.AutoSize = true;
            this.cb_rvsr.Location = new System.Drawing.Point(44, 176);
            this.cb_rvsr.Name = "cb_rvsr";
            this.cb_rvsr.Size = new System.Drawing.Size(15, 14);
            this.cb_rvsr.TabIndex = 13;
            this.cb_rvsr.UseVisualStyleBackColor = true;
            // 
            // cb_tide
            // 
            this.cb_tide.AutoSize = true;
            this.cb_tide.Location = new System.Drawing.Point(44, 253);
            this.cb_tide.Name = "cb_tide";
            this.cb_tide.Size = new System.Drawing.Size(15, 14);
            this.cb_tide.TabIndex = 13;
            this.cb_tide.UseVisualStyleBackColor = true;
            // 
            // btnBackupSet
            // 
            this.btnBackupSet.Location = new System.Drawing.Point(620, 114);
            this.btnBackupSet.Name = "btnBackupSet";
            this.btnBackupSet.Size = new System.Drawing.Size(115, 30);
            this.btnBackupSet.TabIndex = 7;
            this.btnBackupSet.Text = "备份数据库设置";
            this.btnBackupSet.UseVisualStyleBackColor = true;
            this.btnBackupSet.Click += new System.EventHandler(this.btnBackupSet_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(591, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 12);
            this.label1.TabIndex = 14;
            // 
            // btn2000
            // 
            this.btn2000.Location = new System.Drawing.Point(661, 332);
            this.btn2000.Name = "btn2000";
            this.btn2000.Size = new System.Drawing.Size(75, 23);
            this.btn2000.TabIndex = 15;
            this.btn2000.Text = "导出";
            this.btn2000.UseVisualStyleBackColor = true;
            this.btn2000.Click += new System.EventHandler(this.btn2000_Click);
            // 
            // tx_beginYear
            // 
            this.tx_beginYear.Location = new System.Drawing.Point(491, 334);
            this.tx_beginYear.Name = "tx_beginYear";
            this.tx_beginYear.Size = new System.Drawing.Size(61, 21);
            this.tx_beginYear.TabIndex = 16;
            // 
            // tx_endYear
            // 
            this.tx_endYear.Location = new System.Drawing.Point(583, 334);
            this.tx_endYear.Name = "tx_endYear";
            this.tx_endYear.Size = new System.Drawing.Size(61, 21);
            this.tx_endYear.TabIndex = 16;
            // 
            // lab_ytoy
            // 
            this.lab_ytoy.AutoSize = true;
            this.lab_ytoy.Location = new System.Drawing.Point(560, 339);
            this.lab_ytoy.Name = "lab_ytoy";
            this.lab_ytoy.Size = new System.Drawing.Size(17, 12);
            this.lab_ytoy.TabIndex = 17;
            this.lab_ytoy.Text = "—";
            // 
            // cb_export
            // 
            this.cb_export.AutoSize = true;
            this.cb_export.Location = new System.Drawing.Point(655, 47);
            this.cb_export.Name = "cb_export";
            this.cb_export.Size = new System.Drawing.Size(72, 16);
            this.cb_export.TabIndex = 18;
            this.cb_export.Text = "导出数据";
            this.cb_export.UseVisualStyleBackColor = true;
            this.cb_export.CheckedChanged += new System.EventHandler(this.cb_export_CheckedChanged);
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(380, 328);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(78, 27);
            this.btn_test.TabIndex = 19;
            this.btn_test.Text = "测试";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // lbl_stbprp_trans
            // 
            this.lbl_stbprp_trans.AutoSize = true;
            this.lbl_stbprp_trans.Location = new System.Drawing.Point(491, 201);
            this.lbl_stbprp_trans.Name = "lbl_stbprp_trans";
            this.lbl_stbprp_trans.Size = new System.Drawing.Size(0, 12);
            this.lbl_stbprp_trans.TabIndex = 20;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 396);
            this.Controls.Add(this.lbl_stbprp_trans);
            this.Controls.Add(this.btn_test);
            this.Controls.Add(this.cb_export);
            this.Controls.Add(this.lab_ytoy);
            this.Controls.Add(this.tx_endYear);
            this.Controls.Add(this.tx_beginYear);
            this.Controls.Add(this.btn2000);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_tide);
            this.Controls.Add(this.cb_rvsr);
            this.Controls.Add(this.cb_river);
            this.Controls.Add(this.cb_pptn);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSystemSet);
            this.Controls.Add(this.btnDestSet);
            this.Controls.Add(this.btnBackupSet);
            this.Controls.Add(this.btnSourceSet);
            this.Controls.Add(this.labelTIDE);
            this.Controls.Add(this.labelRSVR);
            this.Controls.Add(this.progressTIDE);
            this.Controls.Add(this.progressRSVR);
            this.Controls.Add(this.labelRIVER);
            this.Controls.Add(this.progressRIVER);
            this.Controls.Add(this.labelPPTN);
            this.Controls.Add(this.progressPPTN);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressPPTN;
        private System.Windows.Forms.Label labelPPTN;
        private System.Windows.Forms.Label labelRIVER;
        private System.Windows.Forms.ProgressBar progressRIVER;
        private System.Windows.Forms.Label labelRSVR;
        private System.Windows.Forms.ProgressBar progressRSVR;
        private System.Windows.Forms.ProgressBar progressTIDE;
        private System.Windows.Forms.Label labelTIDE;
        private System.Windows.Forms.Button btnSourceSet;
        private System.Windows.Forms.Button btnDestSet;
        private System.Windows.Forms.Button btnSystemSet;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.CheckBox cb_pptn;
        private System.Windows.Forms.CheckBox cb_river;
        private System.Windows.Forms.CheckBox cb_rvsr;
        private System.Windows.Forms.CheckBox cb_tide;
        private System.Windows.Forms.Button btnBackupSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn2000;
        private System.Windows.Forms.TextBox tx_beginYear;
        private System.Windows.Forms.TextBox tx_endYear;
        private System.Windows.Forms.Label lab_ytoy;
        private System.Windows.Forms.CheckBox cb_export;
        private System.Windows.Forms.Button btn_test;
        private System.Windows.Forms.Label lbl_stbprp_trans;
    }
}

