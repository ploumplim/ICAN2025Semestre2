using UnityEngine;

public class MovingState : PlayerState
{
    //Animator myAnimator;
    public override void Tick()
    {
        base.Tick();
        PlayerScript.Move(false);
        //myAnimator.SetBool("IsWalking", true);
    }
}
