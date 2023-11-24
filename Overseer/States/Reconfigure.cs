using System;
using Overseer.Components;

namespace Overseer.States {
    public class Reconfigure : BaseState {
        public override void OnEnter()
        {
            base.OnEnter();
            GetComponent<OverseerController>().ReconfigureUnits();
            AkSoundEngine.PostEvent(WwiseEvents.Play_MULT_R_variant_activate, base.gameObject);
            outer.SetNextStateToMain();
        }
    }
}