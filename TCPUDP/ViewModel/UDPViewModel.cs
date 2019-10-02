using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace TCPUDP.ViewModel
{
    public class UDPViewModel : BaseConnectionViewModel
    {
        public UDPViewModel() : this("127.0.0.1", 11000, "127.0.0.1", 10000)
        {
        }
        public UDPViewModel(string startingMyIP, int startingMyPort, string startingIP, int startingPort) : base(startingMyIP, startingMyPort, startingIP, startingPort)
        {
            ButtonConnect = new RelayCommand(Connect, CanConnect);
            ButtonListen = new RelayCommand(Listen, CanListen);
            strokeCollection = new StrokeCollection();
            imageReceieved = new StrokeCollection();
        }

        private void Listen(object obj)
        {
            if (currentTask != null)
            {
                cts.Cancel();
                currentTask = null;
                IsListening = false;
            }
            else
            {
                cts = new CancellationTokenSource();
                token = cts.Token;
                currentTask = Task.Factory.StartNew(() => ListenTask(), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
        private void ListenTask()
        {
            IPAddress endpointAddr = System.Net.IPAddress.Parse(string.IsNullOrWhiteSpace(MyIPAddress) ? "127.0.0.1" : IPAddress);
            IsListening = true;
            UdpClient listener = new UdpClient(MyPort);
            IPEndPoint groupEP = new IPEndPoint(endpointAddr, Port);
            try
            {
                while (true)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }
                    if (listener.Available > 0)
                    {
                        byte[] bytes = listener.Receive(ref groupEP);

                        ErrorMessage = string.Format($"Received broadcast from {groupEP} :");
                        try
                        {
                            ImageReceieved = ImageToStroke(bytes);
                        }
                        catch (ArgumentException e)
                        {
                            ErrorMessage = string.Format(e.Message);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                ErrorMessage = string.Format(e.Message);
            }
            finally
            {
                listener.Close();
                IsListening = false;
            }
        }

        private void Connect(object argument)
        {
            if (currentTask != null)
            {
                cts.Cancel();
                currentTask = null;
                IsConnected = false;
            }
            else
            {
                cts = new CancellationTokenSource();
                token = cts.Token;
                currentTask = Task.Factory.StartNew(() => ConnectTask(), token, TaskCreationOptions.None, TaskScheduler.Default);
            }
        }
        private void ConnectTask()
        {

            IsConnected = true;
            IPAddress localAddr = System.Net.IPAddress.Parse(string.IsNullOrWhiteSpace(MyIPAddress) ? "127.0.0.1" : MyIPAddress);
            IPEndPoint udpClientEndPoint = new IPEndPoint(localAddr, MyPort);

            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(udpClientEndPoint);
            try
            {
                udpClient.Connect(IPAddress, Port);
                while (true)
                {
                    Thread.Sleep(100);
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }
                    MemoryStream ms = ConvertStrokestoImage();
                    byte[] bmpBytes = ms.GetBuffer();
                    ms.Close();
                    udpClient.Send(bmpBytes, bmpBytes.Length);
                }

                udpClient.Close();
            }
            catch (Exception e)
            {
                ErrorMessage = string.Format(e.ToString());
            }
            finally
            {
                IsConnected = false;
            }
        }
        private static int SendVarData(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;
            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = s.Send(datasize);
            while (total < size)
            {
                sent = s.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
            return total;
        }

        public MemoryStream ConvertStrokestoImage()
        {
            StrokeCollection sc = ImageToShow;
            using (MemoryStream inkMemStream = new MemoryStream())
            {
                sc.Save(inkMemStream);
                return inkMemStream;
            }
        }
        public StrokeCollection ImageToStroke(byte[] bmpBytes)
        {
            StrokeCollection sc = ImageToShow;
            using (MemoryStream inkMemStream = new MemoryStream(bmpBytes))
            {
                return new System.Windows.Ink.StrokeCollection(inkMemStream);
            }
        }
        private StrokeCollection imageReceieved;
        public StrokeCollection ImageReceieved
        {
            get { return imageReceieved; }
            set
            {
                SetProperty(ref imageReceieved, value, "ImageReceieved");
            }
        }
        private StrokeCollection strokeCollection;
        public StrokeCollection ImageToShow
        {
            get { return strokeCollection; }
            set
            {
                SetProperty(ref strokeCollection, value, "ImageToShow");
            }
        }
    }
}
