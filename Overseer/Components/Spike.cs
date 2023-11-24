using System;
using Overseer.Units;

namespace Overseer.Components {
    public class Spike : MonoBehaviour {
        internal OverseerController owner;
        internal OverseerSpikeController spikeController;
        [SerializeField]
        private List<MonoBehaviour> ToEnable;

        private HealthComponent component;

        internal bool hasFired = false;
        // internal LockOnMarker marker;
        internal bool shouldCheck = false;
        internal bool hasChecked = false;

        public void Fire() {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            foreach (MonoBehaviour com in ToEnable) {
                com.enabled = true;
            }
            hasFired = true;
        }

        public void OnHit() {
            shouldCheck = true;
        }

        public void Start() {
            component = GetComponent<HealthComponent>();
            spikeController = GetComponent<ProjectileController>().owner.GetComponent<OverseerSpikeController>();
            owner = spikeController.GetComponent<OverseerController>();
            spikeController.Spikes.Add(base.gameObject);

            foreach (Collider collider in GetComponentsInChildren<Collider>()) {
                foreach (IUnitC unit in owner.activeUnits) {
                    foreach (Collider col in unit.GetSelf().GetComponentsInChildren<Collider>()) {
                        Physics.IgnoreCollision(collider, col, true);
                    }
                }
            }
        }

        public void OnDestroy() {
            if (NetworkServer.active) {
                // GetComponent<ProjectileImpactExplosion>().DetonateServer();
            } 
            spikeController.spikeStockTarget.DeductStock(1);
            spikeController.Spikes.Remove(this.gameObject);
            EffectManager.SpawnEffect(Assets.GameObject.ExplosionDroneDeath, new EffectData {
                    origin = base.transform.position
            }, false);

            //if (marker) {
            //    GameObject.Destroy(marker);
            //}
        }

        public void FixedUpdate() {
            if (component.alive == false) {
                Destroy(base.gameObject);
            }

            if (shouldCheck && !hasChecked) {
                ProjectileStickOnImpact impact = GetComponent<ProjectileStickOnImpact>();

                if (impact.stuckBody) {
                    Main.Log("stuckbody found");
                    CharacterBody body = impact.stuckBody;

                    hasChecked = true;
                    
                    if (body.GetComponent<UnitController>()) {
                        Main.Log("we hit a unit, return");
                        return;
                    }

                    if (body.teamComponent.teamIndex == TeamIndex.Player) {
                        Main.Log("hit a friendly, return");
                        return;
                    }

                    Main.Log("retargeting to: " + body.mainHurtBox);

                    owner.Retarget(body.mainHurtBox);
                    //if (body.GetComponent<LockOnMarker>()) {
                    //    return;
                    //} 

                    DamageInfo info = new();
                    info.damage = owner.body.damage * 3f;
                    info.crit = Util.CheckRoll(owner.body.crit, owner.body.master);
                    info.position = base.transform.position;
                    info.procCoefficient = 1f;
                    info.attacker = owner.body.gameObject;

                    body.healthComponent.TakeDamage(info);
                    GlobalEventManager.instance.OnHitEnemy(info, body.gameObject);
                    GlobalEventManager.instance.OnHitAll(info, body.gameObject);
                }
            }
        }
    }

}