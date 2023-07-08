using System;

namespace Overseer.Units.Support {
    public class BeaconController : UnitController
    {
        public override HurtBox Target { get; set; }
        public override bool OrbitalPivotIsTarget => true;
        public override float OrbitDistance => 4f;
        public override float FreeformDistance => 5f;
        public override HurtboxTracker.TargetType TargetType => HurtboxTracker.TargetType.Friendly;
        
        private float stopwatch = 0f;

        public override void OverrideTarget(HurtBox newTarget)
        {
            Target = newTarget;
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            OrbitalMovement(totalUnits, thisUnit, 360f, 0f, 1f, 2f);
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

            if (stopwatch >= 1f && Target) {
                stopwatch = 0f;
                Target.healthComponent.body.AddTimedBuff(Content.BeaconSpeedBoost.BeaconSpeed, 1f);
            }
        }

        public override void Recall()
        {
            Target = owner.body.mainHurtBox;
        }
    }
}