using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Remote_Administration_Tool.Helpers
{
    //used for calling the events.
    public delegate void ConnectionChanged(bool connected, Socket socket);
    public delegate void DataReceived(string dataString, byte[] data, CommandHandler.Commands command, Socket socket);
    public delegate void Listening(string ip, int port);

    public class ServerSocket
    {
        //this event handles all connections.
        public event ConnectionChanged onConnectionChanged;
        //this event handles data being received from the server.
        public event DataReceived onDataReceived;
        //this event handles when the socket is initialized
        public event Listening onListening;

        //socket variable that will be used to communicate with the stub.
        private Socket socket;
        //connections list. will be used for storing all client sockets.
        public List<Socket> connections;
        //data buffer for data being received/sent.
        private byte[] buffer;

        public ServerSocket()
        {
            //initializes socket variable
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //initializes connections variable
            connections = new List<Socket>();
            //initializes buffer variable.
            buffer = new byte[1000000];
        }

        public void Initialize(string ip, int port)
        {
            //calls the listening event.
            onListening(ip, port);
            //associates the socket to a local ip end point and port.
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            //places the socket in a listening state.
            socket.Listen(0);
            //begins an async operation to accept new incoming connections..
            socket.BeginAccept(Accept, null);
        }

        private void Accept(IAsyncResult result)
        {
            //new client handler. ends incoming connection.
            Socket clientSocket = socket.EndAccept(result);
            //adds the client to connections list.
            connections.Add(clientSocket);
            //calls the connection changed event.
            onConnectionChanged(true, clientSocket);
            //begins async operation to recieve data from client.
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), clientSocket);
            //allows main socket to accept more incoming connections.
            socket.BeginAccept(Accept, null);
        }

        private void Receive(IAsyncResult result)
        {
            //new variable using information from the callback.
            Socket clientSocket = (Socket)result.AsyncState;
            try
            {
                //gets the length of the data & ends async recieve.
                int l = clientSocket.EndReceive(result);
                //creates a new byte using the length of the data.
                byte[] data = new byte[l];
                //copies buffer amount to data.
                Array.Copy(buffer, data, l);
                //gets string from data
                string dataString = Encoding.ASCII.GetString(data);
                //calls the data received event.
                onDataReceived(dataString.Substring(1), data, new CommandHandler().getCommand(dataString), clientSocket);
            }
            catch (Exception ex) when (ex.Message.Contains("Socket"))
            {
                //if failed then client disconnected.

                //checks to see if list contains client socket.
                if (connections.Contains(clientSocket))
                {
                    //removes the socket from the list
                    connections.Remove(clientSocket);
                    //calls the connection changed event.
                    onConnectionChanged(false, clientSocket);
                }
            }
        }

        public void SendData(string dataString, CommandHandler.Commands command, Socket clientSocket)
        {
            //converts string into byte array
            byte[] data = Encoding.ASCII.GetBytes((int)command + " " + dataString);
            try
            {
                //sends the data to the client socket.
                clientSocket.Send(data);
                //begins recieving data back from the client (async).
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Receive), clientSocket);
            }
            catch (Exception ex) when (ex.Message.Contains("Socket"))
            {
                //something went wrong client side -- disconnected??

                //checks to see if list contains client socket.
                if (connections.Contains(clientSocket))
                {
                    //removes the socket from the list
                    connections.Remove(clientSocket);
                    //calls the connection changed event.
                    onConnectionChanged(false, clientSocket);
                }
            }
        }

        public void Close(Socket clientSocket)
        {
            //checks to see if the list contains the client.
            if (connections.Contains(clientSocket))
            {
                //sends "bye" to the client which closes the connection.
                SendData("bye", CommandHandler.Commands.EXIT, clientSocket);
                //removed the client from the list.
                connections.Remove(clientSocket);
                //calls the connection changed event.
                onConnectionChanged(false, clientSocket);
            }
        }

        public void CloseAll()
        {
            //loops through each socket in connections.
            foreach (Socket socket in connections)
            {
                //removes the socket from the list.
                connections.Remove(socket);
                //calls the connection changed event.
                onConnectionChanged(false, socket);
            }
        }
    }
}
