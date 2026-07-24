using AK.Wwise;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JakakaSkills.MyEntityStates
{
    public class LightshowCharge : BaseSkillState
    {
        private float Duration = 0.65f;
        private bool hasSwitchedToTimerLoop;
        Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();

            AkSoundEngine.LoadBank("char_railgunner", out uint railgunnerBankID);

            PlayAnimation("Gesture, Override", "CommandPaint");
            modelAnimator = GetModelAnimator();
            if (modelAnimator)
            {
                modelAnimator.SetBool("paintActive", true);
            }
            Util.PlaySound("Play_railgunner_R_preFire", gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority && fixedAge >= Duration)
            {
                bool isFireDown = this.inputBank && this.inputBank.skill4.down;
                if (isFireDown && !hasSwitchedToTimerLoop)
                {
                    AkSoundEngine.ExecuteActionOnEvent("Play_railgunner_R_preFire", AkActionOnEventType.AkActionOnEventType_Stop, base.gameObject);
                    Util.PlaySound("Play_railgunner_R_gun_timer_loop", gameObject);
                    hasSwitchedToTimerLoop = true;
                }
                if (!isFireDown)
                {
                    Util.PlaySound("Stop_railgunner_R_gun_timer_loop", gameObject);
                    if (modelAnimator)
                    {
                        modelAnimator.SetBool("paintActive", false);
                    }
                    outer.SetNextState(new Lightshow());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
