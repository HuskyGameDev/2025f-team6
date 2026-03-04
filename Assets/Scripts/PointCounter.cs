using Unity.VisualScripting;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    private float points;
    private float pointsPerSecond = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Increase by a set amount per unit of time
        AddPoints(pointsPerSecond * Time.deltaTime);
    }

    public void AddPoints(float add)
    {
        points += add;
    }

    public void SetPoints(float set)
    {
        points = set;
    }
    public int GetPoints()
    {
        return (int) Mathf.Floor(points);
    }

    public void UpdatePPS(float newPPS)
    {
        pointsPerSecond = newPPS;
    }
    
}
