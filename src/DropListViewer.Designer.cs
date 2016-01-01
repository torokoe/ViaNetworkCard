namespace VIANetWorkCard
{
    partial class DropListViewer
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
            this.label1 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 272);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Double click to Run , Right Click to view details";
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(9, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(340, 256);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // DropListViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 291);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label1);
            this.Name = "DropListViewer";
            this.Text = "Favourite";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DropListViewer_FormClosed);
            this.Load += new System.EventHandler(this.DropListViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listView1;
    }
}