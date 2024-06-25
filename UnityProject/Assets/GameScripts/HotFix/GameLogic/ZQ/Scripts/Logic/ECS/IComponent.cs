using System;
using System.Text;
using Lockstep.Framework;
using static UnityEngine.GraphicsBuffer;


namespace Lockstep.Game
{
    public abstract class IComponent : BaseFormater
    {
        public Entity Entity { get; private set; }
        public CTransform2D transform { get; private set; }

        public virtual void BindEntity(Entity entity)
        {
            this.Entity = entity;
            this.transform = entity.transform;
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update(LFloat deltaTime) { }
        public virtual void Destroy() { }

        public abstract void WriteBackup(Serializer writer);
        public abstract void ReadBackup(Deserializer reader);
        public abstract int GetHash(ref int idx);
        public abstract void DumpStr(StringBuilder sb, string prefix);
    }
}