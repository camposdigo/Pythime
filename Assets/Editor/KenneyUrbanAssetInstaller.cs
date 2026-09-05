#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using UnityEditor;
using UnityEngine;

namespace Pythime.EditorTools
{
    [InitializeOnLoad]
    public static class KenneyUrbanAssetInstaller
    {
        private const string DownloadUrl = "https://opengameart.org/sites/default/files/kenney_RPGurbanPack.zip";
        private const string Root = "Assets/Resources/PythimeArt/KenneyRPGUrban";
        private const string Marker = Root + "/.installed";
        private static bool installing;

        static KenneyUrbanAssetInstaller()
        {
            EditorApplication.delayCall += EnsureInstalled;
        }

        [MenuItem("Pythime/Install CC0 Urban Art")]
        public static async void EnsureInstalled()
        {
            if (installing || File.Exists(Marker)) return;
            installing = true;

            try
            {
                Directory.CreateDirectory(Root);
                var tempZip = Path.Combine(Path.GetTempPath(), "pythime_kenney_rpg_urban.zip");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    var data = await client.GetByteArrayAsync(DownloadUrl);
                    File.WriteAllBytes(tempZip, data);
                }

                using (var archive = ZipFile.OpenRead(tempZip))
                {
                    foreach (var entry in archive.Entries)
                    {
                        var normalized = entry.FullName.Replace('\\', '/');
                        if (!normalized.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                            !normalized.EndsWith("License.txt", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (!normalized.Contains("/Tilemap/") &&
                            !normalized.Contains("/Tiles/") &&
                            !normalized.EndsWith("Sample.png", StringComparison.OrdinalIgnoreCase) &&
                            !normalized.EndsWith("Preview.png", StringComparison.OrdinalIgnoreCase) &&
                            !normalized.EndsWith("License.txt", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var fileName = normalized.Split('/').Last();
                        string targetFolder;

                        if (normalized.Contains("/Tilemap/"))
                            targetFolder = Root + "/Tilemap";
                        else if (normalized.Contains("/Tiles/"))
                            targetFolder = Root + "/Tiles";
                        else
                            targetFolder = Root;

                        Directory.CreateDirectory(targetFolder);
                        entry.ExtractToFile(Path.Combine(targetFolder, fileName), true);
                    }
                }

                File.WriteAllText(Marker,
                    "Kenney RPG Urban Pack\nSource: https://opengameart.org/content/rpg-urban-pack\nLicense: CC0 1.0\n");

                AssetDatabase.Refresh();
                ConfigureTextures(Root);
                AssetDatabase.Refresh();
                Debug.Log("Pythime: pack urbano CC0 instalado. A arte de referência já está disponível no protótipo.");
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Pythime: não foi possível baixar o pack CC0 automaticamente. Use Pythime > Install CC0 Urban Art para tentar novamente. {exception.Message}");
            }
            finally
            {
                installing = false;
            }
        }

        private static void ConfigureTextures(string directory)
        {
            if (!Directory.Exists(directory)) return;

            foreach (var file in Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories))
            {
                var assetPath = file.Replace('\\', '/');
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;

                if (assetPath.EndsWith("/Sample.png", StringComparison.OrdinalIgnoreCase))
                {
                    importer.textureType = TextureImporterType.Default;
                    importer.isReadable = true;
                }
                else
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 16f;
                }

                importer.SaveAndReimport();
            }
        }
    }
}
#endif
