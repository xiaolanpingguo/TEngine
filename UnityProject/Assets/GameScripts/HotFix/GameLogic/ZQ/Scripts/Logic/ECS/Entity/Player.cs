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
        private CSkill _skill= null;
        private CHealth _health = null;

        public PlayerView EntityView = null;

        private int _damage = 10;

        public override void Start()
        {
            _health = new CHealth(this);
            _health.OnDamage += OnTakeDamage;

            _characterController = new CCharacterController(this);
            var config = GameConfigSingleton.Instance.GetSkillConfig();
            _skill = new CSkill(this, config);
            _skill.OnSkillStartHandler = OnSkillStart;
            _skill.OnSkillPartStartHandler = OnSkillPartStart;
            _skill.OnSkillDoneHandler = OnSkillDone;
            RegisterComponent(_health);
            RegisterComponent(_characterController);
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
            if (_health.IsDead)
            {
                return;
            }

            EntityView?.OnTakeDamage(amount, hitPoint);
            if (_health.IsDead)
            {
                EntityView?.OnDead();
                PhysicSystem.Instance.RemoveCollider(this);
                World.Instance.DestroyEntity(this);
            }
        }
    }
}