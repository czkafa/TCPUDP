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

        public TCPIPViewModel(string startingMyIP, int startingMyPort, string startingIP, int startingPort) : base(startingMyIP, startingMyPort, startingIP, startingPort)
        {
            ButtonConnect = new RelayCommand(Connect, CanConnect);
            ButtonListen = new RelayCommand(Listen, CanListen);
            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
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

        private void Listen(object obj)
        {
            if (currentTask != null)
            {
                cts.Cancel();
                currentTask = null;
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
                IPAddress localAddr = System.Net.IPAddress.Parse(MyIPAddress);
                server = new TcpListener(localAddr, MyPort);
                server.Start();
                IsListening = true;

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
            using (NetworkStream stream = client.GetStream())
            {
                while (client.Connected)
                {
                    if (cts.IsCancellationRequested)
                    {
                        stream.Close();
                        client.Close();
                        return;
                    }
                    else if (messagesToSend.Count > 0)
                    {
                        SendMessage(stream, messagesToSend.Dequeue());
                    }
                    else if (stream.DataAvailable)
                    {
                        Byte[] bytes = new Byte[256];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        string data = null;
                        data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                        ErrorMessage = string.Format("Received: {0}", data);
                        MessageReceived = data;
                    }
                    else if (!IsTcpIPStill(client))
                    {
                        stream.Close();
                        client.Close();
                        return;
                    }
                    Thread.Sleep(300);
                }
            }
        }

        private bool IsTcpIPStill(TcpClient tcpClient)
        {
            System.Net.NetworkInformation.IPGlobalProperties ipProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.NetworkInformation.TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint)).ToArray();

            if (tcpConnections != null && tcpConnections.Length > 0)
            {
                System.Net.NetworkInformation.TcpState stateOfConnection = tcpConnections.First().State;
                if (stateOfConnection == System.Net.NetworkInformation.TcpState.Established)
                {
                    // Connection is OK
                    return true;
                }
                else
                {
                    // No active tcp Connection to hostName:port
                    return false;
                }
            }
            return false;
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
                TcpClient client = new TcpClient(IPAddress, Port);
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

        private void SendMessage(NetworkStream stream, string message)
        {
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);
            ErrorMessage = string.Format("Sent: {0}", message);
        }
    }
}
