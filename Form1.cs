using System;
using System.Drawing;
using System.Windows.Forms;

namespace MOPR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        //Массив для координат X, Y, взятых из условия
        double[,] realXY = new double[2, 4] {
        { 6, 4.6, 1.9, 0.6 },
        { 3.7, 5.6, 5.7, 4.8 }
        };
        //Длины суставов манипулятора
        double l1 = 6.0;
        double l2 = 3.0;
        int p1 = 20;
        int p2 = 20;
        int p3 = 40;
        //Углы psi для разных t
        double[] psi = { 90.0, 150.0, 210.0, 270.0 };
        double[] time;
        //Массив времени t
        double[] t = { 0, 1.0, 2.0, 4.0 }; 
        int nx, ny;
        //Массив скоростей v
        double[] v = { 0, 20, 40, 0 };
        #region Методы
        //Разбить все время выполнения на одинаковые интервалы
        public void TimeIntervals()
        {
            time = new double[p1 + p2 + p3];
            time[0] = t[0];
            time[p1 - 1] = t[1];
            time[p1 + p2 - 1] = t[2];
            time[p1 + p2 + p3 - 1] = t[3];
            for (int i = 1; i < p1 - 1; i++)
            {
                time[i] = time[i - 1] + (t[1] - t[0]) / p1;
            }
            for (int i = p1; i < p1 + p2 - 1; i++)
            {
                time[i] = time[i - 1] + (t[2] - t[1]) / p2;
            }
            for (int i = p1 + p2; i < p1 + p2 + p3 - 1; i++)
            {
                time[i] = time[i - 1] + (t[3] - t[2]) / p3;
            }
        }
        //Создание матрицы однородных преобразований
        public double[,] Result4x4(double psi, double x, double y)
        {
            int fi = 0;
            int eta = 0;
            psi = psi * Math.PI / 180;
            double[,] arrresult = new double[4, 4];
            arrresult[0, 0] = Math.Cos(fi) * Math.Cos(eta);
            arrresult[0, 1] = Math.Cos(fi) * Math.Sin(eta) * Math.Sin(psi) -
            Math.Sin(fi) * Math.Cos(psi);
            arrresult[0, 2] = Math.Cos(fi) * Math.Sin(eta) * Math.Cos(psi) +
            Math.Sin(fi) * Math.Sin(psi);
            arrresult[1, 0] = Math.Sin(fi) * Math.Cos(eta);
            arrresult[1, 1] = Math.Sin(fi) * Math.Sin(eta) * Math.Sin(psi) -
            Math.Cos(fi) * Math.Cos(psi);
            arrresult[1, 2] = Math.Sin(fi) * Math.Sin(eta) * Math.Cos(psi) -
            Math.Cos(fi) * Math.Sin(psi);
            arrresult[2, 0] = -Math.Sin(eta);
            arrresult[2, 1] = Math.Cos(eta) * Math.Sin(psi);
            arrresult[2, 2] = Math.Cos(eta) * Math.Cos(psi);
            arrresult[0, 3] = x;
            arrresult[1, 3] = y;
            arrresult[2, 3] = 0;
            arrresult[3, 0] = 0;
            arrresult[3, 1] = 0;
            arrresult[3, 2] = 0;
            arrresult[3, 3] = 1;
            return arrresult;
        }
        //Обратная задача кинематики(найти углы Q1, Q2, Q3 в градусах)
        public double[,] OZK()
        {
            double c2, s2;
            double[,] qr = new double[3, realXY.GetLength(1)];
            for (int i = 0; i < realXY.GetLength(1); i++)
            {
                c2 = (realXY[0, i] * realXY[0, i] + realXY[1, i] * realXY[1, i]
                - l1 * l1 - l2 * l2) / (2 * l1 * l2);
                s2 = Math.Sqrt(1 - c2 * c2);
                if (c2 > 0) qr[1, i] = 180 / Math.PI * Math.Atan(s2 / c2);
                if (c2 < 0) qr[1, i] = 180 + 180 / Math.PI * Math.Atan(s2 / c2);
                qr[0, i] = 180 / Math.PI * Math.Atan(realXY[1, i] / realXY[0,
                i]) - 180 / (Math.PI) * Math.Atan((l2 * s2) / (l1 + l2 * c2));
                qr[2, i] = psi[i] - qr[0, i] - qr[1, i];
            }
            return qr;
        }
        //Сплайн-интерполяция
        public double[,] Spline(double[,] arr)
        {
            TimeIntervals();
            double[,] splineQR = new double[3, p1 + p2 + p3];
            double tau, tm, k0, k1, k2, k3;
            double[,] mass = OZK();
            int min = 0;
            int max = 80;
            for (int j = 1; j < 4; j++)
            {
                if (j == 1) { min = 0; max = p1; }
                if (j == 2) { min = p1 - 1; max = p1 + p2; }
                if (j == 3) { min = p1 + p2 - 1; max = p1 + p2 + p3; }

                for (int i = min; i < max; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tau = time[i] - t[j - 1];
                        tm = t[j] - t[j - 1];
                        k0 = mass[k, j - 1];
                        k1 = v[j - 1];
                        k2 = (3 * mass[k, j] - 3 * mass[k, j - 1] - 2 * v[j -
                       1] * tm - v[j] * tm) / (tm * tm);
                        k3 = (2 * mass[k, j - 1] - 2 * mass[k, j] + v[j] * tm
                       + v[j - 1] * tm) / (tm * tm * tm);
                        splineQR[k, i] = k3 * tau * tau * tau + k2 * tau * tau +
                       k1 * tau + k0;
                    }
                }
            }

            return splineQR;
        }
        //Нахождение координат скорости
        public double[,] SplineSpeed(double[,] arr)
        {
            TimeIntervals();
            double[,] splineQR = new double[3, p1 + p2 + p3];
            double tau, tm, k0, k1, k2, k3;
            double[,] mass = OZK();
            int min = 0;
            int max = 80;
            //Присвоить координаты Y
            for (int j = 1; j < 4; j++)
            {
                if (j == 1) { min = 0; max = p1; }
                if (j == 2) { min = p1 - 1; max = p1 + p2; }
                if (j == 3) { min = p1 + p2 - 1; max = p1 + p2 + p3; }
                for (int i = min; i < max; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tau = time[i] - t[j - 1];
                        tm = t[j] - t[j - 1];
                        k0 = mass[k, j - 1];
                        k1 = v[j - 1];
                        k2 = (3 * mass[k, j] - 3 * mass[k, j - 1] - 2 * v[j - 1] * tm - v[j] * tm) / (tm * tm);
                        k3 = (2 * mass[k, j - 1] - 2 * mass[k, j] + v[j] * tm + v[j - 1] * tm) / (tm * tm * tm);
                        splineQR[k, i] = k3 * tau * tau * tau + k2 * tau * tau + k1 * tau + k0;
                    }
                }
            }
            return splineQR;
        }
        //Нахождение координат ускорения
        public double[,] SplineAcceleration(double[,] arr)
        {
            TimeIntervals();
            double[,] splineQR = new double[3, p1 + p2 + p3];
            double tau, tm, k0, k1, k2, k3;
            double[,] mass = OZK();
            int min = 0;
            int max = 80;
            //Присвоить координаты Y
            for (int j = 1; j < 4; j++)
            {
                if (j == 1) { min = 0; max = p1; }
                if (j == 2) { min = p1 - 1; max = p1 + p2; }
                if (j == 3) { min = p1 + p2 - 1; max = p1 + p2 + p3; }
                for (int i = min; i < max; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        tau = time[i] - t[j - 1];
                        tm = t[j] - t[j - 1];
                        k0 = mass[k, j - 1];
                        k1 = v[j - 1];
                        k2 = (3 * mass[k, j] - 3 * mass[k, j - 1] - 2 * v[j - 1] * tm - v[j] * tm) / (tm * tm);
                        k3 = (2 * mass[k, j - 1] - 2 * mass[k, j] + v[j] * tm + v[j - 1] * tm) / (tm * tm * tm);
                        splineQR[k, i] = k3 * tau * tau * tau + k2 * tau * tau + k1 * tau + k0;
                    }
                }
            }
            return splineQR;
        }
        //Прямая задача кинематики(найти координаты точек манипулятора)
        public double[,] PZK(double[,] arr)
        {
            double[,] pzkXY = new double[8, p1 + p2 + p3];
            //Перевод градусов в радианы
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < p1 + p2 + p3; j++)
                {
                    arr[i, j] = arr[i, j] * Math.PI / 180;
                }
            }
            for (int i = 0; i < p1 + p2 + p3; i++)
            {
                //Координаты точки 1
                pzkXY[0, i] = l1 * Math.Cos(arr[0, i]);
                pzkXY[1, i] = l1 * Math.Sin(arr[0, i]);
                //Координаты точки 2
                pzkXY[2, i] = l1 * Math.Cos(arr[0, i]) + l2 * Math.Cos(arr[0,
                i] + arr[1, i]);
                pzkXY[3, i] = l1 * Math.Sin(arr[0, i]) + l2 * Math.Sin(arr[0, i]
                + arr[1, i]);
                //Нужны для построения оси Y точки 2
                pzkXY[4, i] = l1 * Math.Cos(arr[0, i]) + l2 * Math.Cos(arr[0,
                i] + arr[1, i]) + Math.Sin(arr[2, i]);
                pzkXY[5, i] = l1 * Math.Sin(arr[0, i]) + l2 * Math.Sin(arr[0, i]
                + arr[1, i]) + Math.Cos(arr[2, i]);
                //Нужны для построения оси X точки 2
                pzkXY[6, i] = l1 * Math.Cos(arr[0, i]) + l2 * Math.Cos(arr[0,
                i] + arr[1, i]) + Math.Sin(arr[2, i] - 90 * Math.PI / 180);
                pzkXY[7, i] = l1 * Math.Sin(arr[0, i]) + l2 * Math.Sin(arr[0, i]
                + arr[1, i]) + Math.Cos(arr[2, i] - 90 * Math.PI / 180);
            }
            return pzkXY;
        }
        //Примеры вызова PZK
        //double[,] mass = PZK(Spline(OZK()));
        #endregion
        #region Рисование
        public void DrawDimension(Graphics g, Pen blackpen, SolidBrush brush)
        {
            Font font = new Font("Arial", 9);
            string x, y;
            int ii = 0;
            int j = 19;
            for (int k = 0; k < 4; k++)
            {
                g.DrawLine(blackpen, new Point(ii + 25 + nx, 270), new
                Point(ii + 25 + nx, 280));
                x = time[j].ToString();
                g.DrawString(x, font, brush, ii + 25 + nx - 5, 290.0F);
                ii = ii + nx;
                j = j + 20;
            }
            ii = 275;
            for (int k = 0; k < 6; k++)
            {
                g.DrawLine(blackpen, new Point(20, ii - ny), new Point(30, ii - ny));
                y = (k + 1).ToString();
                g.DrawString(y, font, brush, 5, ii - ny - 5);
                ii = ii - ny;
            }
            ii = 275;
            for (int k = 0; k < 6; k++)
            {
                g.DrawLine(blackpen, new Point(20, ii + ny), new Point(30, ii
                + ny));
                y = (-k - 1).ToString();
                g.DrawString(y, font, brush, 5, ii + ny - 5);

                ii = ii + ny;
            }
            x = "0";
            g.DrawString(x, font, brush, 5, 270.0F);
            font.Dispose();
        }
        private void DrawManipulator(object sender, EventArgs e)
        {
            nx = 40;
            ny = 40;
            double[,] promD = PZK(Spline(OZK()));
            int[,] prom = new int[promD.GetLength(0),
            promD.GetLength(1)];
            for (int i = 0; i < promD.GetLength(1); i++)
            {
                prom[0, i] = (int)(promD[0, i] * nx) + 300;
                prom[1, i] = 275 - (int)(promD[1, i] * ny);
                prom[2, i] = (int)(promD[2, i] * nx) + 300;
                prom[3, i] = 275 - (int)(promD[3, i] * ny);
                prom[4, i] = (int)(promD[4, i] * nx) + 300;
                prom[5, i] = 275 - (int)(promD[5, i] * ny);
                prom[6, i] = (int)(promD[6, i] * nx) + 300;
                prom[7, i] = 275 - (int)(promD[7, i] * ny);
            }
            using (Graphics g = this.CreateGraphics())
            {
                Pen blackpen = new Pen(Color.Black, 5);
                Pen bluepen = new Pen(Color.Blue, 1);
                Pen bp = new Pen(Color.Black, 1);
                Pen red = new Pen(Color.Red, 2);
                Font font = new Font("Arial", 14);
                Font f = new Font("Arial", 8);
                SolidBrush brush = new SolidBrush(Color.Black);
                string osx = "X";
                string osy = "Y";
                string op;

                for (int i = 0; i < p1 + p2 + p3; i++)
                {
                    g.Clear(Color.White);
                    //Начертить звенья
                    g.DrawLine(blackpen, new Point(300, 275), new
                    Point(prom[0, i], prom[1, i]));
                    g.DrawLine(blackpen, new Point(prom[0, i], prom[1, i]),
                    new Point(prom[2, i], prom[3, i]));
                    g.DrawLine(bluepen, new Point(prom[2, i], prom[3, i]), new
                    Point(prom[4, i], prom[5, i]));
                    g.DrawLine(bluepen, new Point(prom[2, i], prom[3, i]), new
                    Point(prom[6, i], prom[7, i]));
                    //Начертить оси
                    g.DrawLine(bp, new Point(300, 25), new Point(300, 475));
                    g.DrawLine(bp, new Point(25, 275), new Point(575, 275));
                    //Стрелки
                    g.DrawLine(bp, new Point(300, 25), new Point(288, 50));
                    g.DrawLine(bp, new Point(300, 25), new Point(312, 50));
                    g.DrawLine(bp, new Point(575, 275), new Point(550, 263));
                    g.DrawLine(bp, new Point(575, 275), new Point(550, 287));
                    //Подпись
                    g.DrawString(osx, font, brush, 580.0F, 275.0F);
                    g.DrawString(osy, font, brush, 305.0F, 10.0F);
                    //Подписать доп. оси
                    g.DrawString(osx, f, brush, prom[4, i] + 5, prom[5, i] - 5);
                    g.DrawString(osy, f, brush, prom[6, i] + 5, prom[7, i] - 5);
                    //Обозначить пересечения звеньев
                    g.DrawEllipse(blackpen, prom[2, i], prom[3, i], 5.0F, 5.0F);
                    g.DrawEllipse(blackpen, 300, 275, 5.0F, 5.0F);
                    g.DrawEllipse(blackpen, prom[0, i], prom[1, i], 5.0F, 5.0F);
                    //Нарисовать схват
                    g.DrawLine(blackpen, new Point(prom[2, i], prom[3, i]),
                    new Point(prom[2, i] - 10, prom[3, i] - 10));
                    g.DrawLine(blackpen, new Point(prom[2, i], prom[3, i]),
                    new Point(prom[2, i] + 10, prom[3, i] - 10));
                    //Подписать оси X
                    for (int ii = 300 + nx; ii <= 575;)
                    {
                        g.DrawLine(bp, new Point(ii, 270), new Point(ii, 280));
                        op = ((ii - 300) / nx).ToString();
                        g.DrawString(op, f, brush, ii - 5, 290);
                        ii += nx;
                    }
                    for (int ii = 300 - nx; ii >= 25;)
                    {
                        g.DrawLine(bp, new Point(ii, 270), new Point(ii, 280));
                        op = ((ii - 300) / nx).ToString();
                        g.DrawString(op, f, brush, ii - 5, 290);
                        ii -= nx;
                    }
                    //Подписать оси Y
                    for (int ii = 275 - ny; ii >= 25;)
                    {
                        g.DrawLine(bp, new Point(295, ii), new Point(305, ii));
                        op = (Math.Abs(ii - 275) / ny).ToString();
                        g.DrawString(op, f, brush, 280, ii - 5);
                        ii -= ny;
                    }
                    for (int ii = 275 + ny; ii <= 475;)
                    {
                        g.DrawLine(bp, new Point(295, ii), new Point(305, ii));
                        op = ((-ii + 275) / ny).ToString();
                        g.DrawString(op, f, brush, 280, ii - 5);
                        ii += ny;
                    }
                    op = "0";
                    g.DrawString(op, f, brush, 280, 260);
                    System.Threading.Thread.Sleep(30);
                }
                //Траектория движения и опорные точки
                for (int i = 0; i < p1 + p2 + p3; i++)
                {
                    g.DrawEllipse(red, prom[2, i], prom[3, i], 2.0F, 2.0F);
                }
                g.DrawEllipse(blackpen, prom[2, 0], prom[3, 0], 6.0F, 6.0F);
                g.DrawEllipse(blackpen, prom[2, p1 - 1], prom[3, p1 - 1],
                6.0F, 6.0F);
                g.DrawEllipse(blackpen, prom[2, p1 + p2 - 1], prom[3, p1 + p2
                - 1], 6.0F, 6.0F);
                g.DrawEllipse(blackpen, prom[2, p1 + p2 + p3 - 1], prom[3, p1
                + p2 + p3 - 1], 6.0F, 6.0F);
                red.Dispose();
                bp.Dispose();
                blackpen.Dispose();
                bluepen.Dispose();
                font.Dispose();
                brush.Dispose();
            }
        }
        private void DrawPosition(object sender, EventArgs e)
        {
            nx = 135;
            ny = 70;
            double[,] promD = Spline(OZK());
            int[,] prom = new int[promD.GetLength(0) * 2,
            promD.GetLength(1)];
            for (int i = 0; i < promD.GetLength(1); i++)
            {
                prom[0, i] = 25 + (int)(time[i] * nx);
                prom[1, i] = 275 - (int)(promD[0, i] * (Math.PI / 180) * ny);
                prom[2, i] = 25 + (int)(time[i] * nx);
                prom[3, i] = 275 - (int)(promD[1, i] * (Math.PI / 180) * ny);
                prom[4, i] = 25 + (int)(time[i] * nx);
                prom[5, i] = 275 - (int)(promD[2, i] * (Math.PI / 180) * ny);
            }
            using (Graphics g = this.CreateGraphics())
            {
                Pen blackpen = new Pen(Color.Black, 1);
                Pen redpen = new Pen(Color.Red, 2);
                Pen bluepen = new Pen(Color.Blue, 2);
                Pen greenpen = new Pen(Color.Green, 2);
                Font font = new Font("Arial", 14);
                SolidBrush brush = new SolidBrush(Color.Black);
                string osx = "t,с";
                string osy = "Q(t),рад";
                string one = "Звено 1";
                string two = "Звено 2";
                string three = "Звено 3";
                g.Clear(Color.White);
                g.DrawLine(blackpen, new Point(25, 25), new Point(25, 475));
                g.DrawLine(blackpen, new Point(25, 275), new Point(575, 275));
                g.DrawLine(blackpen, new Point(25, 25), new Point(13, 50));
                g.DrawLine(blackpen, new Point(25, 25), new Point(37, 50));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 263));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 287));
                g.DrawString(osx, font, brush, 580.0F, 275.0F);
                g.DrawString(osy, font, brush, 15.0F, 0.0F);
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(redpen, new Point(prom[0, i], prom[1, i]), new
                    Point(prom[0, i + 1], prom[1, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(bluepen, new Point(prom[2, i], prom[3, i]), new
                    Point(prom[2, i + 1], prom[3, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(greenpen, new Point(prom[4, i], prom[5, i]),
                    new Point(prom[4, i + 1], prom[5, i + 1]));
                }
                g.DrawString(one, font, brush, 95.0F, 10.0F);
                g.DrawLine(redpen, new Point(175, 20), new Point(205, 20));
                g.DrawString(two, font, brush, 245.0F, 10.0F);
                g.DrawLine(bluepen, new Point(325, 20), new Point(355, 20));
                g.DrawString(three, font, brush, 395.0F, 10.0F);
                g.DrawLine(greenpen, new Point(475, 20), new Point(505, 20));
                DrawDimension(g, blackpen, brush);
                redpen.Dispose();
                blackpen.Dispose();
                bluepen.Dispose();
                greenpen.Dispose();
                font.Dispose();
                brush.Dispose();
            }
        }
        private void DrawSpeed(object sender, EventArgs e)
        {
            nx = 135;
            ny = 100;
            TimeIntervals();
            double[,] promD = SplineSpeed(OZK());
            int[,] prom = new int[promD.GetLength(0) * 2,
            promD.GetLength(1)];
            for (int i = 0; i < promD.GetLength(1); i++)
            {
                prom[0, i] = 25 + (int)(time[i] * nx);
                prom[1, i] = 275 - (int)(promD[0, i] * (Math.PI / 180) * ny);
                prom[2, i] = 25 + (int)(time[i] * nx);
                prom[3, i] = 275 - (int)(promD[1, i] * (Math.PI / 180) * ny);
                prom[4, i] = 25 + (int)(time[i] * nx);
                prom[5, i] = 275 - (int)(promD[2, i] * (Math.PI / 180) * ny);
            }
            using (Graphics g = this.CreateGraphics())
            {
                Pen blackpen = new Pen(Color.Black, 1);
                Pen redpen = new Pen(Color.Red, 2);
                Pen bluepen = new Pen(Color.Blue, 2);
                Pen greenpen = new Pen(Color.Green, 2);
                Font font = new Font("Arial", 14);
                SolidBrush brush = new SolidBrush(Color.Black);
                string osx = "t,с";
                string osy = "Q'(t),рад";
                string one = "Звено 1";
                string two = "Звено 2";
                string three = "Звено 3";
                g.Clear(Color.White);
                g.DrawLine(blackpen, new Point(25, 25), new Point(25, 475));
                g.DrawLine(blackpen, new Point(25, 275), new Point(575, 275));
                g.DrawLine(blackpen, new Point(25, 25), new Point(13, 50));
                g.DrawLine(blackpen, new Point(25, 25), new Point(37, 50));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 263));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 287));
                g.DrawString(osx, font, brush, 580.0F, 275.0F);
                g.DrawString(osy, font, brush, 15.0F, 0.0F);
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(redpen, new Point(prom[0, i], prom[1, i]), new
                    Point(prom[0, i + 1], prom[1, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(bluepen, new Point(prom[2, i], prom[3, i]), new
                    Point(prom[2, i + 1], prom[3, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(greenpen, new Point(prom[4, i], prom[5, i]),
                    new Point(prom[4, i + 1], prom[5, i + 1]));
                }
                g.DrawString(one, font, brush, 95.0F, 10.0F);
                g.DrawLine(redpen, new Point(175, 20), new Point(205, 20));
                g.DrawString(two, font, brush, 245.0F, 10.0F);
                g.DrawLine(bluepen, new Point(325, 20), new Point(355, 20));
                g.DrawString(three, font, brush, 395.0F, 10.0F);
                g.DrawLine(greenpen, new Point(475, 20), new Point(505, 20));
                DrawDimension(g, blackpen, brush);
                redpen.Dispose();
                blackpen.Dispose();
                bluepen.Dispose();
                greenpen.Dispose();
                font.Dispose();
                brush.Dispose();
            }

        }
        private void DrawAcceleration(object sender, EventArgs e)
        {
            nx = 135;
            ny = 70;
            TimeIntervals();
            double[,] promD = SplineAcceleration(OZK());
            int[,] prom = new int[promD.GetLength(0) * 2,
            promD.GetLength(1)];
            for (int i = 0; i < promD.GetLength(1); i++)
            {
                prom[0, i] = 25 + (int)(time[i] * nx);
                prom[1, i] = 275 - (int)(promD[0, i] * (Math.PI / 180) * ny);
                prom[2, i] = 25 + (int)(time[i] * nx);
                prom[3, i] = 275 - (int)(promD[1, i] * (Math.PI / 180) * ny);
                prom[4, i] = 25 + (int)(time[i] * nx);
                prom[5, i] = 275 - (int)(promD[2, i] * (Math.PI / 180) * ny);
            }
            using (Graphics g = this.CreateGraphics())
            {
                Pen blackpen = new Pen(Color.Black, 1);
                Pen redpen = new Pen(Color.Red, 2);
                Pen bluepen = new Pen(Color.Blue, 2);
                Pen greenpen = new Pen(Color.Green, 2);
                Font font = new Font("Arial", 14);
                SolidBrush brush = new SolidBrush(Color.Black);
                string osx = "t,с";
                string osy = "Q''(t),рад";
                string one = "Звено 1";
                string two = "Звено 2";
                string three = "Звено 3";
                g.Clear(Color.White);
                g.DrawLine(blackpen, new Point(25, 25), new Point(25, 475));
                g.DrawLine(blackpen, new Point(25, 275), new Point(575, 275));
                g.DrawLine(blackpen, new Point(25, 25), new Point(13, 50));
                g.DrawLine(blackpen, new Point(25, 25), new Point(37, 50));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 263));
                g.DrawLine(blackpen, new Point(575, 275), new Point(550, 287));
                g.DrawString(osx, font, brush, 580.0F, 275.0F);
                g.DrawString(osy, font, brush, 15.0F, 0.0F);
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(redpen, new Point(prom[0, i], prom[1, i]), new
                    Point(prom[0, i + 1], prom[1, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(bluepen, new Point(prom[2, i], prom[3, i]), new
                    Point(prom[2, i + 1], prom[3, i + 1]));
                }
                for (int i = 0; i < promD.GetLength(1) - 1; i++)
                {
                    g.DrawLine(greenpen, new Point(prom[4, i], prom[5, i]),
                    new Point(prom[4, i + 1], prom[5, i + 1]));
                }
                g.DrawString(one, font, brush, 95.0F, 10.0F);
                g.DrawLine(redpen, new Point(175, 20), new Point(205, 20));
                g.DrawString(two, font, brush, 245.0F, 10.0F);
                g.DrawLine(bluepen, new Point(325, 20), new Point(355, 20));
                g.DrawString(three, font, brush, 395.0F, 10.0F);
                g.DrawLine(greenpen, new Point(475, 20), new Point(505, 20));
                DrawDimension(g, blackpen, brush);
                redpen.Dispose();
                blackpen.Dispose();
                bluepen.Dispose();
                greenpen.Dispose();
                font.Dispose();
                brush.Dispose();
            }
        }
        #endregion
        #region Кнопки
        private void buttonDecart_Click(object sender, EventArgs e)
        {
            DrawManipulator(sender, e);
        }
        private void buttonObobs_Click(object sender, EventArgs e)
        {
            DrawPosition(sender, e);
        }
        private void buttonSkor_Click(object sender, EventArgs e)
        {
            DrawSpeed(sender, e);
        }
        private void buttonUskor_Click(object sender, EventArgs e)
        {
            DrawAcceleration(sender, e);
        }
        #endregion
    }
}
