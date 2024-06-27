using Lockstep.Framework;
using UnityEngine;
using UnityEngine.UI;


namespace Lockstep.Game
{
    public class PlayerView : EntityView
    {
        private Player _player;
        private UIFloatBar _uiFloatBar;

        public override void BindEntity(Entity e, Entity oldEntity = null)
        {
            base.BindEntity(e, oldEntity);
            _player = e as Player;
            _player.EntityView = this;
            if (oldEntity != null)
            {
                Player oldPlayer = oldEntity as Player;
                _uiFloatBar = oldPlayer.EntityView._uiFloatBar;
            }
            else
            {
                CHealth health = _player.GetComponent<CHealth>();
                _uiFloatBar = FloatBarManager.CreateFloatBar(transform, health.CurHealth, health.MaxHealth);
            }
        }

        public override void OnTakeDamage(int amount, LVector3 hitPoint)
        {
            CHealth health = _player.GetComponent<CHealth>();
            _uiFloatBar.UpdateHp(health.CurHealth, health.MaxHealth);
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