using UnityEngine;

public class LaneEnable : MonoBehaviour
{
    public ObstacleSpawner obstacleSpawner;

    public void DisableLane(int lane)
    {
        obstacleSpawner.disabledLane = lane;
    }

    public void EnableLanes()
    {
        obstacleSpawner.disabledLane = -1;
    }
}
