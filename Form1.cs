using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Data.Common;






namespace course
{
    public partial class Form1 : Form
    {
        private ServerClass server;
        private SqlConnection sqlConnection;
        private SqlDataAdapter DataAdapter;
        private DataSet db;
        private int TimeInterval_mode0 = 5000;      // default Time Interval for Standart Monitoring
        private int TimeInterval_mode2 = 5000;      // default Time Interval for Dynamics Search
        private int TimeInterval_mode3 = 3000;      // default Time Interval for In-Depth Monitoring
        private int[] NumberOfMeasurements = { 8, 8 };

        private int[,] SensitivityLevels = { { 1, 1, 1, 5 }, { 1, 3, 1, 15 } };   // array of default values for Sensitivity in Dynamics Search
        private bool[,] CheckBoxArray = new bool[4, 4];         // array of ON Parameters for each Mode
        private string[] TableArrary = { "StandartMonitoring", "OnCommand", "DynamicsSearch", "InDepthMonitoring" };


        public Form1()
        {
            InitializeComponent();
        }


        private void LoadTable()
        {
            DataAdapter = new SqlDataAdapter($"SELECT * FROM [{TableArrary[SelectedTable_comboBox.SelectedIndex]}]", sqlConnection);       // load selected Table
            db = new DataSet();
            DataAdapter.Fill(db);
            dataGridView1.DataSource = db.Tables[0];      // fill dataGridView with data from Table
            dataGridView1.AutoResizeColumns();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);   // connecting to Database
            sqlConnection.Open();
            if (sqlConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Подключение с базой данных не установлено");   // connection error
            }
            SelectedTable_comboBox.SelectedIndex = 0;                                                                                      // default Table
            LoadTable();

            server = new ServerClass(ServerLog_richTextBox, sqlConnection);

            SelectedDevice_comboBox.SelectedIndex = 0;                                   // default Device is None
            DeviceMode_comboBox.SelectedIndex = 0;                                       // default Mode is Standart Monitoring
            DynamicsSearchMode_comboBox.SelectedIndex = 0;                               // default Mode for Dynamics Search is Comparison with the Last One

            TemperatureSensitivity_textBox.Text = SensitivityLevels[0, 0].ToString();
            HumiditySensitivity_textBox.Text = SensitivityLevels[0, 1].ToString();       // setting default values for Sensitivity in Dynamics Search
            PressureSensitivity_textBox.Text = SensitivityLevels[0, 2].ToString();
            CO2Sensitivity_textBox.Text = SensitivityLevels[0, 3].ToString();

            NumberOfMeasurements_textBox.Text = NumberOfMeasurements[0].ToString();  // default Number of Measurements for In-Depth Monitoring and Comparison with the Average in Dynamics Search

            Temperature_checkBox.Checked = true;
            Humidity_checkBox.Checked = true;               // by default all Parameters are ON for all Modes
            Pressure_checkBox.Checked = true;
            CO2_checkBox.Checked = true;

            for (int i = 0; i < 4; i++)                     // saving ON Parameters in array
            {
                for (int j = 0; j < 4; j++)
                {
                    CheckBoxArray[i, j] = true;
                }
            }

            StopLog_button.Enabled = false;
        }




        private void AddDevice_button_Click(object sender, EventArgs e)              //Add Device
        {
            server.AddDevice(SelectedDevice_comboBox);
        }

        private void RemoveDevice_button_Click(object sender, EventArgs e)          //Remove Device
        {
            if (SelectedDevice_comboBox.SelectedIndex != 0)
            {
                server.RemoveDevice(SelectedDevice_comboBox.Text, SelectedDevice_comboBox);
                SelectedDevice_comboBox.SelectedIndex = 0;
            }
            else
            {
                ServerLog_richTextBox.AppendText("Device isn't selected\n\n");      // Device is None
            }
        }


