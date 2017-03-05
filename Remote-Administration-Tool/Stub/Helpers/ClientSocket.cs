using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Stub.Helpers
{
    //used for calling the events.
    public delegate void ConnectionChanged(bool connected);
    public delegate void CommandReceived(CommandHandler.Commands command, string data);

    public class ClientSocket
    {
        //this event handles connections
        public event ConnectionChanged onConnectionChanged;
        //this event handles data being recieved
        public event CommandReceived onCommandReceived;

        //this is the socket that will connect to the server.
        public Socket socket;
        //byte array for recieving and sending data
        private byte[] buffer;

        public ClientSocket()
        {
            //initializes socket variable
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //initializes buffer variable
            buffer = new byte[1000000];
        }

        public void Connect(string ip, int port)
        {
            //while socket is not connected.
            while (!(socket.Connected))
            {
                try
                {
                    //attempts to connect to socket
                    socket.Connect(IPAddress.Parse(ip), port);
                    //calls connection changed event
                    onConnectionChanged(true);
                    //begins recieving data from socket
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);

                }
                catch
                {
                    //failed to connect to socket
                    Console.WriteLine("failed to connect");
                    //stops the thread for 1 second
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private void Receive(IAsyncResult result)
        {
            try
            {
                //gets the length of the data & ends async recieve.
                int l = socket.EndReceive(result);
                //creates a new byte using the length of the data.
                byte[] data = new byte[l];
                //copies buffer amount to data.
                Array.Copy(buffer, data, l);
                //gets string from data
                string dataString = Encoding.ASCII.GetString(data);
                //checks to see if the datastring contains a space
                if (dataString.Contains(" "))
                {
                    //calls command recieved event.
                    onCommandReceived(new CommandHandler().getCommand(dataString), dataString.Substring(1));
                }
                else
                {
                    //calls command recieved event.
                    onCommandReceived(new CommandHandler().getCommand(dataString), string.Empty);
                }
                //begin recieving again
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
            }
            catch
            {
                //if failed then server was closed
                onConnectionChanged(false);
            }
        }

        //TODO: ADD COMMAND VARIABLE HERE.
        public void SendData(byte[] data)
        {
            try
            {
                //sends the data to the client socket.
                socket.Send(data);
                //begins recieving data back from the client (async).
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
            }
            catch
            {
                //something went wrong server side -- closed??
                onConnectionChanged(false);
            }
        }

        public void SendString(string dataString, CommandHandler.Commands command)
        {
            //gets bytes from data
            byte[] data = Encoding.ASCII.GetBytes((int)command + " " + dataString);
            try
            {
                //sends the data to the client socket.
                socket.Send(data);
                //begins recieving data back from the client (async).
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), socket);
            }
            catch
            {
                //something went wrong server side -- closed??
                onConnectionChanged(false);
            }
        }
    }
}
