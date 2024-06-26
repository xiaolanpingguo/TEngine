using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Player : Entity
    {
        public PlayerCommands input = new PlayerCommands();

        private CCharacterController _characterController = null;
        private CAnimator _animator = null;
        private CSkill _skill= null;

        public PlayerView EntityView = null;

        private int _damage = 10;
        private bool _isInvincible = false;

        public int CurHealth;
        public int MaxHealth = 100;
        public bool IsDead => CurHealth <= 0;

        public override void Start()
        {
            var config = GameConfigSingleton.Instance.GetSkillConfig();
            _skill = new CSkill(this, config);
            _skill.OnSkillStartHandler = OnSkillStart;
            _skill.OnSkillPartStartHandler = OnSkillPartStart;
            _skill.OnSkillDoneHandler = OnSkillDone;

            _characterController = new CCharacterController(this);
            _animator = new CAnimator(this);
            RegisterComponent(_animator);
            RegisterComponent(_characterController);
            RegisterComponent(_skill);

            CurHealth = MaxHealth;

            base.Start();
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
            if (input.skillId != 0)
            {
                _skill.Fire();
            }
        }


        public void StopSkill()
        {
            _skill.ForceStop();
        }

        public void OnSkillStart(CSkill skill)
        {
            //_animator?.Play(AnimName);
        }

        public void OnSkillPartStart(CSkill skill)
        {
            //Debug.Log("OnSkillPartStart " + skill.SkillInfo.animName );
        }

        public void OnSkillDone(CSkill skill)
        {
            //_animator?.Play(AnimDefine.Idle);
        }

        public override void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            engineTransform = null;
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

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
            writer.Write(CurHealth);
            writer.Write(_damage);
            writer.Write(_isInvincible);
            writer.Write(MaxHealth);
            //writer.Write(_moveSpd);
            //writer.Write(_turnSpd);
            _animator.WriteBackup(writer);
            colliderData.WriteBackup(writer);
            //input.WriteBackup(writer);
            _characterController.WriteBackup(writer);
            rigidbody.WriteBackup(writer);
            _skill.WriteBackup(writer);
            LTrans2D.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            CurHealth = reader.ReadInt32();
            _damage = reader.ReadInt32();
            _isInvincible = reader.ReadBoolean();
            MaxHealth = reader.ReadInt32();
            //_moveSpd = reader.ReadLFloat();
            //_turnSpd = reader.ReadLFloat();
            _animator.ReadBackup(reader);
            colliderData.ReadBackup(reader);
            //input.ReadBackup(reader);
            _characterController.ReadBackup(reader);
            rigidbody.ReadBackup(reader);
            _skill.ReadBackup(reader);
            LTrans2D.ReadBackup(reader);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += CurHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _damage.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _isInvincible.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += MaxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += _moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += _turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _animator.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += input.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _characterController.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _skill.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
            sb.AppendLine(prefix + "curHealth" + ":" + CurHealth.ToString());
            sb.AppendLine(prefix + "damage" + ":" + _damage.ToString());
            sb.AppendLine(prefix + "isInvincible" + ":" + _isInvincible.ToString());
            sb.AppendLine(prefix + "maxHealth" + ":" + MaxHealth.ToString());
            //sb.AppendLine(prefix + "moveSpd" + ":" + _moveSpd.ToString());
            //sb.AppendLine(prefix + "turnSpd" + ":" + _turnSpd.ToString());
            sb.AppendLine(prefix + "animator" + ":"); _animator.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "colliderData" + ":"); colliderData.DumpStr(sb, "\t" + prefix);
            //sb.AppendLine(prefix + "input" +":");  input.DumpStr(sb,"\t" + prefix);
            sb.AppendLine(prefix + "mover" + ":"); _characterController.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "skillBox" + ":"); _skill.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); LTrans2D.DumpStr(sb, "\t" + prefix);
        }
    }
}