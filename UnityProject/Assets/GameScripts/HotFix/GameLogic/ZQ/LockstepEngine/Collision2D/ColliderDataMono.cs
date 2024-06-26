using System;
using System.Text;
using UnityEngine;


namespace Lockstep.Framework 
{
    public class ColliderData : IComponent
    {
        [Header("Offset")]
        public LFloat y;
        public LVector2 pos;

        [Header("Collider data")]
        public LFloat high;
        public LFloat radius;
        public LVector2 size;
        public LVector2 up;
        public LFloat deg;

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(deg);
            writer.Write(high);
            writer.Write(pos);
            writer.Write(radius);
            writer.Write(size);
            writer.Write(up);
            writer.Write(y);
        }

        public override void ReadBackup(Deserializer reader)
        {
            deg = reader.ReadLFloat();
            high = reader.ReadLFloat();
            pos = reader.ReadLVector2();
            radius = reader.ReadLFloat();
            size = reader.ReadLVector2();
            up = reader.ReadLVector2();
            y = reader.ReadLFloat();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += deg.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += high.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += pos.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += radius.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += size.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += up.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += y.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "deg" + ":" + deg.ToString());
            sb.AppendLine(prefix + "high" + ":" + high.ToString());
            sb.AppendLine(prefix + "pos" + ":" + pos.ToString());
            sb.AppendLine(prefix + "radius" + ":" + radius.ToString());
            sb.AppendLine(prefix + "size" + ":" + size.ToString());
            sb.AppendLine(prefix + "up" + ":" + up.ToString());
            sb.AppendLine(prefix + "y" + ":" + y.ToString());
        }
    }
}