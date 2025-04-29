using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace course
{
    class ClientClass
    {
        public string ClientIPAddress;
        public TcpClient client;   

        public byte Temperature;
        public byte Humidity;
        public ushort Pressure;
        public ushort CO2;

        public byte MinTemperature;
        public byte MinHumidity;
        public ushort MinPressure;
        public ushort MinCO2;

        public byte MaxTemperature;
        public byte MaxHumidity;
        public ushort MaxPressure;
        public ushort MaxCO2;

        public ClientClass()
        {
            this.client = new TcpClient();
        }
    }
}
