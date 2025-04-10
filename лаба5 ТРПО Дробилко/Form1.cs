using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json;

namespace лаба5_ТРПО_Дробилко
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        List<City> cities = new List<City>();

        BindingSource bs = new BindingSource();

        BindingSource Savebs = new BindingSource();

        public Form1()
        {
            InitializeComponent();

            Savebs.DataSource = new List<City>();
            bs.DataSource = new List<City>();

            // Здесь nameof возвращает строку названия свойства, 
            // к-рое в него передаем. В add() используем nameof, 
            // чтобы не ошибиться с название строки (например, "size", вместо правильного "Size")
            //label1.DataBindings.Add("Text", this, nameof(Size));
            /*label1.DataBindings.Add("Text", this, nameof(CurrentTime));

            t.Interval = 30;
            t.Tick += (o, e) => CurrentTime = DateTime.Now;
            t.Enabled = true;*/

            Savebs.Add(new City("Санкт-Петербург", 5000000, "Россия", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\spb.png"));
            Savebs.Add(new City("Стокгольм", 900000, "Швеция", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\stock.png"));
            Savebs.Add(new City("Париж", 2000000, "Франция", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\parij.png"));

            bs.Add(new City("Санкт-Петербург", 5000000, "Россия", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\spb.png"));
            bs.Add(new City("Стокгольм", 900000, "Швеция", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\stock.png"));
            bs.Add(new City("Париж", 2000000, "Франция", @"C:\Users\drobi\source\repos\лаба5 ТРПО Дробилко\лаба5 ТРПО Дробилко\bin\Debug\image\parij.png"));

            //dataGridView1.DataSource = bs;

            dataGridView1.DataSource = bs;
            bindingNavigator1.BindingSource = bs;
            textBox1.DataBindings.Add("Text", this, "CurrentMin");
            textBox1.DataBindings[0].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            propertyGrid1.DataBindings.Add("SelectedObject", bs, "");
            pictureBox1.DataBindings.Add("ImageLocation", bs, "Img", true);
            //pictureBox1.ImageLocation = (bs.Current as City).ImageFileName;

            chart1.DataSource = from w in bs.DataSource as List<City>
                                where w.Size > currentMin
                                group w by w.Country into g
                                select new { Country = g.Key, Avg = g.Average(w => w.Size) };
            chart1.Series[0].XValueMember = "Country";
            chart1.Series[0].YValueMembers = "Avg";
            chart1.Legends.Clear();
            chart1.Titles.Add("Население");

            bs.CurrentChanged += (o, e) => chart1.DataBind();
            //bs.CurrentChanged += ImgHandler;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.InitialDirectory = System.Environment.CurrentDirectory;
            sfd.Filter = "Файл в xml|*.xml|Файл в json|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:
                        LoadXml(sfd.FileName);
                        break;
                        
                    case 2:
                        LoadJson(sfd.FileName);
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = System.Environment.CurrentDirectory;
            sfd.Filter = "Файл в xml|*.xml|Файл в json|*.json";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1:
                        SaveXml(sfd.FileName);
                        break;

                    case 2:
                        SaveJson(sfd.FileName);
                        break;
                }
            }
        }

        private void SaveXml(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<City>));
            using (Stream sw = new FileStream(file, FileMode.Create))
            {
                ser.Serialize(sw, bs.DataSource);
            }
        }

        private void LoadJson(string file)
        {
            string ret = File.ReadAllText(file);
            bs.DataSource = JsonConvert.DeserializeObject<List<City>>(ret);
            Savebs.DataSource = JsonConvert.DeserializeObject<List<City>>(ret);
        }

        private void SaveJson(string file)
        {
            string json = JsonConvert.SerializeObject(bs.DataSource, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        private void LoadXml(string file)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<City>));
            using (Stream sw = new FileStream(file, FileMode.Open))
            {
                bs.DataSource = (List<City>)ser.Deserialize(sw);
            }

            using (Stream sw = new FileStream(file, FileMode.Open))
            {
                Savebs.DataSource = (List<City>)ser.Deserialize(sw);
            }
        }

        /*public void ImgHandler(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = (bs.Current as City).Img;
        }*/

        public DateTime CurrentTime
        {
            get => currentTime;
            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    CurrentTimeChanged(this, EventArgs.Empty);
                }
            }
        }

        public int CurrentMin
        {
            get => currentMin;
            set
            {
                if (currentMin != value)
                {
                    currentMin = value;
                    CurrentMinChanged(this, EventArgs.Empty);

                    Savebs.MoveFirst();
                    bs.Clear();
                    for (int i = 0; i < Savebs.Count; i++)
                    {
                        bs.Add(Savebs.Current as City);
                        Savebs.MoveNext();
                    }

                    Savebs.MoveFirst();
                    bs.MoveFirst();
                    for(int i = 0; i < Savebs.Count; i++)
                    {
                        if((Savebs.Current as  City).Size < CurrentMin) 
                        {
                            bs.RemoveCurrent();
                            Savebs.MoveNext();
                        }
                        else
                        {
                            bs.MoveNext();
                            Savebs.MoveNext();
                        }
                    }
                }
            }
        }

        private int currentMin = 0;
        public event EventHandler CurrentMinChanged;

        private DateTime currentTime;

        public event EventHandler CurrentTimeChanged;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            bs.Add(new City("", 0, ""));
            bs.MoveLast();
            Savebs.Add(new City("", 0, ""));
            Savebs.MoveLast();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            (bs.Current as City).Img = openFileDialog1.FileName;
            bs.ResetBindings(false);
            (Savebs.Current as City).Img = openFileDialog1.FileName;
            Savebs.ResetBindings(false);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            Savebs.MoveFirst();
            bs.MoveFirst();
            for (int i = 0; i < Savebs.Count; i++)
            {
                if (!((Savebs.Current as City).Size < CurrentMin) && !bs.Contains(Savebs.Current)) 
                {
                    Savebs.RemoveCurrent();
                    break;
                }
                Savebs.MoveNext();
                bs.MoveNext();
            }
        }
    }

    public class City
    {
        static public int i = 0;
        private string name;
        private int size;
        private string country;
        private string img;

        [DisplayName("Название"), Category("Сводка")]
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }

        [DisplayName("Население"), Category("Сводка")]
        public int Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                }
            }
        }

        [DisplayName("Страна"), Category("Сводка")]
        public string Country
        {
            get => country;
            set
            {
                if (country != value)
                {
                    country = value;
                }
            }
        }

        //[Browsable(false)]
        public string Img
        {
            get => img;
            set
            {
                img = value;
            }
        }

        public City(string name, int size, string country)
        {
            this.name = name;
            this.size = size;
            this.country = country;
            this.img = @"";
        }

        public City(string name, int size, string country, string img)
        {
            this.name = name;
            this.size = size;
            this.country = country;
            this.img = img;
        }

        public City()
        {

        }
    }
}
