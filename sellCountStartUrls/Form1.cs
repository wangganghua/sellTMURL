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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Thread t = null;
        Thread t2 = null;
        Thread t3 = null;
        //string connectionstring2 = "server=192.168.2.245;uid=sa;pwd=3132_deeposh_0083;database=sanc";
       string connectionstring2  =  "server=192.168.2.236;uid=sa;pwd=All_View_Consulting_2014@;database=CHData";
        dbHelp db;

        private void Form1_Load(object sender, EventArgs e)
        {
            db = new dbHelp(connectionstring2);
            //周度
            t = new Thread(NewMethod_test);
            t.Start();
            //日度 暂停使用
            //t2 = new Thread(DailyNewMethod);
            //t2.Start();
           
        }
        // wgh : 0:05、10:05、16:05、21:05 修改采集时间
        private void NewMethod()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 13 && DateTime.Parse(curenttime).Minute>=5)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 0:10:00");
          //      //curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 0:05:00");
            else if (DateTime.Parse(curenttime).Hour >= 16 && DateTime.Parse(curenttime).Minute >= 5)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 21:05:00");
            else if (DateTime.Parse(curenttime).Hour >= 10 && DateTime.Parse(curenttime).Minute >= 5)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:05:00");
            else
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:05:00");

            this.richTextBox1.Invoke(new ThreadStart(delegate()
            {
                richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
            writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");

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
                            richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.ScrollToCaret();
                        }));
                        writeR(this.richTextBox1, "开始查询URL...");
                        string sqlStr = "select  '{\"Urls\": \"'+REPLACE(Urls,'；',';')+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where need=1";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox1, "URL插入redis 共有："+urlList.Count+" 条");
                        RedisClient client = new RedisClient("117.122.192.50", 6479);
                        //RedisClient client = new RedisClient("192.168.2.245", 6379);
                        client.AddRangeToList("sellCountSpider:start_urls", urlList);
                        //client.AddRangeToList("DailySellCountSpider:start_urls", urlList);
                        writeR(this.richTextBox1, "成功，等待下次...");

                        if (DateTime.Parse(curenttime).Hour >= 13 && DateTime.Parse(curenttime).Minute >= 5)
                        //if (DateTime.Parse(curenttime).Hour >= 20 && DateTime.Parse(curenttime).Minute >= 5)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:10:00");
                        else if (DateTime.Parse(curenttime).Hour >= 16 && DateTime.Parse(curenttime).Minute >= 5)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 21:05:00");
                        else if (DateTime.Parse(curenttime).Hour >= 10 && DateTime.Parse(curenttime).Minute >= 5)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:05:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 10:05:00");

                        writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");
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
        //0        8         16   
        // wgh : 0:05、10:05、16:05、21:05 修改采集时间
        private void NewMethod_old()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 16)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            else if (DateTime.Parse(curenttime).Hour >= 8)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
            else
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");

            this.richTextBox1.Invoke(new ThreadStart(delegate()
            {
                richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
            writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");

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
                            richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.ScrollToCaret();
                        }));
                        writeR(this.richTextBox1, "开始查询URL...");
                        string sqlStr = "select '{\"Urls\": \"'+Urls+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where need=1";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox1, "URL插入redis 共有：" + urlList.Count + "...");
                        RedisClient client = new RedisClient("117.122.192.50", 6479);
                        //client.LPush("proxytest:start_urls", Encoding.UTF8.GetBytes("11111111111111111111111111111111111"));
                        client.AddRangeToList("sellCountSpider:start_urls", urlList);
                        writeR(this.richTextBox1, "成功，等待下次...");

                        if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");
                        writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");
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
                richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.ScrollToCaret();
            }));
            writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");

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
                            richTextBox1.Text = "【周报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.ScrollToCaret();
                        }));
                        writeR(this.richTextBox1, "开始查询URL...");
                        string sqlStr = "select top 10000 '{\"Urls\": \"'+REPLACE(Urls,'；',';')+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where need=1 ";
                        //sqlStr = "select '{\"Urls\": \"'+REPLACE(Urls,'；',';')+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where need=1 and urlleibie in('智能手机','净化器','净水器','彩电','空调','冰箱','冰柜','洗衣机','厨电套餐','油烟机','燃气灶','消毒柜','热水器','微波炉','电烤箱','电蒸箱','洗碗机')";
                        //sqlStr = "select '{\"Urls\": \"'+REPLACE(Urls,'；',';')+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where need=1 and urlleibie not in('智能手机','净化器','净水器','彩电','空调','冰箱','冰柜','洗衣机','厨电套餐','油烟机','燃气灶','消毒柜','热水器','微波炉','电烤箱','电蒸箱','洗碗机')";
                        //sqlStr = "select '{\"Urls\": \"'+REPLACE(Urls,'；',';')+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from urldata_tm where convert(nvarchar(10),writetime,120)>='2017-03-15' and need=1 and urlleibie in('电风扇','净化器','净水器','彩电','空调','冰箱','冰柜','洗衣机','厨电套餐','油烟机','燃气灶','消毒柜','热水器','微波炉','电烤箱','电蒸箱','洗碗机')";
                        sqlStr = "select '{\"Urls\": \"'+REPLACE(页面信息,'；',';')+'\",\"Urlleibie\": \"'+品类+'\",\"Urlweb\": \"'+电商+'\",\"spbjpinpai\": \"'+ISNULL(品牌,'')+'\",\"spbjjixing\": \"'+ISNULL(机型,'')+'\",\"pc\":\"" + curenttime + "\"}' from CHDATA..URLDATA where 电商='天猫商城' AND need=0 and 品类='彩电'";
                        //sqlStr = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM CHDATA..URLDATA WHERE NEED=0 AND 品类='彩电' AND 电商='苏宁易购'";
                        sqlStr = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM CHDATA..URLDATA WHERE NEED=0 AND 品类 in('彩电') AND (  电商='苏宁易购')";
                        //sqlStr = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM CHDATA..URLDATA WHERE NEED=0 AND 品类 in('智能马桶','新风系统') AND (电商='京东商城')";
                        //sqlStr = "SELECT '{\"url\": \"'+页面信息+'\",\"attr\": {\"urlleibie\": \"'+品类+'\",\"urlweb\": \"'+电商+'\",\"brand\": \"'+品牌+'\",\"model\": \"'+机型+'\"}}' FROM CHDATA..URLDATA WHERE NEED=0 AND 电商  not in('苏宁易购','国美在线','天猫商城')";
                        //sqlStr = "SELECT '{\"url\": \"'+A.页面信息+'\",\"attr\": {\"urlleibie\": \"'+A.品类+'\",\"urlweb\": \"'+A.电商+'\",\"brand\": \"\",\"model\": \"\"}}' FROM (SELECT 页面信息,电商,品类 FROM CHDATA..URLDATAJD WHERE (NEED=0 OR NEED IS NULL) AND ISNULL(品类,'')!='' and 品类='彩电' and 电商 in('京东商城','苏宁易购') " +
                        //    ")A LEFT JOIN (SELECT 页面信息,电商,品类 FROM CHDATA..URLDATA WHERE NEED=0 and 品类='彩电' and 电商 in('京东商城','苏宁易购'))B  " +
                        //   "ON  A.页面信息 = B.页面信息 WHERE B.页面信息 IS NULL";

                        //sqlStr = "SELECT '{\"url\": \"'+A.页面信息+'\",\"attr\": {\"urlleibie\": \"'+A.品类+'\",\"urlweb\": \"'+A.电商+'\",\"brand\": \"''\",\"model\": \"''\"}}' FROM (SELECT 页面信息,电商,品类 FROM CHDATA..URLDATAJD WHERE (NEED=0 OR NEED IS NULL) AND ISNULL(品类,'')!='' and 品类 in('智能马桶','新风系统') and 电商 in('京东商城') " +
                        //  ")A LEFT JOIN (SELECT 页面信息,电商,品类 FROM CHDATA..URLDATA WHERE NEED=0 and 品类  in('智能马桶','新风系统') and 电商 in('京东商城'))B  " +
                         //"ON  A.页面信息 = B.页面信息 WHERE B.页面信息 IS NULL";
                        sqlStr = "select '{\"Urls\": \"'+ax.页面信息+'\",\"Urlleibie\": \"'+品类+'\",\"Urlweb\": \"天猫商城\",\"spbjpinpai\": \"'+品牌+'\",\"spbjjixing\": \"'+机型+'\",\"pc\": \"\"}'  from (select * from ( select * from dpcdata..线上周度补码表 where 周度='18w12' and(电商='天猫商城') )a left join (select * from dpcdata.._bf_wghurl)b on spurl=页面信息 where spurl is null )ax";

                        DataTable dt = db.selectDatas(sqlStr);
                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox1, "URL插入redis 共有：" + urlList.Count + "...");
                        //RedisClient client = new RedisClient("117.122.192.50", 6479);
                        RedisClient client = new RedisClient("117.23.4.139", 15480);
                        //client.LPush("proxytest:start_urls", Encoding.UTF8.GetBytes("11111111111111111111111111111111111"));
                        //client.AddRangeToList("sellCountSpider:start_urls", urlList);              
                        client.AddRangeToList("comment_spider_test:start_urls_info", urlList);           
                        //client.AddRangeToList("mjd_comments_spider:start_urls_info", urlList);           
                        writeR(this.richTextBox1, "成功，等待下次...");

                        if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 08:00:00");
                        writeR(this.richTextBox1, "【周报】下次时间:" + curenttime + "...");
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

        //0 8 16 23
        private void DailyNewMethod()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 23)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            else if (DateTime.Parse(curenttime).Hour > 16)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 23:00:00");
            else if (DateTime.Parse(curenttime).Hour > 8)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
            else
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 8:00:00");

            this.richTextBox2.Invoke(new ThreadStart(delegate()
            {
                richTextBox2.Text = "【日报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox2.SelectionStart = richTextBox2.TextLength;
                richTextBox2.ScrollToCaret();
            }));
            writeR(this.richTextBox2, "【日报】下次时间:" + curenttime + "...");

            while (true)
            {
                if (DateTime.Now >= DateTime.Parse(curenttime))
                {
                    try
                    {
                        this.richTextBox2.Invoke(new ThreadStart(delegate()
                        {
                            if (this.richTextBox2.Lines.Length > 40)
                                richTextBox2.Clear();
                            richTextBox2.Text = "【日报】采集......请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox2.SelectionStart = richTextBox2.TextLength;
                            richTextBox2.ScrollToCaret();
                        }));
                        writeR(this.richTextBox2, "开始查询URL...");

                        string sqlStr = "select '{\"Urls\": \"'+Urls+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from(select * from URLDATA_TM where (spbjpinpai like '%海尔%' or spbjpinpai like '%西门子%' or spbjpinpai like '%美的%' or spbjpinpai like '%容声%' or spbjpinpai like '%美菱%' or spbjpinpai like '%康佳%' or spbjpinpai like '%奥马%' or spbjpinpai like '%TCL%' or spbjpinpai like '%三星%' or spbjpinpai like '%海信%' or spbjpinpai like '%韩电%' or spbjpinpai like '%创维%' or spbjpinpai like '%松下%' or spbjpinpai like '%新飞%' or spbjpinpai like '%博世%' or spbjpinpai like '%澳柯玛%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%晶弘%' or spbjpinpai like '%惠而浦%' or spbjpinpai like '%卡萨帝%' or spbjpinpai like '%统帅%') and Urlleibie='冰箱' union select * from URLDATA_TM where (spbjpinpai like '%小天鹅%' or spbjpinpai like '%海尔%' or spbjpinpai like '%西门子%' or spbjpinpai like '%美的%' or spbjpinpai like '%三洋帝度%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%TCL%' or spbjpinpai like '%松下%' or spbjpinpai like '%LG%' or spbjpinpai like '%三星%' or spbjpinpai like '%博世%' or spbjpinpai like '%大宇%' or spbjpinpai like '%海信%' or spbjpinpai like '%康佳%' or spbjpinpai like '%创维%' or spbjpinpai like '%荣事达%' or spbjpinpai like '%惠而浦%' or spbjpinpai like '%吉德%' or spbjpinpai like '%韩电%' or spbjpinpai like '%卡萨帝%' or spbjpinpai like '%统帅%') and Urlleibie='洗衣机' union select * from URLDATA_TM where (spbjpinpai like '%奥克斯%' or spbjpinpai like '%美的%' or spbjpinpai like '%格力%' or spbjpinpai like '%海尔%' or spbjpinpai like '%科龙%' or spbjpinpai like '%TCL%' or spbjpinpai like '%海信%' or spbjpinpai like '%志高%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%长虹%' or spbjpinpai like '%松下%' or spbjpinpai like '%统帅%' or spbjpinpai like '%富士通%' or spbjpinpai like '%三菱电机%' or spbjpinpai like '%扬子%' or spbjpinpai like '%美博%' or spbjpinpai like '%日立%' or spbjpinpai like '%万宝%' or spbjpinpai like '%泰瑞达%' or spbjpinpai like '%现代%') and Urlleibie='空调' union select * from URLDATA_TM where (spbjpinpai like '%创维%' or spbjpinpai like '%海信%' or spbjpinpai like '%TCL%' or spbjpinpai like '%长虹%' or spbjpinpai like '%康佳%' or spbjpinpai like '%海尔%' or spbjpinpai like '%三星%' or spbjpinpai like '%LG%' or spbjpinpai like '%索尼%' or spbjpinpai like '%夏普%' or spbjpinpai like '%乐视%' or spbjpinpai like '%小米%' or spbjpinpai like '%微鲸%' or spbjpinpai like '%酷开%' or spbjpinpai like '%KKTV%' or spbjpinpai like '%PPTV%' or spbjpinpai like '%mooka%' or spbjpinpai like '%看尚%' or spbjpinpai like '%暴风%' or spbjpinpai like '%风行%' or spbjpinpai like '%17TV%') and Urlleibie='彩电') A";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox2, "URL插入redis 共有：" + urlList.Count + "...");
                        RedisClient client = new RedisClient("117.122.192.50", 6479);
                        //client.LPush("proxytest:start_urls", Encoding.UTF8.GetBytes("11111111111111111111111111111111111"));
                        client.AddRangeToList("DailySellCountSpider:start_urls", urlList);

                        writeR(this.richTextBox2, "成功，等待下次...");
                        if (DateTime.Parse(curenttime).Hour >= 23)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 23:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 8:00:00");
                        writeR(this.richTextBox2, "【日报】下次时间:" + curenttime + "...");
                    }
                    catch (Exception ecppp)
                    {
                        writeR(this.richTextBox2, "报错：" + ecppp.ToString());
                    }
                }
                else
                    Thread.Sleep(1000 * 60 * 10);
            }
        }

        // 改成一次：每天6点
        private void sixDailyNewMethod()
        {
            string curenttime = DateTime.Now.ToString();
            if (DateTime.Parse(curenttime).Hour >= 6)
                curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 6:00:00");
            else if (DateTime.Parse(curenttime).Hour > 16)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 23:00:00");
            else if (DateTime.Parse(curenttime).Hour > 8)
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
            else
                curenttime = DateTime.Now.ToString("yyyy-MM-dd 8:00:00");

            this.richTextBox2.Invoke(new ThreadStart(delegate()
            {
                richTextBox2.Text = "【日报】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox2.SelectionStart = richTextBox2.TextLength;
                richTextBox2.ScrollToCaret();
            }));
            writeR(this.richTextBox2, "【日报】下次时间:" + curenttime + "...");

            while (true)
            {
                if (DateTime.Now >= DateTime.Parse(curenttime))
                {
                    try
                    {
                        this.richTextBox2.Invoke(new ThreadStart(delegate()
                        {
                            if (this.richTextBox2.Lines.Length > 40)
                                richTextBox2.Clear();
                            richTextBox2.Text = "【日报】采集......请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox2.SelectionStart = richTextBox2.TextLength;
                            richTextBox2.ScrollToCaret();
                        }));
                        writeR(this.richTextBox2, "开始查询URL...");

                        string sqlStr = "select '{\"Urls\": \"'+Urls+'\",\"Urlleibie\": \"'+Urlleibie+'\",\"Urlweb\": \"'+Urlweb+'\",\"spbjpinpai\": \"'+ISNULL(spbjpinpai,'')+'\",\"spbjjixing\": \"'+ISNULL(spbjjixing,'')+'\",\"pc\":\"" + curenttime + "\"}' from(select * from URLDATA_TM where (spbjpinpai like '%海尔%' or spbjpinpai like '%西门子%' or spbjpinpai like '%美的%' or spbjpinpai like '%容声%' or spbjpinpai like '%美菱%' or spbjpinpai like '%康佳%' or spbjpinpai like '%奥马%' or spbjpinpai like '%TCL%' or spbjpinpai like '%三星%' or spbjpinpai like '%海信%' or spbjpinpai like '%韩电%' or spbjpinpai like '%创维%' or spbjpinpai like '%松下%' or spbjpinpai like '%新飞%' or spbjpinpai like '%博世%' or spbjpinpai like '%澳柯玛%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%晶弘%' or spbjpinpai like '%惠而浦%' or spbjpinpai like '%卡萨帝%' or spbjpinpai like '%统帅%') and Urlleibie='冰箱' union select * from URLDATA_TM where (spbjpinpai like '%小天鹅%' or spbjpinpai like '%海尔%' or spbjpinpai like '%西门子%' or spbjpinpai like '%美的%' or spbjpinpai like '%三洋帝度%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%TCL%' or spbjpinpai like '%松下%' or spbjpinpai like '%LG%' or spbjpinpai like '%三星%' or spbjpinpai like '%博世%' or spbjpinpai like '%大宇%' or spbjpinpai like '%海信%' or spbjpinpai like '%康佳%' or spbjpinpai like '%创维%' or spbjpinpai like '%荣事达%' or spbjpinpai like '%惠而浦%' or spbjpinpai like '%吉德%' or spbjpinpai like '%韩电%' or spbjpinpai like '%卡萨帝%' or spbjpinpai like '%统帅%') and Urlleibie='洗衣机' union select * from URLDATA_TM where (spbjpinpai like '%奥克斯%' or spbjpinpai like '%美的%' or spbjpinpai like '%格力%' or spbjpinpai like '%海尔%' or spbjpinpai like '%科龙%' or spbjpinpai like '%TCL%' or spbjpinpai like '%海信%' or spbjpinpai like '%志高%' or spbjpinpai like '%格兰仕%' or spbjpinpai like '%长虹%' or spbjpinpai like '%松下%' or spbjpinpai like '%统帅%' or spbjpinpai like '%富士通%' or spbjpinpai like '%三菱电机%' or spbjpinpai like '%扬子%' or spbjpinpai like '%美博%' or spbjpinpai like '%日立%' or spbjpinpai like '%万宝%' or spbjpinpai like '%泰瑞达%' or spbjpinpai like '%现代%') and Urlleibie='空调' union select * from URLDATA_TM where (spbjpinpai like '%创维%' or spbjpinpai like '%海信%' or spbjpinpai like '%TCL%' or spbjpinpai like '%长虹%' or spbjpinpai like '%康佳%' or spbjpinpai like '%海尔%' or spbjpinpai like '%三星%' or spbjpinpai like '%LG%' or spbjpinpai like '%索尼%' or spbjpinpai like '%夏普%' or spbjpinpai like '%乐视%' or spbjpinpai like '%小米%' or spbjpinpai like '%微鲸%' or spbjpinpai like '%酷开%' or spbjpinpai like '%KKTV%' or spbjpinpai like '%PPTV%' or spbjpinpai like '%mooka%' or spbjpinpai like '%看尚%' or spbjpinpai like '%暴风%' or spbjpinpai like '%风行%' or spbjpinpai like '%17TV%') and Urlleibie='彩电') A";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox2, "URL插入redis 共有：" + urlList.Count + "...");
                        RedisClient client = new RedisClient("117.122.192.50", 6479);
                        //client.LPush("proxytest:start_urls", Encoding.UTF8.GetBytes("11111111111111111111111111111111111"));
                        client.AddRangeToList("DailySellCountSpider:start_urls", urlList);

                        writeR(this.richTextBox2, "成功，等待下次...");
                        if (DateTime.Parse(curenttime).Hour >= 23)
                            curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 16)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 23:00:00");
                        else if (DateTime.Parse(curenttime).Hour >= 8)
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 16:00:00");
                        else
                            curenttime = DateTime.Now.ToString("yyyy-MM-dd 8:00:00");
                        writeR(this.richTextBox2, "【日报】下次时间:" + curenttime + "...");
                    }
                    catch (Exception ecppp)
                    {
                        writeR(this.richTextBox2, "报错：" + ecppp.ToString());
                    }
                }
                else
                    Thread.Sleep(1000 * 60 * 10);
            }
        }


        //0 8 16 23//暂停使用。
        private void plNewMethod()
        {
            string curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:10:00");

            this.richTextBox3.Invoke(new ThreadStart(delegate()
            {
                richTextBox3.Text = "【评论】采集......\r\n请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                richTextBox3.SelectionStart = richTextBox3.TextLength;
                richTextBox3.ScrollToCaret();
            }));
            writeR(this.richTextBox3, "【评论】下次时间:" + curenttime + "...");

            while (true)
            {
                if (DateTime.Now >= DateTime.Parse(curenttime))
                {
                    try
                    {
                        this.richTextBox3.Invoke(new ThreadStart(delegate()
                        {
                            if (this.richTextBox3.Lines.Length > 40)
                                richTextBox3.Clear();
                            richTextBox3.Text = "【评论】采集......请不要关闭程序......\r\n如有意外，请联系采集人员......\r\n";
                            richTextBox3.SelectionStart = richTextBox3.TextLength;
                            richTextBox3.ScrollToCaret();
                        }));
                        writeR(this.richTextBox3, "开始查询URL...");

                        string sqlStr = "select '{\"Urls\": \"'+Urls+'\",\"urlweb\":\"'+Urlweb+'\",\"urlleibie\":\"'+Urlleibie+'\"}' from URLDATA";
                        DataTable dt = db.selectDatas(sqlStr);

                        List<string> urlList = new List<string>();
                        foreach (DataRow item in dt.Rows)
                        {
                            urlList.Add(item[0].ToString());
                        }
                        writeR(this.richTextBox3, "URL插入redis 共有：" + urlList.Count + "...");
                        RedisClient client = new RedisClient("117.122.192.50", 6479);
                        client.AddRangeToList("comment_spider:start_urls", urlList);

                        writeR(this.richTextBox3, "成功，等待下次...");
                        curenttime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd 00:10:00");
                        writeR(this.richTextBox3, "【评论】下次时间:" + curenttime + "...");
                    }
                    catch (Exception ecppp)
                    {
                        writeR(this.richTextBox3, "报错：" + ecppp.ToString());
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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
                    if (t2 != null)
                    {
                        t2.Abort();
                        System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    if (t3 != null)
                    {
                        t3.Abort();
                        System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                    }
                }
                catch { }
            }
        }
    }

    public class dbHelp
    {
        //server=192.168.2.245;uid=sa;pwd=3132_deeposh_0083;database=sanc
        static string connectionstring = "";
        //带参构造函数
        public dbHelp(string connectionStr)
        {
            connectionstring = connectionStr;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int insertDatas(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1 = new SqlCommand(sql, conn);
                    cmd1.CommandTimeout = 60 * 60 * 5;
                    return cmd1.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public bool ExecuteSqlTran(ArrayList SQLStringList)
        {
            lock (this)
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 600;
                    SqlTransaction tx = conn.BeginTransaction();
                    cmd.Transaction = tx;
                    try
                    {
                        for (int n = 0; n < SQLStringList.Count; n++)
                        {
                            try
                            {
                                string strsql = SQLStringList[n].ToString();
                                if (strsql.Trim().Length > 1)
                                {
                                    cmd.CommandText = strsql;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch { }
                        }
                        if (tx.Connection != null)
                            tx.Commit();
                        return true;
                    }
                    catch
                    {
                        try
                        {
                            if (tx.Connection != null)
                                tx.Rollback();
                        }
                        catch { }
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public DataTable selectDatas(string sql, params SqlParameter[] pars)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.SelectCommand.Parameters.AddRange(pars);
                    da.SelectCommand.CommandTimeout = 3000;
                    da.Fill(dt);
                }
            }
            catch (Exception ecp)
            {
                //MessageBox.Show("SQL查询异常：" + ecp.Message);
            }
            return dt;
        }
    }
}
