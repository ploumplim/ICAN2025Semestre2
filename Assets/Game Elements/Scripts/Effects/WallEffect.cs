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
        None
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (EffectType == WallEffectType.Speed)
        {
            SpeedEffect(collision.gameObject);
        }
        else if (EffectType == WallEffectType.Bounce)
        {
            BounceEffect(collision.gameObject);
        }
        else if (EffectType == WallEffectType.Slow)
        {
            Debug.Log("Collision with Slow effect");
            // Add your logic for Slow effect here
        }
        else
        {
            Debug.Log("Collision with None effect");
            // Add your logic for None effect here
        }
    }
}