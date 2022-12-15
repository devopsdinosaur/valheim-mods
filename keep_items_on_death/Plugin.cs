
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


[BepInPlugin("devopsdinosaur.valheim.keep_items_on_death", "Keep Items on Death", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.keep_items_on_death");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.keep_items_on_death v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	[HarmonyPatch(typeof(Player), "UnequipDeathDropItems")]
	class HarmonyPatch_Player_UnequipDeathDropItems {

		private static bool Prefix() {
			return false;
		}
	}

	[HarmonyPatch(typeof(Player), "CreateTombstone")]
	class HarmonyPatch_Player_CreateTombstone {

		private static bool Prefix() {
			return false;
		}
	}

	[HarmonyPatch(typeof(Inventory), "MoveInventoryToGrave")]
	class HarmonyPatch_Inventory_MoveInventoryToGrave {

		private static bool Prefix() {
			return false;
		}
	}

}