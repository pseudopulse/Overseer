using System;
using System.Diagnostics.Tracing;
using EntityStates.Missions.Arena.NullWard;
using Overseer.Units;

namespace Overseer.States {
    public class Discharge : BaseSkillState {
        public float DamageCoefficient = 4f;

        public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.SetAimTimer(0.1f);

            BulletAttack attack = new();
            attack.damage = base.damageStat * DamageCoefficient;
            attack.damageType = DamageType.Shock5s;
            attack.aimVector = base.GetAimRay().direction;
            attack.isCrit = base.RollCrit();
            attack.origin = inputBank.aimOrigin;
            attack.procCoefficient = 1f;
            attack.owner = base.gameObject;
            attack.tracerEffectPrefab = Assets.GameObject.TracerMageLightningLaser;
            attack.radius = 0.3f;
            attack.hitCallback = (BulletAttack attack, ref BulletAttack.BulletHit hitInfo) => {
                if (hitInfo.hitHurtBox) {
                    Debug.Log("we hit something");
                    UnitController controller = hitInfo.hitHurtBox.GetComponentInParent<UnitController>();

                    if (controller) {
                        Debug.Log("we hit a unit");
                        BlastAttack battack = new();
                        battack.attacker = base.gameObject;
                        battack.baseDamage = base.damageStat * 4f;
                        battack.radius = 10f;
                        battack.crit = false;
                        battack.falloffModel = BlastAttack.FalloffModel.None;
                        battack.damageType = DamageType.Shock5s;
                        battack.position = controller.transform.position;
                        battack.teamIndex = TeamIndex.Player;
                        battack.procCoefficient = 1f;
                        EffectManager.SpawnEffect(Assets.GameObject.CaptainTazerSupplyDropNova, new EffectData() {
                            origin = controller.transform.position,
                            scale = 10f
                        }, false);
                        battack.Fire();
                        controller.owner.RequestDestruction(controller, true);
                    }
                }

                return BulletAttack.DefaultHitCallbackImplementation(attack, ref hitInfo);
            };

            attack.Fire();
            AkSoundEngine.PostEvent(WwiseEvents.Play_roboBall_death_small_explo, base.gameObject);

            outer.SetNextStateToMain();
        }
    }
}