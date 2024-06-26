using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Player : Entity
    {
        public int localId;
        public PlayerCommands input = new PlayerCommands();

        public CMover _mover = null;
        private CAnimator _animator = null;
        private Skill _skill= null;

        public override void Start()
        {
            var config = GameConfigSingleton.Instance.GetSkillConfig();
            _skill = new Skill(this, config);
            _skill.OnSkillStartHandler = OnSkillStart;
            _skill.OnSkillPartStartHandler = OnSkillPartStart;
            _skill.OnSkillDoneHandler = OnSkillDone;

            _mover = new CMover(this);
            _animator = new CAnimator(this);
            RegisterComponent(_animator);
            RegisterComponent(_mover);
            RegisterComponent(_skill);
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

        public void OnSkillStart(Skill skill)
        {
            isInvincible = true;
            //entity.animator?.Play(AnimName);
        }

        public void OnSkillPartStart(Skill skill)
        {
            //Debug.Log("OnSkillPartStart " + skill.SkillInfo.animName );
        }

        public void OnSkillDone(Skill skill)
        {
            isInvincible = false;
            //entity.animator?.Play(AnimDefine.Idle);
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
            writer.Write(curHealth);
            writer.Write(damage);
            writer.Write(isFire);
            writer.Write(isInvincible);
            writer.Write(localId);
            writer.Write(maxHealth);
            writer.Write(moveSpd);
            writer.Write(turnSpd);
            _animator.WriteBackup(writer);
            colliderData.WriteBackup(writer);
            //input.WriteBackup(writer);
            _mover.WriteBackup(writer);
            rigidbody.WriteBackup(writer);
            _skill.WriteBackup(writer);
            LTrans2D.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            curHealth = reader.ReadInt32();
            damage = reader.ReadInt32();
            isFire = reader.ReadBoolean();
            isInvincible = reader.ReadBoolean();
            localId = reader.ReadInt32();
            maxHealth = reader.ReadInt32();
            moveSpd = reader.ReadLFloat();
            turnSpd = reader.ReadLFloat();
            _animator.ReadBackup(reader);
            colliderData.ReadBackup(reader);
            //input.ReadBackup(reader);
            _mover.ReadBackup(reader);
            rigidbody.ReadBackup(reader);
            _skill.ReadBackup(reader);
            LTrans2D.ReadBackup(reader);
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
            hash += localId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += maxHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += moveSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += turnSpd.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _animator.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += colliderData.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //hash += input.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _mover.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += rigidbody.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += _skill.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
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
            sb.AppendLine(prefix + "localId" + ":" + localId.ToString());
            sb.AppendLine(prefix + "maxHealth" + ":" + maxHealth.ToString());
            sb.AppendLine(prefix + "moveSpd" + ":" + moveSpd.ToString());
            sb.AppendLine(prefix + "turnSpd" + ":" + turnSpd.ToString());
            sb.AppendLine(prefix + "animator" + ":"); _animator.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "colliderData" + ":"); colliderData.DumpStr(sb, "\t" + prefix);
            //sb.AppendLine(prefix + "input" +":");  input.DumpStr(sb,"\t" + prefix);
            sb.AppendLine(prefix + "mover" + ":"); _mover.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "rigidbody" + ":"); rigidbody.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "skillBox" + ":"); _skill.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); LTrans2D.DumpStr(sb, "\t" + prefix);
        }
    }
}