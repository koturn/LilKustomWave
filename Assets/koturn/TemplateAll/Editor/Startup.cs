using System.IO;
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
            var guidShaderDir = TemplateAllInspector.GuidShaderDir;
            var dstDirPath = AssetDatabase.GUIDToAssetPath(guidShaderDir);
            if (dstDirPath == "")
            {
                Debug.LogWarning("Cannot find file or directory corresponding to GUID: " + guidShaderDir);
                return;
            }
            if (!Directory.Exists(dstDirPath))
            {
                Debug.LogWarningFormat("Directory not found: {0} ({1})", dstDirPath, guidShaderDir);
                return;
            }

            var line = "#define LIL_CURRENT_VERSION_VALUE " + lilConstants.currentVersionValue;
            var dstFilePath = Path.Combine(dstDirPath, "lil_current_version_value.hlsl");
            if (File.Exists(dstFilePath) && ReadFirstLine(dstFilePath) == line)
            {
                return;
            }

            using (var fs = new FileStream(dstFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(line);
                sw.Write('\n');
            }

            Debug.Log($"Update {dstFilePath}");
        }

        /// <summary>
        /// Read first line of the specified file.
        /// </summary>
        /// <param name="filePath">File to read.</param>
        /// <returns>First line of <paramref name="filePath"/>.</returns>
        private static string ReadFirstLine(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadLine();
            }
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
