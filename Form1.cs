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
using System.IO;

namespace Mask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            webBrowser1.Url = new Uri("https://medvpn.nhi.gov.tw/iwse0000/IWSE0020S01.aspx");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile("https://data.nhi.gov.tw/resource/mask/maskdata.csv", ".\\maskdata.csv");
            FileStream fs = new FileStream(".\\maskdata.csv", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);

            //記錄每次讀取的一行記錄
            DataTable dt = new DataTable();
            string strLine = "";
            //記錄每行記錄中的各字段內容
            string[] aryLine = null;
            string[] tableHead = null;
            //標示列數
            int columnCount = 2;
            //標示是否是讀取的第一行
            bool IsFirst = true;
            //逐行讀取CSV中的數據
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //創建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        tableHead[i] = tableHead[i].Replace("\"", "");
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j].Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[2] + " " + "DESC";
            }
            sr.Close();
            fs.Close();

            DataRow[] drg = dt.Select("醫事機構代碼 = " + textBox1.Text);
            if(drg.Length==0)
            {
                name.Text = Convert.ToString("未知(表示已售完)");
                adult.Text = Convert.ToString("0");
                child.Text = Convert.ToString("0");
                time.Text = Convert.ToString("已售完");
            }
            else
            {
                name.Text = Convert.ToString(drg[0][1].ToString());
                adult.Text = Convert.ToString(drg[0][4].ToString());
                child.Text = Convert.ToString(drg[0][5].ToString());
                time.Text = Convert.ToString(drg[0][6].ToString());
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void adult_Click(object sender, EventArgs e)
        {

        }
    }
}
