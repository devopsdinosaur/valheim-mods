
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;


[BepInPlugin("devopsdinosaur.valheim.map_teleport", "Map Teleport", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.map_teleport");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.map_teleport v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {

	}

	[HarmonyPatch(typeof(Minimap), "OnMapMiddleClick")]
	class HarmonyPatch_Minimap_OnMap_MiddleClick {

		private static bool Prefix(ref Minimap __instance) {
			Vector3 position = __instance.
				GetType().
				GetTypeInfo().
				GetDeclaredMethod("ScreenToWorldPoint").
				Invoke(__instance, (object[]) new Vector3[] {Input.mousePosition});

			return false;
		}
	}

}