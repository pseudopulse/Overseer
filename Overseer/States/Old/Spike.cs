/*using System;
using Overseer.Components;

namespace Overseer.States {
    public class Spike : BaseState {
        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<OverseerSpikeController>().FireSpikes();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (GetComponent<OverseerSpikeController>().spikeStockTarget.stock < 3) {
                outer.SetNextStateToMain();
            }
        }
    }
}*/