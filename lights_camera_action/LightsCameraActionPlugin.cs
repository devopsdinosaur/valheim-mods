
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;


[BepInPlugin("devopsdinosaur.valheim.lights_camera_action", "Lights, Camera, Action", "0.0.3")]
public class LightsCameraActionPlugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.lights_camera_action");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	private static ConfigEntry<string> m_hotkey_modifier;
	private static ConfigEntry<string> m_hotkey_onoff_toggle;
	private static ConfigEntry<string> m_hotkey_intensity_up;
	private static ConfigEntry<string> m_hotkey_intensity_down;
	private static ConfigEntry<string> m_hotkey_move_up;
	private static ConfigEntry<string> m_hotkey_move_down;
	private static ConfigEntry<string> m_hotkey_move_forward;
	private static ConfigEntry<string> m_hotkey_move_back;
	private static ConfigEntry<float> m_intensity;
	private static ConfigEntry<float> m_intensity_delta;
	private static ConfigEntry<float> m_height;
	private static ConfigEntry<float> m_height_delta;
	private static ConfigEntry<float> m_depth;
	private static ConfigEntry<float> m_depth_delta;
	private static ConfigEntry<float> m_red;
	private static ConfigEntry<float> m_green;
	private static ConfigEntry<float> m_blue;

	private const int HOTKEY_MODIFIER = 0;
	private const int HOTKEY_ONOFF_TOGGLE = 1;
	private const int HOTKEY_INTENSITY_UP = 2;
	private const int HOTKEY_INTENSITY_DOWN = 3;
	private const int HOTKEY_MOVE_UP = 4;
	private const int HOTKEY_MOVE_DOWN = 5;
	private const int HOTKEY_MOVE_FORWARD = 6;
	private const int HOTKEY_MOVE_BACK = 7;
	private static Dictionary<int, List<KeyCode>> m_hotkeys = null;

	private void Awake() {
		logger = this.Logger;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
			m_hotkey_modifier = this.Config.Bind<string>("General", "Hotkey Modifier", "LeftControl,RightControl", "Comma-separated list of Unity Keycodes used as the special modifier key (i.e. ctrl,alt,command) one of which is required to be down for hotkeys to work.  Set to '' (blank string) to not require a special key (not recommended).  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_onoff_toggle = this.Config.Bind<string>("General", "On/Off Toggle Hotkey", "Alpha0,Keypad0", "Comma-separated list of Unity Keycodes, any of which will toggle the light on/off.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_intensity_up = this.Config.Bind<string>("General", "Intensity Up Hotkey", "Equals,KeypadPlus", "Comma-separated list of Unity Keycodes, any of which will increase the light intensity.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_intensity_down = this.Config.Bind<string>("General", "Intensity Down Hotkey", "Minus,KeypadMinus", "Comma-separated list of Unity Keycodes, any of which will decrease the light intensity.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_intensity = this.Config.Bind<float>("General", "Starting Intensity", 2.0f, "Light intensity (float, 0.0f - 8.0f) [0 = No light, 8 = Archangel radiance].");
			m_intensity_delta = this.Config.Bind<float>("General", "Intensity Delta", 0.1f, "Change in light intensity with each up/down hotkey tick (float).");
			m_hotkey_move_up = this.Config.Bind<string>("General", "Move Light Up Hotkey", "PageUp", "Comma-separated list of Unity Keycodes, any of which will increase the light height above character's head.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_move_down = this.Config.Bind<string>("General", "Move Light Down Hotkey", "PageDown", "Comma-separated list of Unity Keycodes, any of which will decrease the light height above character's head.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_move_forward = this.Config.Bind<string>("General", "Move Light Forward Hotkey", "Home", "Comma-separated list of Unity Keycodes, any of which will move the light in the direction in which the character's head is facing.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_move_back = this.Config.Bind<string>("General", "Move Light Back Hotkey", "End", "Comma-separated list of Unity Keycodes, any of which will move the light in the opposite direction from which the character's head is facing.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_height = this.Config.Bind<float>("General", "Starting Height", 5.0f, "Light height (in meters) above player's head (float, negative == below head [i.e. in the body], positive == above the head).");
			m_height_delta = this.Config.Bind<float>("General", "Height Delta", 0.1f, "Change in light height (in meters) with each up/down hotkey tick (float).");
			m_depth = this.Config.Bind<float>("General", "Starting Depth", 0f, "Light depth (in meters) in front of/behind player's head (float, zero == directly above head, negative == behind head, positive == in front of head).");
			m_depth_delta = this.Config.Bind<float>("General", "Depth Delta", 0.1f, "Change in light depth (in meters) with each up/down hotkey tick (float).");
			m_red = this.Config.Bind<float>("General", "Red Value", 1.0f, "Red component of light (float, 0.0f - 1.0f).");
			m_green = this.Config.Bind<float>("General", "Green Value", 1.0f, "Green component of light (float, 0.0f - 1.0f).");
			m_blue = this.Config.Bind<float>("General", "Blue Value", 1.0f, "Blue component of light (float, 0.0f - 1.0f).");
			m_hotkeys = new Dictionary<int, List<KeyCode>>();
			set_hotkey(m_hotkey_modifier.Value, HOTKEY_MODIFIER);
			set_hotkey(m_hotkey_onoff_toggle.Value, HOTKEY_ONOFF_TOGGLE);
			set_hotkey(m_hotkey_intensity_up.Value, HOTKEY_INTENSITY_UP);
			set_hotkey(m_hotkey_intensity_down.Value, HOTKEY_INTENSITY_DOWN);
			set_hotkey(m_hotkey_move_up.Value, HOTKEY_MOVE_UP);
			set_hotkey(m_hotkey_move_down.Value, HOTKEY_MOVE_DOWN);
			set_hotkey(m_hotkey_move_forward.Value, HOTKEY_MOVE_FORWARD);
			set_hotkey(m_hotkey_move_back.Value, HOTKEY_MOVE_BACK);
			if (m_enabled.Value) {
				this.m_harmony.PatchAll();
			}
			logger.LogInfo("devopsdinosaur.valheim.lights_camera_action v0.0.3" + (m_enabled.Value ? "" : " [inactive; disabled in config]") + " loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e.StackTrace);
		}
	}

	private static void set_hotkey(string keys_string, int key_index) {
		m_hotkeys[key_index] = new List<KeyCode>();
		foreach (string key in keys_string.Split(',')) {
			string trimmed_key = key.Trim();
			if (trimmed_key != "") {
				m_hotkeys[key_index].Add((KeyCode) System.Enum.Parse(typeof(KeyCode), trimmed_key));
			}
		}
	}

	private static bool is_modifier_hotkey_down() {
		if (m_hotkeys[HOTKEY_MODIFIER].Count == 0) {
			return true;
		}
		foreach (KeyCode key in m_hotkeys[HOTKEY_MODIFIER]) {
			if (Input.GetKey(key)) {
				return true;
			}
		}
		return false;
	}

	private static bool is_hotkey_down(int key_index) {
		foreach (KeyCode key in m_hotkeys[key_index]) {
			if (Input.GetKeyDown(key)) {
				return true;
			}
		}
		return false;
	}

	private static void notify(string message) {
		logger.LogInfo(message);
		MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, message);
	}

	[HarmonyPatch(typeof(Player), "Update")]
	class HarmonyPatch_Player_Update {

		static GameObject light_obj = null;
		static Light light = null;
		
		private static void Postfix(Player __instance, Transform ___m_head) {
			try {
				if (!m_enabled.Value) {
					return;
				}
				if (__instance != Player.m_localPlayer) {
					return;
				}
				if (light_obj == null) {
					light_obj = new GameObject("lights_camera_action_plugin.light_obj");
					light_obj.transform.parent = ___m_head.transform.parent;
					light = light_obj.AddComponent<Light>();
					light.intensity = m_intensity.Value;
					light.color = new Color(m_red.Value, m_green.Value, m_blue.Value);
					light.enabled = true;
				}
				light_obj.transform.position = ___m_head.transform.position + (Vector3.up * m_height.Value) + (Vector3.forward * m_depth.Value);
				if (!is_modifier_hotkey_down()) {
					return;
				}
				bool changed = false;
				if (is_hotkey_down(HOTKEY_ONOFF_TOGGLE)) {
					light.enabled = !light.enabled;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_INTENSITY_UP)) {
					light.intensity += m_intensity_delta.Value;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_INTENSITY_DOWN)) {
					light.intensity -= m_intensity_delta.Value;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_MOVE_UP)) {
					m_height.Value += m_height_delta.Value;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_MOVE_DOWN)) {
					m_height.Value -= m_height_delta.Value;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_MOVE_FORWARD)) {
					m_depth.Value += m_depth_delta.Value;
					changed = true;
				} else if (is_hotkey_down(HOTKEY_MOVE_BACK)) {
					m_depth.Value -= m_depth_delta.Value;
					changed = true;
				}
				if (light.intensity <= 0) {
					light.intensity = 0f;
				} else if (light.intensity > 8f) {
					light.intensity = 8f;
				}
				if (changed) {
					notify("Headlight is " + 
						(light.enabled ? "ON" : "OFF") + 
						" [height: " + m_height.Value + "," + 
						" depth: " + m_depth.Value + "," +
						" intensity: " + light.intensity + "]."
					);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_Player_Update.Postfix ERROR - " + e.StackTrace);
			}
		}
	}
}