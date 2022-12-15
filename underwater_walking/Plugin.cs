
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;


[BepInPlugin("devopsdinosaur.valheim.underwater_walking", "Underwater Walking", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.underwater_walking");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.underwater_walking v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}

    [HarmonyPatch(typeof(SEMan), "Internal_AddStatusEffect")]
    class HarmonyPatch_SEMan_AddStatusEffect {

        private static bool Prefix(ref SEMan __instance, string name) {
            if (name == "Wet") {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Character), "IsSwiming")]
    class HarmonyPatch_Character_InLiquidWetDepth {

        private static bool Prefix(ref Character __instance, ref bool __result) {
            if (__instance == Player.m_localPlayer) {
                __result = false;
                return false;
            }
            return true;
        }
    }
}