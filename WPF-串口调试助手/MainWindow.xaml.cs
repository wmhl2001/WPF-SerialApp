using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;

namespace WPF_串口调试助手
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dtimer = new DispatcherTimer(); //实例化一个定时器
        private MyCom mCom = new MyCom(); //实例化一个串口类
        public MainWindow()
        {
            InitializeComponent();
        }

        //串口打开事件
        void timer_Tick(object sender, EventArgs e)
        {
            if (SerialCom.comdata.Count > 0)
            {
                Revdata.Text += SerialCom.comdata[0] + "\r\n";
                SerialCom.comdata.RemoveAt(0);
            }
            if (SerialCom.OpenState)
            {
                Open.Content = "关闭串口";
            }
            else
            {
                Open.Content = "开启串口";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //在窗口加载事件中添加定时间隔/开启定时器
            dtimer.Interval = TimeSpan.FromMilliseconds(100); //设置定时间隔为100ms
            dtimer.Tick += new EventHandler(timer_Tick); //注册定时中断事件
            dtimer.Start();// 定时器开启timer_Tick

            //串口号
            string[] ports = SerialPort.GetPortNames();
            this.Protname.ItemsSource = ports;
            this.Protname.SelectedIndex = 0;
            //波特率
            string[] baudrate = new string[] { "300", "600", "1200", "2400", "4800", "9600", "19200", "38400", "43000", "56000", "57600", "115200" };
            this.Baudrate.ItemsSource = baudrate;
            this.Baudrate.SelectedIndex = 0;//即默认显示第一条
            //数据位
            this.Databit.Items.Add(8);
            this.Databit.Items.Add(7);
            this.Databit.Items.Add(6);
            this.Databit.Items.Add(5);
            this.Databit.SelectedIndex = 0;
            //校验位
            this.Parity.Items.Add("None");
            this.Parity.Items.Add("Odd");
            this.Parity.Items.Add("Even");
            this.Parity.SelectedIndex = 0;
            //停止位
            this.Stopbit.Items.Add(1);
            this.Stopbit.Items.Add(1.5);
            this.Stopbit.Items.Add(2);
            this.Stopbit.SelectedIndex = 0;
        }

        //串口搜索事件
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            this.Protname.ItemsSource = ports;
            this.Protname.SelectedIndex = 0;
        }

        //添加按钮发送事件
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            mCom.WriteData(Encoding.UTF8.GetBytes(Senddata.Text));
        }

        //添加按钮打开事件
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            SerialCom.com_name = Protname.Text; // 串口号
            SerialCom.com_Bound = int.Parse(Baudrate.Text); // 波特率
            SerialCom.com_DataBit = int.Parse(Databit.Text); // 数据位
            SerialCom.com_StopBit = Stopbit.Text;// 停止位
            SerialCom.com_Verify = Parity.Text;// 校验位
            mCom.ComOpen();
        }
    }

    //串口收发数据类
    class MyCom
    {
        public void ComOpen()
        {
            if (SerialCom.OpenState == false)
            {
                SerialCom.com.PortName = SerialCom.com_name;
                SerialCom.com.BaudRate = SerialCom.com_Bound;
                SerialCom.com.DataBits = SerialCom.com_DataBit;
                if (SerialCom.com_StopBit == "1") SerialCom.com.StopBits = System.IO.Ports.StopBits.One;
                if (SerialCom.com_StopBit == "2") SerialCom.com.StopBits = System.IO.Ports.StopBits.Two;
                if (SerialCom.com_Verify == "None") SerialCom.com.Parity = System.IO.Ports.Parity.None;
                if (SerialCom.com_Verify == "Odd") SerialCom.com.Parity = System.IO.Ports.Parity.Odd;
                if (SerialCom.com_Verify == "Even") SerialCom.com.Parity = System.IO.Ports.Parity.Even;
                SerialCom.com.NewLine = "\r\n"; //接收或者发送数据回车显示
                Comthread();//启动线程
            }
            else
            {
                //否则的话 关闭串口，同时串口的状态为false。
                SerialCom.comdata.Add("关闭串口");
                SerialCom.com.Close();
                SerialCom.OpenState = false;
            }
        }


        //读取串口方法
        public void ReadDada()
        {
            SerialCom.comdata.Add("打开串口完成");
            SerialCom.OpenState = true;
            while (SerialCom.OpenState)
            {
                Thread.Sleep(50);
                try
                {
                    // 查询串口中目前保存了多少数据
                    int n = SerialCom.com.BytesToRead;
                    byte[] buf = new byte[n];
                    SerialCom.com.Read(buf, 0, n);
                    if (buf.Length > 0)
                    {
                        string str = Encoding.Default.GetString(buf);
                        SerialCom.comdata.Add(str);
                    }
                }
                catch
                {
                    SerialCom.OpenState = false;
                    SerialCom.com.Close();
                }
            }
        }

        //串口写数据
        public void WriteData(byte[] bytes)
        {
            try
            {
                if (SerialCom.OpenState && bytes != null)
                {
                    SerialCom.com.Write(bytes, 0, bytes.Length);
                }
            }
            catch
            {

            }   
        }

        //线程读取串口数据
        private void Comthread()
        {
            SerialCom.com.Open(); //打开串口
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            SerialCom.com.Encoding = Encoding.GetEncoding("GB2312"); //设置编码格式
            Thread thread = new Thread(ReadDada);// 实例化一个线程
            thread.IsBackground = true; //设置线程为后台线程
            thread.Start(); //启动线程
        }
    }
}
