namespace course
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            SaveToDatabase_checkBox = new CheckBox();
            DynamicsSearchMode_label = new Label();
            DynamicsSearchMode_comboBox = new ComboBox();
            NumberOfMeasurements_label = new Label();
            NumberOfMeasurements_textBox = new TextBox();
            CO2Sensitivity_label = new Label();
            CO2Sensitivity_textBox = new TextBox();
            PressureSensitivity_label = new Label();
            PressureSensitivity_textBox = new TextBox();
            HumiditySensitivity_label = new Label();
            HumiditySensitivity_textBox = new TextBox();
            Sensitivity_label = new Label();
            TemperatureSensitivity_textBox = new TextBox();
            Measure_button = new Button();
            TimeInterval_label = new Label();
            TimeInterval_textBox = new TextBox();
            ServerLog_richTextBox = new RichTextBox();
            StopLog_button = new Button();
            StartLog_button = new Button();
            CO2_checkBox = new CheckBox();
            Pressure_checkBox = new CheckBox();
            Humidity_checkBox = new CheckBox();
            SelectedSensors_label = new Label();
            Temperature_checkBox = new CheckBox();
            DeviceMode_label = new Label();
            DeviceMode_comboBox = new ComboBox();
            SelectedDevice_label = new Label();
            SelectedDevice_comboBox = new ComboBox();
            ServerLog_label = new Label();
            RemoveDevice_button = new Button();
            AddDevice_button = new Button();
            tabPage2 = new TabPage();
            SelectedTable_comboBox = new ComboBox();
            SelectedTable_label = new Label();
            dataGridView1 = new DataGridView();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(800, 450);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(SaveToDatabase_checkBox);
            tabPage1.Controls.Add(DynamicsSearchMode_label);
            tabPage1.Controls.Add(DynamicsSearchMode_comboBox);
            tabPage1.Controls.Add(NumberOfMeasurements_label);
            tabPage1.Controls.Add(NumberOfMeasurements_textBox);
            tabPage1.Controls.Add(CO2Sensitivity_label);
            tabPage1.Controls.Add(CO2Sensitivity_textBox);
            tabPage1.Controls.Add(PressureSensitivity_label);
            tabPage1.Controls.Add(PressureSensitivity_textBox);
            tabPage1.Controls.Add(HumiditySensitivity_label);
            tabPage1.Controls.Add(HumiditySensitivity_textBox);
            tabPage1.Controls.Add(Sensitivity_label);
            tabPage1.Controls.Add(TemperatureSensitivity_textBox);
            tabPage1.Controls.Add(Measure_button);
            tabPage1.Controls.Add(TimeInterval_label);
            tabPage1.Controls.Add(TimeInterval_textBox);
            tabPage1.Controls.Add(ServerLog_richTextBox);
            tabPage1.Controls.Add(StopLog_button);
            tabPage1.Controls.Add(StartLog_button);
            tabPage1.Controls.Add(CO2_checkBox);
            tabPage1.Controls.Add(Pressure_checkBox);
            tabPage1.Controls.Add(Humidity_checkBox);
            tabPage1.Controls.Add(SelectedSensors_label);
            tabPage1.Controls.Add(Temperature_checkBox);
            tabPage1.Controls.Add(DeviceMode_label);
            tabPage1.Controls.Add(DeviceMode_comboBox);
            tabPage1.Controls.Add(SelectedDevice_label);
            tabPage1.Controls.Add(SelectedDevice_comboBox);
            tabPage1.Controls.Add(ServerLog_label);
            tabPage1.Controls.Add(RemoveDevice_button);
            tabPage1.Controls.Add(AddDevice_button);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(792, 422);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Communication Controls ";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // SaveToDatabase_checkBox
            // 
            SaveToDatabase_checkBox.AutoSize = true;
            SaveToDatabase_checkBox.Location = new Point(384, 125);
            SaveToDatabase_checkBox.Name = "SaveToDatabase_checkBox";
            SaveToDatabase_checkBox.Size = new Size(115, 19);
            SaveToDatabase_checkBox.TabIndex = 33;
            SaveToDatabase_checkBox.Text = "Save to Database";
            SaveToDatabase_checkBox.UseVisualStyleBackColor = true;
            // 
            // DynamicsSearchMode_label
            // 
            DynamicsSearchMode_label.AutoSize = true;
            DynamicsSearchMode_label.Location = new Point(13, 170);
            DynamicsSearchMode_label.Name = "DynamicsSearchMode_label";
            DynamicsSearchMode_label.Size = new Size(131, 15);
            DynamicsSearchMode_label.TabIndex = 32;
            DynamicsSearchMode_label.Text = "Dynamics Search Mode";
            // 
            // DynamicsSearchMode_comboBox
            // 
            DynamicsSearchMode_comboBox.FormattingEnabled = true;
            DynamicsSearchMode_comboBox.Items.AddRange(new object[] { "Comparison with the Last One", "Comparison with the Average" });
            DynamicsSearchMode_comboBox.Location = new Point(13, 188);
            DynamicsSearchMode_comboBox.Name = "DynamicsSearchMode_comboBox";
            DynamicsSearchMode_comboBox.Size = new Size(199, 23);
            DynamicsSearchMode_comboBox.TabIndex = 31;
            DynamicsSearchMode_comboBox.SelectedIndexChanged += DynamicsSearchMode_comboBox_SelectedIndexChanged;
            // 
            // NumberOfMeasurements_label
            // 
            NumberOfMeasurements_label.AutoSize = true;
            NumberOfMeasurements_label.Location = new Point(384, 170);
            NumberOfMeasurements_label.Name = "NumberOfMeasurements_label";
            NumberOfMeasurements_label.Size = new Size(146, 15);
            NumberOfMeasurements_label.TabIndex = 30;
            NumberOfMeasurements_label.Text = "Number of Measurements";
            // 
            // NumberOfMeasurements_textBox
            // 
            NumberOfMeasurements_textBox.Location = new Point(384, 188);
            NumberOfMeasurements_textBox.Name = "NumberOfMeasurements_textBox";
            NumberOfMeasurements_textBox.Size = new Size(100, 23);
            NumberOfMeasurements_textBox.TabIndex = 29;
            NumberOfMeasurements_textBox.TextChanged += NumberOfMeasurements_textBox_TextChanged;
            // 
            // CO2Sensitivity_label
            // 
            CO2Sensitivity_label.AutoSize = true;
            CO2Sensitivity_label.Location = new Point(384, 301);
            CO2Sensitivity_label.Name = "CO2Sensitivity_label";
            CO2Sensitivity_label.Size = new Size(86, 15);
            CO2Sensitivity_label.TabIndex = 28;
            CO2Sensitivity_label.Text = "CO2 Sensitivity";
            // 
            // CO2Sensitivity_textBox
            // 
            CO2Sensitivity_textBox.Location = new Point(384, 319);
            CO2Sensitivity_textBox.Name = "CO2Sensitivity_textBox";
            CO2Sensitivity_textBox.Size = new Size(100, 23);
            CO2Sensitivity_textBox.TabIndex = 27;
            CO2Sensitivity_textBox.TextChanged += CO2Sensitivity_textBox_TextChanged;
            // 
            // PressureSensitivity_label
            // 
            PressureSensitivity_label.AutoSize = true;
            PressureSensitivity_label.Location = new Point(243, 301);
            PressureSensitivity_label.Name = "PressureSensitivity_label";
            PressureSensitivity_label.Size = new Size(107, 15);
            PressureSensitivity_label.TabIndex = 26;
            PressureSensitivity_label.Text = "Pressure Sensitivity";
            // 
            // PressureSensitivity_textBox
            // 
            PressureSensitivity_textBox.Location = new Point(243, 319);
            PressureSensitivity_textBox.Name = "PressureSensitivity_textBox";
            PressureSensitivity_textBox.Size = new Size(100, 23);
            PressureSensitivity_textBox.TabIndex = 25;
            PressureSensitivity_textBox.TextChanged += PressureSensitivity_textBox_TextChanged;
            // 
            // HumiditySensitivity_label
            // 
            HumiditySensitivity_label.AutoSize = true;
            HumiditySensitivity_label.Location = new Point(384, 238);
            HumiditySensitivity_label.Name = "HumiditySensitivity_label";
            HumiditySensitivity_label.Size = new Size(113, 15);
            HumiditySensitivity_label.TabIndex = 24;
            HumiditySensitivity_label.Text = "Humidity Sensitivity";
            // 
            // HumiditySensitivity_textBox
            // 
            HumiditySensitivity_textBox.Location = new Point(384, 256);
            HumiditySensitivity_textBox.Name = "HumiditySensitivity_textBox";
            HumiditySensitivity_textBox.Size = new Size(100, 23);
            HumiditySensitivity_textBox.TabIndex = 23;
            HumiditySensitivity_textBox.TextChanged += HumiditySensitivity_textBox_TextChanged;
            // 
            // Sensitivity_label
            // 
            Sensitivity_label.AutoSize = true;
            Sensitivity_label.Location = new Point(243, 238);
            Sensitivity_label.Name = "Sensitivity_label";
            Sensitivity_label.Size = new Size(129, 15);
            Sensitivity_label.TabIndex = 22;
            Sensitivity_label.Text = "Temperature Sensitivity";
            // 
            // TemperatureSensitivity_textBox
            // 
            TemperatureSensitivity_textBox.Location = new Point(243, 256);
            TemperatureSensitivity_textBox.Name = "TemperatureSensitivity_textBox";
            TemperatureSensitivity_textBox.Size = new Size(100, 23);
            TemperatureSensitivity_textBox.TabIndex = 21;
            TemperatureSensitivity_textBox.TextChanged += TemperatureSensitivity_textBox_TextChanged;
            // 
            // Measure_button
            // 
            Measure_button.Location = new Point(243, 119);
            Measure_button.Name = "Measure_button";
            Measure_button.Size = new Size(90, 29);
            Measure_button.TabIndex = 20;
            Measure_button.Text = "Measure";
            Measure_button.UseVisualStyleBackColor = true;
            Measure_button.Click += Measure_button_Click;
            // 
            // TimeInterval_label
            // 
            TimeInterval_label.AutoSize = true;
            TimeInterval_label.Location = new Point(243, 170);
            TimeInterval_label.Name = "TimeInterval_label";
            TimeInterval_label.Size = new Size(123, 15);
            TimeInterval_label.TabIndex = 19;
            TimeInterval_label.Text = "Measure Time Interval";
            // 
            // TimeInterval_textBox
            // 
            TimeInterval_textBox.Location = new Point(243, 188);
            TimeInterval_textBox.Name = "TimeInterval_textBox";
            TimeInterval_textBox.Size = new Size(100, 23);
            TimeInterval_textBox.TabIndex = 18;
            TimeInterval_textBox.TextChanged += TimeInterval_textBox_TextChanged;
            // 
            // ServerLog_richTextBox
            // 
            ServerLog_richTextBox.Location = new Point(536, 28);
            ServerLog_richTextBox.Name = "ServerLog_richTextBox";
            ServerLog_richTextBox.Size = new Size(248, 338);
            ServerLog_richTextBox.TabIndex = 17;
            ServerLog_richTextBox.Text = "";
            // 
            // StopLog_button
            // 
            StopLog_button.Location = new Point(314, 37);
            StopLog_button.Name = "StopLog_button";
            StopLog_button.Size = new Size(90, 29);
            StopLog_button.TabIndex = 16;
            StopLog_button.Text = "Stop";
            StopLog_button.UseVisualStyleBackColor = true;
            StopLog_button.Click += StopLog_button_Click;
            // 
            // StartLog_button
            // 
            StartLog_button.Location = new Point(218, 37);
            StartLog_button.Name = "StartLog_button";
            StartLog_button.Size = new Size(90, 29);
            StartLog_button.TabIndex = 15;
            StartLog_button.Text = "Start";
            StartLog_button.UseVisualStyleBackColor = true;
            StartLog_button.Click += StartLog_button_Click;
            // 
            // CO2_checkBox
            // 
            CO2_checkBox.AutoSize = true;
            CO2_checkBox.Checked = true;
            CO2_checkBox.CheckState = CheckState.Checked;
            CO2_checkBox.Location = new Point(13, 331);
            CO2_checkBox.Name = "CO2_checkBox";
            CO2_checkBox.Size = new Size(49, 19);
            CO2_checkBox.TabIndex = 14;
            CO2_checkBox.Text = "CO2";
            CO2_checkBox.UseVisualStyleBackColor = true;
            CO2_checkBox.CheckedChanged += CO2_checkBox_CheckedChanged;
            // 
            // Pressure_checkBox
            // 
            Pressure_checkBox.AutoSize = true;
            Pressure_checkBox.Checked = true;
            Pressure_checkBox.CheckState = CheckState.Checked;
            Pressure_checkBox.Location = new Point(13, 306);
            Pressure_checkBox.Name = "Pressure_checkBox";
            Pressure_checkBox.Size = new Size(70, 19);
            Pressure_checkBox.TabIndex = 13;
            Pressure_checkBox.Text = "Pressure";
            Pressure_checkBox.UseVisualStyleBackColor = true;
            Pressure_checkBox.CheckedChanged += Pressure_checkBox_CheckedChanged;
            // 
            // Humidity_checkBox
            // 
            Humidity_checkBox.AutoSize = true;
            Humidity_checkBox.Checked = true;
            Humidity_checkBox.CheckState = CheckState.Checked;
            Humidity_checkBox.Location = new Point(13, 281);
            Humidity_checkBox.Name = "Humidity_checkBox";
            Humidity_checkBox.Size = new Size(76, 19);
            Humidity_checkBox.TabIndex = 12;
            Humidity_checkBox.Text = "Humidity";
            Humidity_checkBox.UseVisualStyleBackColor = true;
            Humidity_checkBox.CheckedChanged += Humidity_checkBox_CheckedChanged;
            // 
            // SelectedSensors_label
            // 
            SelectedSensors_label.AutoSize = true;
            SelectedSensors_label.Location = new Point(13, 238);
            SelectedSensors_label.Name = "SelectedSensors_label";
            SelectedSensors_label.Size = new Size(94, 15);
            SelectedSensors_label.TabIndex = 11;
            SelectedSensors_label.Text = "Selected Sensors";
            // 
            // Temperature_checkBox
            // 
            Temperature_checkBox.AutoSize = true;
            Temperature_checkBox.Checked = true;
            Temperature_checkBox.CheckState = CheckState.Checked;
            Temperature_checkBox.Location = new Point(13, 256);
            Temperature_checkBox.Name = "Temperature_checkBox";
            Temperature_checkBox.Size = new Size(92, 19);
            Temperature_checkBox.TabIndex = 10;
            Temperature_checkBox.Text = "Temperature";
            Temperature_checkBox.UseVisualStyleBackColor = true;
            Temperature_checkBox.CheckedChanged += Temperature_checkBox_CheckedChanged;
            // 
            // DeviceMode_label
            // 
            DeviceMode_label.AutoSize = true;
            DeviceMode_label.Location = new Point(13, 105);
            DeviceMode_label.Name = "DeviceMode_label";
            DeviceMode_label.Size = new Size(76, 15);
            DeviceMode_label.TabIndex = 9;
            DeviceMode_label.Text = "Device Mode";
            // 
            // DeviceMode_comboBox
            // 
            DeviceMode_comboBox.FormattingEnabled = true;
            DeviceMode_comboBox.Items.AddRange(new object[] { "Standart Monitoring", "On Command", "Dynamics Search", "In-depth Monitoring" });
            DeviceMode_comboBox.Location = new Point(13, 123);
            DeviceMode_comboBox.Name = "DeviceMode_comboBox";
            DeviceMode_comboBox.Size = new Size(199, 23);
            DeviceMode_comboBox.TabIndex = 8;
            DeviceMode_comboBox.SelectedIndexChanged += DeviceMode_comboBox_SelectedIndexChanged;
            // 
            // SelectedDevice_label
            // 
            SelectedDevice_label.AutoSize = true;
            SelectedDevice_label.Location = new Point(13, 23);
            SelectedDevice_label.Name = "SelectedDevice_label";
            SelectedDevice_label.Size = new Size(89, 15);
            SelectedDevice_label.TabIndex = 7;
            SelectedDevice_label.Text = "Selected Device";
            // 
            // SelectedDevice_comboBox
            // 
            SelectedDevice_comboBox.FormattingEnabled = true;
            SelectedDevice_comboBox.Items.AddRange(new object[] { "None" });
            SelectedDevice_comboBox.Location = new Point(13, 41);
            SelectedDevice_comboBox.Name = "SelectedDevice_comboBox";
            SelectedDevice_comboBox.Size = new Size(199, 23);
            SelectedDevice_comboBox.TabIndex = 6;
            // 
            // ServerLog_label
            // 
            ServerLog_label.AutoSize = true;
            ServerLog_label.Location = new Point(538, 10);
            ServerLog_label.Name = "ServerLog_label";
            ServerLog_label.Size = new Size(62, 15);
            ServerLog_label.TabIndex = 3;
            ServerLog_label.Text = "Server Log";
            // 
            // RemoveDevice_button
            // 
            RemoveDevice_button.Location = new Point(410, 37);
            RemoveDevice_button.Name = "RemoveDevice_button";
            RemoveDevice_button.Size = new Size(90, 29);
            RemoveDevice_button.TabIndex = 2;
            RemoveDevice_button.Text = "Remove";
            RemoveDevice_button.UseVisualStyleBackColor = true;
            RemoveDevice_button.Click += RemoveDevice_button_Click;
            // 
            // AddDevice_button
            // 
            AddDevice_button.Location = new Point(599, 380);
            AddDevice_button.Name = "AddDevice_button";
            AddDevice_button.Size = new Size(104, 29);
            AddDevice_button.TabIndex = 1;
            AddDevice_button.Text = "Find Device";
            AddDevice_button.UseVisualStyleBackColor = true;
            AddDevice_button.Click += AddDevice_button_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(SelectedTable_comboBox);
            tabPage2.Controls.Add(SelectedTable_label);
            tabPage2.Controls.Add(dataGridView1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(792, 422);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "DataBase";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // SelectedTable_comboBox
            // 
            SelectedTable_comboBox.FormattingEnabled = true;
            SelectedTable_comboBox.Items.AddRange(new object[] { "Standart Monitoring", "On Command", "Dynamics Search", "In-Depth Monitoring" });
            SelectedTable_comboBox.Location = new Point(20, 32);
            SelectedTable_comboBox.Name = "SelectedTable_comboBox";
            SelectedTable_comboBox.Size = new Size(175, 23);
            SelectedTable_comboBox.TabIndex = 2;
            SelectedTable_comboBox.SelectedIndexChanged += SelectedTable_comboBox_SelectedIndexChanged;
            // 
            // SelectedTable_label
            // 
            SelectedTable_label.AutoSize = true;
            SelectedTable_label.Location = new Point(20, 14);
            SelectedTable_label.Name = "SelectedTable_label";
            SelectedTable_label.Size = new Size(81, 15);
            SelectedTable_label.TabIndex = 1;
            SelectedTable_label.Text = "Selected Table";
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.ButtonHighlight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Bottom;
            dataGridView1.Location = new Point(3, 63);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(786, 356);
            dataGridView1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button RemoveDevice_button;
        private Button AddDevice_button;
        private Label ServerLog_label;
        private CheckBox CO2_checkBox;
        private CheckBox Pressure_checkBox;
        private CheckBox Humidity_checkBox;
        private Label SelectedSensors_label;
        private CheckBox Temperature_checkBox;
        private Label DeviceMode_label;
        private ComboBox DeviceMode_comboBox;
        private Label SelectedDevice_label;
        private ComboBox SelectedDevice_comboBox;
        private Button StartLog_button;
        private Button StopLog_button;
        private RichTextBox ServerLog_richTextBox;
        private Label TimeInterval_label;
        private TextBox TimeInterval_textBox;
        private Label Sensitivity_label;
        private TextBox TemperatureSensitivity_textBox;
        private Button Measure_button;
        private Label CO2Sensitivity_label;
        private TextBox CO2Sensitivity_textBox;
        private Label PressureSensitivity_label;
        private TextBox PressureSensitivity_textBox;
        private Label HumiditySensitivity_label;
        private TextBox HumiditySensitivity_textBox;
        private Label NumberOfMeasurements_label;
        private TextBox NumberOfMeasurements_textBox;
        private Label DynamicsSearchMode_label;
        private ComboBox DynamicsSearchMode_comboBox;
        private CheckBox SaveToDatabase_checkBox;
        private ComboBox SelectedTable_comboBox;
        private Label SelectedTable_label;
        private DataGridView dataGridView1;
    }
}
