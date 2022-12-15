
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;


[BepInPlugin("devopsdinosaur.valheim.headlight", "Headlight", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.headlight");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	private static ConfigEntry<float> m_intensity;
	private static ConfigEntry<float> m_red;
	private static ConfigEntry<float> m_green;
	private static ConfigEntry<float> m_blue;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.headlight v0.0.1 loaded.");
		this.m_harmony.PatchAll();
		m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
		m_intensity = this.Config.Bind<float>("General", "Intensity", 1.5f, "Light intensity (float, 0.0f - 8.0f) [0 = No light, 8 = Archangel radiance].");
		m_red = this.Config.Bind<float>("General", "Red Value", 1.0f, "Red component of light (float, 0.0f - 1.0f).");
		m_green = this.Config.Bind<float>("General", "Green Value", 1.0f, "Green component of light (float, 0.0f - 1.0f).");
		m_blue = this.Config.Bind<float>("General", "Blue Value", 1.0f, "Blue component of light (float, 0.0f - 1.0f).");
	}

	[HarmonyPatch(typeof(Player), "FixedUpdate")]
	class HarmonyPatch_Player_FixedUpdate {

		static GameObject headlight = null;

		private static void Postfix(ref Player __instance, ref Transform ___m_head) {
			if (!m_enabled.Value) {
				return;
			}
			if (__instance != Player.m_localPlayer) {
				return;
			}
			if (headlight == null) {
				headlight = new GameObject("headlight");
				headlight.transform.parent = ___m_head.transform.parent;
				headlight.transform.position = GameCamera.instance.transform.position;
				Light light = headlight.AddComponent<Light>();
				light.intensity = m_intensity.Value;
				light.color = new Color(m_red.Value, m_green.Value, m_blue.Value);
				light.enabled = true;
			} else {
				headlight.transform.position = GameCamera.instance.transform.position;
			}
		}
	}
}