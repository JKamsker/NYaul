//namespace NYaul.IO.HardwareDetection.Windows;

//using Microsoft.Win32.SafeHandles;

//using System;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Text;

//// Refactored from https://emoacht.wordpress.com/2012/11/06/csharp-ssd/
//internal class WindowsSeekDetection
//{
//    /// <summary>
//    /// Returns weather the specified disk has a seek penalty. (True: HDD, False: SSD)
//    /// </summary>
//    /// <param name="diskLetter"></param>
//    /// <returns></returns>
//    public static bool HasSeekPenalty(char diskLetter)
//    {
//        return HasSeekPenalty(GetDiskNumber(diskLetter));
//    }

//    public static bool HasNominalMediaRotationRate(char diskLetter)
//    {
//        return HasNominalMediaRotationRate(GetDiskNumber(diskLetter));
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct STORAGE_PROPERTY_QUERY
//    {
//        public uint PropertyId;
//        public uint QueryType;

//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
//        public byte[] AdditionalParameters;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct DEVICE_SEEK_PENALTY_DESCRIPTOR
//    {
//        public uint Version;
//        public uint Size;

//        [MarshalAs(UnmanagedType.U1)]
//        public bool IncursSeekPenalty;
//    }

//    // For CreateFile to get handle to drive
//    private const uint FILE_SHARE_READ = 0x00000001;

//    private const uint FILE_SHARE_WRITE = 0x00000002;
//    private const uint OPEN_EXISTING = 3;
//    private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

//    private const uint FILE_READ_ACCESS = 0x00000001;
//    private const uint FILE_WRITE_ACCESS = 0x00000002;

//    // CreateFile to get handle to drive
//    [DllImport("kernel32.dll", SetLastError = true)]
//    private static extern SafeFileHandle CreateFileW(
//        [MarshalAs(UnmanagedType.LPWStr)]
//        string lpFileName,
//        uint dwDesiredAccess,
//        uint dwShareMode,
//        IntPtr lpSecurityAttributes,
//        uint dwCreationDisposition,
//        uint dwFlagsAndAttributes,
//        IntPtr hTemplateFile);

//    // DeviceIoControl to check no seek penalty
//    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
//               SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    private static extern bool DeviceIoControl(
//        SafeFileHandle hDevice,
//        uint dwIoControlCode,
//        ref STORAGE_PROPERTY_QUERY lpInBuffer,
//        uint nInBufferSize,
//        ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
//        uint nOutBufferSize,
//        out uint lpBytesReturned,
//        IntPtr lpOverlapped);

//    // DeviceIoControl to check nominal media rotation rate
//    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
//               SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    private static extern bool DeviceIoControl(
//        SafeFileHandle hDevice,
//        uint dwIoControlCode,
//        ref ATAIdentifyDeviceQuery lpInBuffer,
//        uint nInBufferSize,
//        ref ATAIdentifyDeviceQuery lpOutBuffer,
//        uint nOutBufferSize,
//        out uint lpBytesReturned,
//        IntPtr lpOverlapped);

//    // For control codes
//    private const uint IOCTL_VOLUME_BASE = 0x00000056;

//    private const uint METHOD_BUFFERED = 0;
//    private const uint FILE_ANY_ACCESS = 0;

//    private static uint CTL_CODE(uint DeviceType, uint Function,
//                                 uint Method, uint Access)
//    {
//        return ((DeviceType << 16) | (Access << 14) |
//                (Function << 2) | Method);
//    }

//    // For DeviceIoControl to get disk extents
//    [StructLayout(LayoutKind.Sequential)]
//    private struct DISK_EXTENT
//    {
//        public uint DiskNumber;
//        public long StartingOffset;
//        public long ExtentLength;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct VOLUME_DISK_EXTENTS
//    {
//        public uint NumberOfDiskExtents;

//        [MarshalAs(UnmanagedType.ByValArray)]
//        public DISK_EXTENT[] Extents;
//    }

