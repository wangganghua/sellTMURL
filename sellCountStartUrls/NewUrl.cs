using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using ServiceStack.Redis;
using System.Xml.Linq;

namespace sellCountStartUrls
{
    public partial class NewUrl : Form
    {
        dbHelp db;
        string connectionstring2 = "server=192.168.2.236;uid=sa;pwd=All_View_Consulting_2014@;database=CHData";
        /// <summary>
        /// 远程redisip
        /// </summary>
        private string redisIp = "117.23.4.139";
        /// <summary>
        /// 远程redis port
        /// </summary>
        private int redisPort = 15480;
        /// <summary>
        /// 远程连接
        /// </summary>
        RedisClient client = new RedisClient("117.23.4.139", 15480);
        /// <summary>
        /// 月销量url  redis key
        /// </summary>
        private string yxlRedisKey = "sellCountSpider:start_urls";
        /// <summary>
        /// 评论url redis key
        /// </summary>
        private string plRedisKey = "comment_spider:start_urls_info";
        
        public NewUrl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开redis服务
        /// </summary>
        private void OpenRedisServer()
        {
            try
            {
                client = new RedisClient(redisIp, redisPort);
            }
            catch
            {
                OpenRedisServer();
            }
        }

        /// <summary>
        /// lable 控件线程调用辅助公用方法
        /// </summary>
        /// <param name="c">lable name</param>
        /// <param name="strtext">赋值的文本内容</param>
        private void writeR(Label c, string strtext)
        {
            try
            {
                lock (this)
                {
                    c.Invoke(new ThreadStart(delegate()
                    {
                        c.Text = strtext;
                    }));
                }
            }
            catch { }
        }

        /// <summary>
        ///  TextBox 控件线程调用辅助公用方法
        /// </summary>
        /// <param name="c">TextBox name</param>
        /// <param name="strtext">赋值的文本内容</param>
        private void textwriteR(TextBox c, string strtext)
        {
            try
            {
                lock (this)
                {
                    c.Invoke(new ThreadStart(delegate()
                    {
                        if (strtext == "" || strtext == null)
                            c.Text = "";
                        else
                            c.Text += "\n\r" + strtext + "\n\r";
                    }));
                }
            }
            catch { }
        }

        /// <summary>
        /// RichTextBox 控件线程调用辅助公用方法
        /// </summary>
        /// <param name="c">RichTextBox name</param>
        /// <param name="strtext">赋值的文本内容</param>
        private void richtextwriteR(RichTextBox c, string strtext)
        {
            try
            {
                lock (this)
                {
                    c.Invoke(new ThreadStart(delegate()
                    {
                        if (strtext == "" || strtext == null)
                            c.Text = "";
                        c.AppendText(strtext + "\r\n");
                        c.Focus();
                    }));
                }
            }
            catch { }
        }

        private void pTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            writeR(label_time, "当前时间：" + DateTime.Now.ToString());
        }

        private void NewUrl_Load(object sender, EventArgs e)
        {
            db = new dbHelp(connectionstring2);
            System.Timers.Timer pTimer = new System.Timers.Timer(1000);//每隔5s执行一次
            pTimer.Elapsed += pTimer_Elapsed;//委托
            pTimer.AutoReset = true;//获取定时器自动执行
            pTimer.Enabled = true;
            Control.CheckForIllegalCrossThreadCalls = false;//调用线程后台调用，不会影响控件的显示
            Thread tdPL = new Thread(urlPingLun);
            tdPL.Start();
            Thread tdTM = new Thread(TMMonthCount);
            tdTM.Start();
        }

