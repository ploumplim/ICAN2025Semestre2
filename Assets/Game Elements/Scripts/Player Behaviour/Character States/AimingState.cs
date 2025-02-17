using UnityEngine;
    public class AimingState : PlayerState
    {
        public override void Tick()
        {
            base.Tick();
            PlayerScript.Move(true);
        }
    }