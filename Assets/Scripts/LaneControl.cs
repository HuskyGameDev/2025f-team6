using UnityEngine;

public class LaneControl : MonoBehaviour
{
    public int distance;
    public GameObject lane;
    static private int numLanes = 5;
    static private int currentLanes = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (currentLanes == 0) lane.transform.Translate(3 * distance * Vector2.left);
        currentLanes++;
        lane.transform.Translate(distance*Vector2.right);
        if(currentLanes < numLanes){
            Instantiate(lane);
        }
    }
}
