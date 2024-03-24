using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;


namespace lilToon
{
    /// <summary>
    /// <see cref="ShaderGUI"/> for the custom shader variations of lilToon.
    /// </summary>
    public class TemplateAllInspector : lilToonInspector
    {
        // Custom properties
        //private MaterialProperty customVariable;

        /// <summary>
        /// A flag whether to fold custom properties or not.
        /// </summary>
        private static bool isShowCustomProperties;

        /// <summary>
        /// GUID of shader directory.
        /// </summary>
        // TODO: Replace to GUID of your "Shader" directory.
        public const string GuidShaderDir = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
        /// <summary>
        /// Name of this custom shader.
        /// </summary>
        private const string ShaderName = "TemplateAll";

        /// <summary>
        /// Load custom language file and make cache of shader properties.
        /// </summary>
        /// <param name="props">Properties of the material.</param>
        /// <param name="material">Target material.</param>
        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("/[Optional] ");

            // If not, set isShowRenderMode to false
            //isShowRenderMode = false;

            //LoadCustomLanguage("");  // TODO: Specify GUID of your lang_custom.txt
            //customVariable = FindProperty("_CustomVariable", props);
        }

        /// <summary>
        /// Draw custom properties.
        /// </summary>
        /// <param name="material">Target material.</param>
        protected override void DrawCustomProperties(Material material)
        {
            // GUIStyles Name   Description
            // ---------------- ------------------------------------
            // boxOuter         outer box
            // boxInnerHalf     inner box
            // boxInner         inner box without label
            // customBox        box (similar to unity default box)
            // customToggleFont label for box

            var titleLoc = GetLoc("sCustomShaderTitle");
            isShowCustomProperties = Foldout(titleLoc, titleLoc, isShowCustomProperties);
            if (!isShowCustomProperties)
            {
                return;
            }

            using (new EditorGUILayout.VerticalScope(boxOuter))
            {
                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategory"), customToggleFont);
                using (new EditorGUILayout.VerticalScope(boxInnerHalf))
                {
                    //m_MaterialEditor.ShaderProperty(customVariable, "Custom Variable");
                }
            }
        }

