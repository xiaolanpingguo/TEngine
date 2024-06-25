namespace Lockstep.Game 
{
    public interface IView 
    {
        void BindEntity(Entity e,Entity oldEntity = null);
    }
}