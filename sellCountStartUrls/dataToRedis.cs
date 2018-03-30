using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceStack.Redis;
using Newtonsoft.Json.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace sellCountStartUrls
{
    public partial class dataToRedis : Form
    {
        public dataToRedis()
        {
            InitializeComponent();
        }

        string connectionstring2 = "server=192.168.2.245;uid=sa;pwd=3132_deeposh_0083;database=sanc";
        dbHelp db;
        Thread t;
        /***
        {
            "datatype": "京东预约数URL",
            "datasource": "sql",
            "redishost": "192.168.100.200",
            "redisport": "6379",
            "rediskey": "yuyue:start_urls",
            "pattern": "select '{\"Urls\":\"'+Urls+'\"}' from URLDATA where Urlweb='JD' and Urlleibie='彩电'",
            "starttime": "2016-08-15 22:30:00",
            "nextaddmin": "1440"
        }
        ***/

        private void dataToRedis_Load(object sender, EventArgs e)
        {
            db = new dbHelp(connectionstring2);
            t = new Thread(NewMethod);
            t.Start();
        }

        private void NewMethod()
        {
            while (true)
            {
                RedisClient client = new RedisClient("192.168.100.200", 6379);
                try
                {
                    byte[] taskbyte = client.RPop("datatoredis:task");
                    if (taskbyte != null)
                    {
                        string taskinfo = System.Text.Encoding.UTF8.GetString(taskbyte);
                        if (taskinfo != "")
                        {
                            var jo = JObject.Parse(taskinfo);
                            string datatype = Convert.ToString(jo["datatype"]);
                            string datasource = Convert.ToString(jo["datasource"]);
                            string redishost = Convert.ToString(jo["redishost"]);
                            int redisport = Convert.ToInt32(jo["redisport"]);
                            string rediskey = Convert.ToString(jo["rediskey"]);
                            string pattern = Convert.ToString(jo["pattern"]);
                            string starttime = Convert.ToString(jo["starttime"]);
                            int nextaddmin = Convert.ToInt32(jo["nextaddmin"]);
                            if (datasource.ToLower() == "sql")
                            {
                                if (DateTime.Now >= Convert.ToDateTime(starttime))
                                {
                                    if (nextaddmin > 0)
                                    {
                                        starttime = Convert.ToDateTime(starttime).AddMinutes(nextaddmin).ToString("yyyy-MM-dd HH:mm:ss");
                                        jo["starttime"] = starttime;
                                        taskinfo = JsonConvert.SerializeObject(jo);
                                        client.LPush("datatoredis:task", Encoding.UTF8.GetBytes(taskinfo));
                                        writeR(this.richTextBox1, "LPush to [datatoredis:task] " + taskinfo);
                                    }
                                    DataTable dtResult = db.selectDatas(pattern);
                                    writeR(this.richTextBox1, "Get pattern for result [" + pattern + "]");
                                    writeR(this.richTextBox1, "Get result " + dtResult.Rows.Count);
                                    List<string> urlList = new List<string>();
                                    foreach (DataRow item in dtResult.Rows)
                                    {
                                        urlList.Add(item[0].ToString());
                                    }
                                    RedisClient dataClient = new RedisClient(redishost, redisport);
                                    dataClient.AddRangeToList(rediskey, urlList);
                                    writeR(this.richTextBox1, "Souccess");
                                    writeR(this.richTextBox1, "***************");
                                    writeR(this.richTextBox1, "***************");
                                }
                                else
                                {
                                    taskinfo = JsonConvert.SerializeObject(jo);
                                    client.LPush("datatoredis:task", Encoding.UTF8.GetBytes(taskinfo));
                                }
                            }
                        }
                    }
                }
                catch (Exception ecp)
                {
                    writeR(this.richTextBox1, "[Error]" + ecp.Message);
                }
                Thread.Sleep(1000 * 60 * 5);
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

        private void dataToRedis_FormClosing(object sender, FormClosingEventArgs e)
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
