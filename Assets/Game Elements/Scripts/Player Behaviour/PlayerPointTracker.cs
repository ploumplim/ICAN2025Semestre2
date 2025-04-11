using UnityEngine;
using UnityEngine.Serialization;

public class PlayerPointTracker : MonoBehaviour
{
    [FormerlySerializedAs("Points")] public int points;

    public void AddPoints(int addpoints)
    {
        this.points += addpoints;
    }

    public void ResetPoints()
    {
        points = 0;
    }
}
