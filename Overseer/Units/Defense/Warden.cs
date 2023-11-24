using System.Runtime;
using System;
using Overseer.Components;

namespace Overseer.Units.Defense {
    public class WardenController : UnitController
    {
        public override HurtBox Target { get; set; }
        private ProjectileController target;
        public LineRenderer lr;
        private bool isReflecting = false;
        private float stopwatch = 0f;

        public override void OverrideTarget(HurtBox newTarget)
        {
            
        }

        public override void FixedUpdate() {
            base.FixedUpdate();
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            OrbitalMovement(totalUnits, thisUnit, 360f, 0f, 3.5f);
        }

        public override void PerformSecondaryAction()
        {
            
        }

        public override void Recall()
        {
            
        }
    }
}