//    // DeviceIoControl to get disk extents
//    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl",
//               SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    private static extern bool DeviceIoControl(
//        SafeFileHandle hDevice,
//        uint dwIoControlCode,
//        IntPtr lpInBuffer,
//        uint nInBufferSize,
//        ref VOLUME_DISK_EXTENTS lpOutBuffer,
//        uint nOutBufferSize,
//        out uint lpBytesReturned,
//        IntPtr lpOverlapped);

//    // For error message
//    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

//    [DllImport("kernel32.dll", SetLastError = true)]
//    private static extern uint FormatMessage(
//        uint dwFlags,
//        IntPtr lpSource,
//        uint dwMessageId,
//        uint dwLanguageId,
//        StringBuilder lpBuffer,
//        uint nSize,
//        IntPtr Arguments);

//    //public static int GetDiskNumber(char diskLetter)
//    public static int GetDiskNumber(char diskLetter)
//    {
//        //\\?\C:

//        string sDrive = $"\\\\?\\{diskLetter}:";

//        SafeFileHandle hDrive = CreateFileW(
//            sDrive,
//            0, // No access to drive
//            FILE_SHARE_READ | FILE_SHARE_WRITE,
//            IntPtr.Zero,
//            OPEN_EXISTING,
//            FILE_ATTRIBUTE_NORMAL,
//            IntPtr.Zero);

//        if (hDrive == null || hDrive.IsInvalid)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"CreateFile failed. {message}");
//        }

//        uint IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = CTL_CODE(
//            IOCTL_VOLUME_BASE, 0,
//            METHOD_BUFFERED, FILE_ANY_ACCESS); // From winioctl.h

//        VOLUME_DISK_EXTENTS query_disk_extents = new VOLUME_DISK_EXTENTS();
//        uint returned_query_disk_extents_size;

//        bool query_disk_extents_result = DeviceIoControl(
//            hDrive,
//            IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
//            IntPtr.Zero,
//            0,
//            ref query_disk_extents,
//            (uint)Marshal.SizeOf(query_disk_extents),
//            out returned_query_disk_extents_size,
//            IntPtr.Zero);

//        hDrive.Close();

//        if (query_disk_extents_result == false ||
//            query_disk_extents.Extents.Length != 1)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"DeviceIoControl failed. {message}");
//        }

//        return (int)query_disk_extents.Extents[0].DiskNumber;
//    }

//    // True: probably HDD
//    // False: probably SSD
//    public static bool HasSeekPenalty(int diskNumber)
//    {
//        SafeFileHandle hDrive = CreateFileW(
//            $"\\\\.\\PhysicalDrive{diskNumber}",
//            0, // No access to drive
//            FILE_SHARE_READ | FILE_SHARE_WRITE,
//            IntPtr.Zero,
//            OPEN_EXISTING,
//            FILE_ATTRIBUTE_NORMAL,
//            IntPtr.Zero);

//        if (hDrive == null || hDrive.IsInvalid)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"CreateFile failed. {message}");
//        }

//        uint IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE(
//            0x0000002d, 0x500,
//            METHOD_BUFFERED, FILE_ANY_ACCESS); // From winioctl.h

//        STORAGE_PROPERTY_QUERY query_seek_penalty = new STORAGE_PROPERTY_QUERY();
//        query_seek_penalty.PropertyId = 7; // StorageDeviceSeekPenaltyProperty
//        query_seek_penalty.QueryType = 0; // PropertyStandardQuery

//        DEVICE_SEEK_PENALTY_DESCRIPTOR query_seek_penalty_desc =
//            new DEVICE_SEEK_PENALTY_DESCRIPTOR();

//        uint returned_query_seek_penalty_size;

//        bool query_seek_penalty_result = DeviceIoControl(
//            hDrive,
//            IOCTL_STORAGE_QUERY_PROPERTY,
//            ref query_seek_penalty,
//            (uint)Marshal.SizeOf(query_seek_penalty),
//            ref query_seek_penalty_desc,
//            (uint)Marshal.SizeOf(query_seek_penalty_desc),
//            out returned_query_seek_penalty_size,
//            IntPtr.Zero);

