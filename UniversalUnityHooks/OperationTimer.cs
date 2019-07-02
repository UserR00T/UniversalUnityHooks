using System.Diagnostics;

namespace UniversalUnityHooks
{
    public class OperationTimer
    {
        public Stopwatch stopwatch = new Stopwatch();

        public long GetElapsedMs => stopwatch.ElapsedMilliseconds;

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
    }
}
