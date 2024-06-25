using System;
using System.Collections.Generic;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class Entity : ILPTriggerEventHandler
    {
        public int EntityId;
        public int PrefabId;
        public CTransform2D transform = new CTransform2D();
        public object engineTransform;
        protected List<IComponent> allComponents;

        public IEntityView EntityView;


        public CRigidbody rigidbody = new CRigidbody();
        public ColliderData colliderData = new ColliderData();
        public CAnimator animator = new CAnimator();
        public CSkillBox skillBox = new CSkillBox();

        public LFloat moveSpd = 5;
        public LFloat turnSpd = 360;
        public int curHealth;
        public int maxHealth = 100;
        public int damage = 10;

        public bool isInvincible;
        public bool isFire;

        public bool isDead => curHealth <= 0;

        public virtual void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            engineTransform = null;
        }

        public virtual void BindRef()
        {
            allComponents?.Clear();
            RegisterComponent(animator);
            RegisterComponent(skillBox);
            rigidbody.BindRef(transform);
        }

        public virtual void Awake()
        {
            if (allComponents == null)
            {
                return;
            }

            foreach (var comp in allComponents)
            {
                comp.Awake();
            }
        }

        public virtual void Start()
        {
            if (allComponents == null)
            {
                return;
            }

            foreach (var comp in allComponents)
            {
                comp.Start();
            }

            rigidbody.Start();
            curHealth = maxHealth;
        }

        public virtual void Update(LFloat deltaTime)
        {
            rigidbody.Update(deltaTime);
            if (allComponents == null)
            {
                return;
            }

            foreach (var comp in allComponents)
            {
                comp.Update(deltaTime);
            }
        }

        public virtual void Destroy()
        {
            if (allComponents == null)
            {
                return;
            }

            foreach (var comp in allComponents)
            {
                comp.Destroy();
            }
        }

        public bool Fire(int idx = 0)
        {
            return skillBox.Fire(idx - 1);
        }

        public void StopSkill(int idx = -1)
        {
            skillBox.ForceStop(idx);
        }

        public virtual void TakeDamage(Entity atker, int amount, LVector3 hitPoint)
        {
            if (isInvincible || isDead)
            {
                return;
            }

            curHealth -= amount;
            EntityView?.OnTakeDamage(amount, hitPoint);
            OnTakeDamage(amount, hitPoint);
            if (isDead)
            {
                OnDead();
            }
        }

        public virtual void OnLPTriggerEnter(ColliderProxy other) { }
        public virtual void OnLPTriggerStay(ColliderProxy other) { }
        public virtual void OnLPTriggerExit(ColliderProxy other) { }

        protected void RegisterComponent(IComponent comp)
        {
            if (allComponents == null)
            {
                allComponents = new List<IComponent>();
            }

            allComponents.Add(comp);
            comp.BindEntity(this);
        }

        protected virtual void OnTakeDamage(int amount, LVector3 hitPoint)
        {
        }

        protected virtual void OnDead()
        {
            EntityView?.OnDead();
            PhysicSystem.Instance.RemoveCollider(this);
            World.Instance.DestroyEntity(this);
        }
    }
}