
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System;


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
			Vector3 position = (Vector3) __instance.
				GetType().
				GetTypeInfo().
				GetDeclaredMethod("ScreenToWorldPoint").
				Invoke(__instance, new object[] {Input.mousePosition});
			Chat.instance.SendPing(position);
			if (Input.GetKey(KeyCode.LeftControl)) {
				Vector3 vector = new Vector3(position.x, Player.m_localPlayer.transform.position.y, position.z);
				Heightmap.GetHeight(vector, out var height);
				vector.y = Math.Max(0f, height);
				Player.m_localPlayer.TeleportTo(vector, Player.m_localPlayer.transform.rotation, distantTeleport: true);
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(Player), "TeleportTo")]
	class HarmonyPatch_Player_TeleportTo {

		private static void Postfix(ref float ___m_teleportTimer, ref bool ___m_distantTeleport) {
			___m_teleportTimer = 8f;
			___m_distantTeleport = false;
		}
	}

}