        /// <summary>
        /// Replace shaders to custom shaders.
        /// </summary>
        protected override void ReplaceToCustomShaders()
        {
            lts         = Shader.Find(ShaderName + "/lilToon");
            ltsc        = Shader.Find("Hidden/" + ShaderName + "/Cutout");
            ltst        = Shader.Find("Hidden/" + ShaderName + "/Transparent");
            ltsot       = Shader.Find("Hidden/" + ShaderName + "/OnePassTransparent");
            ltstt       = Shader.Find("Hidden/" + ShaderName + "/TwoPassTransparent");

            ltso        = Shader.Find("Hidden/" + ShaderName + "/OpaqueOutline");
            ltsco       = Shader.Find("Hidden/" + ShaderName + "/CutoutOutline");
            ltsto       = Shader.Find("Hidden/" + ShaderName + "/TransparentOutline");
            ltsoto      = Shader.Find("Hidden/" + ShaderName + "/OnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/" + ShaderName + "/TwoPassTransparentOutline");

            ltsoo       = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Opaque");
            ltscoo      = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Cutout");
            ltstoo      = Shader.Find(ShaderName + "/[Optional] OutlineOnly/Transparent");

            ltstess     = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Opaque");
            ltstessc    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Cutout");
            ltstesst    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/Transparent");
            ltstessot   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OpaqueOutline");
            ltstessco   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/CutoutOutline");
            ltstessto   = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TransparentOutline");
            ltstessoto  = Shader.Find("Hidden/" + ShaderName + "/Tessellation/OnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/" + ShaderName + "/Tessellation/TwoPassTransparentOutline");

            ltsl        = Shader.Find(ShaderName + "/lilToonLite");
            ltslc       = Shader.Find("Hidden/" + ShaderName + "/Lite/Cutout");
            ltslt       = Shader.Find("Hidden/" + ShaderName + "/Lite/Transparent");
            ltslot      = Shader.Find("Hidden/" + ShaderName + "/Lite/OnePassTransparent");
            ltsltt      = Shader.Find("Hidden/" + ShaderName + "/Lite/TwoPassTransparent");

            ltslo       = Shader.Find("Hidden/" + ShaderName + "/Lite/OpaqueOutline");
            ltslco      = Shader.Find("Hidden/" + ShaderName + "/Lite/CutoutOutline");
            ltslto      = Shader.Find("Hidden/" + ShaderName + "/Lite/TransparentOutline");
            ltsloto     = Shader.Find("Hidden/" + ShaderName + "/Lite/OnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/" + ShaderName + "/Lite/TwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/" + ShaderName + "/Refraction");
            ltsrefb     = Shader.Find("Hidden/" + ShaderName + "/RefractionBlur");
            ltsfur      = Shader.Find("Hidden/" + ShaderName + "/Fur");
            ltsfurc     = Shader.Find("Hidden/" + ShaderName + "/FurCutout");
            ltsfurtwo   = Shader.Find("Hidden/" + ShaderName + "/FurTwoPass");
            ltsfuro     = Shader.Find(ShaderName + "/[Optional] FurOnly/Transparent");
            ltsfuroc    = Shader.Find(ShaderName + "/[Optional] FurOnly/Cutout");
            ltsfurotwo  = Shader.Find(ShaderName + "/[Optional] FurOnly/TwoPass");
            ltsgem      = Shader.Find("Hidden/" + ShaderName + "/Gem");
            ltsfs       = Shader.Find(ShaderName + "/[Optional] FakeShadow");

            ltsover     = Shader.Find(ShaderName + "/[Optional] Overlay");
            ltsoover    = Shader.Find(ShaderName + "/[Optional] OverlayOnePass");
            ltslover    = Shader.Find(ShaderName + "/[Optional] LiteOverlay");
            ltsloover   = Shader.Find(ShaderName + "/[Optional] LiteOverlayOnePass");

            ltsm        = Shader.Find(ShaderName + "/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/" + ShaderName + "/MultiOutline");
            ltsmref     = Shader.Find("Hidden/" + ShaderName + "/MultiRefraction");
            ltsmfur     = Shader.Find("Hidden/" + ShaderName + "/MultiFur");
            ltsmgem     = Shader.Find("Hidden/" + ShaderName + "/MultiGem");
        }


        /// <summary>
        /// Try to replace the shader of the selected material to custom lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Convert material to custom shader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                var shader = GetCorrespondingCustomShader(material.shader);
                if (shader == null)
                {
                    Debug.LogWarningFormat($"Ignore {0}. \"{1}\" is not original lilToon shader.", AssetDatabase.GetAssetPath(material), material.shader.name);
                    continue;
                }

