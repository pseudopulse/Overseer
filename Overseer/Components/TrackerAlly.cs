using System;
using Overseer.Units;

namespace Overseer.Components {
    public class TrackerAlly : HurtboxTracker {
        public override void Start()
        {
            base.Start();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (target != null && target.GetComponent<HurtBox>().healthComponent.GetComponent<IUnitC>() != null) {
                target = null;
            }
        }
    }
}