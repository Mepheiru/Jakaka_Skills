using EntityStates;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;

namespace JakakaSkills.MyEntityStates
{
    public class Molotov : BaseSkillState
    {
        public float BaseDuration = 0.25f;
        public float Duration;

        public override void OnEnter()
        {
            base.OnEnter();

            GameObject MolotovProjOriginal = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MolotovSingleProjectile");
            GameObject MolotovPoolOriginal = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MolotovProjectileDotZone");
            GameObject MolotovProj = PrefabAPI.InstantiateClone(MolotovProjOriginal, "BanditMolotov", true);
            GameObject MolotovPool = PrefabAPI.InstantiateClone(MolotovPoolOriginal, "BanditMolotovPool", true);

            Duration = BaseDuration / attackSpeedStat;
            Ray AimRay = GetAimRay();
            StartAimMode(AimRay, 2f, false);
            PlayAnimation("Gesture, Additive", "SlashBlade", "SlashBlade.playbackRate", 0.5f, 0f);
            Util.PlaySound("bandit_shift_land_01", gameObject);
            var Simple = MolotovProj.GetComponent<ProjectileSimple>();
            {
                Simple.lifetime = 12f;
            }
            var Explosion = MolotovProj.GetComponent<ProjectileImpactExplosion>();
            {
                Explosion.blastDamageCoefficient = 3.5f;
                Explosion.blastProcCoefficient = 1f;
                Explosion.blastRadius *= attackSpeedStat; 
                Explosion.childrenProjectilePrefab = MolotovPool;
                Explosion.childrenCount = 1;
            }
            var PoolFireStats = MolotovPool.GetComponent<ProjectileDotZone>();
            {
                PoolFireStats.damageCoefficient = 0.25f;
                PoolFireStats.overlapProcCoefficient = 0.5f;
                PoolFireStats.lifetime = 8f;
                PoolFireStats.fireFrequency = (((attackSpeedStat * 2) / 3f) * 10.5f);
                PoolFireStats.resetFrequency = -0f;
            }
            var PoolStats = MolotovPool.GetComponent<ProjectileDamage>();
            {
                PoolStats.damageType = DamageType.IgniteOnHit;
            }
            MolotovPool.transform.localScale *= (attackSpeedStat);
            if (isAuthority)
            {
                RoR2.Projectile.ProjectileManager.instance.FireProjectile(
                MolotovProj,
                AimRay.origin,
                Quaternion.LookRotation(AimRay.direction),
                characterBody.gameObject,
                characterBody.damage,
                0f,
                Util.CheckRoll(characterBody.crit, characterBody.master),
                DamageColorIndex.Default,
                null,
                80f);
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= Duration && isAuthority)
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
