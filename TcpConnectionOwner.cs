using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace ProCheck
{
    internal static class TcpConnectionOwner
    {
        private const int AF_INET = 2;
        private const int AF_INET6 = 23;

        private const int TCP_TABLE_OWNER_PID_ALL = 5;

        private const uint NO_ERROR = 0;
        private const uint ERROR_INSUFFICIENT_BUFFER = 122;

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int dwOutBufLen,
            bool bOrder,
            int ulAf,
            int tableClass,
            uint reserved);

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCPROW_OWNER_PID
        {
            public uint State;
            public uint LocalAddr;
            public uint LocalPort;
            public uint RemoteAddr;
            public uint RemotePort;
            public uint OwningPid;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIB_TCP6ROW_OWNER_PID
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] LocalAddr;

            public uint LocalScopeId;
            public uint LocalPort;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] RemoteAddr;

            public uint RemoteScopeId;
            public uint RemotePort;

            public uint State;
            public uint OwningPid;
        }

        /// <summary>
        /// Returns every PID that currently owns a TCP connection
        /// using the specified local port.
        /// </summary>
        public static IReadOnlyList<int> GetOwningPidsForLocalPort(int localPort)
        {
            if (localPort <= 0 || localPort > 65535)
                return Array.Empty<int>();

            var result = new HashSet<int>();

            try
            {
                CollectIPv4(localPort, result);
                CollectIPv6(localPort, result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[TcpConnectionOwner] TCP table lookup failed: {ex.Message}");
            }

            return new List<int>(result);
        }

        /// <summary>
        /// Compatibility helper. Returns the first owner, or -1.
        /// </summary>
        public static int GetOwningPidForLocalPort(int localPort)
        {
            var pids = GetOwningPidsForLocalPort(localPort);

            return pids.Count > 0 ? pids[0] : -1;
        }

        private static void CollectIPv4(
            int localPort,
            HashSet<int> result)
        {
            int bufferSize = 0;

            uint status = GetExtendedTcpTable(
                IntPtr.Zero,
                ref bufferSize,
                true,
                AF_INET,
                TCP_TABLE_OWNER_PID_ALL,
                0);

            if (status != ERROR_INSUFFICIENT_BUFFER || bufferSize <= 0)
                return;

            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);

            try
            {
                status = GetExtendedTcpTable(
                    buffer,
                    ref bufferSize,
                    true,
                    AF_INET,
                    TCP_TABLE_OWNER_PID_ALL,
                    0);

                if (status != NO_ERROR)
                    return;

                int count = Marshal.ReadInt32(buffer);
                IntPtr rowPtr = IntPtr.Add(
                    buffer,
                    sizeof(int));

                int rowSize = Marshal.SizeOf<MIB_TCPROW_OWNER_PID>();

                for (int i = 0; i < count; i++)
                {
                    var row =
                        Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(
                            rowPtr);

                    int port = NetworkToHostPort(row.LocalPort);

                    if (port == localPort && row.OwningPid > 0)
                    {
                        result.Add((int)row.OwningPid);
                    }

                    rowPtr = IntPtr.Add(rowPtr, rowSize);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static void CollectIPv6(
            int localPort,
            HashSet<int> result)
        {
            int bufferSize = 0;

            uint status = GetExtendedTcpTable(
                IntPtr.Zero,
                ref bufferSize,
                true,
                AF_INET6,
                TCP_TABLE_OWNER_PID_ALL,
                0);

            if (status != ERROR_INSUFFICIENT_BUFFER || bufferSize <= 0)
                return;

            IntPtr buffer = Marshal.AllocHGlobal(bufferSize);

            try
            {
                status = GetExtendedTcpTable(
                    buffer,
                    ref bufferSize,
                    true,
                    AF_INET6,
                    TCP_TABLE_OWNER_PID_ALL,
                    0);

                if (status != NO_ERROR)
                    return;

                int count = Marshal.ReadInt32(buffer);
                IntPtr rowPtr = IntPtr.Add(
                    buffer,
                    sizeof(int));

                int rowSize =
                    Marshal.SizeOf<MIB_TCP6ROW_OWNER_PID>();

                for (int i = 0; i < count; i++)
                {
                    var row =
                        Marshal.PtrToStructure<MIB_TCP6ROW_OWNER_PID>(
                            rowPtr);

                    int port = NetworkToHostPort(row.LocalPort);

                    if (port == localPort && row.OwningPid > 0)
                    {
                        result.Add((int)row.OwningPid);
                    }

                    rowPtr = IntPtr.Add(rowPtr, rowSize);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static int NetworkToHostPort(uint networkOrderPort)
        {
            ushort value = (ushort)networkOrderPort;

            return (value >> 8) | (value << 8);
        }
    }
}