        private void NewUrl_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        //推送评论url.
        /// <summary>
        /// 推送评论url
        /// </summary>
        private void urlPingLun()
        {
            string curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:05:00");
            //curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:14:00");
            richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：【周报评论】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......");
            richtextwriteR(this.richTextBox_plurl, "【周报评论】下次时间:" + curenttime + "...0000");
            while (true)
            {
                if (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") == curenttime)
                {
                    try
                    {
                        //查找需要周度推送的品类
                        string weekCategory = ReadXML("WeekCategory");//周度品类
                        richtextwriteR(this.richTextBox_plurl, "周度采集的品类:" + weekCategory + "");
                        if (weekCategory != "")
                            weekCategory = " AND 品类 NOT IN " + weekCategory + " ";
                        string monthCategory = ReadXML("MonthCategory");
                        richtextwriteR(this.richTextBox_plurl, "月度采集的品类:" + monthCategory + "");
                        if (monthCategory != "")
                            monthCategory = " AND 品类 NOT IN " + monthCategory + " ";

                        //推送所有url
                        string strSql = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM URLDATA WHERE NEED=0 " + weekCategory + monthCategory + " AND 电商!='天猫商城'";
                        DataTable dt = db.selectDatas(strSql);
                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：URL插入redis 共有：" + urlList.Count + "");

                        bool isredisclient = true;
                        while (isredisclient)
                        {
                            try
                            {
                                client.AddRangeToList(plRedisKey, urlList);
                                isredisclient = false;
                            }
                            catch (Exception er)
                            {
                                richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                //等等两秒，重新连接
                                Thread.Sleep(2000);
                                OpenRedisServer();
                            }
                        }

                        //开始推送一次天猫品类url
                        string tmCategory = ReadXML("TMCategory");

                        # region 天猫评论url,部分品类 url

                        if (tmCategory != "")//如果为空，则天猫url 不再采集评论
                        {
                            tmCategory = " AND 品类 IN " + tmCategory + "";

                            dt = new DataTable();
                            urlList = new List<string>();
                            strSql = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM URLDATA WHERE NEED=0 " + tmCategory + " AND 电商='天猫商城'";
                            dt = db.selectDatas(strSql);
                            foreach (DataRow item in dt.Rows)
                            {
                                urlList.Add(item[0].ToString());
                            }

                            //client.AddRangeToList(plRedisKey, urlList);
                            isredisclient = true;
                            while (isredisclient)
                            {
                                try
                                {
                                    client.AddRangeToList(plRedisKey, urlList);
                                    isredisclient = false;
                                }
                                catch (Exception er)
                                {
                                    richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                    //等等两秒，重新连接
                                    Thread.Sleep(2000);
                                    OpenRedisServer();
                                }
                            }
                            richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：天猫URL插入redis 共有：" + urlList.Count + "");
                        }

                        #endregion

                        dt = new DataTable();
                        urlList = new List<string>();
                        strSql = "SELECT '{\"url\": \"'+A.页面信息+'\",\"attr\": {\"urlleibie\": \"'+A.品类+'\",\"urlweb\": \"'+A.电商+'\",\"brand\": \"\",\"model\": \"\"}}' FROM (SELECT 页面信息,电商,品类 FROM URLDATAJD WHERE (NEED=0 OR NEED IS NULL) AND ISNULL(品类,'')!='' AND 电商!='天猫商城' " + weekCategory + monthCategory + " " +
                            ")A LEFT JOIN (SELECT 页面信息,电商,品类 FROM URLDATA WHERE NEED=0 AND 电商!='天猫商城' " + weekCategory + monthCategory + ")B " +
                            "ON  A.页面信息 = B.页面信息 WHERE B.页面信息 IS NULL";
                        dt = db.selectDatas(strSql);
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        //client.AddRangeToList(plRedisKey, urlList);
                        isredisclient = true;
                        while (isredisclient)
                        {
                            try
                            {
                                client.AddRangeToList(plRedisKey, urlList);
                                isredisclient = false;
                            }
                            catch (Exception er)
                            {
                                richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                //等等两秒，重新连接
                                Thread.Sleep(2000);
                                OpenRedisServer();
                            }
                        }

                        richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：查询临时url结束， 共： " + urlList.Count + "");

                        if (weekCategory != "")
                        {
                            #region  //周一 凌晨开始采集的url(不包含天猫url)

                            if (DateTime.Now.DayOfWeek.ToString() == "Monday")
                            {
                                dt = new DataTable();
                                urlList = new List<string>();
                                strSql = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM URLDATA WHERE NEED=0 " + weekCategory.ToUpper().ToString().Replace("NOT", "") + " AND 电商!='天猫商城'";
                                dt = db.selectDatas(strSql);
                                foreach (DataRow item in dt.Rows)
                                {
                                    urlList.Add(item[0].ToString());
                                }
                                isredisclient = true;
                                while (isredisclient)
                                {
                                    try
                                    {
                                        client.AddRangeToList(plRedisKey, urlList);
                                        isredisclient = false;
                                    }
                                    catch (Exception er)
                                    {
                                        richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                        //等等两秒，重新连接
                                        Thread.Sleep(2000);
                                        OpenRedisServer();
                                    }
                                }
                                //client.AddRangeToList(plRedisKey, urlList);

                                dt = new DataTable();
                                urlList = new List<string>();
                                strSql = "SELECT '{\"url\": \"'+A.页面信息+'\",\"attr\": {\"urlleibie\": \"'+A.品类+'\",\"urlweb\": \"'+A.电商+'\",\"brand\": \"\",\"model\": \"\"}}' FROM (SELECT 页面信息,电商,品类 FROM URLDATAJD WHERE (NEED=0 OR NEED IS NULL) AND ISNULL(品类,'')!='' AND 电商!='天猫商城' " + weekCategory.ToUpper().ToString().Replace("NOT", "") + " " +
                                    ")A LEFT JOIN (SELECT 页面信息,电商,品类 FROM URLDATA WHERE NEED=0 AND 电商!='天猫商城' " + weekCategory.ToUpper().ToString().Replace("NOT", "") + ")B " +
                                    "ON  A.页面信息 = B.页面信息 WHERE B.页面信息 IS NULL";
                                dt = db.selectDatas(strSql);
                                foreach (DataRow item in dt.Rows)
                                {
                                    urlList.Add(item[0].ToString());
                                }
                                isredisclient = true;
                                while (isredisclient)
                                {
                                    try
                                    {
                                        client.AddRangeToList(plRedisKey, urlList);
                                        isredisclient = false;
                                    }
                                    catch (Exception er)
                                    {
                                        richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                        //等等两秒，重新连接
                                        Thread.Sleep(2000);
                                        OpenRedisServer();
                                    }
                                }
                                //client.AddRangeToList(plRedisKey, urlList);

                                richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：周度品类查询临时url结束， 共： " + urlList.Count + "");
                            }

                            #endregion
                        }
                        richtextwriteR(this.richTextBox_plurl, DateTime.Now.ToString() + "：over");
                        curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:05:00");
                        richtextwriteR(this.richTextBox_plurl, "【周报评论】下次时间:" + curenttime + "...");
                    }
                    catch (Exception e)
                    {
                        richtextwriteR(this.richTextBox_plurl, "报错：" + e.ToString());
                        curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:05:00");
                        richtextwriteR(this.richTextBox_plurl, "【周报评论】下次时间:" + curenttime + "...");
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //推送天猫月销量url
        /// <summary>
        /// 推送天猫月销量url
        /// </summary>
        private void TMMonthCount()
        {
            //wgh  2018-02-07  开始每天推送2次
            string curenttime = DateTime.Now.ToString();

            if (DateTime.Parse(curenttime).Hour >= 16)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            //curenttime = DateTime.Now.ToString("yyyy-MM-dd 19:21:00");
            else if (DateTime.Parse(curenttime).Hour >= 10)
                //curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:10:00");
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            else if (DateTime.Parse(curenttime).Hour >= 8)
                //curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:00:00");
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            else
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            //curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");

            richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：【日报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......");
            richtextwriteR(this.richTextBox_yxlurl, "【日报  天猫明天采集2次：00、10 点】");
            richtextwriteR(this.richTextBox_yxlurl, "【日报】下次时间:" + curenttime + "...");
            while (true)
            {
                if (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToString() == curenttime)
                {
                    try
                    {
                        richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：【日报】开始推送天猫月销量url.....");
                        //推送所有url
                        string strSql = "SELECT '{\"Urls\": \"'+REPLACE(页面信息,'；',';')+'\",\"Urlleibie\": \"'+品类+'\",\"Urlweb\": \"'+电商+'\",\"spbjpinpai\": \"'+ISNULL(品牌,'')+'\",\"spbjjixing\": \"'+ISNULL(机型,'')+'\",\"pc\":\"" + curenttime + "\"}' FROM URLDATA WHERE NEED=0 AND 电商='天猫商城'";
                        DataTable dt = db.selectDatas(strSql);
                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：URL插入redis 共有：" + urlList.Count + "");
                        bool isredisclient = true;
                        while (isredisclient)
                        {
                            try
                            {
                                client.AddRangeToList(yxlRedisKey, urlList);
                                isredisclient = false;
                            }
                            catch (Exception er)
                            {
                                richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                //等等两秒，重新连接
                                Thread.Sleep(2000);
                                OpenRedisServer();
                            }
                        }

                        richtextwriteR(this.richTextBox_yxlurl, "开始查询临时url......");
                        dt = new DataTable();
                        urlList = new List<string>();
                        strSql = "SELECT '{\"Urls\": \"'+REPLACE(A.页面信息,'；',';')+'\",\"Urlleibie\": \"'+A.品类+'\",\"Urlweb\": \"'+A.电商+'\",\"spbjpinpai\": \"''\",\"spbjjixing\": \"''\",\"pc\":\"" + curenttime + "\"}' FROM (SELECT 页面信息,电商,品类 FROM URLDATATM WHERE NEED IS NULL)A LEFT JOIN (SELECT 页面信息,电商,品类 FROM URLDATA WHERE NEED=0 AND 电商='天猫商城')B  " +
                            "ON  A.页面信息 = B.页面信息 WHERE B.页面信息 IS NULL";
                        dt = db.selectDatas(strSql);
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }

                        //client.AddRangeToList(yxlRedisKey, urlList);
                        isredisclient = true;
                        while (isredisclient)
                        {
                            try
                            {
                                client.AddRangeToList(yxlRedisKey, urlList);
                                isredisclient = false;
                            }
                            catch (Exception er)
                            {
                                richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：url 注入redis 失败 ：" + er.Message + "");
                                //等等两秒，重新连接
                                Thread.Sleep(2000);
                                OpenRedisServer();
                            }
                        }

                        richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：查询临时url结束， 共： " + urlList.Count + "");
                        richtextwriteR(this.richTextBox_yxlurl, DateTime.Now.ToString() + "：over");

                        if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        //curenttime = DateTime.Now.ToString("yyyy-MM-dd 19:13:00");
                        else if (DateTime.Parse(curenttime).Hour >= 10)
                            //curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:00:00");
                        else
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        //curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");

                        richtextwriteR(this.richTextBox_yxlurl, "【日报】下次时间:" + curenttime + "...");
                    }
                    catch (Exception e)
                    {
                        richtextwriteR(this.richTextBox_yxlurl, "报错：" + e.ToString());
                        if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        //curenttime = DateTime.Now.ToString("yyyy-MM-dd 19:13:00");
                        else if (DateTime.Parse(curenttime).Hour >= 10)
                            //curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:00:00");
                        else
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        //curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");

                        richtextwriteR(this.richTextBox_yxlurl, "【日报】下次时间:" + curenttime + "...");
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private void NewUrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认关闭", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                e.Cancel = true;
            else
            {
                try
                {
                    System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                }
                catch { }
            }
        }

        private void button_begin_Click(object sender, EventArgs e)
        {
            string value = ReadXML("Weekcategor2y");
            Debug.WriteLine(value);
        }

        /// <summary>
        /// 读取xml文件,
        /// </summary>
        /// <param name="xmlNoteName">标签名</param>
        /// <returns>返回 结果</returns>
        private string ReadXML(string xmlNoteName)
        {
            //string xmlpath = AppDomain.CurrentDomain.BaseDirectory;
            XDocument docment = XDocument.Load("SpecilCategory.xml");
            XElement root = docment.Root;
            var redisip = root.Element(xmlNoteName); //redis ip
            string value = string.Empty;
            if (redisip != null)
            {
                foreach (XElement a in redisip.Nodes())
                {
                    if (a != null)
                    {
                        value = a.Value;
                    }
                }
            }
            else
            {
                value = "";
            }
            return value;
        }
    }
}
