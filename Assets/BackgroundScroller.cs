using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private bool isScrolling = true;
    [SerializeField] public float scrollSpeed = 1;
    [SerializeField] private Camera camera;
    private Vector3 startPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            transform.position -= new Vector3(0, scrollSpeed * Time.deltaTime, 0);
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
    }
}
