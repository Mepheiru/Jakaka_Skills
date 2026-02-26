using BepInEx;
using EntityStates;
using JakakaSkills.MyEntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills
{
    [BepInDependency(LanguageAPI.PluginGUID)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "com.Mephy.Jakaka_Skills";
        public const string PluginName = "Jakaka Skills";
        public const string PluginVersion = "1.6.0";

        public void Awake()
        {
            AddKeywords();
            AddVendettaSkill();
            AddBlunderbussSkill();
            AddMolotovSkill();
            AddSwitchbackSkill();
            Assembly assembly = typeof(Main).Assembly;
            bool flag = default;

            foreach (Type type in assembly.GetTypes())
            {
                if (type.Namespace != null
                    && type.Namespace.StartsWith("JakakaSkills.MyEntityStates")
                    && type.IsSubclassOf(typeof(EntityState))
                    && !type.IsAbstract)
                {
                    ContentAddition.AddEntityState(type, out flag);
                }
            }
        }

        public void AddKeywords()
        {
            LanguageAPI.Add("KEYWORD_RELOADABLE", "<style=cKeywordName>Reloadable</style>" + "<style=cSub>Cooldown can be manually triggered by pressing <style=cIsDamage>fire</style> and <style=cIsUtility>interact</style> at the same time.</style>");
            LanguageAPI.Add("KEYWORD_VENDETTA_ATK_EFFECT", "<style=cKeywordName><style=cDeath>Vendetta</style></style>" + "<style=cSub><style=cIsUtility>Attack speed doesn't apply normally</style>, it instead increases <style=cDeath>Vendetta</style> damage at <style=cIsDamage>50% effectiveness</style>.</style>");
            LanguageAPI.Add("KEYWORD_BLUNDERBUSS_ATK_EFFECT", "<style=cKeywordName><style=cDeath>Blunderbuss</style></style>" + "<style=cSub><style=cIsUtility>Attack speed doesn't apply normally</style>, it instead increases <style=cDeath>Blunderbuss</style> projectile count at <style=cIsDamage>100% effectiveness</style>.</style>");
            LanguageAPI.Add("KEYWORD_MOLOTOV_ATK_EFFECT", "<style=cKeywordName><style=cDeath>Molotov</style></style>" + "<style=cSub><style=cIsUtility>Attack speed</style> is <style=cIsDamage>67% as effective</style>, and also increases <style=cDeath>Molotov</style> radius at <style=cIsDamage>100% effectiveness</style>.</style>");
            LanguageAPI.Add("KEYWORD_SWITCHBACK_SHOTGUN_ATK_EFFECT", "<style=cKeywordName><style=cDeath>Shotgun</style></style>" + "<style=cSub><style=cIsUtility>Attack speed doesn't apply normally</style>, it instead increases <style=cDeath>Shotgun</style> projectile count at <style=cIsDamage>33% effectiveness</style>.</style>");
            LanguageAPI.Add("KEYWORD_SWITCHBACK_AUTO_ATK_EFFECT", "<style=cKeywordName><style=cDeath>Full Auto</style></style>" + "<style=cSub><style=cIsUtility>Attack speed</style> applies to <style=cDeath>Full Auto</style> at <style=cIsDamage>100% effectiveness</style>.</style>");
        }

        public void AddVendettaSkill()
        {
            GameObject BanditBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Body.prefab").WaitForCompletion();
            LanguageAPI.Add("REVOLVER_NAME", "Vendetta");
            LanguageAPI.Add("REVOLVER_DESCRIPTION", "<style=cDeath>Vendetta</style>. <style=cIsUtility>Reloadable</style>. Tap to fire a single shot for <style=cIsDamage>450% damage</style>. Hold down to blindly fire every remaining bullet in the gun for <style=cIsDamage>300% damage each</style>.");
            SkillDef Vendetta = ScriptableObject.CreateInstance<SkillDef>();
            Vendetta.activationState = new SerializableEntityStateType(typeof(MyEntityStates.Vendetta));
            Vendetta.activationStateMachineName = "Weapon";
            Vendetta.baseMaxStock = 6;
            Vendetta.rechargeStock = 9999;
            Vendetta.baseRechargeInterval = 0.85f;
            Vendetta.dontAllowPastMaxStocks = true;
            Vendetta.isCooldownBlockedUntilManuallyReset = true;
            Vendetta.beginSkillCooldownOnSkillEnd = true;
            Vendetta.resetCooldownTimerOnUse = true;
            Vendetta.canceledFromSprinting = true;
            Vendetta.cancelSprintingOnActivation = true;
            Vendetta.fullRestockOnAssign = true;
            Vendetta.interruptPriority = 0;
            Vendetta.isCombatSkill = true;
            Vendetta.mustKeyPress = false;
            Vendetta.requiredStock = 1;
            Vendetta.stockToConsume = 0;
            Vendetta.autoHandleLuminousShot = true;
            Vendetta.icon = null; // jakakaassets.LoadAsset<Sprite>("Slayer.png");
            Vendetta.skillDescriptionToken = "REVOLVER_DESCRIPTION";
            Vendetta.skillName = "Vendetta";
            Vendetta.skillNameToken = "REVOLVER_NAME";
            Vendetta.keywordTokens = ["KEYWORD_VENDETTA_ATK_EFFECT", "KEYWORD_RELOADABLE"];
            ContentAddition.AddSkillDef(Vendetta);
            SkillLocator Locator = BanditBody.GetComponent<SkillLocator>();
            SkillFamily Slot = Locator.primary.skillFamily;
            Array.Resize(ref Slot.variants, Slot.variants.Length + 1);
            Slot.variants[Slot.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Vendetta,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Vendetta.skillNameToken, false)
            };
        }

        private void AddBlunderbussSkill()
        {
            GameObject BanditBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Body.prefab").WaitForCompletion();
            LanguageAPI.Add("SHOTGUN_NAME", "Blunderbuss");
            LanguageAPI.Add("SHOTGUN_DESCRIPTION", "<style=cDeath>Blunderbuss</style>. Fire a powerful blunderbuss that gets more projectiles per stock for <style=cIsDamage>3-7-14-24x65% damage</style>.");
            SkillDef Blunderbuss = ScriptableObject.CreateInstance<SkillDef>();
            Blunderbuss.activationState = new SerializableEntityStateType(typeof(MyEntityStates.Blunderbuss));
            Blunderbuss.activationStateMachineName = "Weapon";
            Blunderbuss.baseMaxStock = 4;
            Blunderbuss.rechargeStock = 1;
            Blunderbuss.baseRechargeInterval = 0.5f;
            Blunderbuss.dontAllowPastMaxStocks = true;
            Blunderbuss.beginSkillCooldownOnSkillEnd = true;
            Blunderbuss.resetCooldownTimerOnUse = true;
            Blunderbuss.canceledFromSprinting = false;
            Blunderbuss.cancelSprintingOnActivation = true;
            Blunderbuss.hideStockCount = false;
            Blunderbuss.fullRestockOnAssign = true;
            Blunderbuss.interruptPriority = 0;
            Blunderbuss.isCombatSkill = true;
            Blunderbuss.mustKeyPress = true;
            Blunderbuss.autoHandleLuminousShot = true;
            Blunderbuss.requiredStock = 1;
            Blunderbuss.stockToConsume = 4;
            Blunderbuss.icon = null; //jakakaassets.LoadAsset<Sprite>("Blunder.png");
            Blunderbuss.skillDescriptionToken = "SHOTGUN_DESCRIPTION";
            Blunderbuss.skillName = "Blunderbuss";
            Blunderbuss.skillNameToken = "SHOTGUN_NAME";
            Blunderbuss.keywordTokens = ["KEYWORD_BLUNDERBUSS_ATK_EFFECT"];
            ContentAddition.AddSkillDef(Blunderbuss);
            SkillLocator Locator = BanditBody.GetComponent<SkillLocator>();
            SkillFamily Slot = Locator.primary.skillFamily;
            Array.Resize(ref Slot.variants, Slot.variants.Length + 1);
            Slot.variants[Slot.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Blunderbuss,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Blunderbuss.skillNameToken, false)
            };
        }

        private void AddMolotovSkill()
        {
            GameObject BanditBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Body.prefab").WaitForCompletion();
            LanguageAPI.Add("MOLOTOV_NAME", "Molotov");
            LanguageAPI.Add("MOLOTOV_DESCRIPTION", "<style=cDeath>Molotov</style>. Throw a molotov that shatters on the ground for <style=cIsDamage>350% damage</style>, creating a pool of fire that <style=cIsDamage>Ignites</style> enemies and deals <style=cIsDamage>175% damage per second</style>.");
            SkillDef Molotov = ScriptableObject.CreateInstance<SkillDef>();
            Molotov.activationState = new SerializableEntityStateType(typeof(MyEntityStates.Molotov));
            Molotov.activationStateMachineName = "Weapon";
            Molotov.baseMaxStock = 3;
            Molotov.rechargeStock = 1;
            Molotov.baseRechargeInterval = 6f;
            Molotov.beginSkillCooldownOnSkillEnd = true;
            Molotov.resetCooldownTimerOnUse = true;
            Molotov.canceledFromSprinting = false;
            Molotov.cancelSprintingOnActivation = false;
            Molotov.hideStockCount = false;
            Molotov.fullRestockOnAssign = false;
            Molotov.interruptPriority = 0;
            Molotov.isCombatSkill = true;
            Molotov.mustKeyPress = true;
            Molotov.autoHandleLuminousShot = true;
            Molotov.requiredStock = 1;
            Molotov.stockToConsume = 1;
            Molotov.icon = null; //jakakaassets.LoadAsset<Sprite>("Molotov.png");
            Molotov.skillDescriptionToken = "MOLOTOV_DESCRIPTION";
            Molotov.skillName = "Molotov";
            Molotov.skillNameToken = "MOLOTOV_NAME";
            Molotov.keywordTokens = ["KEYWORD_MOLOTOV_ATK_EFFECT", "KEYWORD_IGNITE"];
            ContentAddition.AddSkillDef(Molotov);
            SkillLocator Locator = BanditBody.GetComponent<SkillLocator>();
            SkillFamily Slot = Locator.secondary.skillFamily;
            Array.Resize(ref Slot.variants, Slot.variants.Length + 1);
            Slot.variants[Slot.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Molotov,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Molotov.skillNameToken, false)
            };
        }

        private void AddSwitchbackSkill()
        {
            GameObject CommandoBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion();
            LanguageAPI.Add("SWITCHBACK_NAME", "Switchback");
            LanguageAPI.Add("SWITCHBACK_DESCRIPTION", "<style=cDeath>Shotgun</style>. <style=cDeath>Full Auto</style>. <style=cIsUtility>Reloadable</style>. Tap to fire a shotgun blast that deals <style=cIsDamage>2x6x50% damage</style>. Hold down to rapidly fire at full auto for <style=cIsDamage>65% damage</style> a shot.");
            SkillDef Switchback = ScriptableObject.CreateInstance<SkillDef>();
            Switchback.activationState = new SerializableEntityStateType(typeof(MyEntityStates.Switchback));
            Switchback.activationStateMachineName = "Weapon";
            Switchback.baseMaxStock = 48;
            Switchback.rechargeStock = 9999;
            Switchback.baseRechargeInterval = 1.15f;
            Switchback.isCooldownBlockedUntilManuallyReset = true;
            Switchback.dontAllowPastMaxStocks = true;
            Switchback.beginSkillCooldownOnSkillEnd = true;
            Switchback.resetCooldownTimerOnUse = true;
            Switchback.canceledFromSprinting = false;
            Switchback.cancelSprintingOnActivation = true;
            Switchback.fullRestockOnAssign = true;
            Switchback.interruptPriority = 0;
            Switchback.isCombatSkill = true;
            Switchback.mustKeyPress = false;
            Switchback.requiredStock = 1;
            Switchback.stockToConsume = 0;
            Switchback.autoHandleLuminousShot = true;
            Switchback.icon = null; //jakakaassets.LoadAsset<Sprite>("Marshall.png");
            Switchback.skillDescriptionToken = "SWITCHBACK_DESCRIPTION";
            Switchback.skillName = "Switchback";
            Switchback.skillNameToken = "SWITCHBACK_NAME";
            Switchback.keywordTokens = ["KEYWORD_SWITCHBACK_SHOTGUN_ATK_EFFECT", "KEYWORD_SWITCHBACK_AUTO_ATK_EFFECT", "KEYWORD_RELOADABLE"];
            ContentAddition.AddSkillDef(Switchback);
            SkillLocator Locator = CommandoBody.GetComponent<SkillLocator>();
            SkillFamily Slot = Locator.primary.skillFamily;
            Array.Resize(ref Slot.variants, Slot.variants.Length + 1);
            Slot.variants[Slot.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = Switchback,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(Switchback.skillNameToken, false)
            };
        }
    }
}
