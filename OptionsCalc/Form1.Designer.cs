namespace OptionsCalc
{
    partial class Form1
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
            this.PortfoliosDG = new System.Windows.Forms.DataGridView();
            this.portfoliosBS = new System.Windows.Forms.BindingSource(this.components);
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnAddInstrumetn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PortfoliosDG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portfoliosBS)).BeginInit();
            this.SuspendLayout();
            // 
            // PortfoliosDG
            // 
            this.PortfoliosDG.AutoGenerateColumns = false;
            this.PortfoliosDG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PortfoliosDG.DataSource = this.portfoliosBS;
            this.PortfoliosDG.Location = new System.Drawing.Point(12, 12);
            this.PortfoliosDG.Name = "PortfoliosDG";
            this.PortfoliosDG.Size = new System.Drawing.Size(761, 280);
            this.PortfoliosDG.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 311);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnAddInstrumetn
            // 
            this.btnAddInstrumetn.Location = new System.Drawing.Point(123, 311);
            this.btnAddInstrumetn.Name = "btnAddInstrumetn";
            this.btnAddInstrumetn.Size = new System.Drawing.Size(98, 23);
            this.btnAddInstrumetn.TabIndex = 2;
            this.btnAddInstrumetn.Text = "AddInstrument";
            this.btnAddInstrumetn.UseVisualStyleBackColor = true;
            this.btnAddInstrumetn.Click += new System.EventHandler(this.btnAddInstrumetn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 348);
            this.Controls.Add(this.btnAddInstrumetn);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.PortfoliosDG);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PortfoliosDG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portfoliosBS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PortfoliosDG;
        private System.Windows.Forms.BindingSource portfoliosBS;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAddInstrumetn;
    }
}

