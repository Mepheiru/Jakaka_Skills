using EntityStates;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{

    public class Vendetta : BaseSkillState
    {
        private float Duration = 0.45f;

        private Animator Animator;
        private int BodySideWeaponLayerIndex;

        private float Threshold = 0.8f;     
        private float FireInt = 0.050f;      
        private float Timer = 0f;
        private bool Magdump = false;
        private bool AllTheSingleLadies = false;

        public GameObject MuzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/MuzzleflashBandit2.prefab").WaitForCompletion();
        public GameObject CrosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolverFire.prefab").WaitForCompletion();
        public GameObject HitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit2Pistol.prefab").WaitForCompletion();
        public GameObject TracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/TracerBanditPistol.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            skillLocator.primary.isCooldownBlocked = true;
            Animator = GetModelAnimator();
            BodySideWeaponLayerIndex = Animator.GetLayerIndex("Body, SideWeapon");
            Animator.SetLayerWeight(BodySideWeaponLayerIndex, 1f);
            CrosshairUtils.RequestOverrideForBody(characterBody, CrosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (inputBank.interact.down)
            {
                skillLocator.primary.stock = 0;
            }
                
            if (!Magdump && !inputBank.skill1.down && fixedAge < Threshold)
            {
                FireNormalShot();
            }

            if (!Magdump && fixedAge >= Threshold && inputBank.skill1.down)
            {
                Magdump = true;
                Timer = 0f;
            }

            if (!Magdump && fixedAge == 0.15f && skillLocator.primary.stock > 0) 
            { 
                PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", 0.65f, 0f); 
                Util.PlaySound("Play_bandit2_R_load", gameObject); 
            }

            if (Magdump)
            {
                if (skillLocator.primary.stock <= 0)
                {
                    PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", 0.85f, 0f);
                    Util.PlaySound("Play_bandit2_R_load", gameObject);
                    skillLocator.primary.isCooldownBlocked = false;
                    outer.SetNextState(new VendettaExit());
                }

                Timer += Time.fixedDeltaTime;
                if (Timer >= FireInt && skillLocator.primary.stock > 0)
                {
                    Timer = 0f;
                    FireDumpShot();
                    skillLocator.primary.stock--;
                }
            }

            if (skillLocator.primary.stock <= 0)
            {
                PlayAnimation("Gesture, Additive", "MainToSide", "MainToSide.playbackRate", 0.85f, 0f);
                Util.PlaySound("Play_bandit2_R_load", gameObject);
                skillLocator.primary.isCooldownBlocked = false;
                outer.SetNextState(new VendettaExit());
            }

            if (fixedAge >= Duration && inputBank.skill1.down == false)
            {
                outer.SetNextState(new VendettaExit());
                return;
            }
        }

        private void FireNormalShot()
        {
            if (AllTheSingleLadies) return;
            AllTheSingleLadies = true;

            if (skillLocator.primary.stock > 0)
                skillLocator.primary.stock--;

            FireBulletSingle();
        }

        private void FireDumpShot()
        {
            DumpBullet();
        }

        private void DumpBullet()
        {
            if (!isAuthority) return;

            Transform Speen = FindModelChild("SpinningPistolFX");

            Speen.gameObject.SetActive(false);

            Ray AimRay = GetAimRay();
            Animator = GetModelAnimator();
            StartAimMode(AimRay, 2f, false);
            BodySideWeaponLayerIndex = Animator.GetLayerIndex("Body, SideWeapon");
            Animator.SetLayerWeight(BodySideWeaponLayerIndex, 1f);
            PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", 1f, 0f);
            Util.PlaySound("Play_bandit2_R_fire", gameObject);
            AddRecoil(-2f, -6f, -2f, 3f);

            EffectManager.SimpleMuzzleFlash(MuzzleFlash, gameObject, "MuzzlePistol", false);

            new BulletAttack
            {
                owner = gameObject,
                weapon = gameObject,
                origin = AimRay.origin,
                aimVector = AimRay.direction,
                minSpread = 0f,
                maxSpread = characterBody.spreadBloomAngle * 1.85f,
                bulletCount = 1u,
                procCoefficient = 0.65f,
                damage = characterBody.damage * ((attackSpeedStat / 2f) * 6f),
                force = 4f,
                falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                tracerEffectPrefab = TracerEffectPrefab,
                muzzleName = "MuzzlePistol",
                hitEffectPrefab = HitEffectPrefab,
                isCrit = RollCrit(),
                HitEffectNormal = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                smartCollision = true,
                damageType = DamageTypeCombo.GenericPrimary,
                maxDistance = 450f
            }.Fire();
        }

        private void FireBulletSingle()
        {
            if (!isAuthority) return;

            Transform Speen = FindModelChild("SpinningPistolFX");
            
            Speen.gameObject.SetActive(false);
            
            Ray AimRay = GetAimRay();
            Animator = GetModelAnimator();
            StartAimMode(AimRay, 2f, false);
            BodySideWeaponLayerIndex = Animator.GetLayerIndex("Body, SideWeapon");
            Animator.SetLayerWeight(BodySideWeaponLayerIndex, 1f);
            PlayAnimation("Gesture, Additive", "FireSideWeapon", "FireSideWeapon.playbackRate", 1f, 0f);
            Util.PlaySound("Play_bandit2_R_fire", gameObject);
            AddRecoil(-2f, -6f, -2f, 3f);

            EffectManager.SimpleMuzzleFlash(MuzzleFlash, gameObject, "MuzzlePistol", false);

            new BulletAttack
            {
                owner = gameObject,
                weapon = gameObject,
                origin = AimRay.origin,
                aimVector = AimRay.direction,
                minSpread = 0f,
                maxSpread = characterBody.spreadBloomAngle,
                bulletCount = 1u,
                procCoefficient = 1.5f,
                damage = characterBody.damage * ((attackSpeedStat / 2f) * 9f),
                force = 4f,
                falloffModel = BulletAttack.FalloffModel.None,
                tracerEffectPrefab = TracerEffectPrefab,
                muzzleName = "MuzzlePistol",
                hitEffectPrefab = HitEffectPrefab,
                isCrit = RollCrit(),
                HitEffectNormal = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                smartCollision = true,
                damageType = DamageTypeCombo.GenericPrimary,
                maxDistance = 3000f
            }.Fire();
        }
        
        public override void OnExit()
        {
            if (Animator != null)
            {
                Animator.SetLayerWeight(BodySideWeaponLayerIndex, 0f);
            }   
            base.OnExit();           
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
