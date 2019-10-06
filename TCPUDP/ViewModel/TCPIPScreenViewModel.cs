using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPUDP.ViewModel
{
    public class TCPIPScreenViewModel : ViewModelBase
    {
        public String LocalIP
        {
            get; set;
        }

        public TCPIPViewModel TCPSend
        {
            get; set;
        }

        public TCPIPViewModel TCPReceive
        {
            get; set;
        }
        public TCPIPScreenViewModel()
        {
            LocalIP = "IP :";
            string myIP = "192.168.0.1";
            TCPSend = new TCPIPViewModel(myIP, 13000, myIP, 14000);
            TCPReceive = new TCPIPViewModel(myIP, 13000, myIP, 14000);
        }
        public TCPIPScreenViewModel(string myIP, int myPort, int port)
        {
            LocalIP = "IP :" + myIP;
            TCPSend = new TCPIPViewModel(myIP, myPort, myIP, port);
            TCPReceive = new TCPIPViewModel(myIP, port, myIP, myPort);
        }
    }
}
