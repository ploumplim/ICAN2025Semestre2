using System.Collections.Generic;
using UnityEngine;

public class WallEffect : Effect
{
    public WallEffectType EffectType;

    public enum WallEffectType
    {
        Speed,
        Bounce,
        Slow,
        NoGravity,
        PlayerMagnet,
        Gravity,
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject CollisionObject = collision.gameObject;
        if (EffectType == WallEffectType.Speed)
        {
            SpeedEffect(CollisionObject);
        }
        else if (EffectType == WallEffectType.Bounce)
        {
            BounceEffect(CollisionObject);
        }
        else if (EffectType == WallEffectType.Slow)
        {
            Debug.Log("Collision with Slow effect");
            // Add your logic for Slow effect here
        }
        
        else if (EffectType == WallEffectType.PlayerMagnet)
        {
            PlayerMagnetEffect(CollisionObject);
        }
        else if (EffectType == WallEffectType.Gravity)
        {
            
        }
        
        // else if (EffectType == WallEffectType.NoGravity)
        // {
        //     NoGravityEffect(CollisionObject);
        // }
        else
        {
            Debug.Log("Collision with None effect");
            // Add your logic for None effect here
        }
    }
}