using System;

namespace Overseer.Units {
    [CreateAssetMenu(menuName = "Overseer/Action", fileName = "Action")]
    public class UnitAction : ScriptableObject {
        public Sprite Icon;
        public KeyCode ActivationKey;
        public ActionType ActionType;
        public string KeyCodeText;
    }

    public enum ActionType {
        Recall,
        Retarget,
        Terminate,
        BuildOffense,
        BuildDefense,
        BuildSupport,
        Cancel
    }
}