using System;
using Overseer.Components;

namespace Overseer.States {
    public class Spectral : BaseSkillState {
        public static GameObject ProjectileStandard;
        public static GameObject ProjectileLast;
        public static int TotalShots = 3;
        public int ShotsFired = 0;

        public float DamageCoefficient => ShotsFired > 2 ? 1.6f : 0.7f;
        public GameObject ProjectilePrefab => ShotsFired > 2 ? ProjectileLast : ProjectileStandard;
        public float FireDelay = 0.2f;

        static Spectral() {
            ProjectileStandard = RuntimePrefabManager.CreatePrefab(Assets.GameObject.RoboBallProjectile, "OverseerPlasmaShot");

            ProjectileStandard.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1f;
            ProjectileStandard.GetComponent<ProjectileImpactExplosion>().blastRadius = 3f;

            ProjectileLast = RuntimePrefabManager.CreatePrefab(Assets.GameObject.RoboBallProjectile, "OverseerPlasmaShotEnd");
            ProjectileLast.AddComponent<MarkOnHit>();

            ProjectileLast.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1f;
            ProjectileLast.GetComponent<ProjectileImpactExplosion>().blastRadius = 3f;

            Main.contentPack.RegisterGameObject(ProjectileStandard);
            Main.contentPack.RegisterGameObject(ProjectileLast);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            FireDelay /= base.attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterBody.SetAimTimer(0.5f);

            if (base.fixedAge >= FireDelay) {
                base.fixedAge = 0f;
                Fire();
            }

            if (ShotsFired >= 3) {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public void Fire() {
            ShotsFired++;
            
            FireProjectileInfo info = new();
            info.damage = base.damageStat * DamageCoefficient;
            info.projectilePrefab = ProjectilePrefab;
            info.owner = base.gameObject;
            info.position = inputBank.aimOrigin;
            info.crit = base.RollCrit();
            info.rotation = Util.QuaternionSafeLookRotation(base.inputBank.aimDirection);
            info.useSpeedOverride = true;
            info.speedOverride = 200;

            ProjectileManager.instance.FireProjectile(info);

            // AkSoundEngine.PostEvent(WwiseEvents.Play_roboBall_attack1_shoot, base.gameObject);
        }

        public class MarkOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                if (impactInfo.collider) {
                    HurtBox box = impactInfo.collider.GetComponent<HurtBox>();

                    if (!box) {
                        return;
                    }

                    if (box.healthComponent.body.teamComponent.teamIndex != TeamIndex.Player) {
                        ProjectileController c = GetComponent<ProjectileController>();
                        OverseerController cont = c.owner.GetComponent<OverseerController>();
                        cont.Retarget(box);
                        box.healthComponent.gameObject.AddComponent<Marker>();
                    }
                }
            }
        }
    }
}