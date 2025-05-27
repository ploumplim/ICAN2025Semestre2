using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Events")]
    [field: SerializeField] public EventReference Hit_Sound { get; private set; }
    [field: SerializeField] public EventReference ClassicBounce_FX { get; private set; }

    [field: SerializeField] public EventReference GrabPress_FX { get; private set; }
    [field: SerializeField] public EventReference GrabBall_FX { get; private set; }
    [field: SerializeField] public EventReference GrabOut_FX { get; private set; }





    [field: SerializeField] public EventReference KnockOut_FX { get; private set; }
    [field: SerializeField] public EventReference KnockBack_FX { get; private set; }




    [field: SerializeField] public EventReference PressHit_FX { get; private set; }
    [field: SerializeField] public EventReference PlayerSpawn_FX { get; private set; }
    [field: SerializeField] public EventReference BallTouched_FX { get; private set; }
    [field: SerializeField] public EventReference Dash_FX { get; private set; }













    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one FMODEvents in the scene");
        }
        instance = this;
    }
}