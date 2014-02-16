namespace KeePassRPC.Forms
{
    partial class AuthForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthForm));
            this.localizer1 = new KeePassRPC.Localizer();
            this.buttonDeny = new System.Windows.Forms.Button();
            this.richTextBoxClientID = new System.Windows.Forms.RichTextBox();
            this.richTextBoxPassword = new System.Windows.Forms.RichTextBox();
            this.richTextBoxSecurityLevel = new System.Windows.Forms.RichTextBox();
            this.richTextBoxConfirmInstruction = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            resources = new SingleAssemblyComponentResourceManager(typeof(AuthForm));
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDeny
            // 
            resources.ApplyResources(this.buttonDeny, "buttonDeny");
            this.buttonDeny.Name = "buttonDeny";
            this.buttonDeny.UseVisualStyleBackColor = true;
            this.buttonDeny.Click += new System.EventHandler(this.buttonDeny_Click);
            // 
            // richTextBoxClientID
            // 
            this.richTextBoxClientID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richTextBoxClientID, "richTextBoxClientID");
            this.richTextBoxClientID.Name = "richTextBoxClientID";
            this.richTextBoxClientID.ReadOnly = true;
            // 
            // richTextBoxPassword
            // 
            this.richTextBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxPassword.DetectUrls = false;
            resources.ApplyResources(this.richTextBoxPassword, "richTextBoxPassword");
            this.richTextBoxPassword.ForeColor = System.Drawing.Color.Red;
            this.richTextBoxPassword.Name = "richTextBoxPassword";
            this.richTextBoxPassword.ReadOnly = true;
            // 
            // richTextBoxSecurityLevel
            // 
            this.richTextBoxSecurityLevel.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxSecurityLevel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxSecurityLevel.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.richTextBoxSecurityLevel, "richTextBoxSecurityLevel");
            this.richTextBoxSecurityLevel.Name = "richTextBoxSecurityLevel";
            this.richTextBoxSecurityLevel.ReadOnly = true;
            // 
            // richTextBoxConfirmInstruction
            // 
            this.richTextBoxConfirmInstruction.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxConfirmInstruction.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richTextBoxConfirmInstruction, "richTextBoxConfirmInstruction");
            this.richTextBoxConfirmInstruction.Name = "richTextBoxConfirmInstruction";
            this.richTextBoxConfirmInstruction.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.richTextBoxConfirmInstruction);
            this.panel1.Controls.Add(this.richTextBoxPassword);
            this.panel1.ForeColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // AuthForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxSecurityLevel);
            this.Controls.Add(this.richTextBoxClientID);
            this.Controls.Add(this.buttonDeny);
            this.Name = "AuthForm";
            this.Load += new System.EventHandler(this.AuthForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDeny;
        private System.Windows.Forms.RichTextBox richTextBoxClientID;
        private System.Windows.Forms.RichTextBox richTextBoxPassword;
        private System.Windows.Forms.RichTextBox richTextBoxSecurityLevel;
        private System.Windows.Forms.RichTextBox richTextBoxConfirmInstruction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private Localizer localizer1;
    }
}