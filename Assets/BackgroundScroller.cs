//using UnityEngine;

//public class BackgroundScroller : MonoBehaviour
//{
//    [SerializeField] private bool isScrolling = true;
//    [SerializeField] public float scrollSpeed = 1;
//    [SerializeField] private new Camera camera;
//    private Vector3 startPos;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        startPos = this.transform.position;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (isScrolling)
//        {
//            transform.position -= new Vector3(0, scrollSpeed * Time.deltaTime, 0);
//            if (this.transform.position.y <= camera.transform.position.y - camera.orthographicSize)
//            {
//                this.transform.position = startPos;
//            }
//        }
//    }

//    void startScrolling()
//    {
//        isScrolling = true;
//    }

//    void stopScrolling()
//    {
//        isScrolling = false;
//    }

//    void setScrollSpeed(float speed)
//    {
//        scrollSpeed = speed;
//    }
//}


using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private bool isScrolling = true;

    [Tooltip("Multiplier relative to obstacle speed (1 = same speed)")]
    [SerializeField] private float speedMultiplier = 1f;

    [SerializeField] private new Camera camera;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isScrolling) return;

        GameSpeedController speedController = GameSpeedController.GetOrCreate();
        float scrollSpeed = speedController.CurrentSpeed * speedMultiplier;

        transform.position -= Vector3.up * scrollSpeed * Time.deltaTime;

        if (transform.position.y <= camera.transform.position.y - camera.orthographicSize)
        {
            transform.position = startPos;
        }
    }

    public void StartScrolling()
    {
        isScrolling = true;
    }

    public void StopScrolling()
    {
        isScrolling = false;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
}