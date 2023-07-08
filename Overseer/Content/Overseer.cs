using System;
using BepInEx.Configuration;
using Overseer.Components;
using Overseer.Units;

namespace Overseer {
    [ConfigSection("Overseer")]
    public class Overseer : GenericBase<Overseer> {
        public SurvivorDef sdOverseer;
        public GameObject OverseerBody;
        public override void Initialize(YAUContentPack pack, ConfigFile config, string identifier)
        {
            contentPack = pack;

            sdOverseer = Main.assets.LoadAsset<SurvivorDef>("sdOverseer.asset");
            OverseerBody = Main.assets.LoadAsset<GameObject>("OverseerBody.prefab");

            contentPack.RegisterGameObject(OverseerBody);
            contentPack.RegisterScriptableObject(sdOverseer);

            On.RoR2.Projectile.ProjectileController.IgnoreCollisionsWithOwner += Ignore;
            On.RoR2.BulletAttack.Fire += AlliesDontEatShots;

            SetupLang();
        }

        private static void Ignore(On.RoR2.Projectile.ProjectileController.orig_IgnoreCollisionsWithOwner orig, ProjectileController self, bool ignore) {
            orig(self, ignore);
            if (!self.owner || !self.owner.GetComponent<CharacterBody>() || !self.owner.GetComponent<CharacterBody>().master) {
                return;
            }

            if (self.owner.GetComponent<OverseerController>()) {
                foreach (IUnitC unit in self.owner.GetComponent<OverseerController>().activeUnits) {
                    foreach (Collider col in self.myColliders) {
                        Physics.IgnoreCollision(unit.GetSelf().GetComponentInChildren<Collider>(), col, true);
                    }
                }
            }
        }

        private static void AlliesDontEatShots(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self) {
            if (self.filterCallback == BulletAttack.defaultFilterCallback) {
                self.filterCallback = delegate (BulletAttack attack, ref BulletAttack.BulletHit hit) {
                    if (hit.hitHurtBox && hit.hitHurtBox.healthComponent && hit.hitHurtBox.healthComponent.body.GetComponent<IUnitC>() != null) {
                        return false;
                    }
                    else {
                        return BulletAttack.DefaultFilterCallbackImplementation(attack, ref hit);
                    }
                };
            }
            orig(self);
        }

        private void SetupLang() {
            "OVERSEER_NAME".Add("Overseer");

            "OVERSEER_PASSIVE1_NAME".Add("Defense System");
            "OVERSEER_PASSIVE1_DESC".Add("A defensive shield absorbs heavy hits. Regenerates 50% slower when assembling a unit.");

            "OVERSEER_PASSIVE2_NAME".Add("Infernal Mechanics");
            "OVERSEER_PASSIVE2_DESC".Add("Units seek out targets on death, exploding for <style=cIsDamage>800%</style> damage.");

            "OVERSEER_PASSIVE3_NAME".Add("Swarm Commander");
            "OVERSEER_PASSIVE3_DESC".Add("Increase unit cap and unit assembly speed by 100%. Units are 50% weaker.");

            "OVERSEER_PRIMARY_NAME".Add("Execute");
            "OVERSEER_PRIMARY_DESC".Add("Issue a command to your units");

            "OVERSEER_SECONDARY_NAME".Add("Secondary Weapon");
            "OVERSEER_SECONDARY_DESC".Add("Order your units to trigger their alternate fire.");

            "OVERSEER_UTILITY_NAME".Add("Beam Transmission");
            "OVERSEER_UTILITY_DESC".Add("Perform a short intangible dash.");

            "OVERSEER_SPECIAL_NAME".Add("Reconfigure");
            "OVERSEER_SPECIAL_DESC".Add("Change your active production unit.");

            "OVERSEER_SENTINEL_NAME".Add("Unit: Sentinel");
            "OVERSEER_SENTINEL_DESC".Add("An agile unit that fires piercing lasers at targets. Alt fire launches a homing missile.");

            "OVERSEER_MENDER_NAME".Add("Unit: Mender");
            "OVERSEER_MENDER_DESC".Add("A sturdy unit that repairs you and nearby units. Alt fire releases a pulse of barrier.");

            "OVERSEER_BEACON_NAME".Add("Unit: Beacon");
            "OVERSEER_BEACON_DESC".Add("A fragile unit that overclocks you and nearby units, increasing their speed. Alt fire does ???");
        }
    }
}