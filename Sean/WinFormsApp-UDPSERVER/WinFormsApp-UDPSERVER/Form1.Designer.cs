using System.Net.Sockets;
using System.Net;
using System.Text;
namespace WinFormsApp_UDPSERVER
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.server_on = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.startAsyncButton = new System.Windows.Forms.Button();
            this.cancelAsyneButtom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("標楷體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // server_on
            // 
            this.server_on.Location = new System.Drawing.Point(12, 189);
            this.server_on.Name = "server_on";
            this.server_on.Size = new System.Drawing.Size(229, 48);
            this.server_on.TabIndex = 1;
            this.server_on.Text = "UDP server ON";
            this.server_on.UseVisualStyleBackColor = true;
            this.server_on.Click += new System.EventHandler(this.server_on_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("標楷體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // startAsyncButton
            // 
            this.startAsyncButton.Location = new System.Drawing.Point(12, 243);
            this.startAsyncButton.Name = "startAsyncButton";
            this.startAsyncButton.Size = new System.Drawing.Size(229, 48);
            this.startAsyncButton.TabIndex = 3;
            this.startAsyncButton.Text = "start bgworker";
            this.startAsyncButton.UseVisualStyleBackColor = true;
            this.startAsyncButton.Click += new System.EventHandler(this.startAsyncButton_Click);
            // 
            // cancelAsyneButtom
            // 
            this.cancelAsyneButtom.Location = new System.Drawing.Point(12, 297);
            this.cancelAsyneButtom.Name = "cancelAsyneButtom";
            this.cancelAsyneButtom.Size = new System.Drawing.Size(229, 48);
            this.cancelAsyneButtom.TabIndex = 4;
            this.cancelAsyneButtom.Text = "cancel bgworker";
            this.cancelAsyneButtom.UseVisualStyleBackColor = true;
            this.cancelAsyneButtom.Click += new System.EventHandler(this.cancelAsyneButtom_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1211, 623);
            this.Controls.Add(this.cancelAsyneButtom);
            this.Controls.Add(this.startAsyncButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.server_on);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Button server_on;
        private Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button startAsyncButton;
        private Button cancelAsyneButtom;
    }
}