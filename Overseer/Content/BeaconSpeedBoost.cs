using System;
using BepInEx.Configuration;

namespace Overseer.Content {
    public class BeaconSpeedBoost : GenericBase<BeaconSpeedBoost> {
        public static BuffDef BeaconSpeed;
        public override void Initialize(YAUContentPack pack, ConfigFile config, string identifier)
        {
            BeaconSpeed = Main.assets.LoadAsset<BuffDef>("bdBeaconSpeed.asset");

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) => {
                orig(self);

                if (NetworkServer.active && self.HasBuff(BeaconSpeed)) {
                    float increase = 1f + (0.05f * (self.GetBuffCount(BeaconSpeed)));
                    self.moveSpeed *= increase;
                }
            };

            pack.RegisterScriptableObject(BeaconSpeed);
        }
    }
}