namespace CryptLab3AES
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1encode = new System.Windows.Forms.Button();
            this.textBox1key = new System.Windows.Forms.TextBox();
            this.button1decode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1encode
            // 
            this.button1encode.Location = new System.Drawing.Point(78, 145);
            this.button1encode.Name = "button1encode";
            this.button1encode.Size = new System.Drawing.Size(75, 23);
            this.button1encode.TabIndex = 0;
            this.button1encode.Text = "Encode";
            this.button1encode.UseVisualStyleBackColor = true;
            this.button1encode.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1key
            // 
            this.textBox1key.Location = new System.Drawing.Point(32, 47);
            this.textBox1key.Name = "textBox1key";
            this.textBox1key.Size = new System.Drawing.Size(177, 20);
            this.textBox1key.TabIndex = 1;
            // 
            // button1decode
            // 
            this.button1decode.Location = new System.Drawing.Point(78, 189);
            this.button1decode.Name = "button1decode";
            this.button1decode.Size = new System.Drawing.Size(75, 23);
            this.button1decode.TabIndex = 2;
            this.button1decode.Text = "Decode";
            this.button1decode.UseVisualStyleBackColor = true;
            this.button1decode.Click += new System.EventHandler(this.button1decode_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1decode);
            this.Controls.Add(this.textBox1key);
            this.Controls.Add(this.button1encode);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1encode;
        private System.Windows.Forms.TextBox textBox1key;
        private System.Windows.Forms.Button button1decode;
    }
}

