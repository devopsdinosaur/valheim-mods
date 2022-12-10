
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

    [HarmonyPatch(typeof(Character), "IsSwiming")]
    class HarmonyPatch_Character_IsSwiming {

        private static bool Prefix(ref bool __result) {
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(GameCamera), "UpdateNearClipping")]
    class HarmonyPatch_GameCamera_UpdateNearClipping {

        private static bool Prefix(ref GameCamera __instance) {
            //__instance.GetType().GetTypeInfo().GetProperty("m_waterClipping").SetValue(null, false);
            return true;
        }
    }
}