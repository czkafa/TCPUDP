using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCPUDP.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public String MyIP
        {
            get; set;
        }
        public String LocalIP
        {
            get; set;
        }

        //public TCPIPScreenViewModel TCPIPScreenViewModel
        //{
        //    get; set;
        //}

        //public UDPScreenViewModel UDPScreenViewModel
        //{
        //    get; set;
        //}

        public static readonly DependencyProperty CurrentPageViewModelProperty =
DependencyProperty.Register("CurrentPageViewModel", typeof(DependencyObject), typeof(ViewModelBase), new PropertyMetadata());

        public DependencyObject CurrentPageViewModel
        {
            get
            {
                return (DependencyObject)GetValue(CurrentPageViewModelProperty);
            }
            private set
            {
                SetValue(CurrentPageViewModelProperty, value);
            }
        }

        public RelayCommand TCPCommand { get; set; }
        public RelayCommand UDPCommand { get; set; }

        public void ExecuteTCPIP(object parameter)
        {
            if (DictionaryViewModel.TryGetValue("TCP", out ViewModelBase viewModel))
            {
                CurrentPageViewModel = viewModel;
            }
        }
        public void ExecuteUDP(object parameter)
        {
            if (DictionaryViewModel.TryGetValue("UDP", out ViewModelBase viewModel))
            {
                CurrentPageViewModel = viewModel;
            }
        }

        Dictionary<string, ViewModelBase> DictionaryViewModel
        {
            get; set;
        }

        public MainWindowViewModel()
        {
            string hostName = Dns.GetHostName();
            MyIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            LocalIP = "IP :" + MyIP;

            DictionaryViewModel = new Dictionary<string, ViewModelBase>()
            {
                { "TCP", new TCPIPScreenViewModel(MyIP, 13000, 14000)},
                { "UDP", new UDPScreenViewModel(MyIP, 13000, 14000)},
            };




            //TCPIPScreenViewModel = new TCPIPScreenViewModel(myIP, 13000, 14000);
            //UDPScreenViewModel = new UDPScreenViewModel(myIP, 13000, 14000);
            ExecuteTCPIP(null);
            //CurrentPageViewModel = new TCPIPScreenViewModel(MyIP, 13000, 14000);
            TCPCommand = new RelayCommand(ExecuteTCPIP);
            UDPCommand = new RelayCommand(ExecuteUDP);









            //TCPSend = new TCPIPViewModel(myIP, 13000, myIP, 14000);
            //TCPReceive = new TCPIPViewModel(myIP, 14000, myIP, 13000);
            //UDPSend = new UDPViewModel(myIP, 11000, myIP, 10000);
            //UDPReceive = new UDPViewModel(myIP, 10000, myIP, 11000);


            //TCPSend = new TCPIPViewModel("127.0.0.1", 13000, "127.0.0.1", 14000);
            //TCPReceive = new TCPIPViewModel("127.0.0.1", 14000, "127.0.0.1", 13000);
            //UDPSend = new UDPViewModel("127.0.0.1", 11000, "127.0.0.1", 10000);
            //UDPReceive = new UDPViewModel("127.0.0.1", 10000, "127.0.0.1", 11000);
        }
    }
}
