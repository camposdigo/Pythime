#if UNITY_EDITOR
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Pythime.EditorTools
{
    [InitializeOnLoad]
    public static class KenneyUrbanAssetInstaller
    {
        private static readonly PackSpec[] Packs =
        {
            new PackSpec(
                "RPG Urban Pack",
                "https://opengameart.org/sites/default/files/kenney_RPGurbanPack.zip",
                "Assets/Resources/PythimeArt/KenneyRPGUrban",
                true),
            new PackSpec(
                "Roguelike Modern City",
                "https://kenney.nl/media/pages/assets/roguelike-modern-city/0ff3dfff2b-1677694743/kenney_roguelike-modern-city.zip",
                "Assets/Resources/PythimeArt/KenneyModernCity",
                false),
            new PackSpec(
                "Roguelike Indoors",
                "https://kenney.nl/media/pages/assets/roguelike-indoors/4d5b520b03-1702169567/kenney_roguelike-indoors.zip",
                "Assets/Resources/PythimeArt/KenneyIndoors",
                false)
        };

        private static bool installing;

        static KenneyUrbanAssetInstaller()
        {
            EditorApplication.delayCall += EnsureInstalled;
        }

        [MenuItem("Pythime/Install or Update CC0 Art Packs")]
        public static async void EnsureInstalled()
        {
            if (installing) return;
            installing = true;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(45);
                    foreach (var pack in Packs)
                        await EnsurePack(client, pack);
                }

                AssetDatabase.Refresh();
                foreach (var pack in Packs)
                    ConfigureTextures(pack.Root);
                AssetDatabase.Refresh();

                Debug.Log("Pythime: packs CC0 urbanos atualizados. RPG Urban, Modern City e Indoors estão disponíveis.");
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Pythime: não foi possível atualizar todos os packs CC0 automaticamente. Use Pythime > Install or Update CC0 Art Packs para tentar novamente. " + exception.Message);
            }
            finally
            {
                installing = false;
            }
        }

        private static async Task EnsurePack(HttpClient client, PackSpec pack)
        {
            var marker = Path.Combine(pack.Root, ".installed_v4");
            if (File.Exists(marker)) return;

            Directory.CreateDirectory(pack.Root);
            var safeName = pack.Name.Replace(" ", "_").ToLowerInvariant();
            var tempZip = Path.Combine(Path.GetTempPath(), "pythime_" + safeName + ".zip");
            var data = await client.GetByteArrayAsync(pack.Url);
            File.WriteAllBytes(tempZip, data);

            using (var archive = ZipFile.OpenRead(tempZip))
            {
                foreach (var entry in archive.Entries)
                {
                    var normalized = entry.FullName.Replace('\\', '/');
                    if (!normalized.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                        !normalized.EndsWith("License.txt", StringComparison.OrdinalIgnoreCase) &&
                        !normalized.EndsWith("license.txt", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var isTilemap = normalized.IndexOf("/Tilemap/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    normalized.IndexOf("/Tilemaps/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    normalized.IndexOf("/Spritesheet/", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    normalized.IndexOf("/Spritesheets/", StringComparison.OrdinalIgnoreCase) >= 0;
                    var isIndividualTile = pack.ExtractIndividualTiles && normalized.IndexOf("/Tiles/", StringComparison.OrdinalIgnoreCase) >= 0;
                    var isReference = normalized.EndsWith("Sample.png", StringComparison.OrdinalIgnoreCase) ||
                                      normalized.EndsWith("Preview.png", StringComparison.OrdinalIgnoreCase);
                    var isLicense = normalized.EndsWith("License.txt", StringComparison.OrdinalIgnoreCase) ||
                                    normalized.EndsWith("license.txt", StringComparison.OrdinalIgnoreCase);

                    if (!isTilemap && !isIndividualTile && !isReference && !isLicense) continue;

                    var folder = isIndividualTile ? "Tiles" : isTilemap ? "Tilemap" : "Reference";
                    var targetFolder = Path.Combine(pack.Root, folder);
                    Directory.CreateDirectory(targetFolder);
                    var fileName = normalized.Split('/').Last();
                    if (string.IsNullOrWhiteSpace(fileName)) continue;
                    entry.ExtractToFile(Path.Combine(targetFolder, fileName), true);
                }
            }

            File.WriteAllText(marker,
                pack.Name + "\nSource: " + pack.Url + "\nLicense: Creative Commons CC0 1.0\n");
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

                if (assetPath.IndexOf("/Tiles/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 16f;
                }
                else
                {
                    importer.textureType = TextureImporterType.Default;
                    importer.isReadable = true;
                }

                importer.SaveAndReimport();
            }
        }

        private readonly struct PackSpec
        {
            public readonly string Name;
            public readonly string Url;
            public readonly string Root;
            public readonly bool ExtractIndividualTiles;

            public PackSpec(string name, string url, string root, bool extractIndividualTiles)
            {
                Name = name;
                Url = url;
                Root = root;
                ExtractIndividualTiles = extractIndividualTiles;
            }
        }
    }
}
#endif
