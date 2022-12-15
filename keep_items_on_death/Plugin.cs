
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;


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

	/*
	[HarmonyPatch(typeof(Player), "UseHotbarItem")]
	class HarmonyPatch_Player_UseHotbarItem {

		Not currently working...

		private static bool Prefix(ref Player __instance, int index, ref Inventory ___m_inventory) {
			ItemDrop.ItemData item = ___m_inventory.GetItemAt(index, 0);
			if (item != null) {
				logger.LogInfo(item.ToString());
			}
			if (item != null && item.m_equiped) {
				return false;
			}
			return true;
		}
	}
	*/

	[HarmonyPatch(typeof(Player), "UnequipDeathDropItems")]
	class HarmonyPatch_Player_UnequipDeathDropItems {

		private static bool Prefix() {
			return false;
		}
	}

	[HarmonyPatch(typeof(Player), "CreateTombStone")]
	class HarmonyPatch_Player_CreateTombStone {

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

	[HarmonyPatch(typeof(Player), "OnDeath")]
	class HarmonyPatch_Player_OnDeath {

		static List<Player.Food> foods;

		private static bool Prefix(ref List<Player.Food> ___m_foods) {
			foods = new List<Player.Food>(___m_foods);
			return true;
		}

		private static void Postfix(ref List<Player.Food> ___m_foods) {
			___m_foods = foods;
		}
	}

	[HarmonyPatch(typeof(Game), "RequestRespawn")]
	class HarmonyPatch_Game_RequestRespawn {

		private static bool Prefix(ref Game __instance) {
			__instance.CancelInvoke("_RequestRespawn");
			__instance.Invoke("_RequestRespawn", 0);
			return false;
		}
	}
}