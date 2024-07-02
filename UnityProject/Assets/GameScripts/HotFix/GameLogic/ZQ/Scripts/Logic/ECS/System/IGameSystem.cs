using Lockstep.Framework;
using System.Text;


namespace Lockstep.Game
{
    public class IGameSystem
    {
        public World World;
        public bool Enable = true;

        public IGameSystem(World world) {  World = world; }
        public virtual void Init() { }
        public virtual void Update(LFloat deltaTime) { }
        public virtual void Destroy() { }

        protected virtual FuncUndoCommands GetRollbackFunc() { return null; }
        public virtual int GetHash(ref int idx) { return 0; }
        public virtual void DumpStr(StringBuilder sb, string prefix) { }
        public virtual void Backup(int tick) { }
        public virtual void RollbackTo(int tick) { }
        public virtual void Clean(int maxVerifiedTick) { }
    }
}