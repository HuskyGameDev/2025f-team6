using UnityEngine;
using System.Collections;
using TMPro;

namespace TurboPursuit.UI 
{
    public class CarouselTextBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text headline;

        [SerializeField] private bool fadeText = true;
        private float fadeDuration = 0.5f;
        private float halfFadeDuration => fadeDuration * 0.5f;
        private Coroutine fadeCoroutine;


        public void SetTextWithoutFade(string headlineText)
        {
            headline.SetText(headlineText);
            headline.alpha = 1;
        }

        public void SetText(string headlineText, float fadingDuration = 0f)
        {
            if (!fadeText || fadingDuration <= 0)
            {
                SetTextWithoutFade(headlineText);
                return;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                headline.alpha = 1;
            }

            fadeDuration = fadingDuration;
            fadeCoroutine = StartCoroutine(FadeText(headlineText));
        }

        private IEnumerator FadeText(string headlineText)
        {
            float time = 0;
            while (time < halfFadeDuration)
            {
                time += Time.deltaTime;
                float lerpValue = 1 - (time / halfFadeDuration);
                headline.alpha = lerpValue;
                yield return null;
            }

            headline.SetText(headlineText);

            time = 0;

            while (time < halfFadeDuration)
            {
                time += Time.deltaTime;
                float lerpValue = time / halfFadeDuration;
                headline.alpha = lerpValue;
                yield return null;
            }

            headline.alpha = 1;
        }

    }

}