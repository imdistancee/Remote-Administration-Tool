using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Microsoft.VisualBasic;

namespace Remote_Administration_Tool.Forms
{
    public partial class MainForm : Form
    {
        //creates a new server socket variable
        private Helpers.ServerSocket serverSocket;

        public MainForm()
        {
            InitializeComponent();

            //initailizes the server socket class
            serverSocket = new Helpers.ServerSocket();
            //onlisten event
            serverSocket.onListening += ServerSocket_onListening;
            //onconnectionchanged event
            serverSocket.onConnectionChanged += ServerSocket_onConnectionChanged;
            //ondatareceived event
            serverSocket.onDataReceived += ServerSocket_onDataReceived;
            //event is called when a mouse button is pressed
            lstClients.MouseDown += LstClients_MouseDown;
        }

        private void LstClients_MouseDown(object sender, MouseEventArgs e)
        {
            //if right clicked
            if (e.Button == MouseButtons.Right)
            {
                //shows the context menu strip on the listview at a location.
                contextMenuStrip1.Show(lstClients, e.Location);
            }
        }

        private void ServerSocket_onListening(string ip, int port)
        {
            //sets the label text color to green
            labelStatus.Invoke(new Action(() => labelStatus.ForeColor = Color.Green));
            //sets the text of the label
            labelStatus.Invoke(new Action(() => labelStatus.Text = $"Status: Listening on {ip}:{port}"));
        }

        private void ServerSocket_onConnectionChanged(bool connected, Socket socket)
        {
            //clear the clients (were gonna update it)
            lstClients.Invoke(new Action(() => lstClients.Items.Clear()));

            //loop through all the clients
            foreach (Socket clientSocket in serverSocket.connections)
            {
                //send data "info" so we can refresh client listview with info
                serverSocket.SendData(string.Empty, Helpers.CommandHandler.Commands.GET_INFO, clientSocket);
            }
            
            //set the amount of connections label text
            labelConnections.Invoke(new Action(() => labelConnections.Text = $"Connections: {serverSocket.connections.Count}"));
        }

        private void ServerSocket_onDataReceived(string dataString, byte[] data, Helpers.CommandHandler.Commands command, Socket socket)
        {
            //checks the command.
            if (command == Helpers.CommandHandler.Commands.GET_INFO)
            {
                //splits all the data between ~
                string[] splitter = dataString.Split('~');

                //adds all the info to a new listviewitem
                ListViewItem lvi = new ListViewItem(splitter[0]);
                lvi.SubItems.AddRange(new string[] { splitter[1].Split('~')[0], splitter[2].Split('~')[0], splitter[3].Split('~')[0],
                    splitter[4].Split('~')[0], splitter[5].Split('~')[0], splitter[6] });

                //adds the listviewitem to the listview
                lstClients.Invoke(new Action(() => lstClients.Items.Add(lvi)));
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //set check box (checked) to auto listen variable
            checkAutoListen.Checked = Properties.Settings.Default.AutoListen;
            //check to see if auto listening is enabled.
            if (Properties.Settings.Default.AutoListen)
            {
                //assigns listen ip textbox text to ip in settings.
                txtListenIP.Text = Properties.Settings.Default.IP;
                //assigns listen port textbox text to port in settings.
                txtListenPort.Text = Properties.Settings.Default.Port.ToString();
                //starts listening.
                serverSocket.Initialize(txtListenIP.Text, int.Parse(txtListenPort.Text));
                //saves settings.
                Properties.Settings.Default.Save();
            }
        }

        private void btnStartListening_Click(object sender, EventArgs e)
        {
            //checks to see if textboxes aren't empty.
            if (txtListenIP.Text != string.Empty && txtListenPort.Text != string.Empty)
            {
                //checks to see if ip is valid ipaddress and port is valid int
                if (IPAddress.TryParse(txtListenIP.Text, out IPAddress ip) && int.TryParse(txtListenPort.Text, out int i))
                {
                    //starts server socket with ip & port
                    serverSocket.Initialize(txtListenIP.Text, i);
                    //assigns auto listen to checkbox var.
                    Properties.Settings.Default.AutoListen = checkAutoListen.Checked;
                    //checks to see if the checkbox is checked.
                    if (checkAutoListen.Checked)
                    {
                        //assigns ip and port variables to the settings.
                        Properties.Settings.Default.IP = txtListenIP.Text;
                        Properties.Settings.Default.Port = i;
                        //saves the settings.
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //checks to see if any items are selected
            if (lstClients.SelectedItems.Count > 0)
            {
                //gets the index of the first selected item > in connections
                Socket clientSocket = serverSocket.connections[lstClients.SelectedItems[0].Index];
                //closes the connection
                serverSocket.Close(clientSocket);
            }
        }

        private void screenCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //checks to see if any items are selected
            if (lstClients.SelectedItems.Count > 0)
            {
                //gets the index of the first selected item > in connections
                Socket clientSocket = serverSocket.connections[lstClients.SelectedItems[0].Index];
                //opens screen capture and uses client socket as socket.
                new ScreenCapture(clientSocket).Show();
            }
        }

        private void openWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //checks to see if any items are selected
            if (lstClients.SelectedItems.Count > 0)
            {
                //gets the index of the first selected item > in connections
                Socket clientSocket = serverSocket.connections[lstClients.SelectedItems[0].Index];
                //opens an input box for the user to enter a url and sets the variable > users input
                string urlInput = Interaction.InputBox("Enter a url to open on the remote computer", "Open Url", "https://", -1, -1);
                //checks if url starts with http
                if (urlInput.StartsWith("http"))
                {
                    //sends the command to the client for the url to be opened
                    serverSocket.SendData(urlInput, Helpers.CommandHandler.Commands.OPEN_PROCESS, clientSocket);
                }
            }
        }

        private void taskManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //checks to see if any items are selected
            if (lstClients.SelectedItems.Count > 0)
            {
                //gets the index of the first selected item > in connections
                Socket clientSocket = serverSocket.connections[lstClients.SelectedItems[0].Index];
                //shows the task manager form and uses the selected client
                new TaskManager(clientSocket).Show();
            }
        }
    }
}
