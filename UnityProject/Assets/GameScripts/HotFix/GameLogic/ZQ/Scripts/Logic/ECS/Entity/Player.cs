using System;
using System.Text;
using Lockstep.Framework;
using UnityEngine;
using UnityEngine.Assertions.Must;


namespace Lockstep.Game
{
    [Serializable]
    public class Player : Entity
    {
        private CCharacterController _characterController = null;
        private CSkill _skill= null;
        private CHealth _health = null;

        public PlayerView EntityView = null;

        public override void Start()
        {
            PlayerConfig playerConfig = GameConfigSingleton.Instance.PlayerConfig;

            _health = new CHealth(this);
            _health.MaxHealth = playerConfig.MaxHealth;
            _health.OnDamage += OnTakeDamage;

            _characterController = new CCharacterController(this);
            _characterController.MoveSpd = playerConfig.MoveSpd;
            _characterController.TurnSpd = playerConfig.TurnSpd;

            _skill = new CSkill(this);
            _skill.OnSkillStartHandler = OnSkillStart;
            _skill.OnSkillPartStartHandler = OnSkillPartStart;
            _skill.OnSkillDoneHandler = OnSkillDone;
            AddComponent(_health);
            AddComponent(_characterController);
            AddComponent(_skill);
            base.Start();
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
        }

        public void StopSkill()
        {
            _skill.ForceStop();
        }

        public void OnSkillStart(CSkill skill)
        {
        }

        public void OnSkillPartStart(CSkill skill)
        {
        }

        public void OnSkillDone(CSkill skill)
        {
        }

        public override void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            UserData = null;
        }

        public void OnTakeDamage(Entity attacker, int amount, LVector3 hitPoint)
        {
            EntityView?.OnTakeDamage(amount, hitPoint);
            if (_health.IsDead)
            {
                EntityView?.OnDead();
                World.Instance.GetSystem<PhysicSystem>().RemoveCollider(this);
                World.Instance.DestroyEntity(this);
            }
        }
    }
}