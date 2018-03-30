using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using System.Threading;
using ServiceStack.Redis;

namespace sellCountStartUrls
{
    public partial class Formmjw : Form
    {
        public Formmjw()
        {
            InitializeComponent();
        }
        Thread t = null;
        string connectionstring2 = "server=192.168.2.245;uid=sa;pwd=3132_deeposh_0083;database=sanc";
        dbHelp db;

        private void Formmjw_Load(object sender, EventArgs e)
        {
            db = new dbHelp(connectionstring2);
            //周度
            t = new Thread(NewMethod_test);
            t.Start();
        }
        //每天9点
        private void NewMethod()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 9 && DateTime.Parse(curenttime).Minute>=5)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 09:00:00");
           
            this.richTextBox1.Invoke(new ThreadStart(delegate()
            {
                richTextBox1.Text = "【卖家网】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
            writeR(this.richTextBox1, "【卖家网】下次时间:" + curenttime + "...");

            while (true)
            {
                if (DateTime.Now >= DateTime.Parse(curenttime))
                {
                    try
                    {
                        this.richTextBox1.Invoke(new ThreadStart(delegate()
                        {
                            if (this.richTextBox1.Lines.Length > 40)
                                richTextBox1.Clear();
                            richTextBox1.Text = "【卖家网】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.ScrollToCaret();
                        }));
                        writeR(this.richTextBox1, "开始查询URL...");
                        string sqlStr = "select '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox1, "URL插入redis 共有："+urlList.Count+" 条");
                        RedisClient client = new RedisClient("192.168.2.245", 6379);
                        client.AddRangeToList("dpc_maijiawang:url_list", urlList);
                        writeR(this.richTextBox1, "成功，等待下次...");

                        if (DateTime.Parse(curenttime).Hour >= 9 && DateTime.Parse(curenttime).Minute >= 5)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 09:00:00");

                        writeR(this.richTextBox1, "【卖家网】下次时间:" + curenttime + "...");
                    }
                    catch (Exception ecppp)
                    {
                        writeR(this.richTextBox1, "报错：" + ecppp.ToString());
                    }
                }
                else
                    Thread.Sleep(1000);

                this.label1.Invoke(new ThreadStart(delegate()
                {
                    label1.Text = "当前时间："+DateTime.Now.ToString()+"";
                }));
            }
        }
        //临时使用
        private void NewMethod_test()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 8)
                //curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            else if (DateTime.Parse(curenttime).Hour >= 8)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:09:00");
            else
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");

            this.richTextBox1.Invoke(new ThreadStart(delegate()
            {
                richTextBox1.Text = "【卖家网】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
            writeR(this.richTextBox1, "【卖家网】下次时间:" + curenttime + "...");

            while (true)
            {
                if (DateTime.Now >= DateTime.Parse(curenttime))
                {
                    try
                    {
                        this.richTextBox1.Invoke(new ThreadStart(delegate()
                        {
                            if (this.richTextBox1.Lines.Length > 40)
                                richTextBox1.Clear();
                            richTextBox1.Text = "【卖家网】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.ScrollToCaret();
                        }));
                        writeR(this.richTextBox1, "开始查询URL...");
                        string sqlStr = "select  '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送 ";
                        //sqlStr = "select  '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送 where category='净水器'";
                        sqlStr = "select  '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送 where category='投影仪'";
                        //sqlStr = "select  '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送 where category='电烤箱'";
                        //sqlStr = "select  '{\"category\": \"'+category+'\",\"url\": \"'+url+'\"}' from  dba.dbo.卖家网采集页面信息定时推送 where category='微波炉'";
                        sqlStr = " select top 100  '{\"url\": \"'+Urls+'\",\"attr\":{\"urlleibie\": \"'+Urlleibie+'\",\"urlweb\": \"'+(case when urlweb='tm' then'天猫商城' when urlweb='jd' then '京东商城' when urlweb='sn' then '苏宁易购' when urlweb='yhd' then '1号电' when urlweb='dd' then '当当网' when urlweb='gm' then '国美在线' when urlweb='ymx' then '亚马逊' when urlweb='tb' then '淘宝网' else urlweb end)+'\",\"brand\": \"'+ISNULL(spbjpinpai,'')+'\",\"model\": \"'+ISNULL(spbjjixing,'')+'\"}}' from  URLDATA_TM where NEED=1  AND URLLEIBIE='智能手机'";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox1, "URL插入redis 共有：" + urlList.Count + "...");
                        //RedisClient client = new RedisClient("192.168.2.245", 6379);
                        //RedisClient client = new RedisClient("117.122.192.50", 6479);
                        RedisClient client = new RedisClient("117.23.4.139", 15480);
                        client.AddRangeToList("dpc_maijiawang:url_list", urlList);
                        //client.AddRangeToList("SqlServerInsertAllUrl:start_urls_info", urlList);
                        //client.AddRangeToList("comment_spider:start_urls_info", urlList);
                        writeR(this.richTextBox1, "成功，等待下次...");

                        if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");
                        writeR(this.richTextBox1, "【卖家网】下次时间:" + curenttime + "...");
                    }
                    catch (Exception ecppp)
                    {
                        writeR(this.richTextBox1, "报错：" + ecppp.ToString());
                    }
                }
                else
                    Thread.Sleep(1000 * 60 * 10);
            }
        }
        /// richTextBox1 显示信息
        /// </summary>
        /// <param name="c">richTextBox1</param>
        /// <param name="aa">显示信息</param>
        public void writeR(RichTextBox c, string aa)
        {
            try
            {
                lock (this)
                {
                    c.Invoke(new ThreadStart(delegate()
                    {
                        c.Text += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + aa + "\r\n";
                        c.SelectionStart = c.TextLength;
                        c.ScrollToCaret();
                    }));
                }
            }
            catch { }
        }

        private void Formmjw_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认关闭", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
                e.Cancel = true;
            else
            {
                try
                {
                    if (t != null)
                    {
                        t.Abort();
                        System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                    }
                }
                catch { }
            }
        }
    }
}
