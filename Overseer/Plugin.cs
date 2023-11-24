using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using Overseer.Units;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace Overseer {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("pseudopulse.YAU")]
    public class Main : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "pseudopulse";
        public const string PluginName = "Overseer";
        public const string PluginVersion = "0.7.0";
        private const bool ShouldShowLog = false;

        [StubShaders]
        public static AssetBundle assets;
        public static BepInEx.Logging.ManualLogSource ModLogger;

        public static YAUContentPack contentPack;

        public void Awake() {
            // assetbundle loading 
            assets = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("Overseer.dll", "overseerbundle"));

            // set logger
            ModLogger = Logger;

            contentPack = ContentPackManager.CreateContentPack(Assembly.GetExecutingAssembly(), "Overseer");

            ContentScanner.ScanTypes<GenericBase>(Assembly.GetExecutingAssembly(), x => x.Initialize(contentPack, Config, "Overseer"));
            ConfigManager.HandleConfigAttributes(Assembly.GetExecutingAssembly(), Config);

            UnitCatalog.Initialize();
            States.Dash.CreatePrefabs();

            foreach (Material material in assets.LoadAllAssets<Material>()) {
                material.shader = Assets.Shader.HGStandard;
            }
        }

        public static void Log(string text) {
            if (!ShouldShowLog) {
                return;
            }

            Debug.Log(text);
        }
    }
}