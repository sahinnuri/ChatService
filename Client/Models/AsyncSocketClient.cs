using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Models
{
    public class AsyncSocketClient
    {
        private const int port = 4343;
        private static ManualResetEvent connectCompleted = new ManualResetEvent(false);
        private static ManualResetEvent sendCompleted = new ManualResetEvent(false);
        private static ManualResetEvent receiveCompleted = new ManualResetEvent(false);
        private static string response = String.Empty;

        public static void StartClient()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ip = ipHost.AddressList[0];
                IPEndPoint remoteEndPoint = new IPEndPoint(ip, port);

                Socket client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectionCallback), client);
                Send(client, "This is socket message <EOF>");
                sendCompleted.WaitOne();

                Receive(client);
                receiveCompleted.WaitOne();
                Console.WriteLine($"Response received {response}");
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                ObjectState state = new ObjectState();
                state.wSocket = client;
                client.BeginReceive(state.buffer, 0, ObjectState.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ObjectState state = (ObjectState)ar.AsyncState;
                var client = state.wSocket;
                int byteRead = client.EndReceive(ar);
                if (byteRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, byteRead));
                    client.BeginReceive(state.buffer, 0, ObjectState.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    receiveCompleted.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int byteSent = client.EndSend(ar);
                Console.WriteLine($"Sent: {byteSent} bytes to server");
                sendCompleted.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Console.WriteLine($"Socket connection: {client.RemoteEndPoint.ToString()}");
                connectCompleted.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
