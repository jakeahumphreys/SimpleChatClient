using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Packets;

namespace SimpleClient
{
    public partial class ChatForm : Form
    {
        delegate void UpdateChatWindowDelgate(string message);
        UpdateChatWindowDelgate _updateChatWindowDelgate;

        delegate void UpdateUsersDel(List<string> users);
        UpdateUsersDel _updateUsersDel;

        SimpleClient client;
        public List<String> currentUsers;
        public ChatForm(SimpleClient client)
        {
            this.client = client;
            InitializeComponent();
            _updateChatWindowDelgate = new UpdateChatWindowDelgate(UpdateChatWindow);
        }
        
       
        public void UpdateChatWindow(string message)
        {
            if (InvokeRequired)
            {
                Invoke(_updateChatWindowDelgate, message);
            }
            else
            {
                txtOutput.Text += message + "\n";
            }

        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Sending");
            string s = String.Empty;
            s = txtInput.Text;
            txtInput.Text = String.Empty;
            char commandCheck = s[0];
            if (commandCheck == '!')
            {
                CommandPacket command = new CommandPacket(s);
                client.SendMessage(command);
            }
            else
            {
                sendMsg(s);
            }
        }

        public void sendMsg(String msg)
        {
            Console.WriteLine("Send Message");
            ChatMessagePacket message = new ChatMessagePacket(msg);
            client.SendMessage(message);
        }

        public void DisplayUsers(List<string> users)
        {
            if (InvokeRequired)
            {
                UpdateUsersDel del = new UpdateUsersDel(DisplayUsers);
                this.Invoke(del, new object[] { users });
            } else
            {
                lstUsers.Items.Clear();
                if (users != null)
                {
                    foreach (string s in users)
                    {
                        lstUsers.Items.Add(s);
                    }
                }
                
            }
        }
        private void btnUpdateList_Click(object sender, EventArgs e)
        {
            if (currentUsers != null)
            {
                foreach (String s in currentUsers)
                {
                    UpdateChatWindow(s.ToString());
                }
            }
            else
            {
                UpdateChatWindow("Nobody's online yet!");
            }
           
        }

        private void btnNick_Click_1(object sender, EventArgs e)
        {
            //NickNamePacket nickname = new NickNamePacket(txtNick.Text);
            UserPacket user = new UserPacket(txtNick.Text, "Online");
            client.SendMessage(user);
            ServerMessagePacket message = new ServerMessagePacket(txtNick.Text + " has joined the chat.");
            client.SendMessage(message);
            txtNick.Enabled = false;
            btnNick.Enabled = false;
            btnSend.Enabled = true;
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectPacket dc = new DisconnectPacket();
            client.SendMessage(dc);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtNick.Text != null)
            {
                UserPacket user = new UserPacket(txtNick.Text, comboBox1.Text);
                client.SendMessage(user);
                ServerMessagePacket server = new ServerMessagePacket(txtNick.Text + " is now " + comboBox1.Text + ".");
                client.SendMessage(server);
            }
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BackColor = Color.FromName(comboBox2.Text);
        }

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtNick.Text != null)
            {
                string text = lstUsers.GetItemText(lstUsers.SelectedItem);
                if (text != null)
                {
                    string nameToSend = text.Split('{')[0];
                    Console.WriteLine(nameToSend);
                    PokePacket poke = new PokePacket(txtNick.Text, nameToSend);
                    client.SendMessage(poke);
                    Console.WriteLine("sent poker");
                    
                }
            }
            
           
            
        }
    }
}


