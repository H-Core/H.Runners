using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace H.Runners.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileUtilities
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
            [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string? lpRootPathName,
            out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
            out uint lpTotalNumberOfClusters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Win32Exception"></exception>
        /// <returns></returns>
        public static long GetFileSizeOnDisk(string path)
        {
            path = path ?? throw new ArgumentNullException(nameof(path));
            
            var info = new FileInfo(path);
            var label = info.Directory?.Root.FullName;

            var result = GetDiskFreeSpaceW(label, 
                out var sectorsPerCluster, 
                out var bytesPerSector, 
                out _, 
                out _);
            if (result == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var clusterSize = sectorsPerCluster * bytesPerSector;
            var lowSize = GetCompressedFileSizeW(path, out var highSize);
            var size = (long)highSize << 32 | lowSize;

            return (size + clusterSize - 1) / clusterSize * clusterSize;
        }
    }
}
