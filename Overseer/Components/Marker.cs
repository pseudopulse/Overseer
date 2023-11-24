using System;

namespace Overseer.Components {
    // does nothing, only used to mark enemies that have been locked onto
    public class Marker : MonoBehaviour {
        private float timeAlive = 0f;
        public void FixedUpdate() {
            timeAlive += Time.fixedDeltaTime;

            if (timeAlive >= 5f) {
                Destroy(this);
            }
        }
    }
}