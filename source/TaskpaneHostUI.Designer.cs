namespace solid_macro
{
    partial class TaskpaneHostUI
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
            this.open_assembly_button = new System.Windows.Forms.Button();
            this.execute_test_button = new System.Windows.Forms.Button();
            this.screw_macro = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // open_assembly_button
            // 
            this.open_assembly_button.Location = new System.Drawing.Point(10, 10);
            this.open_assembly_button.Name = "open_assembly_button";
            this.open_assembly_button.Size = new System.Drawing.Size(122, 40);
            this.open_assembly_button.TabIndex = 0;
            this.open_assembly_button.Text = "Open Bike Assembly";
            this.open_assembly_button.UseVisualStyleBackColor = true;
            this.open_assembly_button.Click += new System.EventHandler(this.OpenAssembly_Click);
            // 
            // execute_test_button
            // 
            this.execute_test_button.Enabled = false;
            this.execute_test_button.Location = new System.Drawing.Point(10, 57);
            this.execute_test_button.Name = "execute_test_button";
            this.execute_test_button.Size = new System.Drawing.Size(122, 40);
            this.execute_test_button.TabIndex = 1;
            this.execute_test_button.Text = "Execute Bike Test";
            this.execute_test_button.UseVisualStyleBackColor = true;
            this.execute_test_button.Click += new System.EventHandler(this.BikeTest_Click);
            // 
            // screw_macro
            // 
            this.screw_macro.Location = new System.Drawing.Point(10, 104);
            this.screw_macro.Name = "screw_macro";
            this.screw_macro.Size = new System.Drawing.Size(122, 40);
            this.screw_macro.TabIndex = 2;
            this.screw_macro.Text = "ScrewMacro";
            this.screw_macro.UseVisualStyleBackColor = true;
            this.screw_macro.Click += new System.EventHandler(this.screw_macro_Click);
            // 
            // TaskpaneHostUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.screw_macro);
            this.Controls.Add(this.execute_test_button);
            this.Controls.Add(this.open_assembly_button);
            this.Name = "TaskpaneHostUI";
            this.Size = new System.Drawing.Size(141, 197);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button open_assembly_button;
        private System.Windows.Forms.Button execute_test_button;
        private System.Windows.Forms.Button screw_macro;
    }
}
