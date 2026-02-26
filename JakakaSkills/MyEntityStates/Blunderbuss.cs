using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{
    public class Blunderbuss : BaseSkillState
    {
        public float BaseDuration = 0.25f;

        private float Duration;

        private float SpreadMult;

        private float RecoilMult;

        private uint BulletAmount;

        public GameObject HitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion();
        public GameObject TracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBandit2Shotgun.prefab").WaitForCompletion();
        public GameObject MuzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            GetStock();
            Duration = BaseDuration / attackSpeedStat;
            PlayAnimation("Gesture, Additive", "FireMainWeapon", "FireMainWeapon.playbackRate", 0.65f, 0f);
            Ray AimRay = GetAimRay();
            Util.PlaySound("Play_bandit2_m1_rifle", gameObject);
            StartAimMode(AimRay, 2f, false);
            AddRecoil(-24f * RecoilMult, -32f * RecoilMult, -12f * RecoilMult, 12f * RecoilMult);

            if (isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(MuzzleFlash, gameObject, "MuzzleShotgun", false);

                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = AimRay.origin,
                    aimVector = AimRay.direction,
                    minSpread = 0f,
                    maxSpread = (characterBody.spreadBloomAngle + 2.2f) * SpreadMult,
                    bulletCount = BulletAmount,
                    procCoefficient = 0.15f,
                    damage = characterBody.damage * 0.65f,
                    force = 32f,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    tracerEffectPrefab = TracerEffectPrefab,
                    muzzleName = "MuzzleShotgun",
                    hitEffectPrefab = HitEffectPrefab,
                    isCrit = RollCrit(),
                    HitEffectNormal = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    smartCollision = true,
                    damageType = DamageTypeCombo.GenericPrimary,
                    maxDistance = 800f
                }.Fire();
            }
        }

        private void GetStock()
        {
            int Stock = activatorSkillSlot.stock - 1;
            switch (Stock)
            {
                case -4:
                    SpreadMult = 0.2f;
                    RecoilMult = 0.33f;
                    BulletAmount = (uint)(attackSpeedStat * 3);
                    break;
                case -3:
                    SpreadMult = 0.45f;
                    RecoilMult = 0.5f;
                    BulletAmount = (uint)(attackSpeedStat * 7); ;
                    break;
                case -2:
                    SpreadMult = 0.8f;
                    RecoilMult = 0.8f;
                    BulletAmount = (uint)(attackSpeedStat * 14); ;
                    break;
                default:
                    SpreadMult = 1f;
                    RecoilMult = 1f;
                    BulletAmount = (uint)(attackSpeedStat * 24); ;
                    break;
            }
        }

        public override void OnExit()
        {
            if (activatorSkillSlot.stock <= 0)
            {
                activatorSkillSlot.stock = 0;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= Duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}