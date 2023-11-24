using System;
using BepInEx.Configuration;

namespace Overseer.Content {
    public class WardenRes : GenericBase<BeaconSpeedBoost> {
        public static BuffDef WardenResBuff;
        public override void Initialize(YAUContentPack pack, ConfigFile config, string identifier)
        {
            WardenResBuff = Main.assets.LoadAsset<BuffDef>("bdWardenRes.asset");

            On.RoR2.HealthComponent.TakeDamage += (orig, self, info) => {
                if (self.body.HasBuff(WardenResBuff)) {
                    float mult = 1f - (0.02f * self.body.GetBuffCount(WardenResBuff));
                    info.damage *= mult;
                }

                orig(self, info);
            };

            pack.RegisterScriptableObject(WardenResBuff);
        }
    }
}