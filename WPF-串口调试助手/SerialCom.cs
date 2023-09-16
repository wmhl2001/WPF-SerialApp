using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace WPF_串口调试助手
{
    //串口类
    class SerialCom
    {
        public static SerialPort com { get; set; } = new SerialPort();
        //串口名字
        public static string com_name { get; set; }
        // 波特率
        public static int com_Bound { get; set; }
        //数据位
        public static int com_DataBit { get; set; }
        // 校验位
        public static string com_Verify { get; set; }
        // 停止位
        public static string com_StopBit { get; set; }
        // 串口的打开状态标记位
        public static bool OpenState { get; set; }
        //数据显示
        public static List<string> comdata = new List<string>();
    }
}
