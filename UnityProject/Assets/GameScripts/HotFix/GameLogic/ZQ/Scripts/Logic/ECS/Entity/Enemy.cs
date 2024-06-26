using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Enemy : Entity
    {
        private CAIController _aiontroller = null;

        public EnemyView EntityView = null;

        private bool _isInvincible = false;

        public int CurHealth;
        public int MaxHealth = 100;
        public bool IsDead => CurHealth <= 0;

        public override void Start()
        {
            _aiontroller = new CAIController(this);
            RegisterComponent(_aiontroller);
            CurHealth = MaxHealth;
            base.Start();
        }

        public void TakeDamage(Entity atker, int amount, LVector3 hitPoint)
        {
            if (_isInvincible || IsDead)
            {
                return;
            }

            CurHealth -= amount;
            EntityView?.OnTakeDamage(amount, hitPoint);
            if (IsDead)
            {
                EntityView?.OnDead();
                PhysicSystem.Instance.RemoveCollider(this);
                World.Instance.DestroyEntity(this);
            }
        }

        public override void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            UserData = null;
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
            writer.Write(CurHealth);
            writer.Write(_isInvincible);
            writer.Write(MaxHealth);
            _aiontroller.WriteBackup(writer);
            ColliderData.WriteBackup(writer);
            Rigidbody.WriteBackup(writer);
            LTrans2D.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            CurHealth = reader.ReadInt32();
            _isInvincible = reader.ReadBoolean();
            MaxHealth = reader.ReadInt32();
            _aiontroller.ReadBackup(reader);
            ColliderData.ReadBackup(reader);
            Rigidbody.ReadBackup(reader);
            LTrans2D.ReadBackup(reader);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CurHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MaxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _aiontroller.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += ColliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
            sb.AppendLine(prefix + "curHealth" + ":" + CurHealth.ToString());
            sb.AppendLine(prefix + "isInvincible" + ":" + _isInvincible.ToString());
            sb.AppendLine(prefix + "maxHealth" + ":" + MaxHealth.ToString());
            sb.AppendLine(prefix + "brain" + ":"); _aiontroller.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "colliderData" + ":"); ColliderData.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); Rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); LTrans2D.DumpStr(sb, "\t" + prefix);
        }
    }
}