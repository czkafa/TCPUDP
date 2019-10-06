using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPUDP.ViewModel
{
    public class UDPScreenViewModel : ViewModelBase
    {
        public String LocalIP
        {
            get; set;
        }

        public UDPViewModel UDPSend
        {
            get; set;
        }
        UDPViewModel uDPViewModel;
        public UDPViewModel UDPReceive
        {
            get; set;
        }
        public UDPScreenViewModel()
        {
            LocalIP = "IP :";
            string myIP = "192.168.0.1";
            UDPSend = new UDPViewModel(myIP, 13000, myIP, 14000);
            UDPReceive = new UDPViewModel(myIP, 13000, myIP, 14000);
        }
        public UDPScreenViewModel(string myIP, int myPort, int port)
        {
            LocalIP = "IP :" + myIP;
            UDPSend = new UDPViewModel(myIP, myPort, myIP, port);
            UDPReceive = new UDPViewModel(myIP, port, myIP, myPort);
        }
    }
}
