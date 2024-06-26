using System.Collections;
using Lockstep.Framework;
using UnityEngine;

namespace Lockstep.Game
{
    public class EnemyView : EntityView
    {
        private UIFloatBar _uiFloatBar;
        private Enemy _enemy;

        public override void BindEntity(Entity e, Entity oldEntity = null)
        {
            base.BindEntity(e, oldEntity);
            _enemy = e as Enemy;
            if (oldEntity != null)
            {
                _uiFloatBar = (oldEntity.EntityView as EnemyView)._uiFloatBar;
            }
            else
            {
                _uiFloatBar = FloatBarManager.CreateFloatBar(transform, _enemy.CurHealth, _enemy.MaxHealth);
            }
        }

        public override void OnTakeDamage(int amount, LVector3 hitPoint)
        {
            _uiFloatBar.UpdateHp(_enemy.CurHealth, _enemy.MaxHealth);
            FloatTextManager.CreateFloatText(hitPoint.ToVector3(), -amount);
        }

        public override void OnDead()
        {
            if (_uiFloatBar != null)
            {
                FloatBarManager.DestroyText(_uiFloatBar);
            }

            GameObject.Destroy(gameObject);
        }

        public override void OnRollbackDestroy()
        {
            if (_uiFloatBar != null)
            {
                FloatBarManager.DestroyText(_uiFloatBar);
            }

            GameObject.Destroy(gameObject);
        }

        private void Update()
        {
            var pos = Entity.LTrans2D.Pos3.ToVector3();
            transform.position = Vector3.Lerp(transform.position, pos, 0.3f);
            var deg = Entity.LTrans2D.deg.ToFloat();
            //deg = Mathf.Lerp(transform.rotation.eulerAngles.y, deg, 0.3f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, deg, 0), 0.3f);
        }

        private void OnDrawGizmos()
        {
            //if (entity.skillBox.isFiring)
            //{
            //    var skill = entity.skillBox.curSkill;
            //    skill?.OnDrawGizmos();
            //}
        }
    }
}