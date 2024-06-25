using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;
using System.Text;


namespace Lockstep.Game
{
    [Serializable]
    public class CSkillBox : IComponent, ISkillEventHandler
    {
        public int configId;
        public bool isFiring;
        [HideInInspector]public SkillBoxConfig config;
        private int _curSkillIdx = 0;
        private List<Skill> _skills = new List<Skill>();
        public Skill curSkill => (_curSkillIdx >= 0) ? _skills[_curSkillIdx] : null;

        public override void BindEntity(Entity e)
        {
            base.BindEntity(e);
            config = GameConfigSingleton.Instance.GetSkillConfig(configId);
            if (config == null) return;
            if (config.skillInfos.Count != _skills.Count)
            {
                _skills.Clear();
                foreach (var info in config.skillInfos)
                {
                    var skill = new Skill();
                    _skills.Add(skill);
                    skill.BindEntity(Entity, info, this);
                    skill.Start();
                }
            }

            for (int i = 0; i < _skills.Count; i++)
            {
                var skill = _skills[i];
                skill.BindEntity(Entity, config.skillInfos[i], this);
            }
        }

        public override void Update(LFloat deltaTime)
        {
            if (config == null) return;
            foreach (var skill in _skills)
            {
                skill.Update(deltaTime);
            }
        }

        public bool Fire(int idx)
        {
            if (config == null || idx < 0 || idx > _skills.Count)
            {
                return false;
            }

            if (isFiring)
            {
                return false;
            }

            var skill = _skills[idx];
            if (skill.Fire())
            {
                _curSkillIdx = idx;
                return true;
            }

            Debug.Log($"TryFire failure {idx} {skill.CdTimer}  {skill.State}");
            return false;
        }

        public void ForceStop(int idx = -1)
        {
            if (config == null) return;
            if (idx == -1)
            {
                idx = _curSkillIdx;
            }

            if (idx < 0 || idx > _skills.Count)
            {
                return;
            }

            if (curSkill != null)
            {
                curSkill.ForceStop();
            }
        }

        public void OnSkillStart(Skill skill)
        {
            Debug.Log("OnSkillStart " + skill.SkillInfo.animName);
            isFiring = true;
            Entity.isInvincible = true;
        }

        public void OnSkillDone(Skill skill)
        {
            Debug.Log("OnSkillDone " + skill.SkillInfo.animName);
            isFiring = false;
            Entity.isInvincible = false;
        }

        public void OnSkillPartStart(Skill skill)
        {
            //Debug.Log("OnSkillPartStart " + skill.SkillInfo.animName );
        }

        public void OnDrawGizmos()
        {
            if (config == null) return;

            foreach (var skill in _skills)
            {
                skill.OnDrawGizmos();
            }
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(_curSkillIdx);
            writer.Write(configId);
            writer.Write(isFiring);
            writer.Write(_skills);
        }

        public override void ReadBackup(Deserializer reader)
        {
            _curSkillIdx = reader.ReadInt32();
            configId = reader.ReadInt32();
            isFiring = reader.ReadBoolean();
            _skills = reader.ReadList(this._skills);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += _curSkillIdx.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += configId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += isFiring.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            if (_skills != null) foreach (var item in _skills) { if (item != default(Skill)) hash += item.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++); }
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "_curSkillIdx" + ":" + _curSkillIdx.ToString());
            sb.AppendLine(prefix + "configId" + ":" + configId.ToString());
            sb.AppendLine(prefix + "isFiring" + ":" + isFiring.ToString());
            BackUpUtil.DumpList("_skills", _skills, sb, prefix);
        }
    }
}