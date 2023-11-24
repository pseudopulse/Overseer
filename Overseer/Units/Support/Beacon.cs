using System;
using Overseer.States;

namespace Overseer.Units.Support {
    public class BeaconController : UnitController
    {
        public override HurtBox Target { get; set; }
        public override bool OrbitalPivotIsTarget => true;
        public override float OrbitDistance => 4f;
        public override float FreeformDistance => 5f;
        public override HurtboxTracker.TargetType TargetType => HurtboxTracker.TargetType.Friendly;
        
        private float stopwatch = 3f;
        private float tpStopwatch = 0f;
        private float tpDelay = 1f;

        public override void OverrideTarget(HurtBox newTarget)
        {
            
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            tpStopwatch += Time.fixedDeltaTime;

            if (tpStopwatch >= tpDelay) {
                tpDelay = Random.Range(0.7f, 2f);

                Vector3 current = base.transform.position;
                Vector3[] positions = Utils.GetSafePositionsWithinDistance(Target.transform.position, 8f).Where(x => HasLOS(x)).ToArray();

                if (positions.Length <= 0) {
                    return;
                }

                tpStopwatch = 0f;

                Vector3 pos = positions.GetRandom(Run.instance.runRNG);
                pos += (Vector3.up * 1.5f + (Random.onUnitSphere));

                if (Vector3.Distance(pos, base.transform.position) < 3f) {
                    return;
                }

                base.transform.position = pos;

                EffectManager.SpawnEffect(Dash.TPTracer, new EffectData {
                    origin = pos,
                    start = current,
                    scale = 0.6f
                }, true);
            }
        }

        public bool HasLOS(Vector3 pos) {
            return Physics.Raycast(pos, (Target.transform.position - pos).normalized, 2000f, LayerIndex.world.mask);
        }

        public override void PerformSecondaryAction()
        {
            
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            if (!Target) {
                Target = owner.body.mainHurtBox;
            }

            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= 1f && Target && Vector3.Distance(base.transform.position, Target.transform.position) < 10f) {
                stopwatch = 0f;
                Target.healthComponent.body.AddTimedBuff(Content.BeaconSpeedBoost.BeaconSpeed, 3f);
            }
        }

        public override void Recall()
        {
            Target = owner.body.mainHurtBox;
        }
    }
}