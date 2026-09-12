using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace ProCheck
{
    internal static class TcpConnectionOwner
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;
            public uint localPort1;
            public uint localPort2;
            public uint remoteAddr;
            public uint remotePort1;
            public uint remotePort2;
            public uint owningPid;
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen,
            bool sort, int ipVersion, int tblClass, uint reserved);

        private const int AF_INET = 2;
        private const int TCP_TABLE_OWNER_PID_ALL = 5;

        // <summary>
        // Returns the PID that owns the given local port, or -1 if not found.
        // </summary>
        public static int GetOwningPidForLocalPort(int localPort)
        {
            int bufSize = 0;
            GetExtendedTcpTable(IntPtr.Zero, ref bufSize, true, AF_INET, TCP_TABLE_OWNER_PID_ALL, 0);

            IntPtr tablePtr = Marshal.AllocHGlobal(bufSize);
            try
            {
                uint result = GetExtendedTcpTable(tablePtr, ref bufSize, true, AF_INET, TCP_TABLE_OWNER_PID_ALL, 0);
                if (result != 0) return -1;

                int rowCount = Marshal.ReadInt32(tablePtr);
                IntPtr rowPtr = IntPtr.Add(tablePtr, 4);
                int rowSize = Marshal.SizeOf<MIB_TCPROW_OWNER_PID>();

                for (int i = 0; i < rowCount; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);
                    int port = ((int)row.localPort1 << 8 | (int)row.localPort2) & 0xFFFF;
                    // ports are stored big-endian in this struct; correct extraction:
                    port = ((int)(row.localPort1 & 0xFF) << 8) | (int)(row.localPort2 & 0xFF);

                    if (port == localPort) return (int)row.owningPid;

                    rowPtr = IntPtr.Add(rowPtr, rowSize);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(tablePtr);
            }

            return -1;
        }
    }
}