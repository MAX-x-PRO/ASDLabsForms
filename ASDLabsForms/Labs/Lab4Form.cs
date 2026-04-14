namespace ASDLabsForms.Labs
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    namespace ASD_Labs
    {
        public class Lab4Form : Form
        {
            private int n = 14;
            private int[,] A_dir1, A_undir1, A_dir2, A_cond;
            private PointF[] cDir1, cUndir1, cDir2, cCond;
            private string[] condLbs;
            private GraphRenderer r = new GraphRenderer();

            public Lab4Form()
            {
                this.Text = "Lab 4 - Variant 5344";
                this.Size = new Size(1200, 950);
                this.BackColor = Color.White;
                RunConsoleAnalytics();
                CalculateCoordinates();
            }

            private int[,] GenerateMatrix(int seed, double k)
            {
                int[,] matrix = new int[n, n];
                Random rnd = new Random(seed);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        matrix[i, j] = (rnd.NextDouble() * 2.0 * k >= 1.0) ? 1 : 0;
                return matrix;
            }

            private void RunConsoleAnalytics()
            {
                Console.WriteLine("Lab 4");
                A_dir1 = GenerateMatrix(5344, 0.62);
                A_undir1 = new int[n, n];

                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (A_dir1[i, j] == 1) { A_undir1[i, j] = 1; A_undir1[j, i] = 1; }

                Console.WriteLine("Task 1");
                PrintMatrixToConsole(A_dir1, "Directed Graph (A_dir1)");
                PrintMatrixToConsole(A_undir1, "Undirected Graph (A_undir1)");

                Console.WriteLine("\nTask 2");
                int[] inDeg = new int[n], outDeg = new int[n], degU = new int[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++) { outDeg[i] += A_dir1[i, j]; inDeg[i] += A_dir1[j, i]; degU[i] += A_undir1[i, j]; }
                    Console.WriteLine($"In {i + 1}: Dir(In={inDeg[i]}, Out={outDeg[i]}, Total={inDeg[i] + outDeg[i]}), Undir={degU[i]}");
                }

                bool reg = true;
                for (int i = 1; i < n; i++) if (degU[i] != degU[0]) reg = false;
                Console.WriteLine($"Regular: {reg}");

                Console.Write("Isolated: ");
                for (int i = 0; i < n; i++) if (degU[i] == 0) Console.Write($"{i + 1} ");

                Console.Write("\nPendant: ");
                for (int i = 0; i < n; i++) if (degU[i] == 1) Console.Write($"{i + 1} ");
                Console.WriteLine();

                Console.WriteLine("\nTask 3");
                A_dir2 = GenerateMatrix(5344, 0.69);
                PrintMatrixToConsole(A_dir2, "Directed Graph (A_dir2)");

                Console.WriteLine("\nTask 4");

                Console.WriteLine("\nPaths of length 2:");
                for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) for (int k = 0; k < n; k++)
                    if (A_dir2[i, k] == 1 && A_dir2[k, j] == 1) Console.Write($"({i + 1}-{k + 1}-{j + 1}) ");

                Console.WriteLine("\n\nPaths of length 3:");
                for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) for (int k = 0; k < n; k++)
                    if (A_dir2[i, k] == 1) for (int m = 0; m < n; m++)
                        if (A_dir2[k, m] == 1 && A_dir2[m, j] == 1) Console.Write($"({i + 1}-{k + 1}-{m + 1}-{j + 1}) ");

                int[,] R = new int[n, n], S = new int[n, n];
                for (int i = 0; i < n; i++) { for (int j = 0; j < n; j++) R[i, j] = A_dir2[i, j]; R[i, i] = 1; }
                for (int k = 0; k < n; k++) for (int i = 0; i < n; i++) for (int j = 0; j < n; j++)
                    if (R[i, k] == 1 && R[k, j] == 1) R[i, j] = 1;

                for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) S[i, j] = (R[i, j] == 1 && R[j, i] == 1) ? 1 : 0;

                int[] cMap = Enumerable.Repeat(-1, n).ToArray();
                List<List<int>> comps = new List<List<int>>();
                int cId = 0;

                for (int i = 0; i < n; i++)
                {
                    if (cMap[i] == -1)
                    {
                        List<int> cComp = new List<int>();
                        for (int j = 0; j < n; j++) if (S[i, j] == 1) { cMap[j] = cId; cComp.Add(j); }
                        comps.Add(cComp); cId++;
                    }
                }

                Console.WriteLine($"\n\nКСС: {comps.Count}");
                for (int c = 0; c < comps.Count; c++)
                {
                    Console.Write($"K{c + 1}: ");
                    foreach (int v in comps[c]) Console.Write($"{v + 1} ");
                    Console.WriteLine();
                }

                A_cond = new int[comps.Count, comps.Count];
                condLbs = new string[comps.Count];
                for (int i = 0; i < comps.Count; i++) condLbs[i] = $"K{i + 1}";

                for (int i = 0; i < n; i++) for (int j = 0; j < n; j++)
                    if (A_dir2[i, j] == 1 && cMap[i] != cMap[j]) A_cond[cMap[i], cMap[j]] = 1;
            }

            private PointF[] GetTriangleCoords(int count, float offsetX, float offsetY)
            {
                PointF[] c = new PointF[count];
                PointF[] s = { new PointF(offsetX + 250, offsetY), new PointF(offsetX + 500, offsetY + 350), new PointF(offsetX, offsetY + 350), new PointF(offsetX + 250, offsetY) };
                int[] ps = { 5, 5, 4 }; int idx = 0;
                for (int side = 0; side < 3; side++)
                {
                    float dx = (s[side + 1].X - s[side].X) / ps[side], dy = (s[side + 1].Y - s[side].Y) / ps[side];
                    for (int i = 0; i < ps[side]; i++) c[idx++] = new PointF(s[side].X + dx * i, s[side].Y + dy * i);
                }
                return c;
            }

            private void CalculateCoordinates()
            {
                cDir1 = GetTriangleCoords(n, 50, 50); cUndir1 = GetTriangleCoords(n, 600, 50); cDir2 = GetTriangleCoords(n, 50, 500);
                int numCond = condLbs.Length;
                cCond = new PointF[numCond];
                for (int i = 0; i < numCond; i++)
                    cCond[i] = new PointF(850 + (float)(Math.Cos(2 * Math.PI * i / numCond) * 150), 675 + (float)(Math.Sin(2 * Math.PI * i / numCond) * 150));
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
                r.DrawGraph(e.Graphics, cDir1, A_dir1, true, "1. Dir (k=0.62)");
                r.DrawGraph(e.Graphics, cUndir1, A_undir1, false, "2. Undir (k=0.62)");
                r.DrawGraph(e.Graphics, cDir2, A_dir2, true, "3. Dir (k=0.69)");
                r.DrawGraph(e.Graphics, cCond, A_cond, true, "4. Condensation", customLabels: condLbs);
            }
        }
    }
}
