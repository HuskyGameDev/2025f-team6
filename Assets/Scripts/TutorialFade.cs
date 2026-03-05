using UnityEngine;
using System.Collections;

public class AutoFadeOrClose : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDelay = 1.5f;
    public float fadeDuration = 2.0f;
    private Coroutine fadeCoroutine;

    private KeyCode moveLeft1 = KeyCode.A;
    private KeyCode moveLeft2 = KeyCode.LeftArrow;
    private KeyCode moveRight1 = KeyCode.D;
    private KeyCode moveRight2 = KeyCode.RightArrow;
    private KeyCode honkHorn = KeyCode.Space;
    private KeyCode powerupButton = KeyCode.E;

    private void Start()
    {
        moveLeft1 = KeybindManager.GetMoveLeft1();
        moveLeft2 = KeybindManager.GetMoveLeft2();
        moveRight1 = KeybindManager.GetMoveRight1();
        moveRight2 = KeybindManager.GetMoveRight2();
        honkHorn = KeybindManager.GetHonkHorn();
        powerupButton = KeybindManager.GetUsePowerup();
    }

    private void Update()
    {
        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            ManualClose();
        }
    }


    private void OnEnable()
    {
        // Reset alpha and start the auto-fade whenever the object is shown
        canvasGroup.alpha = 1f;
        fadeCoroutine = StartCoroutine(FadeAndHide());
    }

    IEnumerator FadeAndHide()
    {

        yield return new WaitForSeconds(fadeDelay);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        
        // Hide the object completely once faded
        gameObject.SetActive(false);
    }


    public void ManualClose()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        gameObject.SetActive(false);

    }
}
