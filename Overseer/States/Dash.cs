using System;

namespace Overseer.States {
    public class Dash : BaseState {
        public static GameObject TPEffect;
        public static GameObject TPTracer;
        public Vector3 blinkVector;
        public CharacterModel model;
        public HurtBoxGroup hbg;
        public float duration = 0.2f;
        public float speedCoeff = 5f;
        public Vector3 startPos;
        public override void OnEnter()
        {
            base.OnEnter();

            AkSoundEngine.PostEvent(WwiseEvents.Play_huntress_shift_mini_blink, base.gameObject);
            model = GetModelTransform().GetComponent<CharacterModel>();
            hbg = model.GetComponent<HurtBoxGroup>();

            model.invisibilityCount++;
            hbg.hurtBoxesDeactivatorCounter++;
            blinkVector = GetBlinkVector();
            startPos = base.characterBody.corePosition;
        }

        public Vector3 GetBlinkVector() {
            return base.inputBank.moveVector == Vector3.zero ? base.characterDirection.forward : base.inputBank.moveVector;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterMotor.velocity = Vector3.zero;
            characterMotor.rootMotion += blinkVector * (moveSpeedStat * speedCoeff * Time.fixedDeltaTime);

            if (base.fixedAge >= duration) {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            EffectManager.SpawnEffect(TPTracer, new EffectData {
                origin = startPos,
                start = characterBody.corePosition
            }, true);

            model.invisibilityCount--;
            hbg.hurtBoxesDeactivatorCounter--;
        }

        public static void CreatePrefabs() {
            TPEffect = RuntimePrefabManager.CreatePrefab(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Parent/ParentTeleportEffect.prefab").WaitForCompletion(), "LunarConstructTeleport");
            var particles = TPEffect.transform.GetChild(0);
            var ringParticle = particles.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var moonRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLunarWispFire.png").WaitForCompletion();

            var newRing = GameObject.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Parent/matParentTeleportPortal.mat").WaitForCompletion());
            newRing.SetTexture("_RemapTex", moonRamp);

            ringParticle.sharedMaterial = newRing;

            particles.GetChild(1).gameObject.SetActive(false);

            var energyInitialParticle = particles.GetChild(3).GetComponent<ParticleSystemRenderer>();
            energyInitialParticle.sharedMaterial = newRing;
            energyInitialParticle.gameObject.transform.localScale = Vector3.one * 0.25f;

            var eps = particles.GetChild(3).GetComponent<ParticleSystem>().main;
            eps.duration = 0.17f;

            particles.GetChild(4).gameObject.SetActive(false);

            TPTracer = RuntimePrefabManager.CreatePrefab(Assets.GameObject.VoidSurvivorBeamTracer, "OverloadingTracer");
            TPTracer.transform.GetChild(0).gameObject.SetActive(false);
            TPTracer.transform.GetChild(1).gameObject.SetActive(false);

            var lineRenderer = TPTracer.GetComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.33f;
            lineRenderer.numCapVertices = 10;

            var newMat = GameObject.Instantiate(Assets.Material.matVoidSurvivorBeamTrail);
            newMat.SetTexture("_RemapTex", Assets.Texture2D.texRampLunarWardDecal);

            lineRenderer.material = newMat;

            var animateShaderAlpha = TPTracer.GetComponent<AnimateShaderAlpha>();
            animateShaderAlpha.timeMax = 0.4f;

            Main.contentPack.RegisterGameObject(TPTracer);
            Main.contentPack.RegisterGameObject(TPEffect);
        }
    }
}