using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace yeniFormDenemesi
{
    public partial class Form1 : Form
    {
        private int parameterNumber = 0;
        private List<double> weightsList = new List<double>();
        public List<double> islemSonucu = new List<double>();
        public List<double> sonSonucu = new List<double>();
        private int currentRowIndex;
        private int currentColumnIndex;

        double sonuc;
        double toplam;
        private int currentIndex = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateDataGridView(int.Parse(textBox2.Text));
        }
        private void UpdateDataGridView(int columnCount)
        {
            // DataGridView'i güncelle
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Belirtilen satır ve sütun sayısına göre DataGridView'i oluştur

            for (int i = 0; i < columnCount + 2; i++)
            {

                dataGridView1.Columns.Add($"Column{i + 2}", $"Alternative {i - 1}");
            }


            dataGridView1.Rows.Add();

            dataGridView1.Columns[0].HeaderText = "Parameters";
            dataGridView1.Columns[1].HeaderText = "Weights";
            dataGridView1.Rows[0].Cells[0].ReadOnly = true;
            dataGridView1.Rows[0].Cells[1].ReadOnly = true;
        }
       
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kullanıcının tıkladığı hücrenin satır ve sütun indekslerini al
            currentRowIndex = e.RowIndex;
            currentColumnIndex = e.ColumnIndex;

        }     

        private void button3_Click(object sender, EventArgs e)
        {
            ExtractValuesFromColumn(dataGridView1,1);//weightler listede suan
            alternatifHesap();//COK İSE YARİYO
            //gridview'i bir de kolon indexini yolluyorsun kaç değer olduğunu görüyor
            parameterNumber = CountFilledCellsInColumn(dataGridView1, 0);
            label3.Text = "Results in order: " + string.Join(", ", sonSonucu);
            yeniFormDene();
            pieGrafik();
        }
        private List<List<object>> GetAllColumnValues()
        {
            List<List<object>> allColumnValues = new List<List<object>>();

            // Sütun sayısı üzerinden dön
            for (int columnIndex = 0; columnIndex < dataGridView1.Columns.Count; columnIndex++)
            {
                List<object> columnValues = new List<object>();

                // DataGridView satırları üzerinden dön ve her sütundaki değeri listeye ekle
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[columnIndex].Value != null)
                    {
                        columnValues.Add(row.Cells[columnIndex].Value);
                    }
                }

                // Oluşturulan sütun değerlerini ana listeye ekle
                allColumnValues.Add(columnValues);
            }

            return allColumnValues;
        }
        private void ExtractValuesFromColumn(DataGridView dataGridView, int columnIndex)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // Sütundaki değeri double tipine çevirip listeye ekleyin
                if (row.Cells[columnIndex].Value != null && double.TryParse(row.Cells[columnIndex].Value.ToString(), out double value))
                {
                    weightsList.Add(value);
                }
            }
        }
        private void alternatifHesap()
        {
            for (int columnIndex = 2; columnIndex < dataGridView1.Columns.Count; columnIndex++) {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string stringValue = row.Cells[columnIndex].Value?.ToString();
                    if (double.TryParse(stringValue, out double value))
                    {
                        // Dönüşüm başarılı, değeri kullanabilirsiniz
                        islemSonucu.Add(value);
                    }
                    



                    //BURAYI DÜŞÜN
                }
                for (int i = 0; i < islemSonucu.Count; i++)
                {
                    sonuc = weightsList[i] * islemSonucu[i];
                    toplam = toplam + sonuc;
                }
                sonSonucu.Add(toplam);
                toplam = 0;
                islemSonucu.Clear();
            }
            
        }
        public int CountFilledCellsInColumn(DataGridView dataGridView, int columnIndexParam)
        {
            int valueCount = 0;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[columnIndexParam].Value != null)
                {
                    // Eğer hücrede değer varsa, valueCount'u arttır
                    valueCount++;
                }
            }

            return valueCount;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
        private void yeniFormDene()
        {   
            for(int i=2; i< dataGridView1.Columns.Count; i++)
            {
                object cellValue = dataGridView1.Rows[0].Cells[i].Value;// Job ismini aliyor
                chart1.Series["Alternatives"].Points.AddXY(cellValue, sonSonucu[i-2]);
              

            }
            

            chart1.ChartAreas.Add("ChartArea");
            chart1.ChartAreas["ChartArea"].AxisX.Title = "Jobs";
            chart1.ChartAreas["ChartArea"].AxisY.Title = "Values";

            chart1.Titles.Add("Alternative comparison");

            this.Controls.Add(chart1);
        }
       
        private void pieGrafik()
        {
            for (int i = 0; i < parameterNumber; i++)
            {
                object cellValue = dataGridView1.Rows[i + 1].Cells[0].Value.ToString();// parametreAdini aliyor

                chart2.Series["Series1"].Points.AddXY(cellValue, weightsList[i]);               
            }



            // Veri ekleyin (örnek için)
            Series series = new Series("MySeries");
            series.ChartType = SeriesChartType.Pie; // Dilim grafiği olarak ayarla
        
            // Seriyi Chart kontrolüne ekleyin
            chart1.Series.Add(series);

            // Grafiği güncelleyin
            chart1.Update();
        }
    }
}
