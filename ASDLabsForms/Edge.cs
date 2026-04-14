using System;

namespace ASDLabsForms
{
    public class Edge : IComparable<Edge>
    {
        public int U;
        public int V;
        public int Weight;
        public int CompareTo(Edge other) => this.Weight.CompareTo(other.Weight);
    }
}
