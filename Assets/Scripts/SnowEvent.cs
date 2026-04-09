using UnityEngine;
using System.Collections;

public class SnowEventController : MonoBehaviour
{
    [Header("Event Timing")]
    public float initialDelay = 10f;
    public float eventDuration = 20f;
    public float minWaitBetween = 10f;
    public float maxWaitBetween = 30f;

    [Header("Visuals")]
    [Range(0, 1)]
    public float maxWhiteoutAlpha = 0.5f;

    [Header("References")]
    public ParticleSystem snowParticles;
    public SpriteRenderer whiteoutSprite;

    void Start()
    {   
        // Only run snow event on snowy stage
        if (VehicleManager.instance.GetStage() == 1) {
            SetWhiteoutAlpha(0);
            if (snowParticles.isPlaying) snowParticles.Stop();
            StartCoroutine(SnowLoop());
        }
    }

    IEnumerator SnowLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            float fadeInTime = eventDuration * (2f / 3f);
            float fadeOutTime = eventDuration * (1f / 3f);

            snowParticles.Play();
            yield return StartCoroutine(FadeWhiteout(0, maxWhiteoutAlpha, fadeInTime));

            snowParticles.Stop(); 
            yield return StartCoroutine(FadeWhiteout(maxWhiteoutAlpha, 0, fadeOutTime));

            float randomWait = Random.Range(minWaitBetween, maxWaitBetween);
            yield return new WaitForSeconds(randomWait);
        }
    }

    IEnumerator FadeWhiteout(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetWhiteoutAlpha(currentAlpha);
            yield return null;
        }
        SetWhiteoutAlpha(endAlpha);
    }

    void SetWhiteoutAlpha(float alpha)
    {
        if (whiteoutSprite == null) return;
        Color tempColor = whiteoutSprite.color;
        tempColor.a = alpha;
        whiteoutSprite.color = tempColor;
    }
}
