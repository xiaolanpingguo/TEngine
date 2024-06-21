using Lockstep.Framework;


namespace Lockstep.Game 
{
    public interface IAnimatorView
    {
        void Play(string name, bool isCross);
        void Sample(LFloat time);
    }
}