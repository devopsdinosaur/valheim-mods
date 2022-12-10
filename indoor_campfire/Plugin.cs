
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;


[BepInPlugin("devopsdinosaur.valheim.indoor_campfire", "Indoor Campfire", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.indoor_campfire");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.indoor_campfire v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}
	
	[HarmonyPatch(typeof(Smoke), "Awake")]
    class HarmonyPatch_Smoke_Awake {

        private static void Postfix(ref Smoke __instance) {
			__instance.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}