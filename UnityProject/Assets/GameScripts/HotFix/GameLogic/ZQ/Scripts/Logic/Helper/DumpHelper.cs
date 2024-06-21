using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace Lockstep.Game
{
    public class DumpHelper
    {
        private World _world;

        public Dictionary<int, StringBuilder> _tick2RawFrameData = new Dictionary<int, StringBuilder>();
        public Dictionary<int, StringBuilder> _tick2OverrideFrameData = new Dictionary<int, StringBuilder>();

        public DumpHelper()
        {
        }


#if UNITY_EDITOR
        private string dumpPath => Path.Combine(UnityEngine.Application.dataPath, "Dump.dump");
#endif
#if UNITY_STANDALONE_WIN
        private string dumpAllPath => "c:/temp/Tutorial/LockstepTutorial/DumpLog";
#else
        private string dumpAllPath => "/tmp/Tutorial/LockstepTutorial/DumpLog";
#endif  
        private HashHelper _hashHelper;
        private StringBuilder _curSb;
        public bool enable = false;

        public void DumpFrame(bool isNewFrame)
        {
            if (!enable) return;

            _curSb = DumpFrame();

            int tick = _world.Tick;
            if (isNewFrame)
            {
                _tick2RawFrameData[tick] = _curSb;
                _tick2OverrideFrameData[tick] = _curSb;
            }
            else
            {
                _tick2OverrideFrameData[tick] = _curSb;
            }
        }



        public void DumpToFile(bool withCurFrame = false)
        {
            if (!enable) return;

#if UNITY_EDITOR
            var path = dumpPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            //var minTick = _tick2OverrideFrameData.Keys.Min();
            StringBuilder sbResume = new StringBuilder();
            StringBuilder sbRaw = new StringBuilder();

            int tick = _world.Tick;
            for (int i = 0; i <= tick; i++)
            {
                sbRaw.AppendLine(_tick2RawFrameData[i].ToString());
                sbResume.AppendLine(_tick2OverrideFrameData[i].ToString());
            }

            File.WriteAllText(dumpPath + "/resume.txt", sbResume.ToString());
            File.WriteAllText(dumpPath + "/raw.txt", sbRaw.ToString());
            if (withCurFrame)
            {
                _curSb = DumpFrame();
                var curHash = _hashHelper.CalcHash(true);
                File.WriteAllText(dumpPath + "/cur_single.txt", _curSb.ToString());
                File.WriteAllText(dumpPath + "/raw_single.txt", _tick2RawFrameData[tick].ToString());
            }

            UnityEngine.Debug.Break();
#endif 
        }


        public void OnFrameEnd()
        {
            _curSb = null;
        }

        public void Trace(string msg, bool isNewLine = false, bool isNeedLogTrace = false)
        {
            if (_curSb == null) return;
            if (isNewLine)
            {
                _curSb.AppendLine(msg);
            }
            else
            {
                _curSb.Append(msg);
            }

            if (isNeedLogTrace)
            {
                StackTrace st = new StackTrace(true);
                StackFrame[] sf = st.GetFrames();
                for (int i = 2; i < sf.Length; ++i)
                {
                    var frame = sf[i];
                    _curSb.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" + frame.GetMethod().Name);
                }
            }

        }
        public void DumpAll()
        {
            if (!enable) return;
            var path = dumpAllPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            StringBuilder sbRaw = new StringBuilder();

            int tick = _world.Tick;
            for (int i = 0; i <= tick; i++)
            {
                if (_tick2RawFrameData.TryGetValue(i, out var data))
                {
                    sbRaw.AppendLine(data.ToString());
                }
            }

            File.WriteAllText(dumpAllPath + $"/All_0.txt", sbRaw.ToString());
        }

        private StringBuilder DumpFrame()
        {
            int tick = _world.Tick;
            var sb = new StringBuilder();
            sb.AppendLine("Tick : " + tick + "--------------------");
            _DumpStr(sb, "");
            return sb;
        }

        private void _DumpStr(System.Text.StringBuilder sb, string prefix)
        {
            //foreach (var svc in _serviceContainer.GetAllServices()) 
            //{
            //    if (svc is IDumpStr hashSvc)
            //    {
            //        sb.AppendLine(svc.GetType() + " --------------------");
            //        hashSvc.DumpStr(sb, "\t" + prefix);
            //    }
            //}
        }
    }
}