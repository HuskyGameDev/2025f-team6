using UnityEngine;
using UnityEngine.UI;

public class RawImageAnimator : MonoBehaviour
{
    public RawImage image;
    public Texture[] frames;
    public float frameDelay = 0.1f;

    int index;
    float timer;

    void Update()
    {
        if (image == null || frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer >= frameDelay)
        {
            timer = 0f;
            index = (index + 1) % frames.Length;
            image.texture = frames[index];
        }
    }
}
