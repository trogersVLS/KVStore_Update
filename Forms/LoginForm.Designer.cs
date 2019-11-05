namespace KVStore_Update
{
    partial class LoginForm
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.Label_Username = new System.Windows.Forms.Label();
            this.Label_Password = new System.Windows.Forms.Label();
            this.Button_Login = new System.Windows.Forms.Button();
            this.Button_Exit = new System.Windows.Forms.Button();
            this.Field_User = new System.Windows.Forms.TextBox();
            this.Field_Pass = new System.Windows.Forms.TextBox();
            this.Login_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Label_Username
            // 
            this.Label_Username.AutoSize = true;
            this.Label_Username.Location = new System.Drawing.Point(9, 60);
            this.Label_Username.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_Username.Name = "Label_Username";
            this.Label_Username.Size = new System.Drawing.Size(71, 16);
            this.Label_Username.TabIndex = 0;
            this.Label_Username.Text = "Username";
            // 
            // Label_Password
            // 
            this.Label_Password.AutoSize = true;
            this.Label_Password.Location = new System.Drawing.Point(9, 91);
            this.Label_Password.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label_Password.Name = "Label_Password";
            this.Label_Password.Size = new System.Drawing.Size(68, 16);
            this.Label_Password.TabIndex = 1;
            this.Label_Password.Text = "Password";
            // 
            // Button_Login
            // 
            this.Button_Login.Location = new System.Drawing.Point(139, 126);
            this.Button_Login.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Button_Login.Name = "Button_Login";
            this.Button_Login.Size = new System.Drawing.Size(100, 28);
            this.Button_Login.TabIndex = 2;
            this.Button_Login.Text = "Login";
            this.Button_Login.UseVisualStyleBackColor = true;
            this.Button_Login.Click += new System.EventHandler(this.ButtonLogin_Click);
            // 
            // Button_Exit
            // 
            this.Button_Exit.Location = new System.Drawing.Point(259, 126);
            this.Button_Exit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Button_Exit.Name = "Button_Exit";
            this.Button_Exit.Size = new System.Drawing.Size(100, 28);
            this.Button_Exit.TabIndex = 3;
            this.Button_Exit.Text = "Exit";
            this.Button_Exit.UseVisualStyleBackColor = true;
            this.Button_Exit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // Field_User
            // 
            this.Field_User.Location = new System.Drawing.Point(87, 60);
            this.Field_User.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Field_User.Name = "Field_User";
            this.Field_User.Size = new System.Drawing.Size(329, 22);
            this.Field_User.TabIndex = 4;
            // 
            // Field_Pass
            // 
            this.Field_Pass.Location = new System.Drawing.Point(87, 87);
            this.Field_Pass.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Field_Pass.Name = "Field_Pass";
            this.Field_Pass.PasswordChar = '*';
            this.Field_Pass.Size = new System.Drawing.Size(329, 22);
            this.Field_Pass.TabIndex = 5;
            // 
            // Login_Label
            // 
            this.Login_Label.AutoSize = true;
            this.Login_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Login_Label.Location = new System.Drawing.Point(133, 11);
            this.Login_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Login_Label.Name = "Login_Label";
            this.Login_Label.Size = new System.Drawing.Size(181, 20);
            this.Login_Label.TabIndex = 6;
            this.Login_Label.Text = "Please login to continue.";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 204);
            this.Controls.Add(this.Login_Label);
            this.Controls.Add(this.Field_Pass);
            this.Controls.Add(this.Field_User);
            this.Controls.Add(this.Button_Exit);
            this.Controls.Add(this.Button_Login);
            this.Controls.Add(this.Label_Password);
            this.Controls.Add(this.Label_Username);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "LoginForm";
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Username;
        private System.Windows.Forms.Label Label_Password;
        private System.Windows.Forms.Button Button_Login;
        private System.Windows.Forms.Button Button_Exit;
        private System.Windows.Forms.TextBox Field_User;
        private System.Windows.Forms.TextBox Field_Pass;
        private System.Windows.Forms.Label Login_Label;
    }
}
