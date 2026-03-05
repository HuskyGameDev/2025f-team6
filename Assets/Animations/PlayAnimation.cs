using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class PlayTimelineRegularly : MonoBehaviour
{
    private PlayableDirector director;
    public GameObject criminal;
    public GameObject[] lanes;
    public float interval = 60f;
    public float offset = 15f;
    public bool play = true;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        StartCoroutine(PlayTimelineRoutine(offset));
    }

    IEnumerator PlayTimelineRoutine(float offset)
    {
        yield return new WaitForSeconds(offset);

        while (play)
        {
            if (director != null)
            {
                // Randomize lane
                //criminal.transform.parent = lanes[Random.Range(0, 4)];
                director.Play(); // Plays the timeline from the beginning
            }
            yield return new WaitForSeconds(interval);
        }
    }
}
