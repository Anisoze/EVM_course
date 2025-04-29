using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace course
{
    class ListenerTCP
    {
        public IPAddress ListenerIPAddress; // Server IP
        public TcpListener listener;
        public string TargetIPAddress;      // Client IP

        public ListenerTCP(IPAddress ip, int port)
        {
            ListenerIPAddress = ip;
            this.listener = new TcpListener(ip, port);
        }
    }
}
