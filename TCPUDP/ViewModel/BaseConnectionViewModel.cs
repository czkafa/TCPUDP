using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPUDP.ViewModel
{
    public abstract class BaseConnectionViewModel : ViewModelBase
    {
        private string ipAddress;

        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (System.Net.IPAddress.TryParse(value, out System.Net.IPAddress ip))
                {
                    SetProperty(ref ipAddress, value, "IPAddress");
                }
            }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set
            {
                SetProperty(ref port, value, "Port");
            }
        }
        private string myIPAddress;

        public string MyIPAddress
        {
            get { return ipAddress; }
            set
            {
                if (System.Net.IPAddress.TryParse(value, out System.Net.IPAddress ip))
                {
                    SetProperty(ref myIPAddress, value, "MyIPAddress");
                }
            }
        }

        private int myPort;

        public int MyPort
        {
            get { return myPort; }
            set
            {
                SetProperty(ref myPort, value, "MyPort");
            }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                SetProperty(ref isConnected, value, "IsConnected");
                OnPropertyChanged("ButtonConnect");
            }
        }
        private bool isListening;

        public bool IsListening
        {
            get { return isListening; }
            set
            {
                SetProperty(ref isListening, value, "IsListening");
                OnPropertyChanged("ButtonListen");
            }
        }

        private string messageReceived;

        public string MessageReceived
        {
            get { return messageReceived; }
            set
            {
                SetProperty(ref messageReceived, value, "MessageReceived");

            }
        }
        private string messageToSend;

        public string MessageToSend
        {
            get { return messageToSend; }
            set
            {
                SetProperty(ref messageToSend, value, "MessageToSend");
            }
        }
        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                SetProperty(ref errorMessage, value, "ErrorMessage");
            }
        }

        protected bool CanListen(object argument)
        {
            return !IsConnected;
        }
        protected bool CanConnect(object argument)
        {
            return !IsListening;
        }
        protected Task currentTask;
        protected CancellationToken token;
        protected CancellationTokenSource cts;

        public RelayCommand ButtonConnect
        {
            get; set;
        }
        public RelayCommand ButtonListen
        {
            get; set;
        }
    }
}
