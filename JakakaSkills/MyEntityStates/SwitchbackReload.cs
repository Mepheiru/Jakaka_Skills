using EntityStates;
using RoR2;
using UnityEngine;

namespace JakakaSkills.MyEntityStates
{
    public class SwitchbackReload : GenericReload
    {
        private static int ReloadPistolsStateHash = Animator.StringToHash("ReloadPistols");

        private static int ReloadPistolsParamHash = Animator.StringToHash("ReloadPistols.playbackRate");

        private static int ReloadPistolsExitStateHash = Animator.StringToHash("ReloadPistolsExit");

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_bandit2_R_load", gameObject);
            PlayAnimation("Gesture, Override", ReloadPistolsStateHash, ReloadPistolsParamHash, 1.15f);
            PlayAnimation("Gesture, Additive", ReloadPistolsStateHash, ReloadPistolsParamHash, 1.15f);
            Transform GunLeft = FindModelChild("GunMeshL");
            {
                GunLeft.gameObject.SetActive(false);
            }
            Transform GunRight = FindModelChild("GunMeshR");
            {
                GunRight.gameObject.SetActive(false);
            }
        }

        public override void OnExit()
        {
            Transform SpinnyLeft = FindModelChild("ReloadFXL");    
            {
                SpinnyLeft.gameObject.SetActive(false);
            }
            Transform SpinnyRight = FindModelChild("ReloadFXR");
            {
                SpinnyRight.gameObject.SetActive(false);
            }
            Transform GunLeft = FindModelChild("GunMeshL");
            {
                GunLeft.gameObject.SetActive(true);
            }
            Transform GunRight = FindModelChild("GunMeshR");
            {
                GunRight.gameObject.SetActive(true);
            }
            PlayAnimation("Gesture, Override", ReloadPistolsExitStateHash);
            PlayAnimation("Gesture, Additive", ReloadPistolsExitStateHash);
            base.OnExit();
        }
    }
}

