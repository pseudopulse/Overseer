/*using System;
using Overseer.Components;
using Overseer.Units;

namespace Overseer.States {
    public class Command : BaseState {
        public GameObject prefab => Main.assets.LoadAsset<GameObject>("CommandMenu.prefab");
        public OverseerController controller;
        public GameObject instance;

        public override void OnEnter()
        {
            base.OnEnter();
            controller = GetComponent<OverseerController>();
            instance = GameObject.Instantiate(prefab);
            instance.GetComponent<ActionSelector>().onActionFired.AddListener(OnFired);
        }

        public void OnFired(ActionType type) {
            Destroy(instance);
            controller.HandleAction(type);
            outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}*/