//        hDrive.Close();

//        if (query_seek_penalty_result == false)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"DeviceIoControl failed. {message}");
//        }

//        return query_seek_penalty_desc.IncursSeekPenalty == true;
//    }

//    public static bool HasNominalMediaRotationRate(int diskNumber)
//    {
//        SafeFileHandle hDrive = CreateFileW(
//            $"\\\\.\\PhysicalDrive{diskNumber}",
//            0, // No access to drive
//            FILE_SHARE_READ | FILE_SHARE_WRITE,
//            IntPtr.Zero,
//            OPEN_EXISTING,
//            FILE_ATTRIBUTE_NORMAL,
//            IntPtr.Zero);

//        if (hDrive == null || hDrive.IsInvalid)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"CreateFile failed. {message}");
//        }

//        uint IOCTL_ATA_PASS_THROUGH = CTL_CODE(
//            0x00000004, 0x040b, METHOD_BUFFERED,
//            FILE_READ_ACCESS | FILE_WRITE_ACCESS); // From ntddscsi.h

//        ATAIdentifyDeviceQuery id_query = new ATAIdentifyDeviceQuery();
//        id_query.data = new ushort[256];

//        id_query.header.Length = (ushort)Marshal.SizeOf(id_query.header);
//        id_query.header.AtaFlags = 0x02; // ATA_FLAGS_DATA_IN
//        id_query.header.DataTransferLength = (uint)(id_query.data.Length * 2); // Size of "data" in bytes
//        id_query.header.TimeOutValue = 3; // Sec
//        id_query.header.DataBufferOffset = (IntPtr)Marshal.OffsetOf(
//            typeof(ATAIdentifyDeviceQuery), "data");
//        id_query.header.PreviousTaskFile = new byte[8];
//        id_query.header.CurrentTaskFile = new byte[8];
//        id_query.header.CurrentTaskFile[6] = 0xec; // ATA IDENTIFY DEVICE

//        uint retval_size;

//        bool result = DeviceIoControl(
//            hDrive,
//            IOCTL_ATA_PASS_THROUGH,
//            ref id_query,
//            (uint)Marshal.SizeOf(id_query),
//            ref id_query,
//            (uint)Marshal.SizeOf(id_query),
//            out retval_size,
//            IntPtr.Zero);

//        hDrive.Close();

//        if (result == false)
//        {
//            string message = GetErrorMessage(Marshal.GetLastWin32Error());
//            throw new IOException($"DeviceIoControl failed. {message}");
//        }

//        const int kNominalMediaRotRateWordIndex = 217; // Word index of nominal media rotation rate

//        return id_query.data[kNominalMediaRotRateWordIndex] != 1;
//    }

//    // Method for error message
//    private static string GetErrorMessage(int code)
//    {
//        StringBuilder message = new StringBuilder(255);

//        FormatMessage(
//          FORMAT_MESSAGE_FROM_SYSTEM,
//          IntPtr.Zero,
//          (uint)code,
//          0,
//          message,
//          (uint)message.Capacity,
//          IntPtr.Zero);

//        return message.ToString();
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct ATA_PASS_THROUGH_EX
//    {
//        public ushort Length;
//        public ushort AtaFlags;
//        public byte PathId;
//        public byte TargetId;
//        public byte Lun;
//        public byte ReservedAsUchar;
//        public uint DataTransferLength;
//        public uint TimeOutValue;
//        public uint ReservedAsUlong;
//        public IntPtr DataBufferOffset;

//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
//        public byte[] PreviousTaskFile;

//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
//        public byte[] CurrentTaskFile;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    private struct ATAIdentifyDeviceQuery
//    {
//        public ATA_PASS_THROUGH_EX header;

//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
//        public ushort[] data;
//    }
//}