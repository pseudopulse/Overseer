using System.Diagnostics.Tracing;
using System;
using RoR2.ConVar;

namespace Overseer.Units.Offense {
    public class SentinelController : UnitController
    {
        public override HurtBox Target { get; set; }
        private bool isFreeform = true;
        private float stopwatch = 0f;

        public override void OverrideTarget(HurtBox newTarget)
        {
            Target = newTarget;
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            if (isFreeform) {
                FreeformMovement();
            }
            else {
                float angle = 0f;
                float offset = 270f;
                float dist = (thisUnit >= (totalUnits / 2)) ? (-(thisUnit - (totalUnits / 2)) - 1) : (thisUnit + 1);
                OrbitalMovement(totalUnits, thisUnit, angle, offset, dist);
            }
        }

        public override void PerformSecondaryAction()
        {
            FireProjectileInfo info = new();
            info.crit = Util.CheckRoll(owner.body.crit);
            info.damage = owner.body.damage * 5f;
            info.owner = owner.gameObject;
            info.position = base.transform.position;
            info.rotation = Util.QuaternionSafeLookRotation((isFreeform && Target ? Target.transform.position - base.transform.position : owner.AimPoint - base.transform.position).normalized);
            info.projectilePrefab = Assets.GameObject.MissileProjectile;

            ProjectileManager.instance.FireProjectile(info);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            if (Target && !Target.isActiveAndEnabled) {
                Target = null;
            }
            
            if (stopwatch >= 1f) {
                
                if (!Target) {
                    Target = FindNearbyEnemy();
                }

                stopwatch = 0f;

                if (Target && isFreeform) {
                    FireLaser();
                }
                else if (!isFreeform) {
                    FireLaserFlank();
                }
            }
        }

        public void FireLaser() {
            BulletAttack attack = new();
            attack.origin = base.transform.position;
            attack.owner = owner.gameObject;
            attack.weapon = base.gameObject;
            attack.aimVector = (Target.transform.position - base.transform.position).normalized;
            attack.damage = owner.body.damage * owner.UnitEffectModifier;
            attack.isCrit = Util.CheckRoll(owner.body.crit);
            attack.minSpread = 0f;
            attack.maxSpread = 1.5f;
            attack.procCoefficient = 1f;
            attack.damageColorIndex = DamageColorIndex.Item;
            attack.tracerEffectPrefab = Assets.GameObject.TracerCaptainDefenseMatrix;
            attack.Fire();

            AkSoundEngine.PostEvent(WwiseEvents.Play_captain_drone_zap, base.gameObject);
        }

        public void FireLaserFlank() {
            BulletAttack attack = new();
            attack.origin = base.transform.position;
            attack.owner = owner.gameObject;
            attack.weapon = base.gameObject;
            attack.aimVector = (owner.AimPoint - base.transform.position).normalized;
            attack.damage = owner.body.damage * owner.UnitEffectModifier;
            attack.isCrit = Util.CheckRoll(owner.body.crit);
            attack.minSpread = 0f;
            attack.maxSpread = 0f;
            attack.procCoefficient = 1f;
            attack.damageColorIndex = DamageColorIndex.Item;
            attack.tracerEffectPrefab = Assets.GameObject.TracerCaptainDefenseMatrix;
            attack.Fire();

            AkSoundEngine.PostEvent(WwiseEvents.Play_captain_drone_zap, base.gameObject);
        }

        public override void Recall()
        {
            isFreeform = !isFreeform;
        }
    }
}