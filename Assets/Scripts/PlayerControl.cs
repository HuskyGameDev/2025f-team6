using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Diagnostics;
using System;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private GameObject laneGroupObject;
    [SerializeField] private Transform[] lanes;
    [SerializeField] private int currentLane;
    [SerializeField] private float speed;
    [SerializeField] private float speedCurve; //Needs to be >0 and <2
    [SerializeField] private float yPos;
    private float interpolator;
    private Vector3 oldPosition;
    private float posDiff;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Start player in center lane
        transform.SetPositionAndRotation(new Vector3(lanes[2].position.x+0.01f, yPos, 0), new Quaternion());
        oldPosition = transform.position;
        currentLane = 2;
        interpolator = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the input from the player to see if they are moving over a lane
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0) currentLane--;
            interpolator = 0;
            oldPosition = transform.position;
            posDiff = Mathf.Abs(oldPosition.x - lanes[currentLane].transform.position.x);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 4) currentLane++;
            interpolator = 0;
            oldPosition = transform.position;
            posDiff = Mathf.Abs(oldPosition.x - lanes[currentLane].transform.position.x);
        }
        //Deprecated movement
        /*if (lanes[currentLane].position.x < transform.position.x) {
            transform.Translate(-speed*Time.deltaTime,0,0);
        }
        if (lanes[currentLane].position.x > transform.position.x) {
            transform.Translate(speed*Time.deltaTime,0,0);
        }//*/

        //Move the player based on the lerp between the positions and the interpolator's current value
        transform.position = new Vector3(Mathf.Lerp(oldPosition.x, lanes[currentLane].transform.position.x, interpolator), yPos, 0);

        //TLDR: These lines makes the car go slower at either extreme of its motion
        //This makes the interpolator move like a Bell Curve, we calculate the percentage of the distance travelled thus far
        //We then subtract 50% from it to get a range of -0.5 to 0.5, where either extreme occurs when close to either extreme of the position
        //We then take the absolute value of that and subtract it from 1 to make sure that in the middle the car is moving at the highest speed 
        float currentDiff = Mathf.Abs(transform.position.x - lanes[currentLane].transform.position.x);
        float diffPercent = 1 - speedCurve * Mathf.Abs(currentDiff/posDiff - 0.5f);

        //Adjust the interpolator
        interpolator += diffPercent * speed * Time.deltaTime;
    }

}
