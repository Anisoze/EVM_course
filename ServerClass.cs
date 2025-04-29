using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace course
{
    class ServerClass
    {
        public IPAddress ServerIP = Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[3];      // get computer IP from settings
        public int ServerPort = 49160;                                                                  // choose unused port (from 49152–65535)
        SqlConnection SqlConnection;
        SqlCommand sqlCommand;

        private List<ListenerTCP> ListOfListeners = new List<ListenerTCP>();
        private ListenerTCP CurListenerTCP;
        private List<ClientClass> ListOfClients = new List<ClientClass>();        // list for used Devices
        private ClientClass CurClient;

        private RichTextBox ServerLog;              // text window in Control Panel 

        private string ControlsPackage;             // 8 bit setting ON Parameters and selected Mode
        private byte[] StopData = { 0 };            // holds message to reset selected Mode
        private byte[] ControlData = new byte[1];   // holds Controls Package
        private byte[] ConfirmData = new byte[1];   // holds Confirmation of Receiving Data to the Device
        private byte[] SetTimeData = new byte[4];   // holds Time Interval
        private int SizeOfReceivedData = 0;         // size of Data to be Received in bytes
        private int totalBytesRead = 0;             // amount of bytes read from ReceivedData
        private byte[] ReceivedData;                // holds Data Received from the Device

        public NetworkStream ns;                                        // Connection with the Device
        private CancellationTokenSource cancellationTokenSource;        // time limit for sending and receiving controls
        public CancellationTokenSource StopTokenSource;                 // allow to interrupt work



        public ServerClass(RichTextBox Log, SqlConnection sql)
        {
            ServerLog = Log;
            SqlConnection = sql;
        }


        private void GetCancellationTokenSource(bool stop)                  // get new Time Limit
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(5000);
            if (stop) StopTokenSource = new CancellationTokenSource();      // and Stop Token if needed
        }


        public async void AddDevice(ComboBox SelectedDevice_ComboBox)       // Adds new Device
        {

            ServerLog.AppendText("Start looking for new Device\n");
            CurListenerTCP = new ListenerTCP(ServerIP, ServerPort);
            ListOfListeners.Add(CurListenerTCP);
            CurListenerTCP.listener.Start();

            CurClient = new ClientClass();
            ListOfClients.Add(CurClient);

            GetCancellationTokenSource(false);

            try
            {
                CurClient.client = await CurListenerTCP.listener.AcceptTcpClientAsync(cancellationTokenSource.Token);       // find willing Device
                CurClient.ClientIPAddress = CurClient.client.Client.RemoteEndPoint.ToString().Remove(12);                   // remove port from IP
                ServerLog.AppendText("Device with IP " + CurClient.ClientIPAddress + " was connected to the server\n");
                SelectedDevice_ComboBox.Items.Add(CurClient.ClientIPAddress);                                               // Add to the Control Panel
                CurListenerTCP.TargetIPAddress = CurClient.ClientIPAddress;
                CurListenerTCP.listener.Stop();
            }
            catch (OperationCanceledException)          // Time Limit expired
            {
                if (CurListenerTCP != null)
                {
                    CurListenerTCP.listener.Dispose();
                    ListOfListeners.Remove(CurListenerTCP);
                }
                if (CurClient != null)
                {
                    CurClient.client.Dispose();
                    ListOfClients.Remove(CurClient);
                }
                ServerLog.AppendText("No Devices could connect to the server\n");
            }
        }



        public void RemoveDevice(string ip, ComboBox SelectedDevice_ComboBox)       // Remove Device
        {
            CurListenerTCP = ListOfListeners.Find(L => L.TargetIPAddress == ip);    // get from saved Devices
            CurClient = ListOfClients.Find(C => C.ClientIPAddress == ip);

            SelectedDevice_ComboBox.Items.Remove(ip);                               // Remove from Control Panel
            ServerLog.AppendText("Devices with IP " + ip + " was removed\n");

            if (CurListenerTCP != null)
            {
                CurListenerTCP.listener.Dispose();
                ListOfListeners.Remove(CurListenerTCP);
            }
            if (CurClient != null)
            {
                CurClient.client.Dispose();
                ListOfClients.Remove(CurClient);
            }
            if (ns != null)        // if Connection with Device ON - Close
            {
                ns.Close();
                ns = null;
            }
        }

        private async Task<NetworkStream> RenewConnection()         // get new Connection with Device when reuse Mode
        {
            try
            {
                if (!CurClient.client.Connected)
                {
                    CurListenerTCP.listener.Start();
                    CurClient.client = await CurListenerTCP.listener.AcceptTcpClientAsync(cancellationTokenSource.Token);
                    CurListenerTCP.listener.Stop();
                }
                return CurClient.client.GetStream();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        private int BoolToInt(bool b)       // turns true to 1 and false to 0
        {
            return b ? 1 : 0;
        }



        private int GetSizeOfReceivedData(bool T, bool H, bool P, bool C)               // finds needed amount of bytes for Received Data
        {
            return (BoolToInt(T) + BoolToInt(H) + 2 * BoolToInt(P) + 2 * BoolToInt(C));
        }






        private byte GetControlPackage(int mode, bool T, bool H, bool P, bool C)        // Makes 8 bit of Controls to send ON Parameters and selected Mode
        {
            ControlsPackage = "";
            ControlsPackage += T ? "1" : "0";
            ControlsPackage += H ? "1" : "0";
            ControlsPackage += P ? "1" : "0";
            ControlsPackage += C ? "1" : "0";
            switch (mode)
            {
                case 0:
                    ControlsPackage += "00";    // Standart Monitoring
                    break;
                case 1:
                    ControlsPackage += "10";    // On Command
                    break;
                case 2:
                    ControlsPackage += "11";    // Dynamics Search
                    break;
                case 3:
                    ControlsPackage += "01";    // In-Depth Monitoring
                    break;
            }
            ControlsPackage += "00";                                         //padding
            SizeOfReceivedData = GetSizeOfReceivedData(T, H, P, C);          // find amount of bytes needed
            if (mode == 3)
            {
                SizeOfReceivedData *= 3;                                     // In-Depth Monitoring needes more bytes for Min and Max Data 
            }
            if (mode != 2) ReceivedData = new byte[SizeOfReceivedData];      //  Dynamics Search finds needed amount of bytes on each turn
            totalBytesRead = 0;

            return Convert.ToByte(ControlsPackage, 2);              // turn 8 bits to byte
        }




        private bool BeginConnect(string ip)                                          // checks for existence of the selected Device
        {
            CurListenerTCP = ListOfListeners.Find(L => L.TargetIPAddress == ip);
            CurClient = ListOfClients.Find(C => C.ClientIPAddress == ip);
            if (CurClient != null && CurListenerTCP != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        private async Task<bool> SendControls(int mode, byte[] Sensitivity, byte[] NumberOfMeasurements)     // sending Controls to the Device
        {
            ServerLog.AppendText("Sending Controls to the Device\n");
            cancellationTokenSource.CancelAfter(5000);
            await ns.WriteAsync(ControlData, 0, ControlData.Length, cancellationTokenSource.Token);     // send ON Parameters and selected Mode
            if (mode != 1)                                                                              // On Command Mode doesn't use Time Interval
            {
                cancellationTokenSource.CancelAfter(5000);
                await ns.WriteAsync(SetTimeData, 0, SetTimeData.Length, cancellationTokenSource.Token);     // send Time Interval
                if (mode == 2)                                                                              // only Dynamics Search uses Sensitivity
                {
                    cancellationTokenSource.CancelAfter(5000);
                    await ns.WriteAsync(Sensitivity, 0, Sensitivity.Length, cancellationTokenSource.Token);
                }
                if (mode == 2 || mode == 3)
                {
                    cancellationTokenSource.CancelAfter(5000);
                    await ns.WriteAsync(NumberOfMeasurements, 0, NumberOfMeasurements.Length, cancellationTokenSource.Token);   // send Number of Measurements
                }
            }
            cancellationTokenSource.CancelAfter(5000);
            ConfirmData[0] = 0;
            await ns.ReadAsync(ConfirmData, 0, ConfirmData.Length, cancellationTokenSource.Token);      // reading Confirmation
            return (ConfirmData[0] == 255);
        }



        private void SaveData(bool T, bool H, bool P, bool C)       // save data to Current Client
        {
            totalBytesRead = 0;                                     //reset amount of byted read
            if (T) CurClient.Temperature = ReceivedData[0];
            if (H) CurClient.Humidity = ReceivedData[BoolToInt(T)];
            if (P) CurClient.Pressure = (ushort)((ReceivedData[BoolToInt(T) + BoolToInt(H)]) << 8 | (ReceivedData[BoolToInt(T) + BoolToInt(H) + 1]));
            if (C) CurClient.CO2 = (ushort)((ReceivedData[BoolToInt(T) + BoolToInt(H) + 2 * BoolToInt(P)]) << 8 | (ReceivedData[BoolToInt(T) + BoolToInt(H) + 2 * BoolToInt(P) + 1]));
        }




        private async void ClearData(bool not_mode1, bool type)         // clear memory 
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;

            if (not_mode1)
            {
                if (type) await ns.WriteAsync(StopData, 0, StopData.Length);
                StopTokenSource.Dispose();
                StopTokenSource = null;
            }

            if (ns != null)
            {
                ns.Close();
                ns = null;
            }
            SizeOfReceivedData = 0;
            if (ReceivedData != null) ReceivedData = null;
        }



        private int SaveToStandartMonitoringOrOnCommandTable(bool T, bool H, bool P, bool C, bool type)        // adding Data to the Table
        {
            if (SqlConnection.State != System.Data.ConnectionState.Open)
            {
                SqlConnection.Open();
            }
            sqlCommand = new SqlCommand($"INSERT INTO [{(type ? "StandartMonitoring" : "OnCommand")}] (Temperature, Humidity, Pressure, CO2, Device) VALUES ('{(T ? CurClient.Temperature : DBNull.Value)}','{(H ? CurClient.Humidity : DBNull.Value)}', '{(P ? CurClient.Pressure : DBNull.Value)}', '{(C ? CurClient.CO2 : DBNull.Value)}', '{CurClient.ClientIPAddress}')", SqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }


        // Standart Monitoring Mode

        public async void StandartMonitoring(bool TempOn, bool HumOn, bool PressOn, bool CO2ON, uint Interval, string ip, bool ToTable)
        {
            ServerLog.AppendText("Standart Monitoring starts on Device with IP " + ip + "\n");
            if (BeginConnect(ip))
            {
                GetCancellationTokenSource(true);                                       // get new Time Limit and Stop Token
                ns = await RenewConnection();                                           // get Connection (renew if needed)
                ControlData[0] = GetControlPackage(0, TempOn, HumOn, PressOn, CO2ON);   // get first 8 bits of Controls to set ON Parameters and selected Mode
                SetTimeData = BitConverter.GetBytes(Interval);                          // get Time Interval

                try
                {
                    if (await SendControls(0, null, null)) ServerLog.AppendText("Controls were received by Device successfully\n");  // sending Controls to the Device
                    ServerLog.AppendText("Waiting for the Data...\n");

                    while (CurClient.client.Connected)              // main loop
                    {
                        if (totalBytesRead < SizeOfReceivedData)     // not all bytes are read
                        {
                            totalBytesRead += await ns.ReadAsync(ReceivedData, totalBytesRead, SizeOfReceivedData - totalBytesRead, StopTokenSource.Token);
                        }
                        else
                        {
                            SaveData(TempOn, HumOn, PressOn, CO2ON);      // save data to Current Client
                            ServerLog.AppendText("Received Data:\n" + (TempOn ? "Temperature = " + CurClient.Temperature.ToString() + "\n" : "") + (HumOn ? "Humidity = " + CurClient.Humidity.ToString() + "\n" : "") + (PressOn ? "Pressure = " + CurClient.Pressure.ToString() + "\n" : "") + (CO2ON ? "CO2 = " + CurClient.CO2.ToString() + "\n" : "") + "\n");
                            await ns.WriteAsync(ConfirmData, 0, ConfirmData.Length, StopTokenSource.Token);         // Confirm Receiving Data
                            if (ToTable) ServerLog.AppendText((SaveToStandartMonitoringOrOnCommandTable(TempOn, HumOn, PressOn, CO2ON, true) != 0) ? "Received Data was added to the Table successfully\n" : "Error, can't add Received Data to the Table\n");
                        }
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationTokenSource.Token)      // Time Limit has expired
                {
                    ServerLog.AppendText("Response time of Device has expired.\n\n");
                    ClearData(true, true);      // clear memory
                    return;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == StopTokenSource.Token)          // Mode was Stoped by the Stop Button
                {
                    ServerLog.AppendText("Stopping Standart Monitoring\n");
                    await ns.WriteAsync(StopData, 0, StopData.Length);          // reset Device
                    ConfirmData[0] = 0;
                    while (ConfirmData[0] != 255)       //read any leftover Data
                    {
                        await ns.ReadAsync(ConfirmData, 0, ConfirmData.Length);
                        if (ConfirmData[0] == 255) ServerLog.AppendText("Standart Monitoring was stopped successfilly\n\n");
                    }
                    ClearData(true, false);     // clear memory
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClearData(true, true);          // clear memory
                    return;
                }
            }
            else
            {
                ServerLog.AppendText("Device with IP " + ip + " wasn't found\n\n");
                return;
            }
        }






        // On Command Mode

        public async void OnCommand(bool TempOn, bool HumOn, bool PressOn, bool CO2ON, Button B, string ip, bool ToTable)
        {
            ServerLog.AppendText("Making mesurement request to Device with IP " + ip + "\n");
            if (BeginConnect(ip))
            {
                GetCancellationTokenSource(false);                                           // get new Time Limit
                ns = await RenewConnection();                                                // get Connection (renew if needed)
                ControlData[0] = GetControlPackage(1, TempOn, HumOn, PressOn, CO2ON);        // get Controls to set ON Parameters and selected Mode

                try
                {
                    if (await SendControls(1, null, null)) ServerLog.AppendText("Controlls were received by Device successfully\n"); // sending Controls to the Device
                    ServerLog.AppendText("Waiting for the Data...\n");
                    cancellationTokenSource.CancelAfter(5000);

                    while (CurClient.client.Connected && totalBytesRead < SizeOfReceivedData)        // not all bytes are read
                    {
                        totalBytesRead += await ns.ReadAsync(ReceivedData, totalBytesRead, SizeOfReceivedData - totalBytesRead, cancellationTokenSource.Token);
                    }
                    if (!CurClient.client.Connected) throw new Exception("Client is disconected");

                    SaveData(TempOn, HumOn, PressOn, CO2ON);    // save data to Current Client
                    ServerLog.AppendText("ReceivedData:\n" + (TempOn ? "Temperature = " + CurClient.Temperature.ToString() + "\n" : "") + (HumOn ? "Humidity = " + CurClient.Humidity.ToString() + "\n" : "") + (PressOn ? "Pressure = " + CurClient.Pressure.ToString() + "\n" : "") + (CO2ON ? "CO2 = " + CurClient.CO2.ToString() + "\n" : "") + "\n");
                    if (ToTable) ServerLog.AppendText((SaveToStandartMonitoringOrOnCommandTable(TempOn, HumOn, PressOn, CO2ON, false) != 0) ? "Received Data was added to the Table successfully\n" : "Error, can't add Received Data to the Table\n");

                    cancellationTokenSource.CancelAfter(5000);
                    await ns.WriteAsync(ConfirmData, 0, ConfirmData.Length, cancellationTokenSource.Token);     // Confirm Receiving Data

                    ServerLog.AppendText("Ending the request\n");
                    cancellationTokenSource.CancelAfter(5000);
                    await ns.WriteAsync(StopData, 0, StopData.Length, cancellationTokenSource.Token);           // reset Device
                    ConfirmData[0] = 0;
                    cancellationTokenSource.CancelAfter(5000);
                    await ns.ReadAsync(ConfirmData, 0, ConfirmData.Length, cancellationTokenSource.Token);
                    if (ConfirmData[0] == 255) ServerLog.AppendText("Request ended successfilly\n\n");
                    ClearData(false, false);        // clear memory
                    B.Enabled = true;               // enable Measure Button
                    return;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationTokenSource.Token)      // Time Limit has expired
                {
                    ServerLog.AppendText("Response time of Device has expired.\n\n");
                    ClearData(false, false);        // clear memory
                    B.Enabled = true;               // enable Measure Button
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClearData(false, false);        // clear memory
                    B.Enabled = true;               // enable Measure Button
                    return;
                }
            }
            else
            {
                ServerLog.AppendText("Device with IP " + ip + " wasn't found\n\n");
                return;
            }
        }






        private int SaveToDynamicsSearchTable(bool T, bool H, bool P, bool C, byte NumOfMeasurements, string Time, byte TempSensitivity, byte HumSensitivity, byte PressSensitivity, byte CO2Sensitivity)        // adding Data to the Table
        {
            if (SqlConnection.State != System.Data.ConnectionState.Open)
            {
                SqlConnection.Open();
            }
            sqlCommand = new SqlCommand($"INSERT INTO DynamicsSearch (Temperature, Humidity, Pressure, CO2, TotalTime, NumberOfMeasurements, TemperatureSensitivity, HumiditySensitivity, PressureSensitivity, CO2Sensitivity, Device) VALUES ('{(T ? CurClient.Temperature : DBNull.Value)}','{(H ? CurClient.Humidity : DBNull.Value)}', '{(P ? CurClient.Pressure : DBNull.Value)}', '{(C ? CurClient.CO2 : DBNull.Value)}', '{Time}', '{NumOfMeasurements}', '{TempSensitivity}','{HumSensitivity}', '{PressSensitivity}', '{CO2Sensitivity}', '{CurClient.ClientIPAddress}')", SqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }


        // Dynamics Search Mode

        public async void DynamicsSearch(bool TempOn, bool HumOn, bool PressOn, bool CO2ON, uint Interval, bool mode, byte NumOfMeasurements, byte TempSensitivity, byte HumSensitivity, byte PressSensitivity, byte CO2Sensitivity, string ip, bool ToTable)
        {
            ServerLog.AppendText("Dynamics Search starts on Device with IP " + ip + "\n");
            if (BeginConnect(ip))
            {
                GetCancellationTokenSource(true);                                                        // get new Time Limit and Stop Token
                ns = await RenewConnection();                                                            // get Connection (renew if needed)
                ControlData[0] = GetControlPackage(2, TempOn, HumOn, PressOn, CO2ON);                    // get first 8 bits of Controls to set ON Parameters and selected Mode
                SetTimeData = BitConverter.GetBytes(Interval);                                           // get Time Interval

                int NumberOfParams = BoolToInt(TempOn) + BoolToInt(HumOn) + BoolToInt(PressOn) + BoolToInt(CO2ON);      // get number of Parameters
                byte[] SetSensitivity = new byte[NumberOfParams];                                                       // create array for Sensitivity Data
                if (TempOn) SetSensitivity[0] = TempSensitivity;
                if (HumOn) SetSensitivity[BoolToInt(TempOn)] = HumSensitivity;                                          // gather Sensitivity Data
                if (PressOn) SetSensitivity[BoolToInt(TempOn) + BoolToInt(HumOn)] = PressSensitivity;
                if (CO2ON) SetSensitivity[BoolToInt(TempOn) + BoolToInt(HumOn) + BoolToInt(PressOn)] = CO2Sensitivity;

                if (mode) NumOfMeasurements = 1;                                        // set Number of Measurements to 1 if in Comparison with the Last One Mode
                byte[] SetNumberOfMeasurements = { NumOfMeasurements };                 // create array for Number of Measurements Data

                bool NewTemp = true;
                bool NewHum = true;
                bool NewPress = true;                   // create bools that tell if new Dynamics are found
                bool NewCO2 = true;
                byte[] NewDataByte = new byte[1];       // create array for byte made of those bools
                bool ReadingNewDataByte = true;         // create flag for checking this array

                try
                {
                    // sending Controls to the Device
                    if (await SendControls(2, SetSensitivity, SetNumberOfMeasurements)) ServerLog.AppendText("Controlls were received by Device successfully\n");
                    ServerLog.AppendText("Waiting for the Data...\n");

                    while (CurClient.client.Connected)      // main loop
                    {
                        if (ReadingNewDataByte)             // checking for new Dynamics
                        {
                            await ns.ReadAsync(NewDataByte, 0, NewDataByte.Length, StopTokenSource.Token);
                            NewTemp = ((int)NewDataByte[0] & 8) == 8;
                            NewHum = ((int)NewDataByte[0] & 4) == 4;        // make bools from byte
                            NewPress = ((int)NewDataByte[0] & 2) == 2;
                            NewCO2 = ((int)NewDataByte[0] & 1) == 1;
                            ReceivedData = null;                                                                // reset Received Data
                            SizeOfReceivedData = GetSizeOfReceivedData(NewTemp, NewHum, NewPress, NewCO2);      // count new Size of Received Data
                            if (SizeOfReceivedData != 0) ReceivedData = new byte[SizeOfReceivedData];           // create new array for Received Data in case of new Dynamics
                            ReadingNewDataByte = false;                                                         // lower the flag
                        }

                        if (totalBytesRead < SizeOfReceivedData)    // not all bytes are read
                        {
                            totalBytesRead += await ns.ReadAsync(ReceivedData, totalBytesRead, SizeOfReceivedData - totalBytesRead, StopTokenSource.Token);
                        }
                        else
                        {
                            ReadingNewDataByte = true;      // rise flag           
                            if (SizeOfReceivedData != 0)    // if case of new Dynamics
                            {
                                SaveData(TempOn && NewTemp, HumOn && NewHum, PressOn && NewPress, CO2ON && NewCO2);     // save data to Current Client
                                ServerLog.AppendText("Received Data:\n" + ((TempOn && NewTemp) ? "Temperature = " + CurClient.Temperature.ToString() + "\n" : "") + ((HumOn && NewHum) ? "Humidity = " + CurClient.Humidity.ToString() + "\n" : "") + ((PressOn && NewPress) ? "Pressure = " + CurClient.Pressure.ToString() + "\n" : "") + ((CO2ON && NewCO2) ? "CO2 = " + CurClient.CO2.ToString() + "\n" : "") + "\n");
                                if (ToTable) ServerLog.AppendText((SaveToDynamicsSearchTable(TempOn && NewTemp, HumOn && NewHum, PressOn && NewPress, CO2ON && NewCO2, NumOfMeasurements, (Interval * NumOfMeasurements).ToString(), TempSensitivity, HumSensitivity, PressSensitivity, CO2Sensitivity) != 0) ? "Received Data was added to the Table successfully\n" : "Error, can't add Received Data to the Table\n");
                                await ns.WriteAsync(ConfirmData, 0, ConfirmData.Length, StopTokenSource.Token);     // Confirm Receiving Data
                            }
                            else ServerLog.AppendText("Received Data:\nNo new Dynamics found\n\n");
                        }
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationTokenSource.Token)      // Time Limit has expired
                {
                    ServerLog.AppendText("Response time of Device has expired.\n\n");
                    ClearData(true, true);          // clear memory
                    return;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == StopTokenSource.Token)      // Mode was Stoped by the Stop Button
                {
                    ServerLog.AppendText("Stopping Dynamics Search\n");
                    await ns.WriteAsync(StopData, 0, StopData.Length);      // reset Device
                    ConfirmData[0] = 0;
                    while (ConfirmData[0] != 255)   //read any leftover Data
                    {
                        await ns.ReadAsync(ConfirmData, 0, ConfirmData.Length);
                        if (ConfirmData[0] == 255) ServerLog.AppendText("Dynamics Search was stopped successfilly\n\n");
                    }
                    ClearData(true, false);         // clear memory
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClearData(true, true);          // clear memory
                    return;
                }
            }
            else
            {
                ServerLog.AppendText("Device with IP " + ip + " wasn't found\n\n");
                return;
            }
        }




        private int SaveToInDepthMonitoringTable(bool T, bool H, bool P, bool C, byte NumOfMeasurements, string Time)
        {
            if (SqlConnection.State != System.Data.ConnectionState.Open)
            {
                SqlConnection.Open();
            }
            sqlCommand = new SqlCommand($"INSERT INTO InDepthMonitoring (Temperature, MinTemperature, MaxTemperature, Humidity, MinHumidity, MaxHumidity, Pressure, MinPressure, MaxPressure, CO2, MinCO2, MaxCO2, TotalTime, NumberOfMeasurements, Device) VALUES ('{(T ? CurClient.Temperature : DBNull.Value)}', '{(T ? CurClient.MinTemperature : DBNull.Value)}', '{(T ? CurClient.MaxTemperature : DBNull.Value)}','{(H ? CurClient.Humidity : DBNull.Value)}', '{(H ? CurClient.MinHumidity : DBNull.Value)}', '{(H ? CurClient.MaxHumidity : DBNull.Value)}', '{(P ? CurClient.Pressure : DBNull.Value)}', '{(P ? CurClient.MinPressure : DBNull.Value)}', '{(P ? CurClient.MaxPressure : DBNull.Value)}', '{(C ? CurClient.CO2 : DBNull.Value)}', '{(C ? CurClient.MinCO2 : DBNull.Value)}', '{(C ? CurClient.MaxCO2 : DBNull.Value)}', '{Time}', '{NumOfMeasurements}', '{CurClient.ClientIPAddress}')", SqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }




        // In-Depth Monitoring Mode

        public async void InDepthMonitoring(bool TempOn, bool HumOn, bool PressOn, bool CO2ON, uint Interval, byte NumOfMeasurements, string ip, bool ToTable)
        {
            ServerLog.AppendText("In-Depth Monitoring starts on Device with IP " + ip + "\n");
            if (BeginConnect(ip))
            {
                GetCancellationTokenSource(true);                                               // get new Time Limit and Stop Token
                ns = await RenewConnection();                                                   // get Connection (renew if needed)
                ControlData[0] = GetControlPackage(3, TempOn, HumOn, PressOn, CO2ON);           // get first 8 bits of Controls to set ON Parameters and selected Mode
                SetTimeData = BitConverter.GetBytes(Interval);                                  // get Time Interval
                byte[] SetNumberOfMeasurements = { NumOfMeasurements };                         // create array for Number of Measurements Data

                try
                {
                    // sending Controls to the Device
                    if (await SendControls(3, null, SetNumberOfMeasurements)) ServerLog.AppendText("Controlls were received by Device successfully\n"); 
                    ServerLog.AppendText("Waiting for the Data...\n");

                    while (CurClient.client.Connected)                   // main loop
                    {
                        if (totalBytesRead < SizeOfReceivedData)         // not all bytes are read
                        {
                            totalBytesRead += await ns.ReadAsync(ReceivedData, totalBytesRead, SizeOfReceivedData - totalBytesRead, StopTokenSource.Token);
                        }
                        else
                        {
                            totalBytesRead = 0;
                            if (TempOn)
                            {
                                CurClient.Temperature = ReceivedData[0];
                                CurClient.MinTemperature = ReceivedData[1];             // read the Data
                                CurClient.MaxTemperature = ReceivedData[2];
                            }

                            if (HumOn)
                            {
                                CurClient.Humidity = ReceivedData[3*BoolToInt(TempOn)];
                                CurClient.MinHumidity = ReceivedData[3 * BoolToInt(TempOn)+1];
                                CurClient.MaxHumidity = ReceivedData[3 * BoolToInt(TempOn)+2];
                            }   
                            if (PressOn)
                            {
                                CurClient.Pressure = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn)]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 1]));
                                CurClient.MinPressure = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 2]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 3]));
                                CurClient.MaxPressure = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 4]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 5]));
                            }
                            if (CO2ON)
                            {
                                CurClient.CO2 = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn)]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn) + 1]));
                                CurClient.MinCO2 = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn) + 2]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn) + 3]));
                                CurClient.MaxCO2 = (ushort)((ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn) + 4]) << 8 | (ReceivedData[3 * BoolToInt(TempOn) + 3 * BoolToInt(HumOn) + 6 * BoolToInt(PressOn) + 5]));
                            }
                            ServerLog.AppendText("Received Data:\n" + (TempOn ? "Average Temperature = " + CurClient.Temperature.ToString() + "\nMin Temperature = " + CurClient.MinTemperature.ToString() + "\nMax Temperature = " + CurClient.MaxTemperature.ToString() +  "\n" : "") + (HumOn ? "Average Humidity = " + CurClient.Humidity.ToString() + "\nMin Humidity = " + CurClient.MinHumidity.ToString() + "\nMax Humidity = " + CurClient.MaxHumidity.ToString() + "\n" : "") + (PressOn ? "Average Pressure = " + CurClient.Pressure.ToString() + "\nMin Pressure = " + CurClient.MinPressure.ToString() + "\nMax Pressure = " + CurClient.MaxPressure.ToString() + "\n" : "") + (CO2ON ? "Average CO2 = " + CurClient.CO2.ToString() + "\nMin CO2 = " + CurClient.MinCO2.ToString() + "\nMax CO2 = " + CurClient.MaxCO2.ToString() + "\n" : "") + "\n");
                            if (ToTable) ServerLog.AppendText((SaveToInDepthMonitoringTable(TempOn, HumOn, PressOn, CO2ON, NumOfMeasurements, (Interval * NumOfMeasurements).ToString()) != 0) ? "Received Data was added to the Table successfully\n" : "Error, can't add Received Data to the Table\n");
                            await ns.WriteAsync(ConfirmData, 0, ConfirmData.Length, StopTokenSource.Token);     // Confirm Receiving Data
                        }
                    }
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cancellationTokenSource.Token)      // Time Limit has expired
                {
                    ServerLog.AppendText("Response time of Device has expired.\n\n");
                    ClearData(true, true);          // clear memory
                    return;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == StopTokenSource.Token)      // Mode was Stoped by the Stop Button
                {
                    ServerLog.AppendText("Stopping In-Depth Monitoring\n");
                    await ns.WriteAsync(StopData, 0, StopData.Length);                  // reset Device
                    ConfirmData[0] = 0;
                    while (ConfirmData[0] != 255)       //read any leftover Data
                    {
                        await ns.ReadAsync(ConfirmData, 0, ConfirmData.Length);
                        if (ConfirmData[0] == 255) ServerLog.AppendText("In-Depth Monitoring was stopped successfilly\n\n");
                    }
                    ClearData(true, false);         // clear memory
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClearData(true, true);          // clear memory
                    return;
                }
            }
            else
            {
                ServerLog.AppendText("Device with IP " + ip + " wasn't found\n\n");
                return;
            }
        }
    }
}
