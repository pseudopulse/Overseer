using System;

namespace Overseer.Components {
    public class OverseerSpikeController : MonoBehaviour {
        public GenericSkill spikeStockTarget;
        public List<GameObject> Spikes = new();
        public GameObject SpikePrefab;
        public Transform[] unitSpawnPoints;

        public void FixedUpdate() {
            if (Spikes.Count() < spikeStockTarget.stock && Spikes.Count() < 3) {
                FireProjectileInfo info = new();
                info.owner = base.gameObject;
                info.damage = GetComponent<CharacterBody>().damage * 2f;
                info.position = base.transform.position;
                info.rotation = Quaternion.identity;
                info.projectilePrefab = SpikePrefab;
                ProjectileManager.instance.FireProjectile(info);
            }

            for (int i = 0; i < Spikes.Count; i++) {
                if (Spikes[i].GetComponent<Spike>().hasFired) {
                    continue;
                }
                
                // Vector3 targetPos = (base.transform.position) + Quaternion.AngleAxis(360 / Spikes.Count * i + -90, base.transform.forward) * base.transform.forward * 1f;
                Vector3 targetPos = unitSpawnPoints[i].transform.position;
                Vector3 outPos = Vector3.Lerp(Spikes[i].transform.position, targetPos, 20 * Time.fixedDeltaTime);
                Spikes[i].transform.position = outPos;
                Spikes[i].transform.forward = GetComponent<CharacterDirection>().forward;
            }
        }

        public void FireSpikes() {
            foreach (GameObject spike in Spikes) {
                if (spike.GetComponent<Spike>().hasFired) {
                    continue;
                }
                spike.transform.forward = GetFlankLaserAimDir(spike.transform.position);
                spike.GetComponent<Spike>().Fire();
            }
        }

        public Vector3 GetFlankLaserAimDir(Vector3 pos) {
            Ray aimRay = GetComponent<CharacterBody>().inputBank.GetAimRay();

            if (Physics.Raycast(aimRay, out RaycastHit info, 4000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore)) {
                Vector3 dir = (info.point - pos).normalized;
                return dir;
            }
            
            return aimRay.direction;
        }
    }
}