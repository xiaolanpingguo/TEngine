namespace Lockstep.Framework
{
    public delegate void FuncCollision(ColliderProxy obj);

    public interface ICollisionSystem
    {
        void Start(bool[] interestingMasks, int[] allTypes);
        void Update(LFloat deltaTime);
        ColliderProxy GetCollider(int id);
        void AddCollider(ColliderProxy collider);
        void RemoveCollider(ColliderProxy collider);
        bool Raycast(int layerType, Ray2D checkRay, out LFloat t, out int id, LFloat maxDistance);
        bool Raycast(int layerType, Ray2D checkRay, out LFloat t, out int id);
        void QueryRegion(int layerType, LVector2 pos, LVector2 size, LVector2 forward, FuncCollision callback);
        void QueryRegion(int layerType, LVector2 pos, LFloat radius, FuncCollision callback);

        //for debug
        void DrawGizmos();
        int ShowTreeId { get; set; }
    }
}