        private void StartLog_button_Click(object sender, EventArgs e)           //Start Log
        {
            if (SelectedDevice_comboBox.SelectedIndex == 0)
            {
                ServerLog_richTextBox.AppendText("Device isn't selected\n\n");  // Device is None

            }
            else
            {
                if (Temperature_checkBox.Checked || Humidity_checkBox.Checked || Pressure_checkBox.Checked || CO2_checkBox.Checked)     // at least one of Parameters is ON
                {
                    TemperatureSensitivity_textBox.Enabled = false;
                    HumiditySensitivity_textBox.Enabled = false;
                    PressureSensitivity_textBox.Enabled = false;
                    CO2Sensitivity_textBox.Enabled = false;
                    TimeInterval_textBox.Enabled = false;
                    NumberOfMeasurements_textBox.Enabled = false;
                    Temperature_checkBox.Enabled = false;
                    Humidity_checkBox.Enabled = false;
                    Pressure_checkBox.Enabled = false;                  // while Log ON changes are prohibited
                    CO2_checkBox.Enabled = false;
                    DeviceMode_comboBox.Enabled = false;
                    SelectedDevice_comboBox.Enabled = false;
                    StartLog_button.Enabled = false;
                    RemoveDevice_button.Enabled = false;
                    StopLog_button.Enabled = true;
                    DynamicsSearchMode_comboBox.Enabled = false;

                    switch (DeviceMode_comboBox.SelectedIndex)
                    {
                        case 0:
                            server.StandartMonitoring(Temperature_checkBox.Checked, Humidity_checkBox.Checked, Pressure_checkBox.Checked, CO2_checkBox.Checked, uint.Parse(TimeInterval_textBox.Text), SelectedDevice_comboBox.Text, SaveToDatabase_checkBox.Checked);
                            break;
                        case 2:
                            server.DynamicsSearch(Temperature_checkBox.Checked, Humidity_checkBox.Checked, Pressure_checkBox.Checked, CO2_checkBox.Checked, uint.Parse(TimeInterval_textBox.Text), DynamicsSearchMode_comboBox.SelectedIndex == 0 ? true : false, byte.Parse(NumberOfMeasurements_textBox.Text), byte.Parse(TemperatureSensitivity_textBox.Text), byte.Parse(HumiditySensitivity_textBox.Text), byte.Parse(PressureSensitivity_textBox.Text), byte.Parse(CO2Sensitivity_textBox.Text), SelectedDevice_comboBox.Text, SaveToDatabase_checkBox.Checked);
                            break;
                        case 3:
                            server.InDepthMonitoring(Temperature_checkBox.Checked, Humidity_checkBox.Checked, Pressure_checkBox.Checked, CO2_checkBox.Checked, uint.Parse(TimeInterval_textBox.Text), byte.Parse(NumberOfMeasurements_textBox.Text), SelectedDevice_comboBox.Text, SaveToDatabase_checkBox.Checked);
                            break;
                    }
                }
                else
                {
                    ServerLog_richTextBox.AppendText("No Parameters are set as ON\n\n");
                }
            }




        }


        private void StopLog_button_Click(object sender, EventArgs e)       //Stop Log
        {
            server.StopTokenSource.Cancel();
            Temperature_checkBox.Enabled = true;
            Humidity_checkBox.Enabled = true;
            Pressure_checkBox.Enabled = true;
            CO2_checkBox.Enabled = true;
            DeviceMode_comboBox.Enabled = true;             // after Log Stops changes are allowed
            SelectedDevice_comboBox.Enabled = true;
            StartLog_button.Enabled = true;
            RemoveDevice_button.Enabled = true;
            StopLog_button.Enabled = false;
            ChangeDeviceModeSettings();                    // allow controls only for selected Mode
        }

        private void DeviceMode_comboBox_SelectedIndexChanged(object sender, EventArgs e)       // allow controls only for selected Mode
        {
            ChangeDeviceModeSettings();
        }

