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
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;

namespace grafikDenemeNumarasi1
{
    public partial class Form1 : Form
    {
        // Sütun isimlerini oku ve string listesinde sakla
        List<string> sutunIsimleri = new List<string>();
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // CSV dosyasının yolu
            string dosyaYolu = "C:\\samet\\how_to_expand.csv";

            // Verileri oku ve DataGridView'e ekle
            List<string[]> veriler = CsvDosyasiniOku(dosyaYolu);




            //burada sutunIsımleri tutuluyor ilk elemeani dogru degil
            using (var reader = new StreamReader(dosyaYolu))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (sutunIsimleri.Count == 0)
                    {
                        sutunIsimleri.AddRange(values);
                        break;
                    }
                }
            }
            DataGridViewTextBoxColumn[] sutunlar = new DataGridViewTextBoxColumn[veriler[0].Length];
            if (veriler != null && veriler.Count > 0)
            {
                // DataGridView'e sütunları ve verileri ekle

                // Sütunları temsil eden bir dizi


                // Her bir başlık sütunu için
                for (int i = 0; i < veriler[0].Length; i++)
                {
                    // Yeni bir sütun oluştur
                    sutunlar[i] = new DataGridViewTextBoxColumn();

                    // Sütun başlığını CSV dosyasındaki başlıktan al
                    sutunlar[i].HeaderText = veriler[0][i];

                    // DataGridView'e sütunu ekle
                    dataGridView1.Columns.Add(sutunlar[i]);
                }

                // Veri satırlarını eklemek için döngü
                for (int i = 1; i < veriler.Count; i++)
                {
                    // DataGridView'e yeni bir satır ekle ve verileri yerleştir
                    dataGridView1.Rows.Add(veriler[i]);
                }
            }

            //veriler boyle aliniyo
            //label10.Text = string.Join(", ", veriler[1]);


            //bu dizinin ismini ayarlama
            object dizininAdı = dataGridView1.Rows[0].Cells[0].Value;


            // İlk satırın [0,1], [0,2], [0,3] elemanlarını saklamak için bir dizi

            int[] salesIncrease = new int[]
            {
            Convert.ToInt32(dataGridView1.Rows[0].Cells[1].Value),
            Convert.ToInt32(dataGridView1.Rows[0].Cells[2].Value),
            Convert.ToInt32(dataGridView1.Rows[0].Cells[3].Value),
            Convert.ToInt32(dataGridView1.Rows[0].Cells[4].Value)
            };


            // İkinci satırın [1,1], [1,2], [1,3] elemanlarını saklamak için bir dizi
            int[] salesDecrease = new int[]
            {
            Convert.ToInt32(dataGridView1.Rows[1].Cells[1].Value),
            Convert.ToInt32(dataGridView1.Rows[1].Cells[2].Value),
            Convert.ToInt32(dataGridView1.Rows[1].Cells[3].Value),
            Convert.ToInt32(dataGridView1.Rows[1].Cells[4].Value)
            };

            label8.Text = "building HQ (" + Optimistic(salesDecrease).ToString() + ")";
            label9.Text = "collaboration (" + Pesimistic(salesIncrease).ToString() + ")";
            label10.Text = "expansion (" + LaPlace(salesIncrease, salesDecrease).ToString() + ")";
            label11.Text = "expansion (" + Savage(salesIncrease, salesDecrease).ToString() + ")";





            double[,] hurwiczDizisi = new double[11, 4];

            // Hurwicz matrisinin hesaplanması

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    double hurwiczSonuc = (i * salesIncrease[j] + (10 - i) * salesDecrease[j]) / 10.0;
                    hurwiczDizisi[i, j] = hurwiczSonuc;
                }
            }

            // Şimdi sütunları ekle ve başlıkları ata
            for (int i = 0; i < (veriler[0].Length); i++)
            {
                // Yeni bir sütun oluştur
                sutunlar[i] = new DataGridViewTextBoxColumn();

                // Sütun başlığını CSV dosyasındaki başlıktan al
                sutunlar[i].HeaderText = veriler[0][i];
                // DataGridView'e sütunu ekle
                dataGridView2.Columns.Add(sutunlar[i]);
            }
            // Önce satırları ekle
            dataGridView2.Columns[0].HeaderText = "h";

            double hValue = 0.0;
            for (int i = 0; i < 11; i++)
            {
                dataGridView2.Rows.Add();
                dataGridView2.Rows[i].Cells[0].Value = hValue.ToString("F1");
                hValue += 0.1;
                for (int j = 0; j < 4; j++)
                {
                    // Satırları ve sütunları ters sırala
                    dataGridView2.Rows[i].Cells[j + 1].Value = hurwiczDizisi[10 - i, j].ToString("F2");
                }
            }

            //Chart kısmı

            var chart = chart1.ChartAreas[0];

            chart.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisX.LabelStyle.IsEndLabelVisible = true;

            chart.AxisX.Minimum = 0;
            chart.AxisY.Minimum = 15;

            chart.AxisX.Interval = 0.1;
            chart.AxisY.Interval = 5;

            chart1.Series[0].IsVisibleInLegend = false;

            chart1.Series.Add(sutunIsimleri[1]);
            chart1.Series[sutunIsimleri[1]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[sutunIsimleri[1]].Color = Color.Blue;

            chart1.Series.Add(sutunIsimleri[2]);
            chart1.Series[sutunIsimleri[2]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[sutunIsimleri[2]].Color = Color.Red;

            chart1.Series.Add(sutunIsimleri[3]);
            chart1.Series[sutunIsimleri[3]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[sutunIsimleri[3]].Color = Color.Green;

            chart1.Series.Add(sutunIsimleri[4]);
            chart1.Series[sutunIsimleri[4]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[sutunIsimleri[4]].Color = Color.Orange;
            for(int j=1; j<5; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    chart1.Series[sutunIsimleri[j]].Points.AddXY(dataGridView2.Rows[i].Cells[0].Value, hurwiczDizisi[10-i, j - 1]);
                }
                
            }
            
        }
        

        // CSV dosyasını okuyan ve verileri liste olarak döndüren metod
        private List<string[]> CsvDosyasiniOku(string dosyaYolu)
        {
            // CSV dosyasındaki verileri tutacak liste
            List<string[]> veriler = new List<string[]>();

            // CSV dosyasını satır satır oku
            try
            {

                // CSV dosyasını satır satır okumak için StreamReader kullanımı
                using (StreamReader reader = new StreamReader(dosyaYolu))
                {
                    // Dosyanın sonuna kadar okuma işlemi
                    while (!reader.EndOfStream)
                    {
                        // Satırı oku ve virgülle ayırarak string dizisine dönüştür
                        string satir = reader.ReadLine();
                        string[] alanlar = satir.Split(',');

                        // Diziyi veriler listesine ekle
                        veriler.Add(alanlar);
                    }
                }
                // Okuma işlemi başarılı olduğunda veriler listesini döndür
                return veriler;
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajını göster ve null döndür
                MessageBox.Show("Hata oluştu: " + ex.Message);
                return null;
            }
        }

        public int Pesimistic(int[] gelenDizi)
        {
            
            //Pesimistik kodlarını yazmalısın
            return gelenDizi.Max();
        }
        public int Optimistic(int[] gelenDizi)
        {
            return gelenDizi.Max();
        }
        public int LaPlace(int[] gelenDizi, int[] gelenDizi2)
        {
            int[] ortalamaDizi = new int[gelenDizi.Length];
            for(int i = 0; i < gelenDizi.Length; i++)
            {
                ortalamaDizi[i] = (gelenDizi[i] + gelenDizi2[i]) / 2;
            }
            return ortalamaDizi.Max();
        }
        public int Savage(int[] gelenDizi,int[] gelenDizi1)
        {
            int[] savage = new int[gelenDizi.Length];

            int[] hesaplanmisDizi = new int[gelenDizi.Length];
            int[] hesaplanmisDizi1 = new int[gelenDizi.Length];
            int enBuyukEleman = gelenDizi.Max();
            int enBuyukEleman1 = gelenDizi1.Max();

            for (int i = 0; i < gelenDizi.Length; i++)
            {
                hesaplanmisDizi[i] = enBuyukEleman - gelenDizi[i];
                hesaplanmisDizi1[i] = enBuyukEleman1 - gelenDizi1[i];

            }

            for (int i = 0; i < 4; i++)
            {
                if (hesaplanmisDizi[i] > hesaplanmisDizi1[i])
                {
                    savage[i] = hesaplanmisDizi[i];
                }
                else if(hesaplanmisDizi1[i] > hesaplanmisDizi[i])
                {
                   savage[i] = hesaplanmisDizi1[i];
                }
            }
            return savage.Min();
        }         
        
    }
}
