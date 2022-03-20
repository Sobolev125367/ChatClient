using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private string placeholder = "Введите ник";

        //Сокет для получения сообщения
        static private Socket Client;
        private IPAddress ip = null;
        private int port = 0;
        private Thread thread;
        public Form1()
        {
            InitializeComponent();

            richTextBox1.Enabled = false;
            richTextBox2.Enabled = false;
            button1.Enabled = false;

            //Создаем обработчик ошибок
            try
            {
                var sr = new StreamReader(@"Client_info/data_info.txt");
                //Переменную для хранения данных из файла
                string buffer = sr.ReadToEnd();
                sr.Close();
                //Делим/парсим инфу
                string[] connect_info = buffer.Split(':');
                ip = IPAddress.Parse(connect_info[0]);
                port = int.Parse(connect_info[1]);

                label4.ForeColor = Color.Blue;
                label4.Text = "Настройки:\n IP сервера:"+connect_info[0]+"\n Порт сервера:" + connect_info[1];
            }
            catch(Exception ex)
            {
                label4.ForeColor=Color.Red;
                label4.Text="Настройки не найдены!";
                Settings settings = new Settings();
                settings.Show(); 
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Gray;
            textBox1.Text = placeholder;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage("\n" + textBox1.Text + ": " + richTextBox2.Text + ";;;5");
            richTextBox2.Clear();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Остановка метода thread
            if(thread!= null)
                thread.Abort();
            if(Client!=null)
            {
                Client.Close();
            }    
            Application.Exit();
        }
        //Получение сообщения от сервера
        void SendMessage(string message)
        {
            if(message !="" && message!="")
            {
                byte[] buffer = new byte[1024];
                buffer=Encoding.UTF8.GetBytes(message);
                Client.Send(buffer);
            }
        }

        void RecvMessage()
        {
            byte[] buffer = new byte[1024];
            //Чистим буфер
            for(int i=0;i<buffer.Length;i++)
            {
                buffer[i] = 0;
            }
            for(; ; )
            {
                try
                {
                    Client.Receive(buffer);
                    //Переводим полученное сообщение
                    string Message=Encoding.UTF8.GetString(buffer);
                    //Конец буфера
                    int count = Message.IndexOf(";;;5");
                    if(count==-1)
                    {
                        continue;
                    }
                    string ClearMessage = "";
                    for(int i=0; i<count;i++)
                    {
                        ClearMessage += Message[i];
                    }
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        richTextBox1.AppendText(ClearMessage);
                    });
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox1.Text !=" "&& textBox1.Text != "")
            {
                button1.Enabled = true;
                richTextBox2.Enabled = true;
                Client=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                {
                    //Создаем запрос
                    if (ip != null)
                    {
                        //Подключаемся к серверу
                        Client.Connect(ip, port);
                        //Запуск метода для получения сообщений от сервера
                        thread = new Thread(delegate () { RecvMessage(); });
                        thread.Start();
                        richTextBox2.Focus();
                    }
                }

            }

        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("See my profile on github: github.com/Sobolev125367");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SendMessage("\n" + textBox1.Text + ": " + richTextBox2.Text + ";;;5");
                richTextBox2.Clear();
            }
        }
        //Пишем фокус для первого текстбокса при входе в него
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == placeholder)
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }
        //Пишем фокус для первого текстбокса при выходе из него
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                textBox1.ForeColor= Color.Gray;
                textBox1.Text = placeholder;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }




        /*
                //Пишем фокус для второго текстбокса при входе в него
                private void richTextBox2_Enter_1(object sender, EventArgs e)
                {
                    if (richTextBox2.Text == placeholder)
                    {
                        richTextBox2.Text = "";
                        richTextBox2.ForeColor = Color.Black;
                    }
                }
                //Пишем фокус для второго текстбокса при выходе из него
                private void richTextBox2_Leave_1(object sender, EventArgs e)
                {
                    if (richTextBox2.Text == "")
                    {
                        richTextBox2.ForeColor = Color.Gray;
                        richTextBox2.Text = placeholder;
                    }
                }
        */
    }
}
