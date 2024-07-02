using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;


namespace Lockstep.Framework
{
    public class CEntityBase : IComponent
    {
        public CEntityBase(Entity entity) : base(entity) { }
        public int EntityId;

        public override void Serialize(Serializer writer)
        {
            writer.Write(EntityId);
        }

        public override void Deserialize(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
        }
    }
}