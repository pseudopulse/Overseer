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
        public const string PluginAuthor = "ModAuthorName";
        public const string PluginName = "Overseer";
        public const string PluginVersion = "1.0.0";

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
        }
    }
}