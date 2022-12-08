
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;


[BepInPlugin("devopsdinosaur.valheim.inside_campfire", "Inside Campfire", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.inside_campfire");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.inside_campfire v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}

	[HarmonyPatch(typeof(Fireplace), "CheckUnderTerrain")]
	class HarmonyPatch_Fireplace_CheckUnderTerrain {

		private static bool Prefix(ref bool ___m_blocked) {
			___m_blocked = false;
			return false;
		}
	}

}