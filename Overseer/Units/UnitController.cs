using System;
using Overseer.Components;
using Overseer.Units;

namespace Overseer.Units {
    public interface IUnitC {
        HurtBox Target { get; set; }
        Unit UnitType { get; set; }
        HurtboxTracker.TargetType TargetType { get; set; }

        void OverrideTarget(HurtBox newTarget);
        void PerformMovement(int totalUnits, int thisUnit);
        void Recall();
        void PerformSecondaryAction();
        void Initialize(OverseerController owner);
        void Destruct(bool ignoreVengeance = false);
        GameObject GetSelf();
    }

    public abstract class UnitController : MonoBehaviour, IUnitC
    {
        public abstract HurtBox Target { get; set; }
        public Unit type;
        public Unit UnitType { get => type; set => type = value; }

        public virtual HurtboxTracker.TargetType TargetType { get; set; } = HurtboxTracker.TargetType.Enemy;

        public abstract void OverrideTarget(HurtBox newTarget);
        public abstract void PerformMovement(int totalUnits, int thisUnit);
        public abstract void PerformSecondaryAction();
        public abstract void Recall();

        public virtual float OrbitDistance { get; } = 3f;
        public virtual float FreeformDistance { get; } = 3f;
        public virtual bool OrbitalPivotIsTarget { get; } = false;

        private Vector3 freeformTargetPos;
        private float freeformUpdateStopwatch;

        public CharacterBody body;
        public OverseerController owner;

        private bool hasRequestedDestruction = false;
        private HurtBox destructionTarget;
        private bool isDoingInfernalDestruction = false;
        private float altFireStopwatch = 0f;
        protected bool isOvercharged = false;
        private float overchargeTimer = 0f;

        public GameObject GetSelf() {
            return this.gameObject;
        }
        public void Initialize(OverseerController owner) {
            this.owner = owner;
            body = GetComponent<CharacterBody>();
        }

        public void Overcharge() {
            
        }

        public void Destruct(bool ignoreVengenace = false) {
            if (owner.isPassiveInfernal && !ignoreVengenace) {
                isDoingInfernalDestruction = true;
                
                destructionTarget = FindNearbyEnemy();
                if (destructionTarget) {
                    freeformTargetPos = destructionTarget.transform.position;
                    return;
                }
            }
            EffectManager.SpawnEffect(Assets.GameObject.ExplosionDroneDeath, new EffectData {
                origin = base.transform.position
            }, true);
            Destroy(this.gameObject);
        }

        public void OrbitalMovement(int totalUnits, int thisUnit, float fullAngle, float offset) {
            if (isDoingInfernalDestruction) {
                return;
            }
            Vector3 targetPos = ((OrbitalPivotIsTarget ? Target.transform.position : owner.body.corePosition) + Vector3.up) + Quaternion.AngleAxis(fullAngle / totalUnits * thisUnit + offset, Vector3.up) * owner.body.characterDirection.forward * OrbitDistance;
            Vector3 outPos = Vector3.Lerp(base.transform.position, targetPos, 20 * Time.fixedDeltaTime);
            base.transform.position = outPos;
        }

        public void OrbitalMovement(int totalUnits, int thisUnit, float fullAngle, float offset, float dist) {
            if (isDoingInfernalDestruction) {
                return;
            }
            Vector3 targetPos = ((OrbitalPivotIsTarget ? Target.transform.position : owner.body.corePosition) + Vector3.up) + Quaternion.AngleAxis(fullAngle / totalUnits * thisUnit + offset, Vector3.up) * owner.body.characterDirection.forward * dist;
            Vector3 outPos = Vector3.Lerp(base.transform.position, targetPos, 20 * Time.fixedDeltaTime);
            base.transform.position = outPos;
        }

        public void OrbitalMovement(int totalUnits, int thisUnit, float fullAngle, float offset, float dist, float vertoffset) {
            if (isDoingInfernalDestruction) {
                return;
            }
            Vector3 targetPos = ((OrbitalPivotIsTarget ? Target.transform.position : owner.body.corePosition) + (Vector3.up * vertoffset)) + Quaternion.AngleAxis(fullAngle / totalUnits * thisUnit + offset, Vector3.up) * owner.body.characterDirection.forward * dist;
            Vector3 outPos = Vector3.Lerp(base.transform.position, targetPos, 20 * Time.fixedDeltaTime);
            base.transform.position = outPos;
        }

        public void FreeformMovement() {
            Vector3 pos = Vector3.Lerp(base.transform.position, freeformTargetPos, 2f * Time.fixedDeltaTime);
            base.transform.position = pos;
        }

        public virtual void FixedUpdate() {
            freeformUpdateStopwatch -= Time.fixedDeltaTime;

            if (isOvercharged) {
                overchargeTimer -= Time.fixedDeltaTime;

                if (overchargeTimer <= 0f) {
                    isOvercharged = false;
                }
            }

            altFireStopwatch -= Time.fixedDeltaTime;
            if (altFireStopwatch <= 0f) {
                altFireStopwatch = Random.Range(3f, 6f);
                PerformSecondaryAction();
            }

            if (isDoingInfernalDestruction) {

                if (destructionTarget) {
                    freeformTargetPos = destructionTarget.transform.position;
                }

                FreeformMovement();

                if (Vector3.Distance(body.corePosition, freeformTargetPos) < 3f) {
                    BlastAttack attack = new();
                    attack.baseDamage = owner.body.damage * 8f;
                    attack.radius = 5f;
                    attack.crit = false;
                    attack.damageColorIndex = DamageColorIndex.Item;
                    attack.falloffModel = BlastAttack.FalloffModel.None;
                    attack.position = body.corePosition;
                    attack.procChainMask = new();
                    attack.procCoefficient = 1f;
                    attack.attacker = owner.gameObject;
                    attack.teamIndex = TeamIndex.Player;
                    attack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                    attack.Fire();
                    
                    EffectManager.SpawnEffect(Assets.GameObject.OmniExplosionVFX, new EffectData {
                        origin = body.corePosition,
                        scale = 5f
                    }, true);

                    AkSoundEngine.PostEvent(WwiseEvents.Play_wGiantJellyExplosion, base.gameObject);

                    Destroy(this.gameObject);
                }
                return;
            }

            if (freeformUpdateStopwatch <= 0f) {
                freeformUpdateStopwatch = 1f;

                if (Target) {
                    freeformTargetPos = Target.healthComponent.body.corePosition + (Random.onUnitSphere * FreeformDistance);
                    freeformTargetPos.y = Target.healthComponent.body.corePosition.y + Random.Range(-0.5f, 1.5f);
                }
                else {
                    freeformTargetPos = owner.body.corePosition + (Random.onUnitSphere * FreeformDistance);
                    freeformTargetPos.y = owner.body.corePosition.y + Random.Range(-0.5f, 1.5f);
                }
            }

            if (!body) {
                return;
            }

            if (!body.healthComponent.alive && !hasRequestedDestruction) {
                hasRequestedDestruction = true;
                owner.RequestDestruction(this);
            }
        }

        public HurtBox FindNearbyEnemy() {
            SphereSearch search = new();
            search.radius = isDoingInfernalDestruction ? 300f : 60f;
            search.origin = base.transform.position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.RefreshCandidates();
            search.OrderCandidatesByDistance();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(TeamIndex.Player));
            search.FilterCandidatesByDistinctHurtBoxEntities();
            return search.GetHurtBoxes().FirstOrDefault();
        }
    }
}