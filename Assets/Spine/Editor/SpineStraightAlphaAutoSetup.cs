#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace SpineTools.Editor
{
    public class SpineStraightAlphaAutoSetup : AssetPostprocessor
    {
        const string WATCH_FOLDER = "Assets/SpineAssets";
        const float DEFAULT_MIX_DURATION = 0.0f;
        const string TARGET_SHADER_NAME = "Custom/Spine_Skeleton_Glow";

        const string STRAIGHT_ALPHA_PROP = "_StraightAlphaInput";
        const string STRAIGHT_ALPHA_KEYWORD = "_STRAIGHT_ALPHA_INPUT";

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(WATCH_FOLDER, System.StringComparison.OrdinalIgnoreCase))
                return;

            if (!HasAtlasFileInSameFolder(assetPath))
                return;

            TextureImporter ti = (TextureImporter)assetImporter;
            ti.sRGBTexture = true;
            ti.alphaIsTransparency = true;
            ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.mipmapEnabled = false;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            Debug.Log($"[SpineSetup] ✓ sRGB = true → {assetPath}");
        }

        static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool hasSpineImport = false;
            foreach (string path in importedAssets)
            {
                if (!path.StartsWith(WATCH_FOLDER, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (path.EndsWith(".atlas.txt", System.StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".atlas", System.StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".skel.bytes", System.StringComparison.OrdinalIgnoreCase))
                {
                    hasSpineImport = true;
                    break;
                }
            }

            if (!hasSpineImport) return;

            EditorApplication.delayCall += RunFixAssets;
        }

        static void RunFixAssets()
        {
            EditorApplication.delayCall -= RunFixAssets;

            bool dirty = false;

            string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { WATCH_FOLDER });
            foreach (string guid in matGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!fileName.EndsWith("_Material", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;

                // Change shader first, then set straight alpha
                if (SetShader(mat, path)) dirty = true;
                if (SetStraightAlpha(mat, path)) dirty = true;
            }

            string[] dataGuids = AssetDatabase.FindAssets("t:SkeletonDataAsset", new[] { WATCH_FOLDER });
            foreach (string guid in dataGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var dataAsset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>(path);
                if (dataAsset == null) continue;

                if (SetDefaultMix(dataAsset, path))
                    dirty = true;
            }

            if (dirty) AssetDatabase.SaveAssets();
        }

        static bool HasAtlasFileInSameFolder(string assetPath)
        {
            string dir = Path.GetDirectoryName(assetPath).Replace('\\', '/');
            string[] ids = AssetDatabase.FindAssets("t:TextAsset", new[] { dir });

            foreach (string guid in ids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (!Path.GetDirectoryName(p).Replace('\\', '/').Equals(
                        dir, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                if (p.EndsWith(".atlas.txt", System.StringComparison.OrdinalIgnoreCase)) return true;
                if (p.EndsWith(".atlas", System.StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        static bool SetShader(Material mat, string matPath)
        {
            Shader target = Shader.Find(TARGET_SHADER_NAME);
            if (target == null)
            {
                Debug.LogWarning(
                    $"[SpineSetup] Shader '{TARGET_SHADER_NAME}' not found → {matPath}", mat);
                return false;
            }

            if (mat.shader == target) return false;

            mat.shader = target;
            EditorUtility.SetDirty(mat);
            Debug.Log($"[SpineSetup] ✓ Shader → {TARGET_SHADER_NAME} → {matPath}");
            return true;
        }

        static bool SetStraightAlpha(Material mat, string matPath)
        {
            if (!mat.HasProperty(STRAIGHT_ALPHA_PROP))
            {
                Debug.LogWarning(
                    $"[SpineSetup] Shader does not have '{STRAIGHT_ALPHA_PROP}' → {matPath}", mat);
                return false;
            }

            if (Mathf.Approximately(mat.GetFloat(STRAIGHT_ALPHA_PROP), 1f)) return false;

            mat.SetFloat(STRAIGHT_ALPHA_PROP, 1f);
            mat.EnableKeyword(STRAIGHT_ALPHA_KEYWORD);
            EditorUtility.SetDirty(mat);
            Debug.Log($"[SpineSetup] ✓ Straight Alpha = true → {matPath}");
            return true;
        }

        static bool SetDefaultMix(Spine.Unity.SkeletonDataAsset dataAsset, string path)
        {
            if (Mathf.Approximately(dataAsset.defaultMix, DEFAULT_MIX_DURATION)) return false;

            dataAsset.defaultMix = DEFAULT_MIX_DURATION;
            EditorUtility.SetDirty(dataAsset);
            Debug.Log($"[SpineSetup] ✓ Default Mix = {DEFAULT_MIX_DURATION}s → {path}");
            return true;
        }

        [MenuItem("Spine/Tools/Fix Straight Alpha + sRGB + Default Mix")]
        static void FixAll()
        {
            int matFixed = 0, shaderFixed = 0, texFixed = 0, dataFixed = 0;

            string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { WATCH_FOLDER });
            foreach (string guid in matGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!fileName.EndsWith("_Material", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;

                if (SetShader(mat, path)) shaderFixed++;
                if (SetStraightAlpha(mat, path)) matFixed++;
            }

            string[] texGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { WATCH_FOLDER });
            foreach (string guid in texGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase)) continue;
                if (!HasAtlasFileInSameFolder(path)) continue;

                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;
                if (ti.sRGBTexture && !ti.alphaIsTransparency) continue;

                ti.sRGBTexture = true;
                ti.alphaIsTransparency = false;
                ti.alphaSource = TextureImporterAlphaSource.FromInput;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                texFixed++;
            }

            string[] dataGuids = AssetDatabase.FindAssets("t:SkeletonDataAsset", new[] { WATCH_FOLDER });
            foreach (string guid in dataGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var dataAsset = AssetDatabase.LoadAssetAtPath<Spine.Unity.SkeletonDataAsset>(path);
                if (dataAsset != null && SetDefaultMix(dataAsset, path)) dataFixed++;
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog(
                "Spine Setup",
                $"✓ {shaderFixed} Material → Shader = {TARGET_SHADER_NAME}\n" +
                $"✓ {matFixed} Material → Straight Alpha = true\n" +
                $"✓ {texFixed} Texture → sRGB = true\n" +
                $"✓ {dataFixed} SkeletonData → Default Mix = {DEFAULT_MIX_DURATION}s",
                "OK");
        }
    }
}
#endif