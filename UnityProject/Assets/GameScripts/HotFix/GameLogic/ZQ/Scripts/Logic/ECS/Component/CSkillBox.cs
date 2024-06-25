using System;
using System.Collections.Generic;
using UnityEngine;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class CSkillBox : IComponent, ISkillEventHandler
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
                    skill.DoStart();
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
                skill.DoUpdate(deltaTime);
            }
        }

        public bool Fire(int idx)
        {
            if (config == null) return false;
            if (idx < 0 || idx > _skills.Count)
            {
                return false;
            }

            //Debug.Log("TryFire " + idx);

            if (isFiring) return false; //
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
    }
}