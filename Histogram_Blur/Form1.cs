using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Histogram_Blur
{
    public partial class Form1 : Form
    {
        private int szer = 0, wys = 0;
        public Form1()
        {
            InitializeComponent();
        }

        //zaladuj
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(openFileDialog1.FileName);
                szer = pictureBox1.Image.Width;
                wys = pictureBox1.Image.Height;
                pictureBox2.Image = new Bitmap(szer, wys);
            }
        }

        //zapisz
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        pictureBox2.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        pictureBox2.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        pictureBox2.Image.Save(fs,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }
        }


        //rysuj histogram
        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap b1 = (Bitmap)pictureBox1.Image;
            double[] r = new double[256];
            double[] g = new double[256];
            double[] b = new double[256];

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();

            for (int i = 0; i < b1.Height; i++)
            {
                for (int j = 0; j < b1.Width; j++)
                {
                    Color pixel = b1.GetPixel(j, i);
                    r[pixel.R]++;
                    g[pixel.G]++;
                    b[pixel.B]++;
                }
            }

            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, r[i]);
                chart1.Series[1].Points.AddXY(i, g[i]);
                chart1.Series[2].Points.AddXY(i, b[i]);
            }
            chart1.Invalidate();
        }

        //filtr usredniajacy
        private void button4_Click(object sender, EventArgs e)
        {
            int[,] maska = new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            Bitmap b1 = (Bitmap)pictureBox1.Image;
            Bitmap b2 = (Bitmap)pictureBox2.Image;
            Color k1, k2;

            for (int i = 1; i < szer-1; i++)
            {
                for (int j = 1; j < wys-1; j++)
                {
                    int r_nowy = 0;
                    int g_nowy = 0;
                    int b_nowy = 0;

                    for(int k =- 1; k <= 1; k++)
                    {
                        for(int o = -1; o <= 1; o++)
                        {
                            k1 = b1.GetPixel(i+k, j+o);
                            r_nowy += k1.R * maska[k + 1, o + 1];
                            g_nowy += k1.G * maska[k + 1, o + 1];
                            b_nowy += k1.B * maska[k + 1, o + 1];
                        }
                    }

                    r_nowy /= 9;
                    g_nowy /= 9;
                    b_nowy /= 9;

                    if (r_nowy > 255) r_nowy = 255;
                    if (r_nowy < 0) r_nowy = 0;
                    if (g_nowy > 255) g_nowy = 255;
                    if (g_nowy < 0) g_nowy = 0;
                    if (b_nowy > 255) b_nowy = 255;
                    if (b_nowy < 0) b_nowy = 0;

                    k2 = Color.FromArgb((int)r_nowy, (int)g_nowy, (int)b_nowy);
                    b2.SetPixel(i, j, k2);
                }
            }
            pictureBox2.Invalidate();

        }


        //filtr gaussa 5x5
        private void button5_Click(object sender, EventArgs e)
        {
            int[,] maska = new int[,] { { 0, 1, 2, 1, 0 }, { 1, 4, 8, 4, 1 }, { 2, 8, 16, 8, 2 }, { 1, 4, 8, 4, 1 }, { 0, 1, 2, 1, 0 } };
            Bitmap b1 = (Bitmap)pictureBox1.Image;
            Bitmap b2 = (Bitmap)pictureBox2.Image;
            Color k1, k2;


            for (int i = 2; i < szer - 2; i++)
            {
                for (int j = 2; j < wys - 2; j++)
                {
                    int maska_p = 0;
                    int r_nowy = 0;
                    int g_nowy = 0;
                    int b_nowy = 0;

                    for (int k = -2; k <= 2; k++)
                    {
                        for (int o = -2; o <= 2; o++)
                        {
                            k1 = b1.GetPixel(i + k, j + o);
                            r_nowy += k1.R * maska[k + 2, o + 2];
                            g_nowy += k1.G * maska[k + 2, o + 2];
                            b_nowy += k1.B * maska[k + 2, o + 2];
                            maska_p += maska[k + 2, o + 2];
                        }
                    }

                    r_nowy /= maska_p;
                    g_nowy /= maska_p;
                    b_nowy /= maska_p;
                    maska_p = 0;

                    if (r_nowy > 255) r_nowy = 255;
                    if (r_nowy < 0) r_nowy = 0;
                    if (g_nowy > 255) g_nowy = 255;
                    if (g_nowy < 0) g_nowy = 0;
                    if (b_nowy > 255) b_nowy = 255;
                    if (b_nowy < 0) b_nowy = 0;

                    k2 = Color.FromArgb((int)r_nowy, (int)g_nowy, (int)b_nowy);
                    b2.SetPixel(i, j, k2);
                }
            }
            pictureBox2.Invalidate();
        }

        //histogram
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap b1 = (Bitmap)pictureBox1.Image;
            Bitmap b2 = (Bitmap)pictureBox2.Image;
            Color k1, k2;
            double[] r = new double[256];
            double[] g = new double[256];
            double[] b = new double[256];

            double[] r_kumulacyjne = new double[256];
            double[] g_kumulacyjne = new double[256];
            double[] b_kumulacyjne = new double[256];

            double[] r_zmodyfikowane = new double[256];
            double[] g_zmodyfikowane = new double[256];
            double[] b_zmodyfikowane = new double[256];

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();

            for (int i = 0; i < b1.Height; i++)
            {
                for (int j = 0; j < b1.Width; j++)
                {
                    Color pixel = b1.GetPixel(j, i);
                    r[pixel.R]++;
                    g[pixel.G]++;
                    b[pixel.B]++;
                }
            }

            r_kumulacyjne[0] = r[0];
            g_kumulacyjne[0] = g[0];
            b_kumulacyjne[0] = b[0];
            for (int i = 1; i < r.Length; i++)
            {
                r_kumulacyjne[i] = r[i] + r_kumulacyjne[i - 1];
                g_kumulacyjne[i] = g[i] + g_kumulacyjne[i - 1];
                b_kumulacyjne[i] = b[i] + b_kumulacyjne[i - 1];
            }

            for(int i = 0; i < szer; i++)
            {
                for(int j = 0; j < wys; j++)
                {
                    k1 = b1.GetPixel(i, j);

                    double red = 0;
                    double green = 0;
                    double blue = 0;

                    red = (double)((255/(double)(wys*szer))*r_kumulacyjne[k1.R]);
                    green = (double)((255 / (double)(wys * szer)) * g_kumulacyjne[k1.G]);
                    blue = (double)((255 / (double)(wys * szer)) * b_kumulacyjne[k1.B]);

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;
                    if (green > 255) green = 255;
                    if (green < 0) green = 0;
                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    k2 = Color.FromArgb((int)red, (int)green, (int)blue);
                    b2.SetPixel(i, j, k2);

                    r_zmodyfikowane[k2.R]++;
                    g_zmodyfikowane[k2.G]++;
                    b_zmodyfikowane[k2.B]++;
                }
            }
            pictureBox2.Invalidate();

            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, r_zmodyfikowane[i]);
                chart1.Series[1].Points.AddXY(i, g_zmodyfikowane[i]);
                chart1.Series[2].Points.AddXY(i, b_zmodyfikowane[i]);
            }
            chart1.Invalidate();
        }
    }
}
