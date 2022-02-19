using BepInEx;
using System.ComponentModel;
using UnityEngine;

using Utilla;

namespace PowerArms
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [ModdedGamemode]
    [Description("HauntedModMenu")]
    public class PowerArmsPlugin : BaseUnityPlugin
    {
        private PowerArmsManager modInstance = null;
        private bool modEnabled = true;

        void Awake()
        {
            // HarmonyPatches.ApplyHarmonyPatches();
        }

        private void OnEnable()
        {
            modEnabled = true;
            if (modInstance != null)
                modInstance.enabled = modEnabled;
        }

        private void OnDisable()
        {
            modEnabled = false;
            if (modInstance != null)
                modInstance.enabled = modEnabled;
        }

        [ModdedGamemodeJoin]
        private void Load(string gamemode)
        {
            // Debug.Log("spawning PowerArmsManager");
            // Debug.Log("PowerArms: mod enabled = " + modEnabled);
            modInstance = gameObject.AddComponent<PowerArmsManager>();
            modInstance.enabled = modEnabled;
        }

        [ModdedGamemodeLeave]
        private void UnLoad(string gamemode)
        {
            GameObject.Destroy(modInstance);
        }
    }
}
