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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Make a list of lanes
        Transform[] temp = laneGroupObject.GetComponentsInChildren<Transform>();
        lanes = new Transform[5];
        //If someone can think of a smarter way to filter out the player spot transforms than hard coding, be my guest
        for (int i = 1; i <= 5; i++)
        {
            this.lanes[i - 1] = temp[i * 3];
        }//*/

        //Start player in the center lane
        transform.position = lanes[2].position;
        currentLane = 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Get the input from the player to see if they are moving over a lane
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0) currentLane--;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 4) currentLane++;
        }
        //Move the player toward the target lane
        //transform.position = lanes[currentLane].position;
        if (lanes[currentLane].position.x < transform.position.x) {
            transform.Translate(-speed*Time.deltaTime,0,0);
        }
        if (lanes[currentLane].position.x > transform.position.x) {
            transform.Translate(speed*Time.deltaTime,0,0);
        }//*/
    }

}
