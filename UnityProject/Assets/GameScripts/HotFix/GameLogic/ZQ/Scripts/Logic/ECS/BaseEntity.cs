using System;
using System.Collections.Generic;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    [NoBackup]
    public partial class BaseEntity : IEntity, ILPTriggerEventHandler
    {
        public int EntityId;
        public int PrefabId;
        public CTransform2D transform = new CTransform2D();
        [NoBackup] public object engineTransform;
        protected List<BaseComponent> allComponents;

        [ReRefBackup] public IEntityView EntityView;


        public void DoBindRef()
        {
            BindRef();
        }

        public virtual void OnRollbackDestroy()
        {
            EntityView?.OnRollbackDestroy();
            EntityView = null;
            engineTransform = null;
        }

        protected virtual void BindRef()
        {
            allComponents?.Clear();
        }

        protected void RegisterComponent(BaseComponent comp)
        {
            if (allComponents == null)
            {
                allComponents = new List<BaseComponent>();
            }

            allComponents.Add(comp);
            comp.BindEntity(this);
        }

        public virtual void DoAwake()
        {
            if (allComponents == null) return;
            foreach (var comp in allComponents)
            {
                comp.DoAwake();
            }
        }

        public virtual void DoStart()
        {
            if (allComponents == null) return;
            foreach (var comp in allComponents)
            {
                comp.DoStart();
            }
        }

        public virtual void DoUpdate(LFloat deltaTime)
        {
            if (allComponents == null) return;
            foreach (var comp in allComponents)
            {
                comp.DoUpdate(deltaTime);
            }
        }

        public virtual void DoDestroy()
        {
            if (allComponents == null) return;
            foreach (var comp in allComponents)
            {
                comp.DoDestroy();
            }
        }

        public virtual void OnLPTriggerEnter(ColliderProxy other) { }
        public virtual void OnLPTriggerStay(ColliderProxy other) { }
        public virtual void OnLPTriggerExit(ColliderProxy other) { }
    }
}