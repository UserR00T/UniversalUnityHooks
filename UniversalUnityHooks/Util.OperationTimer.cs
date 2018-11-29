using System.Diagnostics;

namespace UniversalUnityHooks
{
    public static partial class Util
    {
        public class OperationTimer
        {
            public Stopwatch stopwatch = new Stopwatch();
            public OperationTimer(bool start = true)
            {
                if (start)
                    Start();
            }
            public void Start()
            {
                stopwatch = Stopwatch.StartNew();
            }
            public void Stop()
            {
                stopwatch.Stop();
            }
            public long GetElapsedMs => stopwatch.ElapsedMilliseconds;
        }
    }
}