        private void ChangeDeviceModeSettings()                                                 // allow controls only for selected Mode
        {
            Temperature_checkBox.Checked = CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 0];
            Humidity_checkBox.Checked = CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 1];
            Pressure_checkBox.Checked = CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 2];        // get ON Parameters from array
            CO2_checkBox.Checked = CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 3];

            if (DeviceMode_comboBox.SelectedIndex == 1)
            {
                Measure_button.Enabled = true;
                TimeInterval_textBox.Enabled = false;           // On Command doesn't use Time Interval
                StartLog_button.Enabled = false;
            }
            else
            {
                Measure_button.Enabled = false;
                TimeInterval_textBox.Enabled = true;
                StartLog_button.Enabled = true;
            }

            if (DeviceMode_comboBox.SelectedIndex == 2)
            {
                TemperatureSensitivity_textBox.Enabled = true;
                HumiditySensitivity_textBox.Enabled = true;             // Sensitivity used only by Dynamics Search
                PressureSensitivity_textBox.Enabled = true;
                CO2Sensitivity_textBox.Enabled = true;
            }
            else
            {
                TemperatureSensitivity_textBox.Enabled = false;
                HumiditySensitivity_textBox.Enabled = false;
                PressureSensitivity_textBox.Enabled = false;
                CO2Sensitivity_textBox.Enabled = false;
            }

            NumberOfMeasurements_textBox.Enabled = (DeviceMode_comboBox.SelectedIndex == 3 || (DeviceMode_comboBox.SelectedIndex == 2 && DynamicsSearchMode_comboBox.SelectedIndex == 1)) ? true : false;
            DynamicsSearchMode_comboBox.Enabled = (DeviceMode_comboBox.SelectedIndex == 2) ? true : false;


            switch (DeviceMode_comboBox.SelectedIndex)
            {
                case 0:
                    TimeInterval_textBox.Text = TimeInterval_mode0.ToString();
                    break;
                case 2:
                    TimeInterval_textBox.Text = TimeInterval_mode2.ToString();      // get saved Time Intervals
                    NumberOfMeasurements_textBox.Text = NumberOfMeasurements[1].ToString();
                    break;
                case 3:
                    TimeInterval_textBox.Text = TimeInterval_mode3.ToString();
                    NumberOfMeasurements_textBox.Text = NumberOfMeasurements[0].ToString();
                    break;
            }
        }



        private void TimeInterval_textBox_TextChanged(object sender, EventArgs e)           // change in Time Interval
        {
            try
            {
                if (int.Parse(TimeInterval_textBox.Text) <= 0)
                {
                    MessageBox.Show("Time Interval must be positive");
                    switch (DeviceMode_comboBox.SelectedIndex)
                    {
                        case 0:
                            TimeInterval_textBox.Text = TimeInterval_mode0.ToString();
                            break;
                        case 2:
                            TimeInterval_textBox.Text = TimeInterval_mode2.ToString();      // restore to saved value
                            break;
                        case 3:
                            TimeInterval_textBox.Text = TimeInterval_mode3.ToString();
                            break;
                    }
                }
                else
                {
                    switch (DeviceMode_comboBox.SelectedIndex)
                    {
                        case 0:
                            TimeInterval_mode0 = int.Parse(TimeInterval_textBox.Text);
                            break;
                        case 2:
                            TimeInterval_mode2 = int.Parse(TimeInterval_textBox.Text);         // save new value
                            break;
                        case 3:
                            TimeInterval_mode3 = int.Parse(TimeInterval_textBox.Text);
                            break;
                    }
                }
            }
            catch { }


        }


        // change in ON Parameters

        private void Temperature_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 0] = Temperature_checkBox.Checked;        // save new 
        }

        private void Humidity_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 1] = Humidity_checkBox.Checked;
        }

        private void Pressure_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 2] = Pressure_checkBox.Checked;
        }

        private void CO2_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxArray[DeviceMode_comboBox.SelectedIndex, 3] = CO2_checkBox.Checked;
        }

        private void Measure_button_Click(object sender, EventArgs e)
        {
            Measure_button.Enabled = false;
            server.OnCommand(Temperature_checkBox.Checked, Humidity_checkBox.Checked, Pressure_checkBox.Checked, CO2_checkBox.Checked, Measure_button, SelectedDevice_comboBox.Text, SaveToDatabase_checkBox.Checked);
        }



        // change in Number of Measurements

        private void NumberOfMeasurements_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(NumberOfMeasurements_textBox.Text) <= 0)
                {
                    MessageBox.Show("Number of Measurements must be positive");
                    NumberOfMeasurements_textBox.Text = NumberOfMeasurements[(DeviceMode_comboBox.SelectedIndex == 3) ? 0 : 1].ToString();         // restore to saved value
                }
                else NumberOfMeasurements[(DeviceMode_comboBox.SelectedIndex == 3) ? 0 : 1] = int.Parse(NumberOfMeasurements_textBox.Text); // save new value
            }
            catch { }

        }



        // change in Sensitivity

        private void TemperatureSensitivity_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(TemperatureSensitivity_textBox.Text) <= 0)
                {
                    MessageBox.Show("Temperature Sensitivity must be positive");
                    TemperatureSensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 0].ToString();    // restore to saved value
                }
                else SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 0] = int.Parse(TemperatureSensitivity_textBox.Text);
            }
            catch { }
        }

        private void HumiditySensitivity_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(HumiditySensitivity_textBox.Text) <= 0)
                {
                    MessageBox.Show("Humidity Sensitivity must be positive");
                    HumiditySensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 1].ToString();
                }
                else SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 1] = int.Parse(HumiditySensitivity_textBox.Text);
            }
            catch { }
        }

        private void PressureSensitivity_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(PressureSensitivity_textBox.Text) <= 0)
                {
                    MessageBox.Show("Pressure Sensitivity must be positive");
                    PressureSensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 2].ToString();
                }
                else SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 2] = int.Parse(PressureSensitivity_textBox.Text);
            }
            catch { }
        }

        private void CO2Sensitivity_textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(CO2Sensitivity_textBox.Text) <= 0)
                {
                    MessageBox.Show("CO2 Sensitivity must be positive");
                    CO2Sensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 3].ToString();
                }
                else SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 3] = int.Parse(CO2Sensitivity_textBox.Text);
            }
            catch { }
        }


        // change in Mode of Dynamics Search

        private void DynamicsSearchMode_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NumberOfMeasurements_textBox.Enabled = (DynamicsSearchMode_comboBox.SelectedIndex == 0) ? false : true;
            TemperatureSensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 0].ToString();
            HumiditySensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 1].ToString();
            PressureSensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 2].ToString();
            CO2Sensitivity_textBox.Text = SensitivityLevels[DynamicsSearchMode_comboBox.SelectedIndex, 3].ToString();
        }

        private void SelectedTable_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTable();
        }
    }
}
