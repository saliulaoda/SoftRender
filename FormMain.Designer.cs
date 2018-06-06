namespace SoftRender_Windows
{
    partial class FormMain
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            //this.buttonLeft = new System.Windows.Forms.Button();
            //this.buttonRight = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            //// 
            //// buttonLeft
            //// 
            //this.buttonLeft.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //this.buttonLeft.Location = new System.Drawing.Point(41, 273);
            //this.buttonLeft.Name = "buttonLeft";
            //this.buttonLeft.Size = new System.Drawing.Size(70, 62);
            //this.buttonLeft.TabIndex = 1;
            //this.buttonLeft.Text = "⇐";
            //this.buttonLeft.UseVisualStyleBackColor = true;
            //this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            //// 
            //// buttonRight
            //// 
            //this.buttonRight.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //this.buttonRight.Location = new System.Drawing.Point(647, 273);
            //this.buttonRight.Name = "buttonRight";
            //this.buttonRight.Size = new System.Drawing.Size(74, 62);
            //this.buttonRight.TabIndex = 2;
            //this.buttonRight.Text = "⇒";
            //this.buttonRight.UseVisualStyleBackColor = true;
            //this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            //this.Controls.Add(this.buttonRight);
            //this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonRight;
    }
}

