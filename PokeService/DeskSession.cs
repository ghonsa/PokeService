using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace PokeService
{
    class DeskSession
    {

        // Useful constants

        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = -1;
        public static int WTS_ANY_SESSION = -2;
        public static int WTS_WSD_POWEROFF = 0x00000008;

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int EWX_LOGOFF = 0x00000000;
        internal const int EWX_SHUTDOWN = 0x00000001;
        internal const int EWX_REBOOT = 0x00000002;
        internal const int EWX_FORCE = 0x00000004;
        internal const int EWX_POWEROFF = 0x00000008;
        internal const int EWX_FORCEIFHUNG = 0x00000010;

        // defines for system dll 

        private struct WTS_SESSION_INFO
        {
            public Int32 SessionID;
            [MarshalAs(UnmanagedType.LPStr)]
            public String pWinStationName;

            public WTS_CONNECTSTATE_CLASS State;
        }

        public enum WTS_CONNECTSTATE_CLASS  // session status
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }
        public enum DeskSessionMsgRslt  // taken from Microsofts def of WTSSendMessage
        {
            DSMSG_OK = 1,
            DSMSG_CANCEL,
            DSMSG_ABORT,
            DSMSG_RETRY,
            DSMSG_IGNORE,
            DSMSG_YES,
            DSMSG_NO,
            DSMSG_TIMEOUT = 3200,
            DSMSG_ASYNC
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
        // System DLL entries used..

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSShutdownSystem(IntPtr ServerHandle, long ShutdownFlags);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPStr)] String pServerName);

        [DllImport("wtsapi32.dll")]
        static extern void WTSCloseServer(IntPtr hServer);

        [DllImport("Wtsapi32.dll", EntryPoint = "WTSSendMessage")]
        static extern bool WTSSendMessage(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.I4)] int SessionId,
            String pTitle,
            [MarshalAs(UnmanagedType.U4)] int TitleLength,
            String pMessage,
            [MarshalAs(UnmanagedType.U4)] int MessageLength,
            [MarshalAs(UnmanagedType.U4)] int Style,
            [MarshalAs(UnmanagedType.U4)] int Timeout,
            [MarshalAs(UnmanagedType.U4)] out int pResponse,
            bool bWait);

        [DllImport("Wtsapi32.dll", EntryPoint = "WTSEnumerateSessions")]
        static extern int WTSEnumerateSessions(
                System.IntPtr hServer,
                 [MarshalAs(UnmanagedType.I4)] int Reserved,
                 [MarshalAs(UnmanagedType.I4)] int Version,
                ref System.IntPtr ppSessionInfo,
                ref int pCount);

        [DllImport("wtsapi32.dll")]
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, [MarshalAs(UnmanagedType.I4)] int acc, ref IntPtr
        phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name,
                                                            ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
                                                    ref TokPriv1Luid newst, [MarshalAs(UnmanagedType.I4)] int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool ExitWindowsEx([MarshalAs(UnmanagedType.I4)] int flg, int rea);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InitiateSystemShutdownEx(
            string lpMachineName,
            string lpMessage,
            uint dwTimeout,
            bool bForceAppsClosed,
            bool bRebootAfterShutdown,
            ShutdownReason dwReason);

        [Flags]
        public enum ShutdownReason : uint
        {
            // Microsoft major reasons.
            SHTDN_REASON_MAJOR_OTHER = 0x00000000,
            SHTDN_REASON_MAJOR_NONE = 0x00000000,
            SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,
            SHTDN_REASON_MAJOR_OPERATINGSYSTEM = 0x00020000,
            SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,
            SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,
            SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,
            SHTDN_REASON_MAJOR_POWER = 0x00060000,
            SHTDN_REASON_MAJOR_LEGACY_API = 0x00070000,
            // Microsoft minor reasons.
            SHTDN_REASON_MINOR_OTHER = 0x00000000,
            SHTDN_REASON_MINOR_NONE = 0x000000ff,
            SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,
            SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,
            SHTDN_REASON_MINOR_UPGRADE = 0x00000003,
            SHTDN_REASON_MINOR_RECONFIG = 0x00000004,
            SHTDN_REASON_MINOR_HUNG = 0x00000005,
            SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,
            SHTDN_REASON_MINOR_DISK = 0x00000007,
            SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,
            SHTDN_REASON_MINOR_NETWORKCARD = 0x00000000,
            SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000a,
            SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000b,
            SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000c,
            SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000d,
            SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000e,
            SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,
            SHTDN_REASON_MINOR_SERVICEPACK = 0x00000010,
            SHTDN_REASON_MINOR_HOTFIX = 0x00000011,
            SHTDN_REASON_MINOR_SECURITYFIX = 0x00000012,
            SHTDN_REASON_MINOR_SECURITY = 0x00000013,
            SHTDN_REASON_MINOR_NETWORK_CONNECTIVITY = 0x00000014,
            SHTDN_REASON_MINOR_WMI = 0x00000015,
            SHTDN_REASON_MINOR_SERVICEPACK_UNINSTALL = 0x00000016,
            SHTDN_REASON_MINOR_HOTFIX_UNINSTALL = 0x00000017,
            SHTDN_REASON_MINOR_SECURITYFIX_UNINSTALL = 0x00000018,
            SHTDN_REASON_MINOR_MMC = 0x00000019,
            SHTDN_REASON_MINOR_TERMSRV = 0x00000020,
            // Flags that end up in the event log code.
            SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,
            SHTDN_REASON_FLAG_PLANNED = 0x80000000,
            SHTDN_REASON_UNKNOWN = SHTDN_REASON_MINOR_NONE,
            SHTDN_REASON_LEGACY_API = (SHTDN_REASON_MAJOR_LEGACY_API | SHTDN_REASON_FLAG_PLANNED),
            // This mask cuts out UI flags.
            SHTDN_REASON_VALID_BIT_MASK = 0xc0ffffff
        }

        public static IntPtr OpenServer(String Name)
        {
            IntPtr server = WTSOpenServer(Name);
            return server;
        }
        public static void CloseServer(IntPtr ServerHandle)
        {
            WTSCloseServer(ServerHandle);
        }

        internal static void ShutDownSystem()
        {
            InitiateSystemShutdownEx("localhost", "Power is critically low, shutting down!", 20, true,
                                    false, ShutdownReason.SHTDN_REASON_MINOR_POWER_SUPPLY);
        }

        private static int getActiveSession()
        {
            int activeSession = 1;
            IntPtr server = IntPtr.Zero;
            try
            {
                server = OpenServer("localhost");
                IntPtr ppSessionInfo = IntPtr.Zero;
                Int32 count = 0;
                Int32 retval = WTSEnumerateSessions(server, 0, 1, ref ppSessionInfo, ref count);
                Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                Int64 current = (int)ppSessionInfo;

                if (retval != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                        current += dataSize;
                        if (si.State == WTS_CONNECTSTATE_CLASS.WTSActive)
                        {
                            activeSession = si.SessionID;
                            break;
                        }
                    }
                    WTSFreeMemory(ppSessionInfo);
                }
            }
            finally
            {
                CloseServer(server);
            }
            return activeSession;
        }
        // Display a message on the active sessionn desktop
        internal static int DisplayMessage(string msg, string title, int style)
        {
            int resp = 0;
            try
            {
                bool result = false;
                int tlen = title.Length;
                int mlen = msg.Length;

                result = WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, getActiveSession(), title,
                                                 tlen, msg, mlen, style, 5, out resp, true);
                int err = Marshal.GetLastWin32Error();
            }
            catch (Exception pe)
            {
                Trace.WriteLine(pe.Message);
            }
            return resp;
        }
    }

  
}
