using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Enemy : Entity
    {
        public CBrain brain = new CBrain();

        public override void BindRef()
        {
            base.BindRef();
            RegisterComponent(brain);
            moveSpd = 2;
            turnSpd = 150;
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
            writer.Write(curHealth);
            writer.Write(damage);
            writer.Write(isFire);
            writer.Write(isInvincible);
            writer.Write(maxHealth);
            writer.Write(moveSpd);
            writer.Write(turnSpd);
            animator.WriteBackup(writer);
            brain.WriteBackup(writer);
            colliderData.WriteBackup(writer);
            rigidbody.WriteBackup(writer);
            skillBox.WriteBackup(writer);
            transform.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            curHealth = reader.ReadInt32();
            damage = reader.ReadInt32();
            isFire = reader.ReadBoolean();
            isInvincible = reader.ReadBoolean();
            maxHealth = reader.ReadInt32();
            moveSpd = reader.ReadLFloat();
            turnSpd = reader.ReadLFloat();
            animator.ReadBackup(reader);
            brain.ReadBackup(reader);
            colliderData.ReadBackup(reader);
            rigidbody.ReadBackup(reader);
            skillBox.ReadBackup(reader);
            transform.ReadBackup(reader);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isFire.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += animator.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += brain.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
            sb.AppendLine(prefix + "curHealth" + ":" + curHealth.ToString());
            sb.AppendLine(prefix + "damage" + ":" + damage.ToString());
            sb.AppendLine(prefix + "isFire" + ":" + isFire.ToString());
            sb.AppendLine(prefix + "isInvincible" + ":" + isInvincible.ToString());
            sb.AppendLine(prefix + "maxHealth" + ":" + maxHealth.ToString());
            sb.AppendLine(prefix + "moveSpd" + ":" + moveSpd.ToString());
            sb.AppendLine(prefix + "turnSpd" + ":" + turnSpd.ToString());
            sb.AppendLine(prefix + "animator" + ":"); animator.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "brain" + ":"); brain.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "colliderData" + ":"); colliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "skillBox" + ":"); skillBox.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); transform.DumpStr(sb, "\t" + prefix);
        }
    }
}