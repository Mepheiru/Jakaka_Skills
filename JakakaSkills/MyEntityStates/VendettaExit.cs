using EntityStates;

namespace JakakaSkills.MyEntityStates
{
    public class VendettaExit : EntityStates.Bandit2.Weapon.BaseSidearmState
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
