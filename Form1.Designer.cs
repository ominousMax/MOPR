
namespace MOPR
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
            this.buttonDecart = new System.Windows.Forms.Button();
            this.buttonObobs = new System.Windows.Forms.Button();
            this.buttonSkor = new System.Windows.Forms.Button();
            this.buttonUskor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonDecart
            // 
            this.buttonDecart.Location = new System.Drawing.Point(637, 28);
            this.buttonDecart.Name = "buttonDecart";
            this.buttonDecart.Size = new System.Drawing.Size(138, 77);
            this.buttonDecart.TabIndex = 1;
            this.buttonDecart.Text = "Движение манипулятора в системе декартовых координат";
            this.buttonDecart.UseVisualStyleBackColor = true;
            this.buttonDecart.Click += new System.EventHandler(this.buttonDecart_Click);
            // 
            // buttonObobs
            // 
            this.buttonObobs.Location = new System.Drawing.Point(637, 111);
            this.buttonObobs.Name = "buttonObobs";
            this.buttonObobs.Size = new System.Drawing.Size(138, 77);
            this.buttonObobs.TabIndex = 2;
            this.buttonObobs.Text = "Графики обобщённых координат звеньев";
            this.buttonObobs.UseVisualStyleBackColor = true;
            this.buttonObobs.Click += new System.EventHandler(this.buttonObobs_Click);
            // 
            // buttonSkor
            // 
            this.buttonSkor.Location = new System.Drawing.Point(637, 194);
            this.buttonSkor.Name = "buttonSkor";
            this.buttonSkor.Size = new System.Drawing.Size(138, 77);
            this.buttonSkor.TabIndex = 3;
            this.buttonSkor.Text = "Графики скоростей звеньев";
            this.buttonSkor.UseVisualStyleBackColor = true;
            this.buttonSkor.Click += new System.EventHandler(this.buttonSkor_Click);
            // 
            // buttonUskor
            // 
            this.buttonUskor.Location = new System.Drawing.Point(637, 277);
            this.buttonUskor.Name = "buttonUskor";
            this.buttonUskor.Size = new System.Drawing.Size(138, 77);
            this.buttonUskor.TabIndex = 4;
            this.buttonUskor.Text = "Графики ускорений звеньев";
            this.buttonUskor.UseVisualStyleBackColor = true;
            this.buttonUskor.Click += new System.EventHandler(this.buttonUskor_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonUskor);
            this.Controls.Add(this.buttonSkor);
            this.Controls.Add(this.buttonObobs);
            this.Controls.Add(this.buttonDecart);
            this.Name = "Form1";
            this.Text = "Курсовой проект";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonDecart;
        private System.Windows.Forms.Button buttonObobs;
        private System.Windows.Forms.Button buttonSkor;
        private System.Windows.Forms.Button buttonUskor;
    }
}

