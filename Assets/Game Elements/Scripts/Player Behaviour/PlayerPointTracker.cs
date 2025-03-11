using UnityEngine;
using UnityEngine.Serialization;

public class PlayerPointTracker : MonoBehaviour
{
    [FormerlySerializedAs("Points")] public int points;

    public void AddPoints(int points)
    {
        this.points += points;
    }

    public void ResetPoints()
    {
        points = 0;
    }
}
