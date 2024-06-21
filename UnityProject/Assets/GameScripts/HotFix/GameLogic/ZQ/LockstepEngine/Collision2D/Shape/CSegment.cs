namespace Lockstep.Framework
{
    public class CSegment : CBaseShape
    {
        public override int TypeId => (int)EShape2D.Segment;
        public LVector2 pos1;
        public LVector2 pos2;
    }
}