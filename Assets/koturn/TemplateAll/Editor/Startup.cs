using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace lilToon
{
    /// <summary>
    /// Startup method provider.
    /// </summary>
    internal static class Startup
    {
        /// <summary>
        /// Buffer size of streams.
        /// </summary>
        private const int DefaultBufferSize = 1024;

        /// <summary>
        /// A method called at Unity startup.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnStartup()
        {
            AssetDatabase.importPackageCompleted += Startup_ImportPackageCompleted;
            UpdateVersionDefFile();
        }

        /// <summary>
        /// Update definition file of version value of lilToon, lil_current_version_value.hlsl.
        /// </summary>
        private static void UpdateVersionDefFile()
        {
            var dstDirPath = AssetDatabase.GUIDToAssetPath(AssetGuid.ShaderDir);
            if (dstDirPath == "")
            {
                Debug.LogWarning("Cannot find file or directory corresponding to GUID: " + AssetGuid.ShaderDir);
                return;
            }
            if (!Directory.Exists(dstDirPath))
            {
                Debug.LogWarningFormat("Directory not found: {0} ({1})", dstDirPath, AssetGuid.ShaderDir);
                return;
            }
            UpdateVersionDefFile(Path.Combine(dstDirPath, "lil_current_version.hlsl"));
        }

        /// <summary>
        /// Update definition file of version value of lilToon, lil_current_version_value.hlsl.
        /// </summary>
        /// <param name="filePath">Destination file path.</param>
        /// <param name="bufferSize">Buffer size for temporary buffer and <see cref="FileStream"/>,
        /// and initial capacity of <see cref="MemoryStream"/>.</param>
        public static void UpdateVersionDefFile(string filePath, int bufferSize = DefaultBufferSize)
        {
            using (var ms = new MemoryStream(bufferSize))
            {
                WriteVersionFileBytes(ms);
                var buffer = ms.GetBuffer();
                var length = (int)ms.Length;

                if (CompareFileBytes(filePath, buffer, 0, length, bufferSize))
                {
                    return;
                }

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize))
                {
                    fs.Write(buffer, 0, length);
                }

                Debug.LogFormat("Update {0}", filePath);
            }
        }

        /// <summary>
        /// Write version file content to <see cref="s"/>.
        /// </summary>
        /// <param name="s">Destination stream.</param>
        /// <param name="bufferSize">Buffer size for <see cref="StreamWriter"/>.</param>
        private static void WriteVersionFileBytes(Stream s, int bufferSize = DefaultBufferSize)
        {
            using (var writer = new StreamWriter(s, Encoding.ASCII, bufferSize, true)
            {
                NewLine = "\n"
            })
            {
                writer.WriteLine("#ifndef LIL_CURRENT_VERSION_INCLUDED");
                writer.WriteLine("#define LIL_CURRENT_VERSION_INCLUDED");
                writer.WriteLine();
                writer.WriteLine("#define LIL_CURRENT_VERSION_VALUE {0}", lilConstants.currentVersionValue);

                var match = new Regex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)").Match(lilConstants.currentVersionName);
                if (match.Success)
                {
                    var groups = match.Groups;
                    writer.WriteLine("#define LIL_CURRENT_VERSION_MAJOR {0}", groups[1].Value);
                    writer.WriteLine("#define LIL_CURRENT_VERSION_MINOR {0}", groups[2].Value);
                    writer.WriteLine("#define LIL_CURRENT_VERSION_PATCH {0}", groups[3].Value);
                }

                writer.WriteLine();
                writer.WriteLine("#endif  // LIL_CURRENT_VERSION_INCLUDED");
            }
        }

        /// <summary>
        /// Compare file content with specified byte sequence.
        /// </summary>
        /// <param name="filePath">Target file path.</param>
        /// <param name="contentData">File content data to compare.</param>
        /// <param name="offset">Offset of <paramref name="contentData"/>,</param>
        /// <param name="count">Length of <paramref name="contentData"/>.</param>
        /// <param name="bufferSize">Buffer size for temporary buffer and <see cref="FileStream"/>.</param>
        /// <returns>True if file content is same to <see cref="contentData"/>, otherwise false.</returns>
        private static bool CompareFileBytes(string filePath, byte[] contentData, int offset, int count, int bufferSize = DefaultBufferSize)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            if (new FileInfo(filePath).Length != count)
            {
                return false;
            }

            var minBufferSize = Math.Min(count, bufferSize);
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, minBufferSize))
            {
                var buffer = new byte[minBufferSize];
                int nRead;
                while ((nRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (!CompareMemory(buffer, 0, contentData, offset, nRead))
                    {
                        return false;
                    }
                    offset += nRead;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare two byte data.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static bool CompareMemory(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            if (Environment.Is64BitProcess)
            {
                return CompareMemoryX64(data1, offset1, data2, offset2, count);
            }
            else
            {
                return CompareMemoryX86(data1, offset1, data2, offset2, count);
            }
        }

        /// <summary>
        /// Compare two byte data for x64 environment.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static bool CompareMemoryX64(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            unsafe
            {
                fixed (byte* pData1 = &data1[offset1])
                fixed (byte* pData2 = &data2[offset2])
                {
                    return CompareMemoryX64(pData1, pData2, (ulong)count);
                }
            }
        }

        /// <summary>
        /// Compare two byte data for x64 environment.
        /// </summary>
        /// <param name="pData1">First pointer to byte data array.</param>
        /// <param name="pData2">Second pointer to byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="pData1"/> and <paramref name="pData2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static unsafe bool CompareMemoryX64(byte* pData1, byte* pData2, ulong count)
        {
            const ulong stride = sizeof(ulong);
            var n = count & ~(stride - 1);

            for (ulong i = 0; i < n; i += stride)
            {
                if (*(ulong*)&pData1[i] != *(ulong*)&pData2[i])
                {
                    return false;
                }
            }

            for (ulong i = n; i < count; i++)
            {
                if (pData1[i] != pData2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare two byte data for x86 environment.
        /// </summary>
        /// <param name="data1">First byte data array.</param>
        /// <param name="offset1">Offset of first byte data array.</param>
        /// <param name="data2">Second byte data array.</param>
        /// <param name="offset2">Offset of second byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="data1"/> and <paramref name="data2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static bool CompareMemoryX86(byte[] data1, int offset1, byte[] data2, int offset2, int count)
        {
            unsafe
            {
                fixed (byte* pData1 = &data1[offset1])
                fixed (byte* pData2 = &data2[offset2])
                {
                    return CompareMemoryX86(pData1, pData2, (uint)count);
                }
            }
        }

        /// <summary>
        /// Compare two byte data for x86 environment.
        /// </summary>
        /// <param name="pData1">First pointer to byte data array.</param>
        /// <param name="pData2">Second pointer to byte data array.</param>
        /// <param name="count">Number of bytes comparing <paramref name="pData1"/> and <paramref name="pData2"/>.</param>
        /// <returns>True if two byte data is same, otherwise false.</returns>
        private static unsafe bool CompareMemoryX86(byte* pData1, byte* pData2, uint count)
        {
            const uint stride = sizeof(uint);
            var n = count & ~(stride - 1);

            for (uint i = 0; i < n; i += stride)
            {
                if (*(uint*)&pData1[i] != *(uint*)&pData2[i])
                {
                    return false;
                }
            }

            for (uint i = n; i < count; i++)
            {
                if (pData1[i] != pData2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// A callback method for <see cref="AssetDatabase.importPackageCompleted"/>.
        /// </summary>
        /// <param name="packageName">Imported package name.</param>
        private static void Startup_ImportPackageCompleted(string packageName)
        {
            if (!packageName.StartsWith("lilToon"))
            {
                return;
            }
            UpdateVersionDefFile();
        }
    }
}
