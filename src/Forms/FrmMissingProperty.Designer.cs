
namespace ClassBuilderGenerator.Forms
{
    partial class FrmMissingProperty
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
            if(disposing && (components != null))
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSelectedConstructor = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.rbtnForceCreation = new System.Windows.Forms.RadioButton();
            this.rbtnDoNothing = new System.Windows.Forms.RadioButton();
            this.lbMissingProperties = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Make another choice!";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 29);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(519, 36);
            this.label3.TabIndex = 2;
            this.label3.Text = "The selected constructor takes one or more properties that are not present in the" +
    " base class";
            // 
            // txtSelectedConstructor
            // 
            this.txtSelectedConstructor.Location = new System.Drawing.Point(18, 68);
            this.txtSelectedConstructor.Name = "txtSelectedConstructor";
            this.txtSelectedConstructor.ReadOnly = true;
            this.txtSelectedConstructor.Size = new System.Drawing.Size(509, 22);
            this.txtSelectedConstructor.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(351, 263);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(176, 32);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // rbtnForceCreation
            // 
            this.rbtnForceCreation.Checked = true;
            this.rbtnForceCreation.Location = new System.Drawing.Point(351, 104);
            this.rbtnForceCreation.Name = "rbtnForceCreation";
            this.rbtnForceCreation.Size = new System.Drawing.Size(176, 41);
            this.rbtnForceCreation.TabIndex = 6;
            this.rbtnForceCreation.TabStop = true;
            this.rbtnForceCreation.Text = "Force creation of missing properties";
            this.rbtnForceCreation.UseVisualStyleBackColor = true;
            // 
            // rbtnDoNothing
            // 
            this.rbtnDoNothing.Location = new System.Drawing.Point(351, 151);
            this.rbtnDoNothing.Name = "rbtnDoNothing";
            this.rbtnDoNothing.Size = new System.Drawing.Size(176, 25);
            this.rbtnDoNothing.TabIndex = 7;
            this.rbtnDoNothing.Text = "Do nothing about it";
            this.rbtnDoNothing.UseVisualStyleBackColor = true;
            // 
            // lbMissingProperties
            // 
            this.lbMissingProperties.FormattingEnabled = true;
            this.lbMissingProperties.ItemHeight = 16;
            this.lbMissingProperties.Location = new System.Drawing.Point(12, 104);
            this.lbMissingProperties.Name = "lbMissingProperties";
            this.lbMissingProperties.Size = new System.Drawing.Size(333, 196);
            this.lbMissingProperties.TabIndex = 8;
            // 
            // FrmMissingProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 308);
            this.ControlBox = false;
            this.Controls.Add(this.lbMissingProperties);
            this.Controls.Add(this.rbtnDoNothing);
            this.Controls.Add(this.rbtnForceCreation);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtSelectedConstructor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmMissingProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Missing Property";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSelectedConstructor;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RadioButton rbtnForceCreation;
        private System.Windows.Forms.RadioButton rbtnDoNothing;
        private System.Windows.Forms.ListBox lbMissingProperties;
    }
}