using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;
 using System.Runtime.InteropServices;

namespace sellCountStartUrls
{
    public partial class hhh : Form
    {
        //设置 窗体无边框
        //this.FormBorderStyle = FormBorderStyle.None;
        public hhh()
        {
            InitializeComponent();
            label_min.BringToFront();//保持控件最顶层
            label_move.BringToFront();
            label_close.BringToFront();
            //this.BackColor = Color.Black;
            this.Opacity = 0.7;
        }
        //此位置不能改变
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        bool beginMove = false;//初始化鼠标位置  
        int currentXPosition;
        int currentYPosition;

        private void button1_Click(object sender, EventArgs e)
        {
            string[] BossDaqu = { "华东一区", " 华北一区", " 华西一区", " 华南区", " 华东二区", " 华北二区", " 华西二区", " 华中区" };  //定义大区
            string[][] BossCity ={
                                 new string[]{"嘉兴","温州","台州","宁波","绍兴","丽水","杭州","福州","厦门","泉州"},//华东一区
                                 new string[]{"唐山","石家庄","太原","忻州","大同","济南","临沂","北京","天津","青岛","徐州","泰安","淄博","临汾"},//华北一区
                                 new string[]{"延安","呼和浩特","包头","乐山","南充","宁夏","西安","兰州","武威","成都"},//华西一区
                                 new string[]{"佛山","汕头","南昌","赣州","海口","长沙","常德","广州","深圳","衡阳","黄石"},//华南区
                                 new string[]{"湖州","无锡","常州","镇江","南京","扬州","盐城","上海","苏州","合肥","芜湖"},//华东二区
                                 new string[]{"大连","哈尔滨","大庆","佳木斯","牡丹江","沈阳","吉林","呼伦贝尔","赤峰"},//华北二区
                                 new string[]{"昆明","柳州","南宁","大理","贵阳","重庆","万州","乌鲁木齐","西宁"},//华西二区
                                 new string[]{"郑州","襄阳","宜昌","武汉","洛阳","南阳","平顶山","黄冈","十堰","六安"},//华中区
                             };
            int index = 3;
            for (int i = 0; i < BossDaqu.Length; i++)
            {
                index += 1;
                for (int a = 0; a < BossCity[i].Length; a++)
                    index += 1;
            }
            Debug.WriteLine(index);
        }

        private void hhh_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beginMove = true;
                currentXPosition = MousePosition.X;//鼠标的x坐标为当前窗体左上角x坐标  
                currentYPosition = MousePosition.Y;//鼠标的y坐标为当前窗体左上角y坐标  
            }
        }

        private void hhh_MouseMove(object sender, MouseEventArgs e)
        {
            if (beginMove)
            {
                this.Left += MousePosition.X - currentXPosition;//根据鼠标x坐标确定窗体的左边坐标x  
                this.Top += MousePosition.Y - currentYPosition;//根据鼠标的y坐标窗体的顶部，即Y坐标  
                currentXPosition = MousePosition.X;
                currentYPosition = MousePosition.Y;
            }
        }

        private void hhh_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currentXPosition = 0; //设置初始状态  
                currentYPosition = 0;
                beginMove = false;
            }
        }

        private void label_min_MouseDown(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

    }
}
