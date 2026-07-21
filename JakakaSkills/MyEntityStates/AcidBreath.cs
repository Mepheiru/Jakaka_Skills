using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{
    public class FireOrbAcrid : BaseSkillState
    {
        public static float baseDuration = 0.15f;
        public static float damageCoefficient = 0.5f;
        public static float force = 150f;

        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            // GameObject FlameProjOriginal = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSpit.prefab").WaitForCompletion();
            // RoR2/Base/Golem/LaserGolem.prefab // comedy :3
            GameObject FlameProjOriginal = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentGravSphere.prefab").WaitForCompletion();
            GameObject FlameProj = PrefabAPI.InstantiateClone(FlameProjOriginal, "AcridBreath", true);
            PlayAnimation("Gesture, Mouth", "FireSpit", "FireSpit.playbackRate", duration);
          
            if (isAuthority)
            {
                Ray aimRay = GetAimRay();

                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    projectilePrefab = FlameProj,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    owner = gameObject,
                    damage = damageStat * damageCoefficient,
                    force = force,
                    crit = RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    target = null
                });
            }
        }

        public override void OnExit() => base.OnExit();

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= duration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}