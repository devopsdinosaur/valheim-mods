
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;


[BepInPlugin("devopsdinosaur.valheim.no_death_animations", "No Death Animations", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.no_death_animations");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.no_death_animations v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}

	[HarmonyPatch(typeof(Ragdoll), "Setup")]
	class HarmonyPatch_Ragdoll_Setup {

		private static void Postfix(ref Ragdoll __instance) {
			__instance.
				GetType().
				GetTypeInfo().
				GetDeclaredMethod("DestroyNow").
				Invoke(__instance, new object[] { });
		}
	}

}