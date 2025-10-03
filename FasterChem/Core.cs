using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

// hi :) Fixed By OwlGames_ On Discord

[assembly: MelonInfo(typeof(FasterChem.Core), "FasterChem", "1.0.0", "Owlgames_", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace FasterChem
{
    public class Core : MelonMod
    {
        private static int newCookDuration = 180;
        private static int _divider = 2;

        public override void OnInitializeMelon()
        {
            UnityEngine.Object.DontDestroyOnLoad(new GameObject("FasterChem"));
            LoggerInstance.Msg("FasterChem initialized.");

            var harmony = new HarmonyLib.Harmony("com.xaender.FasterChem");
            harmony.PatchAll();
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
