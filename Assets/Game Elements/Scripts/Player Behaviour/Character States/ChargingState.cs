using UnityEngine;
    public class ChargingState : PlayerState
    {
        public override void Enter()
        {
            base.Enter();
            PlayerScript.chargeValueIncrementor = 0f;
        }

        public override void Tick()
        {
            base.Tick();
            PlayerScript.Move(PlayerScript.speed * PlayerScript.chargeSpeedModifier, PlayerScript.neutralLerpTime);
            ChargingForce();
        }
        
        public void ChargingForce()
        {
                PlayerScript.chargeValueIncrementor += PlayerScript.chargeRate * Time.deltaTime;
                PlayerScript.chargeValueIncrementor = Mathf.Clamp(PlayerScript.chargeValueIncrementor, 0f, 1f);
                // Debug.Log(chargeValueIncrementor);
                
        }

        public override void Exit()
        {
            base.Exit();
        }
    }