using System;
using UnityEditor;
using UnityEngine;

namespace Pythime.EditorTools
{
    public sealed class OfficialAssetPostprocessor : AssetPostprocessor
    {
        private static readonly string[] Paths =
        {
            OfficialPlayerAnimator.AssetPath,
            "Assets/Resources/OfficialMaps/city_1956.png",
            "Assets/Resources/OfficialMaps/city_2026.png",
            "Assets/Resources/OfficialMaps/city_2096.png"
        };

        private void OnPreprocessTexture()
        {
            if (Array.IndexOf(Paths, assetPath) < 0) return;
            try { OfficialPngValidation.ValidateFile(assetPath); }
            catch (Exception ex) { Debug.LogError(OfficialPngValidation.DescribeFile(assetPath, ex.Message)); }
            Configure((TextureImporter)assetImporter);
        }

        private static void Configure(TextureImporter importer)
        {
            bool player = importer.assetPath == OfficialPlayerAnimator.AssetPath;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = player ? 2048 : 4096;
            importer.isReadable = player;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.spritePixelsPerUnit = player ? 100f : 1254f / OfficialEraMapRuntime.MapSize;
            // Platform overrides must not silently shrink/compress these source textures.
            foreach (string platform in new[] { "Standalone", "Android", "iPhone", "WebGL" })
                importer.ClearPlatformTextureSettings(platform);
        }

        [InitializeOnLoadMethod]
        private static void ScheduleImport()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                const string key = "Pythime.OfficialImports.v1";
                if (SessionState.GetBool(key, false)) return;
                SessionState.SetBool(key, true);
                Reimport();
            };
        }

        [MenuItem("Pythime/Reimport Official Assets")]
        public static void Reimport()
        {
            foreach (var path in Paths) AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
