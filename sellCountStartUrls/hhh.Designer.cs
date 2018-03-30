namespace sellCountStartUrls
{
    partial class hhh
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip_close = new System.Windows.Forms.ToolTip(this.components);
            this.label_move = new System.Windows.Forms.Label();
            this.label_close = new System.Windows.Forms.Label();
            this.label_min = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(650, 494);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_move
            // 
            this.label_move.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.label_move.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //this.label_move.Image = global::sellCountStartUrls.Properties.Resources.move;
            this.label_move.Location = new System.Drawing.Point(647, 4);
            this.label_move.Name = "label_move";
            this.label_move.Size = new System.Drawing.Size(16, 16);
            this.label_move.TabIndex = 5;
            this.toolTip_close.SetToolTip(this.label_move, "鼠标拖拽移动");
            this.label_move.MouseDown += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseDown);
            this.label_move.MouseMove += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseMove);
            this.label_move.MouseUp += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseUp);
            // 
            // label_close
            // 
            this.label_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_close.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_close.Image = global::sellCountStartUrls.Properties.Resources.closing;
            this.label_close.Location = new System.Drawing.Point(707, 4);
            this.label_close.Name = "label_close";
            this.label_close.Size = new System.Drawing.Size(16, 16);
            this.label_close.TabIndex = 3;
            this.toolTip_close.SetToolTip(this.label_close, "关闭");
            this.label_close.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            // 
            // label_min
            // 
            this.label_min.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label_min.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_min.Image = global::sellCountStartUrls.Properties.Resources.mining;
            this.label_min.Location = new System.Drawing.Point(677, 4);
            this.label_min.Name = "label_min";
            this.label_min.Size = new System.Drawing.Size(24, 16);
            this.label_min.TabIndex = 2;
            this.toolTip_close.SetToolTip(this.label_min, "最小化");
            this.label_min.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_min_MouseDown);
            // 
            // hhh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 518);
            this.Controls.Add(this.label_move);
            this.Controls.Add(this.label_close);
            this.Controls.Add(this.label_min);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HelpButton = true;
            this.Name = "hhh";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "hhh";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.hhh_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_min;
        private System.Windows.Forms.Label label_close;
        private System.Windows.Forms.Label label_move;
        private System.Windows.Forms.ToolTip toolTip_close;
    }
}