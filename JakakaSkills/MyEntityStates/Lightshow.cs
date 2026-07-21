using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{
    public class Lightshow : BaseSkillState
    {
        private float Duration = 0.25f;

        public GameObject HitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/Drone Tech/NanoPistolImpactVFX.prefab").WaitForCompletion();
        public GameObject TracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/Drone Tech/TracerNanoPistolCharged.prefab").WaitForCompletion();
        public GameObject TracerEffectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/TracerGolem.prefab").WaitForCompletion();
        public GameObject MuzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC3/Drone Tech/NanoPistolMuzzleFlashVFX.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "CommandReleaseShield", "CommandReleaseShield.playbackRate", 0.45f, 0f);
            Util.PlaySound("Play_railgunner_R_alt_fire", gameObject);
            Ray AimRay = GetAimRay();
            StartAimMode(AimRay, 2f, false);
            AddRecoil(-7f, -12f, -3f, 3f);

            if (isAuthority)
            {
                if (characterMotor)
                {
                    Vector3 force = -AimRay.direction * (32.5f);
                    characterMotor.velocity += force;
                }

                EffectManager.SimpleMuzzleFlash(MuzzleFlash, gameObject, "Muzzle", false);

                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = AimRay.origin,
                    aimVector = AimRay.direction,
                    minSpread = 0f,
                    maxSpread = (characterBody.spreadBloomAngle + 3.2f),
                    bulletCount = 12,
                    procCoefficient = 0.25f,
                    damage = characterBody.damage * ((2.25f / 100f) * skillLocator.special.stock),
                    force = 32f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = TracerEffectPrefab,
                    muzzleName = "Muzzle",
                    hitEffectPrefab = HitEffectPrefab,
                    isCrit = RollCrit(),
                    HitEffectNormal = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    smartCollision = true,
                    damageType = DamageTypeCombo.GenericSpecial,
                    maxDistance = 1450f
                }.Fire();

                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = AimRay.origin,
                    aimVector = AimRay.direction,
                    minSpread = 0f,
                    maxSpread = (characterBody.spreadBloomAngle + 1.45f),
                    bulletCount = 4,
                    procCoefficient = 0.5f,
                    damage = characterBody.damage * ((8.0f / 100f) * skillLocator.special.stock),
                    force = 128f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = TracerEffectPrefab2,
                    muzzleName = "Muzzle",
                    hitEffectPrefab = HitEffectPrefab,
                    isCrit = RollCrit(),
                    HitEffectNormal = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    smartCollision = true,
                    damageType = DamageTypeCombo.GenericSpecial,
                    maxDistance = 2250f
                }.Fire();
            }
            skillLocator.special.stock = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= Duration && inputBank.skill1.down == false)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
