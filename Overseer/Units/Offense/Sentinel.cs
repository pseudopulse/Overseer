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
            if (Target && isFreeform && Vector3.Distance(Target.transform.position, base.transform.position) < 5f) {
                return;
            }

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
            if (!isFreeform && !Target) {
                return;
            }

            FireProjectileInfo info = new();
            info.crit = Util.CheckRoll(owner.body.crit);
            info.damage = owner.body.damage * 2f;
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

            if (Target) {
                base.transform.forward = (Target.transform.position - base.transform.position).normalized;
            }
            if (!isFreeform) {
                base.transform.forward = (owner.AimPoint - base.transform.position).normalized;
            }
            
            if (isFreeform && !Target) {
                base.transform.forward = (owner.body.corePosition - base.transform.position).normalized;
            }
            
        }

        public void FireLaser() {
            BulletAttack attack = new();
            attack.origin = base.transform.position;
            attack.owner = owner.gameObject;
            attack.weapon = base.gameObject;
            attack.aimVector = (Target.transform.position - base.transform.position).normalized;
            attack.damage = owner.body.damage * owner.UnitEffectModifier * 0.5f;
            attack.isCrit = Util.CheckRoll(owner.body.crit);
            attack.minSpread = 0f;
            attack.maxSpread = 3f;
            attack.procCoefficient = 1f;
            attack.damageColorIndex = DamageColorIndex.Item;
            attack.tracerEffectPrefab = Assets.GameObject.TracerCaptainDefenseMatrix;
            attack.Fire();

            BaseAI ai = Target.healthComponent.body.master?.GetComponent<BaseAI>() ?? null;

            if (ai) {
                ai.currentEnemy.gameObject = base.gameObject;
            }

            AkSoundEngine.PostEvent(WwiseEvents.Play_captain_drone_zap, base.gameObject);
        }

        public void FireLaserFlank() {
            BulletAttack attack = new();
            attack.origin = base.transform.position;
            attack.owner = owner.gameObject;
            attack.weapon = base.gameObject;
            attack.aimVector = GetFlankLaserAimDir();
            attack.damage = owner.body.damage * owner.UnitEffectModifier * 1f;
            attack.isCrit = Util.CheckRoll(owner.body.crit);
            attack.minSpread = 0f;
            attack.maxSpread = 0f;
            attack.procCoefficient = 1f;
            attack.damageColorIndex = DamageColorIndex.Item;
            attack.tracerEffectPrefab = Assets.GameObject.TracerCaptainDefenseMatrix;
            attack.stopperMask = LayerIndex.world.collisionMask;
            attack.Fire();

            AkSoundEngine.PostEvent(WwiseEvents.Play_captain_drone_zap, base.gameObject);
        }

        public Vector3 GetFlankLaserAimDir() {
            Ray aimRay = owner.body.inputBank.GetAimRay();

            if (Physics.Raycast(aimRay, out RaycastHit info, 4000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore)) {
                Vector3 dir = (info.point - base.transform.position).normalized;
                return dir;
            }
            
            return aimRay.direction;
        }

        public override void Recall()
        {
            isFreeform = !isFreeform;
        }
    }
}