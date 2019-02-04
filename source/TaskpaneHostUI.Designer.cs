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
            this.execute_test_button = new System.Windows.Forms.Button();
            this.screw_macro = new System.Windows.Forms.Button();
            this.bike_tests_num = new System.Windows.Forms.TextBox();
            this.screw_tests_num = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // execute_test_button
            //
            this.execute_test_button.Location = new System.Drawing.Point(10, 3);
            this.execute_test_button.Name = "execute_test_button";
            this.execute_test_button.Size = new System.Drawing.Size(120, 40);
            this.execute_test_button.TabIndex = 1;
            this.execute_test_button.Text = "Execute Bike Test";
            this.execute_test_button.UseVisualStyleBackColor = true;
            this.execute_test_button.Click += new System.EventHandler(this.BikeTest_Click);
            //
            // screw_macro
            //
            this.screw_macro.Location = new System.Drawing.Point(10, 96);
            this.screw_macro.Name = "screw_macro";
            this.screw_macro.Size = new System.Drawing.Size(120, 40);
            this.screw_macro.TabIndex = 2;
            this.screw_macro.Text = "ScrewMacro";
            this.screw_macro.UseVisualStyleBackColor = true;
            this.screw_macro.Click += new System.EventHandler(this.ScrewMacro_Click);
            //
            // bike_tests_num
            //
            this.bike_tests_num.Location = new System.Drawing.Point(10, 49);
            this.bike_tests_num.Name = "bike_tests_num";
            this.bike_tests_num.Size = new System.Drawing.Size(120, 20);
            this.bike_tests_num.TabIndex = 3;
            this.bike_tests_num.Text = "1";
            //
            // screw_tests_num
            //
            this.screw_tests_num.Location = new System.Drawing.Point(10, 143);
            this.screw_tests_num.Name = "screw_tests_num";
            this.screw_tests_num.Size = new System.Drawing.Size(120, 20);
            this.screw_tests_num.TabIndex = 4;
            this.screw_tests_num.Text = "1";
            //
            // TaskpaneHostUI
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.screw_tests_num);
            this.Controls.Add(this.bike_tests_num);
            this.Controls.Add(this.screw_macro);
            this.Controls.Add(this.execute_test_button);
            this.Name = "TaskpaneHostUI";
            this.Size = new System.Drawing.Size(141, 197);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button execute_test_button;
        private System.Windows.Forms.Button screw_macro;
        private System.Windows.Forms.TextBox bike_tests_num;
        private System.Windows.Forms.TextBox screw_tests_num;
    }
}
