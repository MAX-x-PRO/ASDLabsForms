using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ASDLabsForms
{
    public class GraphRenderer
    {
        public int VertexRadius { get; set; } = 16;
        public Font VertexFont { get; set; } = new Font("Arial", 10, FontStyle.Bold);
        public Font WeightFont { get; set; } = new Font("Consolas", 10, FontStyle.Bold);
        public Color DefaultVertexColor { get; set; } = Color.White;
        public Color DefaultEdgeColor { get; set; } = Color.Gray;

        public void DrawGraph(
            Graphics g,
            PointF[] vertices,
            int[,] adjacencyMatrix,
            bool isDirected,
            string title = "",
            string[] customLabels = null,
            Color[] vertexColors = null,  
            Color[,] edgeColors = null,     
            int[,] weights = null)         
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int n = vertices.Length;

            // Drawing title
            float minY = vertices[0].Y;
            for (int i = 1; i < vertices.Length; i++)
            {
                if (vertices[i].Y < minY) minY = vertices[i].Y;
            }
            if (!string.IsNullOrEmpty(title))
            {
                g.DrawString(title, new Font("Arial", 14, FontStyle.Bold), Brushes.DarkSlateBlue, vertices[0].X - 50, minY - 40);
            }

            // Drawing edges
            for (int i = 0; i < n; i++)
            {
                int startJ = isDirected ? 0 : i;

                for (int j = startJ; j < n; j++)
                {
                    if (adjacencyMatrix[i, j] > 0)
                    {
                        Color eColor = (edgeColors != null && edgeColors[i, j] != Color.Empty)
                                       ? edgeColors[i, j]
                                       : DefaultEdgeColor;

                        float penWidth = (eColor != DefaultEdgeColor) ? 3.0f : 1.5f;
                        Pen edgePen = new Pen(eColor, penWidth);

                        if (isDirected)
                            edgePen.CustomEndCap = new AdjustableArrowCap(5, 5, true);

                        bool isSelfLoop = (i == j);
                        string weightText = (weights != null) ? weights[i, j].ToString() : "";

                        DrawEdge(g, edgePen, vertices[i], vertices[j], isSelfLoop, weightText);
                    }
                }
            }

            // Drawing vertices
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            for (int i = 0; i < n; i++)
            {
                Color vColor = (vertexColors != null && i < vertexColors.Length) ? vertexColors[i] : DefaultVertexColor;

                string label = (customLabels != null && i < customLabels.Length) ? customLabels[i] : (i + 1).ToString();

                using (Brush bgBrush = new SolidBrush(vColor))
                {
                    g.FillEllipse(bgBrush, vertices[i].X - VertexRadius, vertices[i].Y - VertexRadius, 2 * VertexRadius, 2 * VertexRadius);
                }

                g.DrawEllipse(Pens.Black, vertices[i].X - VertexRadius, vertices[i].Y - VertexRadius, 2 * VertexRadius, 2 * VertexRadius);
                g.DrawString(label, VertexFont, Brushes.Black, vertices[i].X, vertices[i].Y, sf);
            }
        }

        private void DrawEdge(Graphics g, Pen pen, PointF p1, PointF p2, bool isSelfLoop, string weightText)
        {
            if (isSelfLoop)
            {
                float loopSize = VertexRadius * 1.5f;
                g.DrawEllipse(pen, p1.X - VertexRadius, p1.Y - VertexRadius - loopSize + 5, loopSize, loopSize);

                if (!string.IsNullOrEmpty(weightText))
                {
                    DrawWeightLabel(g, weightText, p1.X - VertexRadius, p1.Y - VertexRadius - loopSize);
                }
                return;
            }

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length == 0) return;

            float offsetX = (dx / length) * VertexRadius;
            float offsetY = (dy / length) * VertexRadius;

            PointF start = new PointF(p1.X + offsetX, p1.Y + offsetY);
            PointF end = new PointF(p2.X - offsetX, p2.Y - offsetY);

            g.DrawLine(pen, start, end);

            // Centring weight label
            if (!string.IsNullOrEmpty(weightText))
            {
                float midX = (start.X + end.X) / 2;
                float midY = (start.Y + end.Y) / 2;
                DrawWeightLabel(g, weightText, midX, midY);
            }
        }

        private void DrawWeightLabel(Graphics g, string text, float x, float y)
        {
            if (string.IsNullOrEmpty(text)) return;

            SizeF textSize = g.MeasureString(text, WeightFont);
            RectangleF bgRect = new RectangleF(x - textSize.Width / 2, y - textSize.Height / 2, textSize.Width, textSize.Height);

            using (Brush bgBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.FillRectangle(bgBrush, bgRect);
            }
            g.DrawString(text, WeightFont, Brushes.DarkRed, bgRect.X, bgRect.Y);
        }
    }
}
