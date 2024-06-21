namespace Lockstep.Framework
{
    public class CCircle : CBaseShape
    {
        public override int TypeId => (int)EShape2D.Circle;
        public LFloat radius;

        public CCircle() : this(LFloat.zero) { }

        public CCircle(LFloat radius)
        {
            this.radius = radius;
        }

        public override string ToString()
        {
            return $"radius:{radius}";
        }
    }
}