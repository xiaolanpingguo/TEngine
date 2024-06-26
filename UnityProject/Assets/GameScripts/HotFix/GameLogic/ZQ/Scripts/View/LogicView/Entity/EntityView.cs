using Lockstep.Framework;
using UnityEngine;


namespace Lockstep.Game
{
    public class EntityView : MonoBehaviour
    {
        public const float LerpPercent = 0.3f;
        public Entity Entity;

        public virtual void BindEntity(Entity e, Entity oldEntity = null)
        {
            this.Entity = e;
            var updateEntity = oldEntity ?? e;
            transform.position = updateEntity.LTrans2D.Pos3.ToVector3();
            transform.rotation = Quaternion.Euler(0, updateEntity.LTrans2D.deg.ToFloat(), 0);
        }

        public virtual void OnTakeDamage(int amount, LVector3 hitPoint)
        {
            FloatTextManager.CreateFloatText(hitPoint.ToVector3(), -amount);
        }

        public virtual void OnDead()
        {
            GameObject.Destroy(gameObject);
        }

        public virtual void OnRollbackDestroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}