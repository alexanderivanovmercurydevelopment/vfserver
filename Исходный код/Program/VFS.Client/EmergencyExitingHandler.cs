namespace VFS.Client
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    internal static class EmergencyExitingHandler
    {
        [DllImport("Kernel32")]
        internal static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        internal delegate bool EventHandler(CtrlType sig);

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        internal enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}