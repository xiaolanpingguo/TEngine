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

        private List<IComponent> _components = new List<IComponent>();
        private Dictionary<Type, IComponent> _componentsMap = new();

        public CRigidbody rigidbody = new CRigidbody();
        public ColliderData colliderData = new ColliderData();

        public Entity()
        {
            rigidbody.BindRef(LTrans2D);
        }

        public virtual void Start()
        {
            foreach (var comp in _components)
            {
                comp.Start();
            }

            rigidbody.Start();
        }

        public virtual void Update(LFloat deltaTime)
        {
            rigidbody.Update(deltaTime);
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

        protected void RegisterComponent(IComponent comp)
        {
            Type type = comp.GetType();
            if (_componentsMap.ContainsKey(type))
            {
                return;
            }

            _componentsMap.Add(type, comp);
            _components.Add(comp);
        }

        protected T GetComponent<T>() where T : IComponent
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

        public virtual void WriteBackup(Serializer writer)
        {
            for (int i = 0; i <  _components.Count; i++) 
            {
                _components[i].WriteBackup(writer);
            }
        }

        public virtual void ReadBackup(Deserializer reader)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].ReadBackup(reader);
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