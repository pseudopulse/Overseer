using System;
using BepInEx.Configuration;
using Overseer.Components;
using Overseer.Units;
using Overseer.Units.Defense;

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
            
            OverseerBody.GetComponent<CameraTargetParams>().cameraParams = Assets.CharacterCameraParams.ccpStandard;

            contentPack.RegisterGameObject(OverseerBody);
            contentPack.RegisterScriptableObject(sdOverseer);

            On.RoR2.Projectile.ProjectileController.IgnoreCollisionsWithOwner += Ignore;
            // On.RoR2.BulletAttack.Fire += AlliesDontEatShots;

            SetupLang();

            OverlayManager.AddOverlay(Assets.Material.matPulverizedOverlay, x => x.body && x.body.GetComponent<Marker>());

            On.RoR2.HealthComponent.TakeDamage += WardenAbsorb;
            On.RoR2.HealthComponent.TakeDamage += MarkedDamage;
        }

        private static void MarkedDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info) {
            if (self.GetComponent<Marker>()) {
                info.damage *= 1.25f;
            }

            orig(self, info);
        }

        private static void WardenAbsorb(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo info) {
            if (!self.GetComponent<OverseerController>()) {
                orig(self, info);
                return;
            }

            OverseerController controller = self.GetComponent<OverseerController>();

            List<WardenController> wardens = new();

            foreach (IUnitC unit in controller.activeUnits) {
                if (unit.GetSelf().GetComponent<WardenController>()) {
                    wardens.Add(unit.GetSelf().GetComponent<WardenController>());
                }
            }

            if (wardens.Count() > 0) {
                float absorption = info.damage * 0.4f;
                info.damage *= 0.6f;

                float perUnit = absorption / wardens.Count();

                foreach (WardenController warden in wardens) {
                    warden.body.healthComponent.TakeDamage(new DamageInfo() {
                        damage = perUnit,
                        position = warden.transform.position,
                        damageColorIndex = DamageColorIndex.Bleed
                    });
                }
            }

            orig(self, info);
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

            "OVERSEER_PASSIVE2_NAME".Add("Vengeance Protocol");
            "OVERSEER_PASSIVE2_DESC".Add("Units <style=cIsUtility>seek out</style> targets on death, exploding for <style=cIsDamage>800%</style> damage.");

            "OVERSEER_PASSIVE3_NAME".Add("Swarm Commander");
            "OVERSEER_PASSIVE3_DESC".Add("Increase unit cap and unit assembly speed by <style=cIsDamage>200%</style>. Units are <style=cDeath>half as effective</style>.");

            "OVERSEER_PRIMARY_NAME".Add("Spectral Cascade");
            "OVERSEER_PRIMARY_DESC".Add("Fire a volley of 3 <style=cIsUtility>plasma blasts</style> for <style=cIsDamage>70% damage</style>. The last shot deals <style=cIsDamage>160%</style> and <style=cDeath>Marks</style> targets.");

            "OVERSEER_SECONDARY_NAME".Add("Discharge");
            "OVERSEER_SECONDARY_DESC".Add("<style=cIsUtility>Shocking</style>. Launch a burst of plasma for <style=cIsDamage>400% damage</style>. Units struck <style=cDeath>detonate</style> in an <style=cIsUtility>electrifying blast</style>.");

            "OVERSEER_UTILITY_NAME".Add("Beam Transmission");
            "OVERSEER_UTILITY_DESC".Add("Perform a short <style=cIsUtility>intangible</style> dash.");

            "OVERSEER_SPECIAL_NAME".Add("Reconfigure");
            "OVERSEER_SPECIAL_DESC".Add("Switch to <style=cIsUtility>producing</style> the next type of <style=cIsDamage>unit</style>.");

            "OVERSEER_SENTINEL_NAME".Add("Unit: Sentinel");
            "OVERSEER_SENTINEL_DESC".Add("An agile unit that fires piercing lasers for <style=cIsDamage>50% damage</style> and launches homing missiles for <style=cIsDamage>200% damage</style>.");

            "OVERSEER_MENDER_NAME".Add("Unit: Mender");
            "OVERSEER_MENDER_DESC".Add("A sturdy unit that <style=cIsHealing>repairs</style> you and nearby units.");

            "OVERSEER_BEACON_NAME".Add("Unit: Beacon");
            "OVERSEER_BEACON_DESC".Add("A fragile unit that overclocks you, <style=cIsUtility>increasing your speed</style>.");

            "OVERSEER_SWARMER_NAME".Add("Unit: Swarmer");
            "OVERSEER_SWARMER_DESC".Add("A fragile unit that performs quick ram attacks at targets for <style=cIsDamage>100% damage per second</style>.");

            "OVERSEER_WARDEN_NAME".Add("Unit: Warden");
            "OVERSEER_WARDEN_DESC".Add("A sturdy unit that redirects <style=cIsUtility>40%</style> of the damage taken by you and nearby units to itself.");
        }
    }
}