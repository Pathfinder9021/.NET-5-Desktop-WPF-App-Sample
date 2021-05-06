using System.Diagnostics;
using System.Threading;

namespace WpfSample.App.Helpers
{
    internal static class MutexLockHelper
    {
        private static Mutex mutex = null;

        public static bool CreateMutex()
        {
            if (mutex != null)
            {
                return true;
            }

            mutex = new Mutex(true, GetCurrentProcessName(), out bool createdNew);

            if (createdNew)
            {
                return true;
            }
            else
            {
                mutex = null;
                return false;
            }
        }

        public static void ReleaseMutex()
        {
            if (mutex == null)
            {
                return;
            }

            mutex.ReleaseMutex();
        }

        private static string GetCurrentProcessName()
        {
            using (var currentProcess = Process.GetCurrentProcess())
            {
                return currentProcess.ProcessName;
            }
        }
    }
}
