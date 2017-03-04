using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using System.ComponentModel;

namespace Remote_Administration_Tool.Forms
{
    public partial class ScreenCapture : Form
    {

        //client socket variable.
        private Socket clientSocket;
        //server socket manager variable.
        private Helpers.ServerSocket serverSocket;

        public ScreenCapture(Socket clientSocket)
        {
            InitializeComponent();

            //this sets the client socket so we can manage it.
            this.clientSocket = clientSocket;
            //this initializes the server socket class.
            serverSocket = new Helpers.ServerSocket();
            //on connection changed event handler
            serverSocket.onConnectionChanged += ServerSocket_onConnectionChanged;
            //on data recieved event handler
            serverSocket.onDataReceived += ServerSocket_onDataReceived;
        }

        private void ServerSocket_onDataReceived(string dataString, byte[] data, Helpers.CommandHandler.Commands command, Socket socket)
        {
            //checking to see if we got data from the client socket.
            if (socket == clientSocket)
            {
                //checks to see if the command is screen cap
                if (command == Helpers.CommandHandler.Commands.SCREEN_CAPTURE)
                {
                    //intializing a type converter
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                    //creating a new bitmap using the type converter and data then setting the image.
                    pictureScreen.Invoke(new Action(() => pictureScreen.Image = (Bitmap)converter.ConvertFrom(data)));
                }
            }
        }

        private void ServerSocket_onConnectionChanged(bool connected, Socket socket)
        {
            //checks to see if connection is false and if the client disconnected
            if (!connected && clientSocket == socket)
            {
                //closing the form
                this.Invoke(new Action(() => this.Close()));
            }
        }

        private void btnTakePic_Click(object sender, EventArgs e)
        {
            //sending "screencap" so we can recieve a screenshot back.
            serverSocket.SendData(string.Empty, Helpers.CommandHandler.Commands.SCREEN_CAPTURE, clientSocket);
        }
    }
}
