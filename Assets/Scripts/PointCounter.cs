using Unity.VisualScripting;
using UnityEngine;

public class PointCounter : MonoBehaviour
{
    [SerializeField] private float points;
    [SerializeField] private float pointsPerSecond;
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
    
}
