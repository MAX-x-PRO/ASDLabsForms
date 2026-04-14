using System;
using System.Drawing;
using System.Windows.Forms;

namespace ASDLabsForms.Labs
{
    public class Lab3Form : Form
    {
        private int n = 14;
        private int[,] A_dir;
        private int[,] A_undir;
        private PointF[] coordsDir;
        private PointF[] coordsUndir;
        private GraphRenderer renderer = new GraphRenderer();

        public Lab3Form()
        {
            this.Text = "Lab 3 - Variant 5344";
            this.Size = new Size(1200, 650);
            this.BackColor = Color.White;

            GenerateData();
            CalculateTriangleCoords();
        }

        private void GenerateData()
        {
            A_dir = new int[n, n];
            A_undir = new int[n, n];
            Random rnd = new Random(5344);
            double k = 0.65; // Calculated: 1.0 - 4*0.02 - 4*0.005 - 0.25

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    A_dir[i, j] = (rnd.NextDouble() * 2.0 * k >= 1.0) ? 1 : 0;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (A_dir[i, j] == 1) { A_undir[i, j] = 1; A_undir[j, i] = 1; }

            PrintMatrixToConsole(A_dir, "Directed Graph (A_dir)");
            PrintMatrixToConsole(A_undir, "Undirected Graph (A_undir)");
        }

        private void CalculateTriangleCoords()
        {
            coordsDir = new PointF[n];
            coordsUndir = new PointF[n];

            PointF top = new PointF(300, 50), right = new PointF(550, 500), left = new PointF(50, 500);
            PointF[] sides = { top, right, left, top };
            int[] perSide = { 5, 5, 4 };

            int idx = 0;
            for (int s = 0; s < 3; s++)
            {
                float stepX = (sides[s + 1].X - sides[s].X) / perSide[s];
                float stepY = (sides[s + 1].Y - sides[s].Y) / perSide[s];
                for (int i = 0; i < perSide[s]; i++)
                {
                    coordsDir[idx] = new PointF(sides[s].X + stepX * i, sides[s].Y + stepY * i);
                    coordsUndir[idx] = new PointF(coordsDir[idx].X + 600, coordsDir[idx].Y);
                    idx++;
                }
            }
        }

        private void PrintMatrixToConsole(int[,] matrix, string title)
        {
            Console.WriteLine($"\n=== {title} ===");
            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write(string.Format("{0,2} ", matrix[i, j]));
                Console.WriteLine();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.DrawGraph(e.Graphics, coordsDir, A_dir, true, "Directed Graph (A_dir)");
            renderer.DrawGraph(e.Graphics, coordsUndir, A_undir, false, "Undirected Graph (A_undir)");
        }
    }
}
