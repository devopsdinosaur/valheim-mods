
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;


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

		private static void Postfix(ref Player __instance, ref Inventory ___m_inventory, ref PieceTable ___m_buildPieces) {
			if (__instance != Player.m_localPlayer || (m_elapsed += Time.fixedDeltaTime) < CHECK_FREQUENCY) {
				return;
			}
			m_elapsed = 0f;
			foreach (ItemDrop.ItemData item in ___m_inventory.GetAllItems()) {
				if (item.IsEquipable()) {
					item.m_durability = item.GetMaxDurability();
				}
			}
			foreach (GameObject obj in ___m_buildPieces.m_pieces) {
				Piece piece = obj.GetComponent<Piece>();
				if (piece != null) {
					piece.m_groundOnly = false;
					piece.m_noInWater = false;
					piece.m_notOnWood = false;
					piece.m_notOnTiltingSurface = false;
					piece.m_notOnFloor = false;
					piece.m_noClipping = false;
				}
			}
		}
	}
}