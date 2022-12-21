
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;


[BepInPlugin("devopsdinosaur.valheim.debugging", "My Sandbox Mod", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.debugging");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.debugging v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}

	[HarmonyPatch(typeof(Player), "FixedUpdate")]
	class HarmonyPatch_Player_FixedUpdate {

		const float CHECK_FREQUENCY = 1.0f;
		static float m_elapsed = 0f;

		private static void Postfix(
			ref Player __instance, 
			ref SEMan ___m_seman,
			ref PieceTable ___m_buildPieces
		) {
			if (__instance != Player.m_localPlayer) {
				return;
			}

			if ((m_elapsed += Time.fixedDeltaTime) < CHECK_FREQUENCY) {
				return;
			}
			m_elapsed = 0f;
			___m_seman.AddStatusEffect("Rested", resetTime: true);
			if (___m_buildPieces != null) {
				Piece piece;
				foreach (GameObject obj in ___m_buildPieces.m_pieces) {
					if (obj != null && (piece = obj.GetComponent<Piece>()) != null) {
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

	[HarmonyPatch(typeof(Player), "GetComfortLevel")]
	class HarmonyPatch_Player_GetComfortLevel {

		private static bool Prefix(ref Player __instance, ref int __result) {
			__result = 99;
			return false;
		}
	}

	[HarmonyPatch(typeof(Terminal), "Awake")]
	class HarmonyPatch_Terminal_InitTerminal {

		static bool created = false;

		private static void Postfix() {
			if (created) {
				return;
			}
			new Terminal.ConsoleCommand("blah", "probably breaks something", delegate (Terminal.ConsoleEventArgs args) {
				args.Context?.AddString("Hey something is working!");

			}, isCheat: false, isNetwork: false, onlyServer: false, isSecret: true);
		}
	}

	[HarmonyPatch(typeof(CreatureSpawner), "Spawn")]
	class HarmonyPatch_CreatureSpawner_Spawn {

		private static void Postfix(ref CreatureSpawner __instance, ZNetView __result) {
			Character character = __result.gameObject.GetComponent<Character>();
			logger.LogInfo("CreatureSpawner::Spawn() - minLevel: " + __instance.m_minLevel + ", maxLevel: " + __instance.m_maxLevel);
			logger.LogInfo("   --> name: " + character.m_name + ", level: " + character.GetLevel());
		}
	}
}