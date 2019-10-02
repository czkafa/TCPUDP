using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TCPUDP.ViewModel
{
    class MainWindowViewModel
    {
        public String LocalIP
        {
            get; set;
        }

        public ViewModelBase TCPSend
        {
            get; set;
        }
        public ViewModelBase TCPReceive
        {
            get; set;
        }
        public ViewModelBase UDPSend
        {
            get; set;
        }
        public ViewModelBase UDPReceive
        {
            get; set;
        }
        public MainWindowViewModel()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            LocalIP = "IP :" + myIP;
            TCPSend = new TCPIPViewModel(myIP, 13000, myIP, 14000);
            TCPReceive = new TCPIPViewModel(myIP, 14000, myIP, 13000);
            UDPSend = new UDPViewModel(myIP, 11000, myIP, 10000);
            UDPReceive = new UDPViewModel(myIP, 10000, myIP, 11000);
            //TCPSend = new TCPIPViewModel("127.0.0.1", 13000, "127.0.0.1", 14000);
            //TCPReceive = new TCPIPViewModel("127.0.0.1", 14000, "127.0.0.1", 13000);
            //UDPSend = new UDPViewModel("127.0.0.1", 11000, "127.0.0.1", 10000);
            //UDPReceive = new UDPViewModel("127.0.0.1", 10000, "127.0.0.1", 11000);
        }
    }
}
