using System;
using UnityEngine;

public class LaneControl : MonoBehaviour
{
    public int distance;
    public GameObject lane;
    static private int numLanes = 5;
    static private GameObject[] lanes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lanes = lane.GetComponents<GameObject>();
    }

    /**
     * getLane 
     * Input number of lane you would like to get and it returns it
     * Returns null if out of bounds
     */
    /*public GameObject getLane(int num)
    {
        try
        {
            return lanes[num];
        }
        catch (Exception e)
        {
            debu
            return null;
        }
    }
    /**
     * getLaneLeft
     * Input number of lane you would like to get and it returns the lane to the left
     * Returns null if out of bounds
     */
    /*public GameObject getLaneLeft(int num)
    {
        try
        {
            return lanes[num-1];
        }
        catch (Exception e)
        {
            return null;
        }
    }
    /**
     * getLaneRight
     * Input number of lane you would like to get and it returns the lane to the left
     * Returns null if out of bounds
     */
    /*public GameObject getLaneRightt(int num)
    {
        try
        {
            return lanes[num+1];
        }
        catch (Exception e)
        {
            return null;
        }
    }//*/
}
