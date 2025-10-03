using System;
using System.IO;
using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

[assembly: MelonInfo(typeof(FasterChem.Core), "FasterChem", "1.0.1", "Owlgames_", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace FasterChem
{
    public class Core : MelonMod
    {
        private static int newCookDuration = 180;
        private static int _divider = 2;
        private static string configPath;
        private static Config config;

        public override void OnInitializeMelon()
        {
            UnityEngine.Object.DontDestroyOnLoad(new GameObject("FasterChem"));
            MelonLogger.Msg("FasterChem initialized.");

            // Harmony patch
            var harmony = new HarmonyLib.Harmony("com.xaender.FasterChem");
            harmony.PatchAll();

            // Load or create config
            configPath = Path.Combine(MelonEnvironment.UserDataDirectory, "FasterChemConfig.json");
            LoadOrCreateConfig();
        }

        private static void LoadOrCreateConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    config = JsonConvert.DeserializeObject<Config>(json);
                    MelonLogger.Msg("Config loaded successfully.");
                }
                else
                {
                    config = new Config
                    {
                        CookDuration = 180,
                        SpeedUpDivider = 2
                    };
                    SaveConfig();
                    MelonLogger.Msg("Config not found. Created default FasterChemConfig.json.");
                }

                newCookDuration = config.CookDuration;
                _divider = config.SpeedUpDivider;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to load or create config: {e}");
            }
        }

        private static void SaveConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Failed to save config: {e}");
            }
        }

        [HarmonyPatch(typeof(ChemistryStation), "SetCookOperation")]
        public class Patch_CookTime
        {
            [HarmonyPostfix]
            public static void Postfix(ChemistryStation __instance)
            {
                if (__instance?.CurrentCookOperation?.Recipe != null)
                {
                    int fasterTime = Math.Max(1, newCookDuration / _divider);
                    __instance.CurrentCookOperation.Recipe.CookTime_Mins = fasterTime;
                    MelonLogger.Msg($"CookTime set to {fasterTime} minutes.");
                }
            }
        }

        private class Config
        {
            public int CookDuration { get; set; }
            public int SpeedUpDivider { get; set; }
        }
    }
}
