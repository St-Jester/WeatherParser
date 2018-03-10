using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeWeather
{
    public struct WeatherCity
    {
        public string city;
        public string tempr;

        public WeatherCity(string _c, string _t)
        {
            city = _c;
            tempr = _t;
        }
    }

    public partial class Form1 : Form
    {
        int S = 0, M = 0, H = 0;


        string addrBarcelona = "https://www.gismeteo.ua/weather-barcelona-1948/";
        string addrRio = "https://www.gismeteo.ua/weather-rio-de-janeiro-10452/";
        string addrKiev = "https://www.gismeteo.ua/weather-kyiv-4944/";


        HttpWebRequest query;
        HttpWebResponse answer;
        List<WeatherCity> _items = new List<WeatherCity>();


        string codeToParce = "";
        public Form1()
        {
            InitializeComponent();
            //get weather info
            ProcessWebSite(addrBarcelona, @"..\..\Barcelona.txt");
            WriteInFile(@"..\..\Barcelona.txt", codeToParce);
            Parcer(codeToParce);
            
            ProcessWebSite(addrRio, @"..\..\Rio.txt");
            WriteInFile(@"..\..\Rio.txt", codeToParce);
            Parcer(codeToParce);

            ProcessWebSite(addrKiev, @"..\..\Kiev.txt");
            WriteInFile(@"..\..\Kiev.txt", codeToParce);
            Parcer(codeToParce);

        }

        private void Parcer(string codeToParce)
        {
            string temperatureFull = ParceTemperature(codeToParce);
            
            string cityName = ParceCity(codeToParce);

            Display(new WeatherCity(cityName, temperatureFull));
        }

        private string ParceCity(string codeToParce)
        {
            string anchor = "section higher";
            int length = anchor.Length;
           
            int pos = codeToParce.IndexOf(anchor) + length;

            string sub = codeToParce.Substring(pos);

            string name = sub.Split('>')[2];
            
            name = name.Split('<')[0];
            
            return name;
        }

        private string ParceTemperature(string codeToParce)
        {
            string anchor = "<dd class=\'value m_temp c\'>";
            int lengh = anchor.Length;
            
            int pos = codeToParce.IndexOf(anchor) + lengh;

            string sub = codeToParce.Substring(pos);
            string sign = sub.Split(';')[0];

            string target = sub.Split(';')[1];
            string tempr = sub.Split('<')[0];

            if (sign == "&minus")
            {
                tempr = $"-" + target.Split('<')[0];
            }
            return tempr;
        }

        private void Display(WeatherCity _item)
        {
            ListViewItem li = listView1.Items.Add(_item.city);
            li.SubItems.Add(_item.tempr);
        }


        string ProcessWebSite(string addr, string path)
        {
            GetAnswer(addr);

            codeToParce = SaveAnswer();

            return codeToParce;
        }

        void GetAnswer(string _addr)
        {
            query = (HttpWebRequest)
                HttpWebRequest.Create(_addr);
            answer = (HttpWebResponse)
                query.GetResponse();
        }

        string SaveAnswer()
        {
            Stream content = answer.GetResponseStream();
            StreamReader sr = new StreamReader(content);
            string code = sr.ReadToEnd();
            sr.Close();
            return code;
        }

        void WriteInFile(string _path, string _code)
        {
            FileStream fs = new FileStream(_path, FileMode.Create,
                FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(_code);
            sw.Close();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {

            if (listView1.Items != null)
            {
                listView1.Items.Clear();
            }

            ProcessWebSite(addrBarcelona, @"..\..\Barcelona.txt");
            Parcer(codeToParce);

            ProcessWebSite(addrRio, @"..\..\Rio.txt");
            Parcer(codeToParce);

            ProcessWebSite(addrKiev, @"..\..\Kiev.txt");
            Parcer(codeToParce);

            timer1.Start();
            S = 0;
            M = 0;
            H = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            S++;
            if(S >=59)
            {
                M++;
                S = 0;
            }
            if(M>=59)
            {
                H++;
                M = 0;
            }

            TimerLabel.Text = H.ToString() + ":" + M.ToString() + ":" + S.ToString();
        }
    }
}
