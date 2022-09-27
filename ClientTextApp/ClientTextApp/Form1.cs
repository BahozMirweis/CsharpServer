using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ClientTextApp
{
    public partial class Form1 : Form
    {
        public static TcpClient Globalclient;
        public static NetworkStream Globalstream;
        public const int bytesize = 1024 * 1024;
        byte[] readMessageBytes = new byte[bytesize];
        byte[] writeMessageBytes = new byte[bytesize];
        public Form1()
        {
            InitializeComponent();
        }

        private void send_Click(object sender, EventArgs e)
        {
            sendDataFromTextBox();
        }

        private void sendDataFromTextBox()
        {
            if (textToSend.Text != "")
            {
                writeMessageBytes = Encoding.Unicode.GetBytes(textToSend.Text);
                Globalstream.Write(writeMessageBytes, 0, writeMessageBytes.Length);
                textToSend.Text = "";
                writeMessageBytes = new byte[bytesize];
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Globalstream != null)
            {
                Globalstream.Dispose();
                Globalclient.Close();
            }
        }

        private void ReadData(IAsyncResult re)
        {
            try
            {
                Globalstream.EndRead(re);
                string message = Encoding.Unicode.GetString(readMessageBytes);
                outputTextBox.Invoke(() =>
                {
                    outputTextBox.Text += message;
                });
                readMessageBytes = new byte[bytesize];
                Globalstream.BeginRead(readMessageBytes, 0, bytesize, new AsyncCallback(ReadData), null);
            } catch (IOException)
            {

            }
            catch(ObjectDisposedException)
            {

            }
        }

        private void usernameButton_Click(object sender, EventArgs e)
        {
            sendUsername();
        }

        private void sendUsername()
        {
            if (usernameTextBox.Text == "")
            {
                MessageBox.Show("Username can't be empty.");
            }
            else
            {
                try
                {
                    usernameTextBox.Enabled = false;
                    Globalclient = new TcpClient("127.0.0.1", 1234);
                    Globalstream = Globalclient.GetStream();
                    byte[] username = Encoding.Unicode.GetBytes(usernameTextBox.Text);
                    Globalstream.Write(username, 0, username.Length);
                    Globalstream.BeginRead(readMessageBytes, 0, bytesize, new AsyncCallback(ReadData), null);
                    usernameButton.Visible = false;
                    usernameTextBox.Visible = false;
                    usernameLabel.Visible = false;
                    textToSend.Visible = true;
                    textToSend.Enabled = true;
                    sendButton.Visible = true;
                    outputTextBox.Visible = true;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("ERROR: couldn't connect to server.");
                    usernameTextBox.Enabled = true;
                }
            }
        }

        private void usernameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                sendUsername();
            }
        }

        private void textToSend_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                sendDataFromTextBox();
            }
        }
    }
}