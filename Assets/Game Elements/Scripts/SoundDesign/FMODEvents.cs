using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Events")]
    [field: SerializeField] public EventReference Hit_Sound { get; private set; }
    [field: SerializeField] public EventReference ClassicBounce_FX { get; private set; }
    [field: SerializeField] public EventReference BallBunt_Sound { get; private set; }
    [field: SerializeField] public EventReference Bunt_FX { get; private set; }



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