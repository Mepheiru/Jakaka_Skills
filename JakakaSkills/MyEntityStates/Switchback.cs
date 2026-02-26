using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{
    public class Switchback : BaseSkillState
    {
        private float Duration = 0.15f;

        private int Shots;

        private float Threshold = 0.185f;
        private float FireInt = 0.0375f;
        private float Timer = 0f;
        private bool Magdump = false;
        private bool AllTheSingleLadies = false;

        private float FireRate;

        public GameObject ShottyFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public GameObject SmgFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashBarrage.prefab").WaitForCompletion();

        public GameObject HitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Hitspark1.prefab").WaitForCompletion();
        public GameObject TracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoDefault.prefab").WaitForCompletion();

        public GameObject ShotgunHitSpark = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public GameObject ShotgunTracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            FireRate = (FireInt) / (attackSpeedStat);
            skillLocator.primary.isCooldownBlocked = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (inputBank.interact.down)
            {
                skillLocator.primary.stock = 0;
            }

            if (!Magdump && !inputBank.skill1.down && fixedAge < Threshold && activatorSkillSlot.stock >= 12)
            {
                FireMulti();
            }

            if (activatorSkillSlot.stock < 12)
            {
                Threshold = 0f;
            }    

            if (!Magdump && fixedAge >= Threshold && inputBank.skill1.down)
            {
                Magdump = true;
                Timer = 0f;
            }

            if (Magdump)
            {
                Timer += Time.fixedDeltaTime;
                if (Timer >= FireRate && activatorSkillSlot.stock > 0)
                {
                    Timer = 0f;
                    FireSmg();
                    skillLocator.primary.DeductStock(1);
                }
            }

            if (activatorSkillSlot.stock <= 0)
            {
                skillLocator.primary.isCooldownBlocked = false;
                outer.SetNextState(new SwitchbackReload());
            }

            if (fixedAge >= Duration && inputBank.skill1.down == false)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void FireMulti()
        {
            if (AllTheSingleLadies) return;
            AllTheSingleLadies = true;

            if (skillLocator.primary.stock > 0)
                skillLocator.primary.DeductStock(12);

            FireMultiBullet("MuzzleRight");
            FireMultiBullet("MuzzleLeft");
        }

        private void FireMultiBullet(string ShottyMuzzle)
        {
            if (characterBody.isSprinting)
            {
                characterBody.isSprinting = false;
            }
            Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
            Ray AimRay = GetAimRay();
            StartAimMode(AimRay, 2f, false);

            PlayAnimation("Gesture Additive, Right", FirePistol2.FirePistolRightStateHash);
            PlayAnimation("Gesture Additive, Left", FirePistol2.FirePistolLeftStateHash);

            EffectManager.SimpleMuzzleFlash(ShottyFlash, gameObject, ShottyMuzzle, false);
            Util.PlaySound(FirePistol2.firePistolSoundString, base.gameObject);
            AddRecoil(-7f, -12f, -7f, 9f);
            if (isAuthority)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = AimRay.origin,
                    aimVector = AimRay.direction,
                    minSpread = 0f,
                    maxSpread = characterBody.spreadBloomAngle + 1.85f,
                    bulletCount = (uint)((attackSpeedStat / 3) * 18u),
                    procCoefficient = 0.275f,
                    damage = characterBody.damage * 0.5f,
                    force = 16f,
                    falloffModel = BulletAttack.FalloffModel.Buckshot,
                    tracerEffectPrefab = ShotgunTracerEffectPrefab,
                    muzzleName = ShottyMuzzle,
                    hitEffectPrefab = ShotgunHitSpark,
                    isCrit = RollCrit(),
                    HitEffectNormal = true,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    smartCollision = true,
                    damageType = DamageTypeCombo.GenericPrimary,
                    maxDistance = 250f
                }.Fire();
            }
        }

        private void FireSmg()
        {
            Shots++;
            if (characterBody.isSprinting)
            {
                characterBody.isSprinting = false;
            }
            string SmgMuzzle;
            Ray AimRay = GetAimRay();
            StartAimMode(AimRay, 2f, false);
            if (Shots % 2 == 0)
            {
                SmgMuzzle = "MuzzleRight";
                PlayAnimation("Gesture Additive, Right", FirePistol2.FirePistolRightStateHash);
            }
            else
            {
                SmgMuzzle = "MuzzleLeft";
                PlayAnimation("Gesture Additive, Left", FirePistol2.FirePistolLeftStateHash);
            }

            EffectManager.SimpleMuzzleFlash(SmgFlash, gameObject, SmgMuzzle, false);
            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
            AddRecoil(-1.25f, -3f, -1.25f, 2f);
            if (isAuthority)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = AimRay.origin,
                    aimVector = AimRay.direction,
                    minSpread = 0f,
                    maxSpread = characterBody.spreadBloomAngle + 1.2f,
                    bulletCount = 1u,
                    procCoefficient = 0.5f,
                    damage = characterBody.damage * 0.65f,
                    force = 4f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    tracerEffectPrefab = TracerEffectPrefab,
                    muzzleName = SmgMuzzle,
                    hitEffectPrefab = HitEffectPrefab,
                    isCrit = RollCrit(),
                    HitEffectNormal = true,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    smartCollision = true,
                    damageType = DamageTypeCombo.GenericPrimary,
                    maxDistance = 1500f
                }.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}