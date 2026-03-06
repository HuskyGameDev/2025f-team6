using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class PlayTimelineRegularly : MonoBehaviour
{
    private PlayableDirector director;
    public LaneEnable laneEnable;
    public GameObject criminal;
    public GameObject[] lanes;
    public float interval = 15f;
    public float offset = 15f;   
    public bool play = true;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        if (director != null)
        {
            StartCoroutine(PlayTimelineRoutine(offset));
        }
    }

    IEnumerator PlayTimelineRoutine(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (play)
        {
            // Randomize Lane
            int laneIndex = Random.Range(0, lanes.Length);

            // Set randomized lane parent and local position of criminal.
            criminal.transform.SetParent(lanes[laneIndex].transform);
            criminal.transform.localPosition = Vector3.zero;
            criminal.transform.localRotation = Quaternion.identity;

            // Disable obstacle spawning in that lane.
            laneEnable.DisableLane(laneIndex);

            // Play animation and wait for it to end.
            director.Play();
            yield return new WaitForSeconds((float) director.duration);

            // Enable obstacle spawning in that lane.
            laneEnable.EnableLanes();

            // Wait for designated interval between animations.
            yield return new WaitForSeconds(interval);

        }
        
    }
}
