using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField]
    private bool isScrolling = true;

    [SerializeField]
    public float scrollSpeed = 1;

    [SerializeField]
    private new Camera camera;
    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = this.transform.position;

        if (ScrollSpeedProvider.Instance != null)
        {
            ScrollSpeedProvider.Instance.SetBaseScrollSpeed(scrollSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            float speed = scrollSpeed;

            if (ScrollSpeedProvider.Instance != null)
            {
                speed = ScrollSpeedProvider.Instance.CurrentSpeed;
            }

            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
            if (this.transform.position.y <= camera.transform.position.y - camera.orthographicSize)
            {
                this.transform.position = startPos;
            }
        }
    }

    void startScrolling()
    {
        isScrolling = true;
    }

    void stopScrolling()
    {
        isScrolling = false;
    }

    void setScrollSpeed(float speed)
    {
        scrollSpeed = speed;

        if (ScrollSpeedProvider.Instance != null)
        {
            ScrollSpeedProvider.Instance.SetBaseScrollSpeed(scrollSpeed);
        }
    }
}
