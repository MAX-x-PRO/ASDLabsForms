using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ASDLabsForms.Labs
{
    public class Lab6Form : Form
    {
        private int vertexCount = 14;
        private int[,] adjacencyUndirected;
        private int[,] weightMatrix;

        private PointF[] coordinatesOriginal;
        private PointF[] coordinatesMST;

        private List<MstStep> kruskalSteps = new List<MstStep>();
        private int currentStepIndex = 0;

        private Button btnNextStep;
        private GraphRenderer graphRenderer = new GraphRenderer();

        public Lab6Form()
        {
            this.Text = "Lab 6 - Variant 5344";
            this.Size = new Size(1300, 800);
            this.BackColor = Color.White;

            GenerateMatrices();
            CalculateGraphCoordinates();
            RunKruskalAlgorithm();
            SetupUserInterface();
        }

        private void GenerateMatrices()
        {
            int[,] A_dir = new int[vertexCount, vertexCount];
            adjacencyUndirected = new int[vertexCount, vertexCount];
            Random rndA = new Random(5344);
            double k = 0.89;

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    A_dir[i, j] = (rndA.NextDouble() * 2.0 * k >= 1.0) ? 1 : 0;

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    if (A_dir[i, j] == 1) { adjacencyUndirected[i, j] = 1; adjacencyUndirected[j, i] = 1; }

            double[,] B = new double[vertexCount, vertexCount];
            int[,] C = new int[vertexCount, vertexCount];
            int[,] D = new int[vertexCount, vertexCount];
            int[,] H = new int[vertexCount, vertexCount];
            int[,] Tr = new int[vertexCount, vertexCount];
            weightMatrix = new int[vertexCount, vertexCount];

            Random rndB = new Random(5344);

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    B[i, j] = rndB.NextDouble() * 2.0;

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    C[i, j] = (int)Math.Ceiling(B[i, j] * 100 * adjacencyUndirected[i, j]);

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    D[i, j] = C[i, j] > 0 ? 1 : 0;

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    H[i, j] = D[i, j] != D[j, i] ? 1 : 0;

            for (int i = 0; i < vertexCount; i++)
                for (int j = 0; j < vertexCount; j++)
                    Tr[i, j] = i < j ? 1 : 0;

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < vertexCount; j++)
                {
                    if (i < j)
                    {
                        weightMatrix[i, j] = (D[i, j] + H[i, j] * Tr[i, j]) * C[i, j];
                        weightMatrix[j, i] = weightMatrix[i, j];
                    }
                }
            }

            Console.WriteLine("Adjacency Matrix (A_undir)");
            PrintMatrixToConsole(adjacencyUndirected);
            Console.WriteLine("\nWeight Matrix (W)");
            PrintMatrixToConsole(weightMatrix);
        }

        private void RunKruskalAlgorithm()
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (weightMatrix[i, j] > 0)
                        edges.Add(new Edge { U = i, V = j, Weight = weightMatrix[i, j] });
                }
            }
            edges.Sort();

            int[] parent = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++) parent[i] = i;

            int Find(int i)
            {
                if (parent[i] == i) return i;
                return parent[i] = Find(parent[i]);
            }

            void Union(int i, int j)
            {
                int rootI = Find(i);
                int rootJ = Find(j);
                if (rootI != rootJ) parent[rootI] = rootJ;
            }

            int[,] currentTree = new int[vertexCount, vertexCount];
            Color[,] currentColors = new Color[vertexCount, vertexCount];
            int currentWeight = 0;

            kruskalSteps.Add(new MstStep { TreeMatrix = (int[,])currentTree.Clone(), EdgeColors = (Color[,])currentColors.Clone(), TotalWeight = 0 });

            foreach (var edge in edges)
            {
                if (Find(edge.U) != Find(edge.V))
                {
                    Union(edge.U, edge.V);
                    currentTree[edge.U, edge.V] = 1;
                    currentTree[edge.V, edge.U] = 1;
                    currentColors[edge.U, edge.V] = Color.Red;
                    currentColors[edge.V, edge.U] = Color.Red;
                    currentWeight += edge.Weight;

                    kruskalSteps.Add(new MstStep { TreeMatrix = (int[,])currentTree.Clone(), EdgeColors = (Color[,])currentColors.Clone(), TotalWeight = currentWeight });
                }
            }
            Console.WriteLine($"\nKruskal's algorithm completed. Total weight: {currentWeight}");
        }

        private void PrintMatrixToConsole(int[,] matrix)
        {
            for (int row = 0; row < vertexCount; row++)
            {
                for (int col = 0; col < vertexCount; col++)
                {
                    Console.Write($"{matrix[row, col],4} ");
                }
                Console.WriteLine();
            }
        }

        private void CalculateGraphCoordinates()
        {
            coordinatesOriginal = new PointF[vertexCount];
            coordinatesMST = new PointF[vertexCount];
            PointF[] triangleCorners = { new PointF(300, 100), new PointF(550, 450), new PointF(50, 450), new PointF(300, 100) };
            int[] pointsPerSide = { 5, 5, 4 };
            int pointIndex = 0;

            for (int side = 0; side < 3; side++)
            {
                float deltaX = (triangleCorners[side + 1].X - triangleCorners[side].X) / pointsPerSide[side];
                float deltaY = (triangleCorners[side + 1].Y - triangleCorners[side].Y) / pointsPerSide[side];
                for (int i = 0; i < pointsPerSide[side]; i++)
                {
                    coordinatesOriginal[pointIndex] = new PointF(triangleCorners[side].X + deltaX * i, triangleCorners[side].Y + deltaY * i);
                    coordinatesMST[pointIndex] = new PointF(coordinatesOriginal[pointIndex].X + 650, coordinatesOriginal[pointIndex].Y);
                    pointIndex++;
                }
            }
        }

        private void SetupUserInterface()
        {
            btnNextStep = new Button { Text = "Next Step", Location = new Point(600, 50), Size = new Size(150, 40) };
            btnNextStep.Click += (sender, args) => { if (currentStepIndex < kruskalSteps.Count - 1) currentStepIndex++; this.Invalidate(); };
            this.Controls.Add(btnNextStep);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            graphRenderer.DrawGraph(e.Graphics, coordinatesOriginal, adjacencyUndirected, false, "Initial Graph", null, null, null, weightMatrix);

            if (kruskalSteps.Count > 0)
            {
                var step = kruskalSteps[currentStepIndex];
                graphRenderer.DrawGraph(e.Graphics, coordinatesMST, step.TreeMatrix, false, $"MST (Step {currentStepIndex}/{kruskalSteps.Count - 1}) Weight: {step.TotalWeight}", null, null, step.EdgeColors, weightMatrix);
            }
        }
    }
}
