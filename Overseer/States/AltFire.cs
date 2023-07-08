using System;
using Overseer.Components;

namespace Overseer.States {
    public class AltFire : BaseState {
        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<OverseerController>().TriggerAltFire();
            outer.SetNextStateToMain();
        }
    }
}