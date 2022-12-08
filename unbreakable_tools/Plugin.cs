
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


[BepInPlugin("devopsdinosaur.valheim.unbreakable_tools", "Unbreakable Tools", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.unbreakable_tools");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.unbreakable_tools v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {

	}

	[HarmonyPatch(typeof(Player), "FixedUpdate")]
	class HarmonyPatch_Player_FixedUpdate {

		const float CHECK_FREQUENCY = 1.0f;
		static float m_elapsed = 0f;

		private static void Postfix(ref Player __instance, ref Inventory ___m_inventory) {
			if (__instance != Player.m_localPlayer || (m_elapsed += Time.fixedDeltaTime) < CHECK_FREQUENCY) {
				return;
			}
			m_elapsed = 0f;
			foreach (ItemDrop.ItemData item in ___m_inventory.GetAllItems()) {
				if (item.IsEquipable()) {
					item.m_durability = item.GetMaxDurability();
				}
			}
			__instance.m_swimStaminaDrainMinSkill = 0f;
			__instance.m_swimStaminaDrainMaxSkill = 0f;
		}
	}

	[HarmonyPatch(typeof(Character), "IsSwiming")]
	class HarmonyPatch_Character_IsSwiming {

		private static bool Prefix(ref bool __result) {
			__result = false;
			return false;
		}
	}
}