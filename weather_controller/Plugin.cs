
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;


[BepInPlugin("devopsdinosaur.valheim.weather_controller", "Weather Controller", "0.0.1")]
public class Plugin : BaseUnityPlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.valheim.weather_controller");
	public static ManualLogSource logger;

	public Plugin() {
	}

	private void Awake() {
		Plugin.logger = this.Logger;
		logger.LogInfo((object) "devopsdinosaur.valheim.weather_controller v0.0.1 loaded.");
		this.m_harmony.PatchAll();
	}

	private void Start() {
	}

    [HarmonyPatch(typeof(EnvMan), "SetEnv")]
    class HarmonyPatch_EnvMan_SetEnv {

        private static bool Prefix(ref EnvSetup env) {
			env.m_fogDensityMorning = 0f;
            env.m_fogDensityDay = 0f;
            env.m_fogDensityEvening = 0f;
            env.m_fogDensityNight = 0f;
			env.m_isColdAtNight = false;
			return true;
        }
    }

}