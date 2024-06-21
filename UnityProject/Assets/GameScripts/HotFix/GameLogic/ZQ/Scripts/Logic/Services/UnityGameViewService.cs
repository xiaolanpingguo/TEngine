using Lockstep.Framework;
using UnityEngine;
using GameBase;


namespace Lockstep.Game
{
    public class UnityGameViewService : Singleton<UnityGameViewService>
    {
        public void BindView(BaseEntity entity, BaseEntity oldEntity = null)
        {
            if (oldEntity != null)
            {
                if (oldEntity.PrefabId == entity.PrefabId)
                {
                    entity.engineTransform = oldEntity.engineTransform;
                    var obj = (oldEntity.engineTransform as Transform).gameObject;
                    var views = obj.GetComponents<IView>();
                    foreach (var view in views)
                    {
                        view.BindEntity(entity, oldEntity);
                    }
                }
                else
                {
                    UnbindView(oldEntity);
                }
            }
            else
            {
                var prefab = World.Instance.LoadPrefab(entity.PrefabId);
                if (prefab == null) return;
                var obj = GameObject.Instantiate(prefab,
                    entity.transform.Pos3.ToVector3(),
                    Quaternion.Euler(new Vector3(0, entity.transform.deg, 0)));

                entity.engineTransform = obj.transform;
                var views = obj.GetComponents<IView>();
                if (views.Length <= 0)
                {
                    var view = obj.AddComponent<BaseEntityView>();
                    view.BindEntity(entity);
                }
                else
                {
                    foreach (var view in views)
                    {
                        view.BindEntity(entity);
                    }
                }
            }
        }

        public void UnbindView(BaseEntity entity)
        {
            entity.OnRollbackDestroy();
        }
    }
}