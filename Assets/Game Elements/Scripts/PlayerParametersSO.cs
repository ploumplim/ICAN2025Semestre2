using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParametersSO", menuName = "Scriptable Objects/PlayerParametersSO")]
public class PlayerParametersSO : ScriptableObject
{
    public enum MoveType
    {
        Velocity,
        Force
    };

    [Header("Movement Types")]
    public MoveType movementType = MoveType.Velocity;

    [Header("Movement variables")]
    public float speed = 5f;
    public float speedWithoutBallsModifier = 1f;
    public float aimSpeedMod = 0f;

    [Header("Knockback")]
    public float knockbackTime = 0.5f;
    public float linearDrag = 3f;
    public float hitLinearDrag = 0f;

    [Header("Rotation Lerps")]
    public float rotationLerpTime = 0.1f;
    public float rotationWhileAimingLerpTime = 0.1f;
    public float rollLerpTime = 0.1f;

    [Header("Charge shot")]
    public float chargeRate = 0.5f;

    [Header("Parry")]
    public float parryCooldown = 0.5f;
    public float parryForce = 10f;
    public float parryWindow = 0.4f;
    public float parryDetectionRadius = 3.5f;

    [Header("Roll")]
    public float rollSpeed = 10f;
    public float rollDuration = 1f;
    public float catchWindow = 0.6f;
    public float rollDetectionRadius = 5f;
    public bool canPassThroughLedges = false;
}