using System;
using Overseer.Units;
using RoR2.UI;
using UnityEngine.Events;

namespace Overseer.Components {
    public class ActionSelector : MonoBehaviour {
        public RectTransform container;
        public GameObject selectorPrefab;
        public List<UnitAction> actions;
        private ActionSelection[] actionElements;
        private int currentSelected = 0;
        internal OnFireAction onActionFired = new();
        public void Start() {
            UIElementAllocator<ActionSelection> allocator = new(container, selectorPrefab);
            allocator.AllocateElements(actions.Count);
            actionElements = allocator.elements.ToArray();

            for (int i = 0; i < actionElements.Length; i++) {
                actionElements[i].action = actions[i];
                actionElements[i].Initialize();
                // Debug.Log(actionElements[i].action);
            }
        }       

        public void FixedUpdate() {
            for (int i = 0; i < actionElements.Length; i++) {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Mouse2)) {
                    onActionFired.Invoke(ActionType.Cancel);
                    Destroy(this);
                    return;
                }
                if (Input.GetKeyDown(actionElements[i].action.ActivationKey)) {
                    onActionFired.Invoke(actionElements[i].action.ActionType);
                    Destroy(this);
                    return;
                }
            }
        }
    }

    public class OnFireAction : UnityEvent<ActionType> {
        int guh;
    }
}