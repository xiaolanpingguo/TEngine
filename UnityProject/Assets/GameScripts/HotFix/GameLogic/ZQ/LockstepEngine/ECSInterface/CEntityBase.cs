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
        public int PrefabId;

        public override void Serialize(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
        }

        public override void Deserialize(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
        }
    }
}