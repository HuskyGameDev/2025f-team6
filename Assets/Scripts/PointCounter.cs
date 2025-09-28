using UnityEngine;

public class PointCounter : MonoBehaviour
{
    private float points;
    public float pointsPerSecond;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //For now just increase by a set amount per unit of time
        points += pointsPerSecond * Time.deltaTime;
    }
    
}
