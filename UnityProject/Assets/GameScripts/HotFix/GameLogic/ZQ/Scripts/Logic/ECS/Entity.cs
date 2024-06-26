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

        public EntityView EntityView;

        public CRigidbody rigidbody = new CRigidbody();
        public ColliderData colliderData = new ColliderData();

        public Entity()
        {
            rigidbody.BindRef(LTrans2D);
        }

        public virtual void Start()
        {
            foreach (var comp in allComponents)
            {
                comp.Start();
            }

            rigidbody.Start();
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

        public virtual void OnLPTriggerEnter(ColliderProxy other)
        {
        }

        public virtual void OnLPTriggerStay(ColliderProxy other)
        { 
        }

        public virtual void OnLPTriggerExit(ColliderProxy other) 
        {
        }

        protected void RegisterComponent(IComponent comp)
        {
            allComponents.Add(comp);
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

        public virtual void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            engineTransform = null;
        }
    }
}