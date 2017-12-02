using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace digits
{
    public partial class Form1 : Form
    {
        private const int n = 5;
        private const int m = 7;
        const int size1 = n * m; //5�7
        const int size2 = 10; //����
        const double T = (double)size1 / 2;
        const double c = 1 / (double)size2;//����. ����������
        double e;//��������
        double[] X1 = new double[size2]; //������ ����
        double[] X2new = new double[size2]; //������ ����
        double[] X2 = new double[size2];
        int[] Y = new int[size1]; // ����������� ����
        double[,] W1 = new double[size2, size1]; //����
        double[,] E = new double[size2, size2]; //�������
        private double[,] t = new double[size2, size1]; //�������

        bool mode = true;

        public Form1()
        {
            InitializeComponent();
            label1.Text = "";
        }

        double[,] read(System.IO.StreamReader reader)
        {
            double[,] r = new double[size2, size1];
            for (int i = 0; i < size2; i++)
            {
                var line = reader.ReadLine().ToCharArray();
                for (int k = 0; k < size1; k++)
                {
                    char a = line[k];
                    if (a == '1')
                        r[i, k] = 1;
                    else r[i, k] = 0;
                }
            }
            return r;
        }

        bool train(double[,] etalons)
        {
            e = (double)numericUpDown1.Value;
            for (int i = 0; i < size2; i++)
            {
                X1[i] = 0;
                X2[i] = 0;
                X2new[i] = 0;
            }
            //������� ���� �� ��������
            for (int i = 0; i < size2; i++)
                for (int j = 0; j < size1; j++)
                    W1[i, j] = etalons[i, j] / 2;
            //������� ���� ��������
            for (int i = 0; i < size2; i++)
                for (int j = 0; j < size2; j++)
                    if (i == j)
                        E[i, j] = 1;
                    else
                        E[i, j] = -c;
            return true;
        }

        private double VecL(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < size2; i++)
            {
                sum += (b[i] - a[i]) * (b[i] - a[i]);
            }
            return Math.Sqrt(sum);
        }

        private double aFunc(double i)
        {
            return i < 0 ? 0 : i;
        }
        private int recognize()
        {
            // ������ ����, ��������� ��������
            for (int i = 0; i < size2; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    X1[i] += (double)Y[j] * W1[i, j];
                }
                X1[i] += T;
            }
            //������ � ������� ����
            for (int i = 0; i < size2; i++)
            {
                X2new[i] = aFunc(X1[i]);
            }
            int iters = 0;
            //������� ������ ����
            do
            {
                for (int i = 0; i < size2; i++)
                {
                    X2[i] = X2new[i];
                }

                for (int i = 0; i < size2; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < size2; j++)
                        if (i != j)
                            sum += X2[j];
                    X2new[i] = X2[i] - c * sum;
                }
                for (int i = 0; i < size2; i++)
                {
                    X2new[i] = aFunc(X2new[i]);
                }
                iters++;
            } while (VecL(X2, X2new) > e);
            for (int i = 0; i < size2; i++)
            {
                if (X2new[i] > 0)
                    dataGridView1.Rows.Add(i, X2new[i]);
            }
            dataGridView1.Sort(dataGridView1.Columns[1],ListSortDirection.Descending);
            return iters;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int i = e.X / 20;
            int j = e.Y / 20;
            if (mode)
                Y[j * n + i] = 1;
            else Y[j * n + i] = 0;
            panel1.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < size1; i++)
            {
                if (Y[i] == 1)
                {
                    int vert = i % n;
                    int gor = i / n;
                    g.FillRectangle(System.Drawing.Brushes.Black, vert * 20, gor * 20, 20, 20);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            dataGridView1.Rows.Clear();
            if (train(t))
            {
                label1.Text=@"���������� �� "+recognize()+@" ��������";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mode = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mode = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("ideal_new.txt");
            t = read(sr);
            sr.Close();
        }
    }
}