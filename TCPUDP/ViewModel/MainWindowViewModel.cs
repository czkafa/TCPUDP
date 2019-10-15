using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCPUDP.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<IPAddress> IPAddresses
        {
            get;
            set;
        }

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
            IPAddresses = new ObservableCollection<IPAddress>(Dns.GetHostByName(hostName).AddressList);
            string myIP = IPAddresses[0].ToString();

            DictionaryViewModel = new Dictionary<string, ViewModelBase>()
            {
                { "TCP", new TCPIPScreenViewModel(myIP, 13000, 14000)},
                { "UDP", new UDPScreenViewModel(myIP, 13000, 14000)},
            };

            ExecuteTCPIP(null);
            TCPCommand = new RelayCommand(ExecuteTCPIP);
            UDPCommand = new RelayCommand(ExecuteUDP);

        }
    }
}
