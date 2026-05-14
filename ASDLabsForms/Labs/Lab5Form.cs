using ASDLabsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ASD_Labs
{
    public class Lab5Form : Form
    {
        private int vertexCount = 14;
        private int[,] adjacencyMatrix;

        private PointF[] coordinatesDFS;
        private PointF[] coordinatesBFS;

        private List<StepState> dfsStepsList = new List<StepState>();
        private List<StepState> bfsStepsList = new List<StepState>();

        private int currentDfsStepIndex = 0;
        private int currentBfsStepIndex = 0;

        private Button btnNextDfs;
        private Button btnNextBfs;
        private GraphRenderer graphRenderer = new GraphRenderer();

        public Lab5Form()
        {
            this.Text = "Lab 5 (Variant 5344)";
            this.Size = new Size(1200, 800);
            this.BackColor = Color.White;

            GenerateAdjacencyMatrix();
            CalculateGraphCoordinates();
            RunDepthFirstSearch();
            RunBreadthFirstSearch();
            SetupUserInterface();
        }

        private void GenerateAdjacencyMatrix()
        {
            adjacencyMatrix = new int[vertexCount, vertexCount];
            Random randomGenerator = new Random(5344);
            double coefficientK = 0.79;

            Console.WriteLine("=== Adjacency Matrix of the Graph ===");
            for (int row = 0; row < vertexCount; row++)
            {
                for (int col = 0; col < vertexCount; col++)
                {
                    adjacencyMatrix[row, col] = (randomGenerator.NextDouble() * 2.0 * coefficientK >= 1.0) ? 1 : 0;
                    Console.Write(adjacencyMatrix[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        private int FindStartVertex(int[] vertexStatuses)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                if (vertexStatuses[i] == 0)
                {
                    for (int j = 0; j < vertexCount; j++)
                    {
                        if (adjacencyMatrix[i, j] > 0) return i;
                    }
                }
            }
            for (int i = 0; i < vertexCount; i++)
            {
                if (vertexStatuses[i] == 0) return i;
            }
            return -1;
        }

        private void RunDepthFirstSearch()
        {
            int[] vertexStatuses = new int[vertexCount];
            Color[,] edgeColorsMatrix = new Color[vertexCount, vertexCount];
            int[,] treeAdjacencyMatrix = new int[vertexCount, vertexCount];
            string[] nodeLabels = new string[vertexCount];
            List<int> visitedNodesOrder = new List<int>();
            int visitCounter = 1;

            for (int i = 0; i < vertexCount; i++) nodeLabels[i] = (i + 1).ToString();

            void SaveCurrentStep()
            {
                StepState state = new StepState
                {
                    VertexColors = new Color[vertexCount],
                    EdgeColors = new Color[vertexCount, vertexCount],
                    VertexLabels = new string[vertexCount],
                    TreeMatrix = (int[,])treeAdjacencyMatrix.Clone()
                };
                for (int i = 0; i < vertexCount; i++)
                {
                    state.VertexColors[i] = vertexStatuses[i] == 0 ? Color.White : (vertexStatuses[i] == 1 ? Color.LightGray : Color.DarkGray);
                    state.VertexLabels[i] = nodeLabels[i];
                    for (int j = 0; j < vertexCount; j++)
                        state.EdgeColors[i, j] = edgeColorsMatrix[i, j] != Color.Empty ? edgeColorsMatrix[i, j] : Color.LightGray;
                }
                dfsStepsList.Add(state);
            }

            void DfsRecursiveVisit(int currentNode)
            {
                vertexStatuses[currentNode] = 1;
                nodeLabels[currentNode] = $"{currentNode + 1} ({visitCounter++})";
                visitedNodesOrder.Add(currentNode + 1);
                SaveCurrentStep();

                for (int neighborNode = 0; neighborNode < vertexCount; neighborNode++)
                {
                    if (adjacencyMatrix[currentNode, neighborNode] > 0 && vertexStatuses[neighborNode] == 0)
                    {
                        edgeColorsMatrix[currentNode, neighborNode] = Color.Red;
                        treeAdjacencyMatrix[currentNode, neighborNode] = 1;
                        SaveCurrentStep();
                        DfsRecursiveVisit(neighborNode);
                    }
                }
                vertexStatuses[currentNode] = 2;
                SaveCurrentStep();
            }

            while (true)
            {
                int startNode = FindStartVertex(vertexStatuses);
                if (startNode == -1) break;
                DfsRecursiveVisit(startNode);
            }

            Console.WriteLine("\n=== DFS Tree (Matrix) ===");
            PrintMatrixToConsole(treeAdjacencyMatrix);
            Console.WriteLine("DFS Vector: " + string.Join(" -> ", visitedNodesOrder));
        }

        private void RunBreadthFirstSearch()
        {
            int[] vertexStatuses = new int[vertexCount];
            Color[,] edgeColorsMatrix = new Color[vertexCount, vertexCount];
            int[,] treeAdjacencyMatrix = new int[vertexCount, vertexCount];
            string[] nodeLabels = new string[vertexCount];
            List<int> visitedNodesOrder = new List<int>();
            int visitCounter = 1;

            for (int i = 0; i < vertexCount; i++) nodeLabels[i] = (i + 1).ToString();

            void SaveCurrentStep()
            {
                StepState state = new StepState
                {
                    VertexColors = new Color[vertexCount],
                    EdgeColors = new Color[vertexCount, vertexCount],
                    VertexLabels = new string[vertexCount],
                    TreeMatrix = (int[,])treeAdjacencyMatrix.Clone()
                };
                for (int i = 0; i < vertexCount; i++)
                {
                    state.VertexColors[i] = vertexStatuses[i] == 0 ? Color.White : (vertexStatuses[i] == 1 ? Color.LightGray : Color.DarkGray);
                    state.VertexLabels[i] = nodeLabels[i];
                    for (int j = 0; j < vertexCount; j++)
                        state.EdgeColors[i, j] = edgeColorsMatrix[i, j] != Color.Empty ? edgeColorsMatrix[i, j] : Color.LightGray;
                }
                bfsStepsList.Add(state);
            }

            while (true)
            {
                int startNode = FindStartVertex(vertexStatuses);
                if (startNode == -1) break;

                Queue<int> traversalQueue = new Queue<int>();
                vertexStatuses[startNode] = 1;
                nodeLabels[startNode] = $"{startNode + 1} ({visitCounter++})";
                visitedNodesOrder.Add(startNode + 1);
                traversalQueue.Enqueue(startNode);
                SaveCurrentStep();

                while (traversalQueue.Count > 0)
                {
                    int currentNode = traversalQueue.Dequeue();
                    for (int neighborNode = 0; neighborNode < vertexCount; neighborNode++)
                    {
                        if (adjacencyMatrix[currentNode, neighborNode] > 0 && vertexStatuses[neighborNode] == 0)
                        {
                            vertexStatuses[neighborNode] = 1;
                            nodeLabels[neighborNode] = $"{neighborNode + 1} ({visitCounter++})";
                            visitedNodesOrder.Add(neighborNode + 1);

                            edgeColorsMatrix[currentNode, neighborNode] = Color.Blue;
                            treeAdjacencyMatrix[currentNode, neighborNode] = 1;
                            SaveCurrentStep();

                            traversalQueue.Enqueue(neighborNode);
                        }
                    }
                    vertexStatuses[currentNode] = 2;
                    SaveCurrentStep();
                }
            }

            Console.WriteLine("\n=== BFS Tree (Matrix) ===");
            PrintMatrixToConsole(treeAdjacencyMatrix);
            Console.WriteLine("BFS Vector: " + string.Join(" -> ", visitedNodesOrder));
        }

        private void PrintMatrixToConsole(int[,] matrixToPrint)
        {
            for (int row = 0; row < vertexCount; row++)
            {
                for (int col = 0; col < vertexCount; col++)
                {
                    Console.Write(matrixToPrint[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        private void CalculateGraphCoordinates()
        {
            coordinatesDFS = new PointF[vertexCount];
            coordinatesBFS = new PointF[vertexCount];
            PointF[] triangleCorners = { new PointF(300, 100), new PointF(550, 450), new PointF(50, 450), new PointF(300, 100) };
            int[] pointsPerSide = { 5, 5, 4 };
            int pointIndex = 0;

            for (int side = 0; side < 3; side++)
            {
                float deltaX = (triangleCorners[side + 1].X - triangleCorners[side].X) / pointsPerSide[side];
                float deltaY = (triangleCorners[side + 1].Y - triangleCorners[side].Y) / pointsPerSide[side];
                for (int i = 0; i < pointsPerSide[side]; i++)
                {
                    coordinatesDFS[pointIndex] = new PointF(triangleCorners[side].X + deltaX * i, triangleCorners[side].Y + deltaY * i);
                    coordinatesBFS[pointIndex] = new PointF(coordinatesDFS[pointIndex].X + 600, coordinatesDFS[pointIndex].Y);
                    pointIndex++;
                }
            }
        }

        private void SetupUserInterface()
        {
            btnNextDfs = new Button { Text = "DFS Step", Location = new Point(250, 50), Size = new Size(100, 30) };
            btnNextDfs.Click += (sender, args) => { if (currentDfsStepIndex < dfsStepsList.Count - 1) currentDfsStepIndex++; this.Invalidate(); };
            this.Controls.Add(btnNextDfs);

            btnNextBfs = new Button { Text = "BFS Step", Location = new Point(850, 50), Size = new Size(100, 30) };
            btnNextBfs.Click += (sender, args) => { if (currentBfsStepIndex < bfsStepsList.Count - 1) currentBfsStepIndex++; this.Invalidate(); };
            this.Controls.Add(btnNextBfs);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (dfsStepsList.Count > 0)
            {
                var currentDfsState = dfsStepsList[currentDfsStepIndex];
                graphRenderer.DrawGraph(e.Graphics, coordinatesDFS, adjacencyMatrix, true,
                    $"DFS step {currentDfsStepIndex}/{dfsStepsList.Count - 1}", currentDfsState.VertexLabels, currentDfsState.VertexColors, currentDfsState.EdgeColors);
            }
            if (bfsStepsList.Count > 0)
            {
                var currentBfsState = bfsStepsList[currentBfsStepIndex];
                graphRenderer.DrawGraph(e.Graphics, coordinatesBFS, adjacencyMatrix, true,
                    $"BFS step {currentBfsStepIndex}/{bfsStepsList.Count - 1}", currentBfsState.VertexLabels, currentBfsState.VertexColors, currentBfsState.EdgeColors);
            }
        }
    }
}