using System;
using System.Collections.Generic;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class Entity : ILPTriggerEventHandler
    {
        public int EntityId;
        public int PrefabId;
        public CTransform2D LTrans2D = new CTransform2D();
        public object engineTransform;
        protected List<IComponent> allComponents = new List<IComponent>();

        public IEntityView EntityView;


        public CRigidbody rigidbody = new CRigidbody();
        public ColliderData colliderData = new ColliderData();

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

        public Entity()
        {
            rigidbody.BindRef(LTrans2D);
        }

        public virtual void Awake()
        {
            foreach (var comp in allComponents)
            {
                comp.Awake();
            }
        }

        public virtual void Start()
        {
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
            foreach (var comp in allComponents)
            {
                comp.Update(deltaTime);
            }
        }

        public virtual void Destroy()
        {
            foreach (var comp in allComponents)
            {
                comp.Destroy();
            }
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
            allComponents.Add(comp);
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

        public virtual void WriteBackup(Serializer writer)
        {

        }

        public virtual void ReadBackup(Deserializer reader)
        {

        }

        public virtual int GetHash(ref int idx)
        {
            return 0;
        }

        public virtual void DumpStr(StringBuilder sb, string prefix)
        {

        }
    }
}