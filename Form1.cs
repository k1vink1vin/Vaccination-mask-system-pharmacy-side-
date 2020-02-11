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
using Microsoft.Win32;
using System.Diagnostics;
using System.Timers;

namespace Mask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            SetWebBrowserFeatures(11);
            webBrowser1.Url = new Uri("https://medvpn.nhi.gov.tw/iwse0000/IWSE0020S01.aspx");

            domainUpDown1.Items.Add(1);
            domainUpDown1.Items.Add(2);
            domainUpDown1.Items.Add(3);
            domainUpDown1.Items.Add(4);
            domainUpDown1.Items.Add(5);
            domainUpDown1.Items.Add(6);
            domainUpDown1.Items.Add(7);
            domainUpDown1.Items.Add(8);
            domainUpDown1.Items.Add(9);
            //預設COM 3
            domainUpDown1.SelectedIndex = 2;

        }

        
        static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //獲取程序及名稱
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到瀏覽器的模式的值
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //設置瀏覽器對應用程序（appName）以什麼模式（ieMode）運行
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser
            //不曉得設置有什麼用
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",appName, 1, RegistryValueKind.DWord);
        }

        /// 通過版本得到瀏覽器模式的值  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            data_update();
        }

        private void data_update()
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
                        DataColumn dc = new DataColumn(tableHead[i], typeof(string));
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
            String a = "醫事機構代碼=" + "'" + Convert.ToString(textBox1.Text.ToString()) + "'";
            DataRow[] drg = dt.Select(a);
            
            if (drg.Length == 0)
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


        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (webBrowser1.IsBusy)
            {
                label3.Text = "系統繁忙中";
                label3.ForeColor = Color.Red;
            }
            else
            {
                label3.Text = "系統閒置中";
                label3.ForeColor = Color.Black;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c" + "start " + Application.StartupPath + "/Security/csUpdate.exe " + (domainUpDown1.SelectedIndex);
            process.StartInfo.UseShellExecute = false;   //是否使用作業系統shell啟動 
            process.StartInfo.CreateNoWindow = false;   //是否在新視窗中啟動該程序的值 (不顯示程式視窗)
            process.Start();
            process.WaitForExit();  //等待程式執行完退出程序
            process.Close();

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            data_update();
        }
    }
}