                Undo.RecordObject(material, ShaderName + "/ConvertMaterialToCustomShaderMenu");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// Menu validation method for <see cref="ConvertMaterialToCustomShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ConvertMaterialToCustomShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to custom shader", true)]
        private static bool ValidateConvertMaterialToCustomShaderMenu()
        {
            var count = 0;
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                if (GetCorrespondingCustomShaderName(material.shader.name) != null)
                {
                    count++;
                }
            }
            return count > 0;
        }

        /// <summary>
        /// Try to replace the shader of the material to original lilToon shader.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", false, 1101)]
        private static void ConvertMaterialToOriginalShaderMenu()
        {
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                var shader = GetCorrespondingOriginalShader(material.shader);
                if (shader == null)
                {
                    Debug.LogWarningFormat($"Ignore {0}. \"{1}\" is not custom lilToon shader, \"" + ShaderName + "\".", AssetDatabase.GetAssetPath(material), material.shader.name);
                    continue;
                }

                Undo.RecordObject(material, ShaderName + "/ConvertMaterialToOriginalShaderMenu");

                var renderQueue = lilMaterialUtils.GetTrueRenderQueue(material);
                material.shader = shader;
                material.renderQueue = renderQueue;
            }
        }

        /// <summary>
        /// Menu validation method for <see cref="ValidateConvertMaterialToOriginalShaderMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="ValidateConvertMaterialToOriginalShaderMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "/Convert material to original shader", true)]
        private static bool ValidateConvertMaterialToOriginalShader()
        {
            var count = 0;
            foreach (var obj in Selection.objects)
            {
                var material = obj as Material;
                if (material == null)
                {
                    continue;
                }

                if (GetCorrespondingOriginalShaderName(material.shader.name) != null)
                {
                    count++;
                }
            }
            return count > 0;
        }

        /// <summary>
        /// Callback method for menu item which refreshes shader cache and reimport.
        /// </summary>
        [MenuItem("Assets/" + ShaderName + "/Refresh shader cache", false, 2000)]
        private static void RefreshShaderCacheMenu()
        {
            var result = NativeMethods.Open("Library/ShaderCache.db", out var dbHandle);
            if (result != 0)
            {
                Debug.LogError($"Failed to open Library/ShaderCache.db [{result}]");
                return;
            }

            try
            {
                result = NativeMethods.Execute(dbHandle, "DELETE FROM shadererrors", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                if (result != 0)
                {
                    Debug.LogError($"SQL failed [{result}]");
                    return;
                }
            }
            finally
            {
                result = NativeMethods.Close(dbHandle);
                if (result != 0)
                {
                    Debug.LogError($"Failed to close database [{result}]");
                }
            }

            var shaderDirPath = AssetDatabase.GUIDToAssetPath(GuidShaderDir);
            if (shaderDirPath == "")
            {
                Debug.LogWarning("Cannot find file or directory corresponding to GUID: " + GuidShaderDir);
                return;
            }
            if (!Directory.Exists(shaderDirPath))
            {
                Debug.LogWarningFormat("Directory not found: {0} ({1})", shaderDirPath, GuidShaderDir);
                return;
            }
            AssetDatabase.ImportAsset(shaderDirPath, ImportAssetOptions.ImportRecursive);
        }

        /// <summary>
        /// Menu validation method for <see cref="RefreshShaderCacheMenu"/>.
        /// </summary>
        /// <returns>True if <see cref="RefreshShaderCacheMenu"/> works, otherwise false.</returns>
        [MenuItem("Assets/" + ShaderName + "Refresh shader cache", true)]
        private static bool ValidateRefreshShaderCacheMenu()
        {
            try
            {
                NativeMethods.Close(IntPtr.Zero);
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get a custom lilToon shader which is corresponding to specified original lilToon shader.
        /// </summary>
        /// <param name="originalShader">Original lilToon shader.</param>
        /// <returns>null if no custom lilToon shader is found, otherwise the one found.</returns>
        private static Shader GetCorrespondingCustomShader(Shader originalShader)
        {
            var customShaderName = GetCorrespondingCustomShaderName(originalShader.name);
            return customShaderName == null ? null : Shader.Find(customShaderName);
        }

        /// <summary>
        /// Get a custom lilToon shader name which is corresponding to specified original lilToon shader name.
        /// </summary>
        /// <param name="originalShaderName">Original lilToon shader name.</param>
        /// <returns>null if no custom lilToon shader name is found, otherwise the one found.</returns>
        private static string GetCorrespondingCustomShaderName(string originalShaderName)
        {
            switch (originalShaderName)
            {
                case "lilToon": return ShaderName + "/lilToon";
                case "Hidden/lilToonCutout": return "Hidden/" + ShaderName + "/Cutout";
                case "Hidden/lilToonTransparent": return "Hidden/" + ShaderName + "/Transparent";
                case "Hidden/lilToonOnePassTransparent": return "Hidden/" + ShaderName + "/OnePassTransparent";
                case "Hidden/lilToonTwoPassTransparent": return "Hidden/" + ShaderName + "/TwoPassTransparent";
                case "Hidden/lilToonOutline": return "Hidden/" + ShaderName + "/OpaqueOutline";
                case "Hidden/lilToonCutoutOutline": return "Hidden/" + ShaderName + "/CutoutOutline";
                case "Hidden/lilToonTransparentOutline": return "Hidden/" + ShaderName + "/TransparentOutline";
                case "Hidden/lilToonOnePassTransparentOutline": return "Hidden/" + ShaderName + "/OnePassTransparentOutline";
                case "Hidden/lilToonTwoPassTransparentOutline": return "Hidden/" + ShaderName + "/TwoPassTransparentOutline";
                case "_lil/[Optional] lilToonOutlineOnly": return ShaderName + "/[Optional] OutlineOnly/Opaque";
                case "_lil/[Optional] lilToonCutoutOutlineOnly": return ShaderName + "/[Optional] OutlineOnly/Cutout";
                case "_lil/[Optional] lilToonTransparentOutlineOnly": return ShaderName + "/[Optional] OutlineOnly/Transparent";
                case "Hidden/lilToonTessellation": return "Hidden/" + ShaderName + "/Tessellation/Opaque";
                case "Hidden/lilToonTessellationCutout": return "Hidden/" + ShaderName + "/Tessellation/Cutout";
                case "Hidden/lilToonTessellationTransparent": return "Hidden/" + ShaderName + "/Tessellation/Transparent";
                case "Hidden/lilToonTessellationOnePassTransparent": return "Hidden/" + ShaderName + "/Tessellation/OnePassTransparent";
                case "Hidden/lilToonTessellationTwoPassTransparent": return "Hidden/" + ShaderName + "/Tessellation/TwoPassTransparent";
                case "Hidden/lilToonTessellationOutline": return "Hidden/" + ShaderName + "/Tessellation/OpaqueOutline";
                case "Hidden/lilToonTessellationCutoutOutline": return "Hidden/" + ShaderName + "/Tessellation/CutoutOutline";
                case "Hidden/lilToonTessellationTransparentOutline": return "Hidden/" + ShaderName + "/Tessellation/TransparentOutline";
                case "Hidden/lilToonTessellationOnePassTransparentOutline": return "Hidden/" + ShaderName + "/Tessellation/OnePassTransparentOutline";
                case "Hidden/lilToonTessellationTwoPassTransparentOutline": return "Hidden/" + ShaderName + "/Tessellation/TwoPassTransparentOutline";
                case "Hidden/lilToonLite": return ShaderName + "/lilToonLite";
                case "Hidden/lilToonLiteCutout": return "Hidden/" + ShaderName + "/Lite/Cutout";
                case "Hidden/lilToonLiteTransparent": return "Hidden/" + ShaderName + "/Lite/Transparent";
                case "Hidden/lilToonLiteOnePassTransparent": return "Hidden/" + ShaderName + "/Lite/OnePassTransparent";
                case "Hidden/lilToonLiteTwoPassTransparent": return "Hidden/" + ShaderName + "/Lite/TwoPassTransparent";
                case "Hidden/lilToonLiteOutline": return "Hidden/" + ShaderName + "/Lite/OpaqueOutline";
                case "Hidden/lilToonLiteCutoutOutline": return "Hidden/" + ShaderName + "/Lite/CutoutOutline";
                case "Hidden/lilToonLiteTransparentOutline": return "Hidden/" + ShaderName + "/Lite/TransparentOutline";
                case "Hidden/lilToonLiteOnePassTransparentOutline": return "Hidden/" + ShaderName + "/Lite/OnePassTransparentOutline";
                case "Hidden/lilToonLiteTwoPassTransparentOutline": return "Hidden/" + ShaderName + "/Lite/TwoPassTransparentOutline";
                case "Hidden/lilToonRefraction": return "Hidden/" + ShaderName + "/Refraction";
                case "Hidden/lilToonRefractionBlur": return "Hidden/" + ShaderName + "/RefractionBlur";
                case "Hidden/lilToonFur": return "Hidden/" + ShaderName + "/Fur";
                case "Hidden/lilToonFurCutout": return "Hidden/" + ShaderName + "/FurCutout";
                case "Hidden/lilToonFurTwoPass": return "Hidden/" + ShaderName + "/FurTwoPass";
                case "_lil/[Optional] lilToonFurOnly": return ShaderName + "/[Optional] FurOnly/Transparent";
                case "_lil/[Optional] lilToonFurOnlyCutout": return ShaderName + "/[Optional] FurOnly/Cutout";
                case "_lil/[Optional] lilToonFurOnlyTwoPass": return ShaderName + "/[Optional] FurOnly/TwoPass";
                case "Hidden/lilToonGem": return "Hidden/" + ShaderName + "/Gem";
                case "_lil/lilToonFakeShadow": return ShaderName + "/[Optional] FakeShadow";
                case "_lil/[Optional] lilToonOverlay": return ShaderName + "/[Optional] Overlay";
                case "_lil/[Optional] lilToonOverlayOnePass": return ShaderName + "/[Optional] OverlayOnePass";
                case "_lil/[Optional] lilToonLiteOverlay": return ShaderName + "/[Optional] LiteOverlay";
                case "_lil/[Optional] lilToonLiteOverlayOnePass": return ShaderName + "/[Optional] LiteOverlayOnePass";
                case "_lil/lilToonMulti": return ShaderName + "/lilToonMulti";
                case "Hidden/lilToonMultiOutline": return "Hidden/" + ShaderName + "/MultiOutline";
                case "Hidden/lilToonMultiRefraction": return "Hidden/" + ShaderName + "/MultiRefraction";
                case "Hidden/lilToonMultiFur": return "Hidden/" + ShaderName + "/MultiFur";
                case "Hidden/lilToonMultiGem": return "Hidden/" + ShaderName + "/MultiGem";
                default: return null;
            }
        }

        /// <summary>
        /// Get a original lilToon shader which is corresponding to specified custom lilToon shader.
        /// </summary>
        /// <param name="customShader">Custom lilToon shader.</param>
        /// <returns>null if no original lilToon shader is found, otherwise the one found.</returns>
        private static Shader GetCorrespondingOriginalShader(Shader customShader)
        {
            var customShaderName = GetCorrespondingOriginalShaderName(customShader.name);
            return customShaderName == null ? null : Shader.Find(customShaderName);
        }

        /// <summary>
        /// Get a original lilToon shader name which is corresponding to specified custom lilToon shader name.
        /// </summary>
        /// <param name="customShaderName">Custom lilToon shader name.</param>
        /// <returns>null if no original lilToon shader name is found, otherwise the one found.</returns>
        private static string GetCorrespondingOriginalShaderName(string customShaderName)
        {
            switch (customShaderName)
            {
                case ShaderName + "/lilToon": return "lilToon";
                case "Hidden/" + ShaderName + "/Cutout": return "Hidden/lilToonCutout";
                case "Hidden/" + ShaderName + "/Transparent": return "Hidden/lilToonTransparent";
                case "Hidden/" + ShaderName + "/OnePassTransparent": return "Hidden/lilToonOnePassTransparent";
                case "Hidden/" + ShaderName + "/TwoPassTransparent": return "Hidden/lilToonTwoPassTransparent";
                case "Hidden/" + ShaderName + "/OpaqueOutline": return "Hidden/lilToonOutline";
                case "Hidden/" + ShaderName + "/CutoutOutline": return "Hidden/lilToonCutoutOutline";
                case "Hidden/" + ShaderName + "/TransparentOutline": return "Hidden/lilToonTransparentOutline";
                case "Hidden/" + ShaderName + "/OnePassTransparentOutline": return "Hidden/lilToonOnePassTransparentOutline";
                case "Hidden/" + ShaderName + "/TwoPassTransparentOutline": return "Hidden/lilToonTwoPassTransparentOutline";
                case ShaderName + "/[Optional] OutlineOnly/Opaque": return "_lil/[Optional] lilToonOutlineOnly";
                case ShaderName + "/[Optional] OutlineOnly/Cutout": return "_lil/[Optional] lilToonCutoutOutlineOnly";
                case ShaderName + "/[Optional] OutlineOnly/Transparent": return "_lil/[Optional] lilToonTransparentOutlineOnly";
                case "Hidden/" + ShaderName + "/Tessellation/Opaque": return "Hidden/lilToonTessellation";
                case "Hidden/" + ShaderName + "/Tessellation/Cutout": return "Hidden/lilToonTessellationCutout";
                case "Hidden/" + ShaderName + "/Tessellation/Transparent": return "Hidden/lilToonTessellationTransparent";
                case "Hidden/" + ShaderName + "/Tessellation/OnePassTransparent": return "Hidden/lilToonTessellationOnePassTransparent";
                case "Hidden/" + ShaderName + "/Tessellation/TwoPassTransparent": return "Hidden/lilToonTessellationTwoPassTransparent";
                case "Hidden/" + ShaderName + "/Tessellation/OpaqueOutline": return "Hidden/lilToonTessellationOutline";
                case "Hidden/" + ShaderName + "/Tessellation/CutoutOutline": return "Hidden/lilToonTessellationCutoutOutline";
                case "Hidden/" + ShaderName + "/Tessellation/TransparentOutline": return "Hidden/lilToonTessellationTransparentOutline";
                case "Hidden/" + ShaderName + "/Tessellation/OnePassTransparentOutline": return "Hidden/lilToonTessellationOnePassTransparentOutline";
                case "Hidden/" + ShaderName + "/Tessellation/TwoPassTransparentOutline": return "Hidden/lilToonTessellationTwoPassTransparentOutline";
                case ShaderName + "/lilToonLite": return "Hidden/lilToonLite";
                case "Hidden/" + ShaderName + "/Lite/Cutout": return "Hidden/lilToonLiteCutout";
                case "Hidden/" + ShaderName + "/Lite/Transparent": return "Hidden/lilToonLiteTransparent";
                case "Hidden/" + ShaderName + "/Lite/OnePassTransparent": return "Hidden/lilToonLiteOnePassTransparent";
                case "Hidden/" + ShaderName + "/Lite/TwoPassTransparent": return "Hidden/lilToonLiteTwoPassTransparent";
                case "Hidden/" + ShaderName + "/Lite/OpaqueOutline": return "Hidden/lilToonLiteOutline";
                case "Hidden/" + ShaderName + "/Lite/CutoutOutline": return "Hidden/lilToonLiteCutoutOutline";
                case "Hidden/" + ShaderName + "/Lite/TransparentOutline": return "Hidden/lilToonLiteTransparentOutline";
                case "Hidden/" + ShaderName + "/Lite/OnePassTransparentOutline": return "Hidden/lilToonLiteOnePassTransparentOutline";
                case "Hidden/" + ShaderName + "/Lite/TwoPassTransparentOutline": return "Hidden/lilToonLiteTwoPassTransparentOutline";
                case "Hidden/" + ShaderName + "/Refraction": return "Hidden/lilToonRefraction";
                case "Hidden/" + ShaderName + "/RefractionBlur": return "Hidden/lilToonRefractionBlur";
                case "Hidden/" + ShaderName + "/Fur": return "Hidden/lilToonFur";
                case "Hidden/" + ShaderName + "/FurCutout": return "Hidden/lilToonFurCutout";
                case "Hidden/" + ShaderName + "/FurTwoPass": return "Hidden/lilToonFurTwoPass";
                case ShaderName + "/[Optional] FurOnly/Transparent": return "_lil/[Optional] lilToonFurOnly";
                case ShaderName + "/[Optional] FurOnly/Cutout": return "_lil/[Optional] lilToonFurOnlyCutout";
                case ShaderName + "/[Optional] FurOnly/TwoPass": return "_lil/[Optional] lilToonFurOnlyTwoPass";
                case "Hidden/" + ShaderName + "/Gem": return "Hidden/lilToonGem";
                case ShaderName + "/[Optional] FakeShadow": return "_lil/lilToonFakeShadow";
                case ShaderName + "/[Optional] Overlay": return "_lil/[Optional] lilToonOverlay";
                case ShaderName + "/[Optional] OverlayOnePass": return "_lil/[Optional] lilToonOverlayOnePass";
                case ShaderName + "/[Optional] LiteOverlay": return "_lil/[Optional] lilToonLiteOverlay";
                case ShaderName + "/[Optional] LiteOverlayOnePass": return "_lil/[Optional] lilToonLiteOverlayOnePass";
                case ShaderName + "/lilToonMulti": return "_lil/lilToonMulti";
                case "Hidden/" + ShaderName + "/MultiOutline": return "Hidden/lilToonMultiOutline";
                case "Hidden/" + ShaderName + "/MultiRefraction": return "Hidden/lilToonMultiRefraction";
                case "Hidden/" + ShaderName + "/MultiFur": return "Hidden/lilToonMultiFur";
                case "Hidden/" + ShaderName + "/MultiGem": return "Hidden/lilToonMultiGem";
                default: return null;
            }
        }


        /// <summary>
        /// Provides some native methods of SQLite3.
        /// </summary>
        internal static class NativeMethods
        {
#if UNITY_EDITOR && !UNITY_EDITOR_WIN
            /// <summary>
            /// Native library name of SQLite3.
            /// </summary>
            private const string LibraryName = "sqlite3";
            /// <summary>
            /// Calling convention of library functions.
            /// </summary>
            private const CallingConvention CallConv = CallingConvention.Cdecl;
#else
            /// <summary>
            /// Native library name of SQLite3.
            /// </summary>
            private const string LibraryName = "winsqlite3";
            /// <summary>
            /// Calling convention of library functions.
            /// </summary>
            private const CallingConvention CallConv = CallingConvention.StdCall;
#endif
            /// <summary>
            /// Open database.
            /// </summary>
            /// <param name="filePath">SQLite3 database file path.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/open.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_open", CallingConvention = CallConv)]
            public static extern int Open(string filename, out IntPtr dbHandle);

            /// <summary>
            /// Close database.
            /// </summary>
            /// <param name="filePath">Database filename.</param>
            /// <param name="db">SQLite db handle.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/close.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_close", CallingConvention = CallConv)]
            public static extern int Close(IntPtr db);

            /// <summary>
            /// Execute specified SQL.
            /// </summary>
            /// <param name="db">An open database.</param>
            /// <param name="sql">SQL to be evaluated.</param>
            /// <param name="callback">Callback function.</param>
            /// <param name="callbackArg">1st argument to callback.</param>
            /// <param name="pErrMsg">Error msg written here.</param>
            /// <returns>Result code.</returns>
            /// <remarks>
            /// <seealso href="https://www.sqlite.org/c3ref/exec.html"/>
            /// </remarks>
            [DllImport(LibraryName, EntryPoint = "sqlite3_exec", CallingConvention = CallConv)]
            public static extern int Execute(IntPtr dbHandle, string sql, IntPtr callback, IntPtr callbackArg, IntPtr pErrMsg);
        }
    }
}
