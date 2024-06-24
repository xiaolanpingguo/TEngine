using Lockstep.Framework;

namespace Lockstep.Game
{
    public class UnityLogHandler
    {
        public static void LockstepLogHandler(LogType type, string log)
        {
            switch (type)
            {
                case LogType.Info:
                    {
                        UnityEngine.Debug.Log(log);
                        break;
                    }
                case LogType.Warn:
                    {
                        UnityEngine.Debug.LogWarning(log);
                        break;
                    }
                case LogType.Error:
                    {
                        UnityEngine.Debug.LogError(log);
                        break;
                    }
            }
        }
    }
}