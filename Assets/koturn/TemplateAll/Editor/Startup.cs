using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using lilToon;


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
        private const int BufferSize = 8192;

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

            using (var ms = new MemoryStream(BufferSize))
            {
                WriteVersionFileBytes(ms);
                var buffer = ms.GetBuffer();
                var length = (int)ms.Length;
                var dstFilePath = Path.Combine(dstDirPath, "lil_current_version.hlsl");
                if (CompareFileBytes(dstFilePath, buffer, 0, length))
                {
                    return;
                }
                using (var fs = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    fs.Write(buffer, 0, length);
                }

                Debug.Log($"Update {dstFilePath}");
            }
        }

        /// <summary>
        /// Write version file content to <see cref="s"/>.
        /// </summary>
        /// <param name="s">Destination stream.</param>
        private static void WriteVersionFileBytes(Stream s)
        {
            using (var writer = new StreamWriter(s, Encoding.ASCII, BufferSize, true))
            {
                writer.Write("#ifndef LIL_CURRENT_VERSION_INCLUDED\n");
                writer.Write("#define LIL_CURRENT_VERSION_INCLUDED\n");
                writer.Write('\n');
                writer.Write("#define LIL_CURRENT_VERSION_VALUE {0}\n", lilConstants.currentVersionValue);

                var verTokens = lilConstants.currentVersionName.Split('.');
                int verNum;
                if (int.TryParse(verTokens[0], out verNum))
                {
                    writer.Write("#define LIL_CURRENT_VERSION_MAJOR {0}\n", verNum);
                }
                if (verTokens.Length > 0 && int.TryParse(verTokens[1], out verNum))
                {
                    writer.Write("#define LIL_CURRENT_VERSION_MINOR {0}\n", verNum);
                }
                if (verTokens.Length > 1 && int.TryParse(verTokens[2], out verNum))
                {
                    writer.Write("#define LIL_CURRENT_VERSION_PATCH {0}\n", verNum);
                }

                writer.Write('\n');
                writer.Write("#endif  // LIL_CURRENT_VERSION_INCLUDED\n");
            }
        }

        /// <summary>
        /// Compare file content with specified byte sequence.
        /// </summary>
        /// <param name="filePath">Target file path.</param>
        /// <param name="contentData">File content data to compare.</param>
        /// <param name="offset">Offset of <paramref name="contentData"/>,</param>
        /// <param name="count">Length of <paramref name="contentData"/>.</param>
        /// <returns>True if file content is same to <see cref="contentData"/>, otherwise false.</returns>
        private static bool CompareFileBytes(string filePath, byte[] contentData, int offset, int count)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            if (new FileInfo(filePath).Length != count)
            {
                return false;
            }

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[Math.Min(BufferSize, count)];
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
