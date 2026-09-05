using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pythime
{
    public sealed class TimeTravelManager : MonoBehaviour
    {
        public static TimeTravelManager Instance { get; private set; }

        private readonly Dictionary<int, GameObject> eras = new();
        private readonly List<int> years = new();
        private Transform player;
        private Vector2 seedPoint;
        private GameObject seedling1956;
        private GameObject tree2026;
        private GameObject tree2096;
        private bool seedPlanted;

        public int CurrentYear { get; private set; } = 2026;
        public bool SeedPlanted => seedPlanted;
        public event Action<int> EraChanged;

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterEra(int year, GameObject root)
        {
            eras[year] = root;
            if (!years.Contains(year))
            {
                years.Add(year);
                years.Sort();
            }
        }

        public void SetInitialYear(int year)
        {
            TravelToYear(year, false);
        }

        public void ConfigureTemporalSeed(
            Transform playerTransform,
            Vector2 patchPosition,
            GameObject pastSeedling,
            GameObject presentTree,
            GameObject futureTree)
        {
            player = playerTransform;
            seedPoint = patchPosition;
            seedling1956 = pastSeedling;
            tree2026 = presentTree;
            tree2096 = futureTree;
            ApplySeedState();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.qKey.wasPressedThisFrame) TravelRelative(-1);
            if (keyboard.eKey.wasPressedThisFrame) TravelRelative(1);
            if (keyboard.tKey.wasPressedThisFrame) ToggleTemporalSeed();
        }

        public void TravelRelative(int direction)
        {
            if (years.Count == 0) return;

            var index = years.IndexOf(CurrentYear);
            if (index < 0) index = 0;
            index = (index + direction + years.Count) % years.Count;
            TravelToYear(years[index]);
        }

        public bool TravelToYear(int year, bool announce = true)
        {
            if (!eras.ContainsKey(year)) return false;

            foreach (var pair in eras)
                pair.Value.SetActive(pair.Key == year);

            CurrentYear = year;
            ApplyCameraMood(year);
            EraChanged?.Invoke(year);

            if (announce) Debug.Log($"Pythime: timeline shifted to {year}.");
            return true;
        }

        public bool ToggleTemporalSeed()
        {
            if (CurrentYear != 1956 || player == null) return false;
            if (Vector2.Distance(player.position, seedPoint) > 1.8f) return false;

            seedPlanted = !seedPlanted;
            ApplySeedState();
            Debug.Log(seedPlanted
                ? "Pythime: seed planted in 1956. Check the future."
                : "Pythime: seed removed from the timeline.");
            return true;
        }

        public bool PlantSeedFromConsole()
        {
            if (CurrentYear != 1956) TravelToYear(1956);
            seedPlanted = true;
            ApplySeedState();
            return true;
        }

        private void ApplySeedState()
        {
            if (seedling1956 != null) seedling1956.SetActive(seedPlanted);
            if (tree2026 != null) tree2026.SetActive(seedPlanted);
            if (tree2096 != null) tree2096.SetActive(seedPlanted);
        }

        private static void ApplyCameraMood(int year)
        {
            if (Camera.main == null) return;

            Camera.main.backgroundColor = year switch
            {
                1956 => new Color(0.90f, 0.82f, 0.65f),
                2026 => new Color(0.54f, 0.76f, 0.83f),
                2096 => new Color(0.16f, 0.12f, 0.27f),
                _ => Color.black
            };
        }
    }
}
