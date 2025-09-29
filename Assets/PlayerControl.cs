using UnityEngine;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject laneGroupObject;
    [SerializeField] private List<GameObject> lanes;
    [SerializeField] private int currentLane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Make a list of lanes
        lanes = new List<GameObject>();
        foreach (Transform childTransform in laneGroupObject.transform)
        {
            lanes.Add(childTransform.gameObject);
        }

        //Start player in the center lane
        transform.position = lanes[2].transform.GetChild(1).transform.position;
        currentLane = 2;
    }

    // Update is called once per frame
        void Update()
    {
        
    }
}
