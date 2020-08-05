namespace D7250_DIO
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn90 = new System.Windows.Forms.Button();
            this.btn180 = new System.Windows.Forms.Button();
            this.btn270 = new System.Windows.Forms.Button();
            this.btnZero = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(168, 87);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 25);
            this.button2.TabIndex = 11;
            this.button2.Text = "Read From ＤＩ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(166, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "DI:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "DO:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(168, 39);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 8;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(22, 39);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 25);
            this.button1.TabIndex = 6;
            this.button1.Text = "Write To ＤＯ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn90
            // 
            this.btn90.Location = new System.Drawing.Point(326, 17);
            this.btn90.Name = "btn90";
            this.btn90.Size = new System.Drawing.Size(51, 24);
            this.btn90.TabIndex = 12;
            this.btn90.Text = "90";
            this.btn90.UseVisualStyleBackColor = true;
            this.btn90.Click += new System.EventHandler(this.btn90_Click);
            // 
            // btn180
            // 
            this.btn180.Location = new System.Drawing.Point(326, 47);
            this.btn180.Name = "btn180";
            this.btn180.Size = new System.Drawing.Size(51, 24);
            this.btn180.TabIndex = 13;
            this.btn180.Text = "180";
            this.btn180.UseVisualStyleBackColor = true;
            this.btn180.Click += new System.EventHandler(this.btn180_Click);
            // 
            // btn270
            // 
            this.btn270.Location = new System.Drawing.Point(326, 77);
            this.btn270.Name = "btn270";
            this.btn270.Size = new System.Drawing.Size(51, 24);
            this.btn270.TabIndex = 14;
            this.btn270.Text = "270";
            this.btn270.UseVisualStyleBackColor = true;
            this.btn270.Click += new System.EventHandler(this.btn270_Click);
            // 
            // btnZero
            // 
            this.btnZero.Location = new System.Drawing.Point(383, 17);
            this.btnZero.Name = "btnZero";
            this.btnZero.Size = new System.Drawing.Size(51, 24);
            this.btnZero.TabIndex = 15;
            this.btnZero.Text = "Zero";
            this.btnZero.UseVisualStyleBackColor = true;
            this.btnZero.Click += new System.EventHandler(this.btnZero_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 148);
            this.Controls.Add(this.btnZero);
            this.Controls.Add(this.btn270);
            this.Controls.Add(this.btn180);
            this.Controls.Add(this.btn90);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn90;
        private System.Windows.Forms.Button btn180;
        private System.Windows.Forms.Button btn270;
        private System.Windows.Forms.Button btnZero;
    }
}

