using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Administration_Tool.Forms
{
    public partial class TaskManager : Form
    {
        //client socket variable
        private Socket clientSocket;
        //server socket manager variable
        private Helpers.ServerSocket serverSocket;

        public TaskManager(Socket clientSocket)
        {
            InitializeComponent();

            //this sets the variable to the socket entered by user when initializing the form.
            this.clientSocket = clientSocket;
            //this sets the server socket variable
            serverSocket = new Helpers.ServerSocket();
            //connection changed event
            serverSocket.onConnectionChanged += ServerSocket_onConnectionChanged;
            //data received event
            serverSocket.onDataReceived += ServerSocket_onDataReceived;
            //on load event
            this.Load += TaskManager_Load;
        }

        private void ServerSocket_onDataReceived(string dataString, byte[] data, Helpers.CommandHandler.Commands command, Socket socket)
        {
            //checks to see if the socket matches clientsocket
            if (socket == clientSocket)
            {
                //checks the command
                if (command == Helpers.CommandHandler.Commands.TASK_LIST)
                {
                    //clears the listview
                    lstTasks.Invoke(new Action(() => lstTasks.Items.Clear()));

                    //splits the string by a new line
                    string[] splitter = dataString.Split('\n');
                    //loops the split 
                    for (int i = 0, l = splitter.Length; i < l; i++)
                    {
                        //gets the current split index and splits it by ~
                        string[] _splitter = splitter[i].Split('~');

                        //adds the process name to listviewitem
                        ListViewItem lvi = new ListViewItem(_splitter[0]);
                        try
                        {
                            //adds the window title to listviewitem
                            lvi.SubItems.Add(_splitter[1]);
                            //adds the listviewitem to listview
                            lstTasks.Invoke(new Action(() => lstTasks.Items.Add(lvi)));
                        }
                        catch (Exception ex)
                        {
                            //writes the stack trace to the console. stack trace includes line number + exactly whats happening.
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
        }

        private void ServerSocket_onConnectionChanged(bool connected, Socket socket)
        {
            //checks to see if current client disconnected
            if (!connected && socket == clientSocket)
            {
                //closes the form if current client disconnected
                this.Invoke(new Action(() => this.Close()));
            }
        }

        public void RefreshList()
        {
            //clears the listview
            lstTasks.Items.Clear();
            //sends command for getting tasks.
            serverSocket.SendData(string.Empty, Helpers.CommandHandler.Commands.TASK_LIST, clientSocket);
        }

        private void TaskManager_Load(object sender, EventArgs e)
        {
            //sends command for getting tasks.
            RefreshList();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //sends command for getting tasks.
            RefreshList();
        }

        private void btnKillTask_Click(object sender, EventArgs e)
        {
            //checks to see if there are any selected items
            if (lstTasks.SelectedItems.Count > 0)
            {
                //loops through the selected items
                foreach (ListViewItem item in lstTasks.SelectedItems)
                {
                    //sends data to client to kill the task
                    serverSocket.SendData(item.Text, Helpers.CommandHandler.Commands.KILL_TASK, clientSocket);
                }
            }

            //sends command for getting tasks. (refresh list)
            RefreshList();
        }

        private void btnStartNew_Click(object sender, EventArgs e)
        {
            //opens an input box for the user to enter a task name
            string taskName = Interaction.InputBox("Enter a process name to open on the remote computer", "Open/Create Process", string.Empty, -1, -1);
            //checks to see if the taskname isnt empty.
            if (taskName != string.Empty)
            {
                //sends the command to the client for the url to be opened
                serverSocket.SendData(taskName, Helpers.CommandHandler.Commands.OPEN_PROCESS, clientSocket);
                //sends command for getting tasks. (refresh list)
                RefreshList();
            }
        }
    }
}
