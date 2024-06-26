using System;
using System.Text;
using Lockstep.Framework;
using static UnityEngine.GraphicsBuffer;


namespace Lockstep.Framework
{
    public abstract class IComponent : ISerializable
    {
        public Entity Entity { get; private set; }

        public IComponent()
        {
        }

        public IComponent(Entity entity)
        {
            this.Entity = entity;
        }

        public virtual void Start() 
        { 
        }

        public virtual void Update(LFloat deltaTime) 
        { 
        }

        public virtual void Destroy()
        {
        }

        public abstract void Serialize(Serializer writer);
        public abstract void Deserialize(Deserializer reader);
        public abstract int GetHash(ref int idx);
        public abstract void DumpStr(StringBuilder sb, string prefix);
    }
}