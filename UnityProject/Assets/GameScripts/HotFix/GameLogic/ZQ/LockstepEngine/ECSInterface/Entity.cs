using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using Lockstep.Framework;
using Lockstep.Game;


namespace Lockstep.Framework
{
    public class Entity : ILPTriggerEventHandler
    {
        private List<IComponent> _components = new List<IComponent>();
        private Dictionary<Type, IComponent> _componentsMap = new();

        public int EntityId
        {
            get { return EntityBase.EntityId; }
            set{ EntityBase.EntityId = value; }
        }

        public object UserData;

        public CEntityBase EntityBase = null;
        public CTransform2D LTrans2D = null;
        public CRigidbody Rigidbody = null;
        public ColliderData ColliderData = null;

        public Entity()
        {
            EntityBase = new CEntityBase(this);
            LTrans2D = new CTransform2D();
            Rigidbody = new CRigidbody();
            Rigidbody.BindRef(LTrans2D);
            ColliderData = new ColliderData();
            AddComponent(EntityBase);
            AddComponent(LTrans2D);
            AddComponent(Rigidbody);
            AddComponent(ColliderData);
        }

        public virtual void Start()
        {
            foreach (var comp in _components)
            {
                comp.Start();
            }
        }

        public virtual void Update(LFloat deltaTime)
        {
            foreach (var comp in _components)
            {
                comp.Update(deltaTime);
            }
        }

        public virtual void Destroy()
        {
            foreach (var comp in _components)
            {
                comp.Destroy();
            }
        }

        public void AddComponent(IComponent comp)
        {
            Type type = comp.GetType();
            if (_componentsMap.ContainsKey(type))
            {
                return;
            }

            _componentsMap.Add(type, comp);
            _components.Add(comp);
        }

        public T GetComponent<T>() where T : IComponent
        {
            Type type = typeof(T);
            if (_componentsMap.TryGetValue(type, out var v))
            {
                return v as T;
            }

            return null;
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

        public virtual void Serialize(Serializer writer)
        {
            for (int i = 0; i <  _components.Count; i++) 
            {
                _components[i].Serialize(writer);
            }
        }

        public virtual void Deserialize(Deserializer reader)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].Deserialize(reader);
            }
        }

        public virtual int GetHash(ref int idx)
        {
            int hash = 0;
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].GetHash(ref hash);
            }

            return hash;
        }

        public virtual void DumpStr(StringBuilder sb, string prefix)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].DumpStr(sb, prefix);
            }
        }

        public virtual void OnRollbackDestroy()
        {
        }
    }
}