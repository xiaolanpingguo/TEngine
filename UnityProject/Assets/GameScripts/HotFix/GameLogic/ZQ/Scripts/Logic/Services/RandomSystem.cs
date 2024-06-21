using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class RandomSystem : IGameSystem
    {
        public class RandomCmd : ICommand
        {
            public ulong RandSeed;

            public override void Do(object param)
            {
                RandSeed = ((RandomSystem)param).Seed;
            }

            public override void Undo(object param)
            {
                ((RandomSystem)param).Seed = RandSeed;
            }
        }

        private LRandom _random = new LRandom();

        public LFloat Value => _random.value;
        public ulong Seed
        {
            set { }
            get { return _random.randSeed; }
        }

        public uint Next()
        {
            return _random.Next();
        }

        public uint Next(uint max)
        {
            return _random.Next(max);
        }

        public int Next(int max)
        {
            return _random.Next(max);
        }

        public uint Range(uint min, uint max)
        {
            return _random.Range(min, max);
        }

        public int Range(int min, int max)
        {
            return _random.Range(min, max);
        }

        public LFloat Range(LFloat min, LFloat max)
        {
            return _random.Range(min, max);
        }

        public override int GetHash(ref int idx)
        {
            return (int)_random.randSeed * PrimerLUT.GetPrimer(idx++);
        }

        public override void DumpStr(System.Text.StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "randSeed" + ":" + Seed.ToString());
        }

        protected override FuncUndoCommands GetRollbackFunc()
        {
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }

        public override void Backup(int tick)
        {

        }
    }
}