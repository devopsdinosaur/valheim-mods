
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;


[BepInPlugin("devopsdinosaur.valheim.lights_camera_action", "Lights, Camera, Action", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.lights_camera_action");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	private static ConfigEntry<float> m_intensity;
	private static ConfigEntry<float> m_intensity_delta;
	private static ConfigEntry<float> m_red;
	private static ConfigEntry<float> m_green;
	private static ConfigEntry<float> m_blue;
	
	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.lights_camera_action v0.0.1 loaded.");
		this.m_harmony.PatchAll();
		m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
		m_intensity = this.Config.Bind<float>("General", "Starting Intensity", 2.0f, "Light intensity (float, 0.0f - 8.0f) [0 = No light, 8 = Archangel radiance].");
		m_intensity_delta = this.Config.Bind<float>("General", "Intensity Delta", 0.1f, "Change in light intensity with each CTRL+ScrollWheel tick (float).");
		m_red = this.Config.Bind<float>("General", "Red Value", 1.0f, "Red component of light (float, 0.0f - 1.0f).");
		m_green = this.Config.Bind<float>("General", "Green Value", 1.0f, "Green component of light (float, 0.0f - 1.0f).");
		m_blue = this.Config.Bind<float>("General", "Blue Value", 1.0f, "Blue component of light (float, 0.0f - 1.0f).");
	}

	[HarmonyPatch(typeof(Player), "FixedUpdate")]
	class HarmonyPatch_Player_FixedUpdate {

		static GameObject light_obj = null;
		static Light light = null;
		
		private static void Postfix(ref Player __instance, ref Transform ___m_head) {
			if (!m_enabled.Value) {
				return;
			}
			if (__instance != Player.m_localPlayer) {
				return;
			}
			if (light_obj == null) {
				light_obj = new GameObject("light_obj");
				light_obj.transform.parent = ___m_head.transform.parent;
				light = light_obj.AddComponent<Light>();
				light.intensity = m_intensity.Value;
				light.color = new Color(m_red.Value, m_green.Value, m_blue.Value);
				light.enabled = true;
			}
			light_obj.transform.position = GameCamera.instance.transform.position;
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
				if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) {
					light.intensity += m_intensity_delta.Value;
				} else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
					light.intensity -= m_intensity_delta.Value;
				}
				if (light.intensity <= 0) {
					light.intensity = 0f;
				} else if (light.intensity > 8f) {
					light.intensity = 8f;
				}
			}
		}
	}
}