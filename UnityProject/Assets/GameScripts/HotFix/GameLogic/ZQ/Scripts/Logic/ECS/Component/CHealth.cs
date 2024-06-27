using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class CHealth : IComponent
    {
        public CHealth(Entity entity) : base(entity)
        {
            CurHealth = MaxHealth;
        }

        public Action<Entity, int, LVector3> OnDamage { get; set; }
        public int CurHealth;
        public int MaxHealth = 100;
        public bool IsDead => CurHealth <= 0;

        public void TakeDamage(Entity attacker, int amount, LVector3 hitPoint)
        {
            if (IsDead)
            {
                return;
            }

            CurHealth -= amount;
            OnDamage?.Invoke(attacker, amount, hitPoint);
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(CurHealth);
            writer.Write(MaxHealth);
        }

        public override void Deserialize(Deserializer reader)
        {
            CurHealth = reader.ReadInt32();
            MaxHealth = reader.ReadInt32();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += CurHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MaxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "curHealth" + ":" + CurHealth.ToString());
            sb.AppendLine(prefix + "maxHealth" + ":" + MaxHealth.ToString());
        }
    }
}