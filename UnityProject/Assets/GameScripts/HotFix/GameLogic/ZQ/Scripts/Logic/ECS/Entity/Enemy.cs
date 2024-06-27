using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Enemy : Entity
    {
        private CAIController _aiontroller = null;
        private CHealth _health = null;

        public EnemyView EntityView = null;


        public override void Start()
        {
            _aiontroller = new CAIController(this);
            _health = new CHealth(this);
            _health.OnDamage += OnTakeDamage;

            RegisterComponent(_aiontroller);
            RegisterComponent(_health);
            base.Start();
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

        public override void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            UserData = null;
        }
    }
}