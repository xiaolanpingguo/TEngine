using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class IComponent : BaseFormater
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
    }
}