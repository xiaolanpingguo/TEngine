namespace Lockstep.Framework
{
    public class CPolygon : CCircle
    {
        public override int TypeId => (int)EShape2D.Polygon;
        public int vertexCount;
        public LFloat deg;
        public LVector2[] vertexes;
    }
}