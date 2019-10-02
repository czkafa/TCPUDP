using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPUDP.ViewModel
{
    public class TCPIPViewModel : BaseConnectionViewModel
    {
        public TCPIPViewModel() : this("127.0.0.1", 13000, "127.0.0.1", 14000)
        {
        }

        public TCPIPViewModel(string startingMyIP, int startingMyPort, string startingIP, int startingPort)
        {
            MyIPAddress = startingMyIP;
            MyPort = startingMyPort;
            IPAddress = startingIP;
            Port = startingPort;
            ButtonConnect = new RelayCommand(Connect, CanConnect);
            ButtonListen = new RelayCommand(Listen, CanListen);
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);

            cts = new CancellationTokenSource();
            token = cts.Token;
        }
        Queue<string> messagesToSend = new Queue<string>();
        private void SendMessage(object obj)
        {
            messagesToSend.Enqueue(MessageToSend);
            MessageToSend = string.Empty;
        }

        private bool CanSendMessage(object arg)
        {
            return IsConnected;
        }

        public RelayCommand SendMessageCommand
        {
            get; set;
        }

        TcpClient currentTcpClient = null;

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
            TcpListener server = null;
            try
            {
                Int32 defaultPort = 13000;
                IPAddress localAddr = System.Net.IPAddress.Parse(string.IsNullOrWhiteSpace(MyIPAddress) ? "127.0.0.1" : MyIPAddress);
                server = new TcpListener(localAddr, Port == 0 ? defaultPort : MyPort);
                server.Start();
                IsListening = true;

                String data = null;

                while (true)
                {
                    while (!server.Pending())
                    {
                        Thread.Sleep(100);
                        if (cts.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                    TcpClient client = server.AcceptTcpClient();

                    IsConnected = client.Connected;
                    Connection(client);
                    IsConnected = client.Connected;
                }
            }
            catch (SocketException e)
            {
                ErrorMessage = string.Format("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
                IsListening = false;
            }
        }

        private void Connection(TcpClient client)
        {
            while (client.Connected)
            {
                Byte[] bytes = new Byte[256];
                string data = null;

                if (messagesToSend.Count > 0)
                {
                    SendMessage(client, messagesToSend.Dequeue());
                }
                else if (cts.IsCancellationRequested)
                {
                    client.Close();
                    return;
                }
                else
                {
                    NetworkStream stream = client.GetStream();
                    int i;
                    if (stream.DataAvailable)
                    {

                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);

                            stream.Write(msg, 0, msg.Length);
                            ErrorMessage = string.Format("Received: {0}", data);
                            MessageReceived = data;
                        }
                    }

                }
                Thread.Sleep(100);
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
                currentTask = Task.Factory.StartNew(() => ConnectTask(), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        private void ConnectTask()
        {
            try
            {
                Int32 defaultPort = 13000;
                string defaultMessage = "{@.@}デフォルトメッセージ";
                TcpClient client = new TcpClient(IPAddress, Port == 0 ? defaultPort : Port);
                IsConnected = client.Connected;

                Connection(client);
                IsConnected = client.Connected;
            }
            catch (ArgumentNullException e)
            {
                ErrorMessage = string.Format("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                ErrorMessage = string.Format("SocketException: {0}", e);
            }
            finally
            {
                IsConnected = false;
            }
        }

        private void SendMessage(TcpClient client, string message)
        {
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            using (NetworkStream stream = client.GetStream())
            {
                stream.Write(data, 0, data.Length);

                data = new Byte[256];

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                ErrorMessage = string.Format("Sent: {0}", responseData);
                //stream.Close();

            }


        }
    }
}
