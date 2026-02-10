using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;


namespace TurboPursuit.UI {
    public class Carousel : MonoBehaviour, IEndDragHandler
    {
        [Header("Parts Setup")]
        [SerializeField] private List<CarouselEntry> entries = new List<CarouselEntry>();

        [Space]
        [SerializeField] private ScrollRect scrollRect;

        [Space]
        [SerializeField] private RectTransform contentBoxHorizontal;
        [SerializeField] private Image carouselEntryPrefab;
        private List<Image> imagesForEntries = new List<Image>();

        [Header("Animation Setup")]
        [SerializeField, Range(0.25f, 1f)] private float duration = 0.5f;
        [SerializeField] private AnimationCurve easeCurve;

        [Header("Auto Scroll Setup")]
        [SerializeField] private bool autoScroll = false;
        [SerializeField] private float autoScrollInterval = 5f;
        private float autoScrollTimer;

        [Header("Info Setup")]
        [SerializeField] private CarouselTextBox textBoxController;
        [SerializeField] private Button callToAction;

        [Header("Carousel Type")]
        [SerializeField] private bool isVehicle = true;

        private int currentIndex = 0;
        private Coroutine scrollCoroutine;


        private void Reset()
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
            textBoxController = GetComponentInChildren<CarouselTextBox>();
        }

        private void Start()
        {
            int startingIndex = 0;
            foreach (var entry in entries)
            {
                Image carouselEntry = Instantiate(carouselEntryPrefab, contentBoxHorizontal);
                carouselEntry.sprite = entry.EntryGraphic;
                imagesForEntries.Add(carouselEntry);

                if (carouselEntry.sprite == VehicleManager.instance.GetVehicle())
                {
                    startingIndex = imagesForEntries.Count-1;
                }
            }

            autoScrollTimer = autoScrollInterval;

            var headline = entries[0].Headline;
            textBoxController.SetTextWithoutFade(headline);

            ScrollTo(startingIndex);
        }

        private void ClearCurrentIndex()
        {
            // callToAction.onClick.RemoveListener(entries[currentIndex].Interact);
        }

        private void ScrollToSpecificIndex(int index)
        {
            ClearCurrentIndex();
            ScrollTo(index);
        }

        public void ScrollToNext()
        {
            ClearCurrentIndex();
            currentIndex = (currentIndex + 1) % imagesForEntries.Count;
            ScrollTo(currentIndex);
        }

        public void ScrollToPrevious()
        {
            ClearCurrentIndex();
            currentIndex = (currentIndex - 1 + imagesForEntries.Count) % imagesForEntries.Count;
            ScrollTo(currentIndex);
        }

        private void ScrollTo(int index)
        {
            currentIndex = index;
            autoScrollTimer = autoScrollInterval;
            float targetHorizontalPosition = (float) currentIndex / (imagesForEntries.Count - 1);

            if (scrollCoroutine != null)
                StopCoroutine(scrollCoroutine);
            
            scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));

            var headline = entries[currentIndex].Headline;

            textBoxController.SetText(headline, duration);
            // callToAction.onClick.AddListener(entries[currentIndex].Interact);

            if (isVehicle)
            {
                VehicleManager.instance.SetVehicle(imagesForEntries[currentIndex].sprite);
            }

        }

        private IEnumerator LerpToPos(float targetHorizontalPosition)
        {
            float elapsedTime = 0f;
            float initialPos = scrollRect.horizontalNormalizedPosition;

            if (duration > 0)
            {
                while (elapsedTime <= duration)
                {
                    float easeValue = easeCurve.Evaluate(elapsedTime / duration);
                    float newPosition = Mathf.Lerp(initialPos, targetHorizontalPosition, easeValue);

                    scrollRect.horizontalNormalizedPosition = newPosition;
                    
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
        }

        private void Update()
        {
            if (!autoScroll)
                return;

            autoScrollTimer -= Time.deltaTime;
            if (autoScrollTimer <= 0)
            {
                ScrollToNext();
                autoScrollTimer = autoScrollInterval;
            }
        }


        public void OnEndDrag(PointerEventData data)
        {
            if (data.delta.x != 0)
            {
                if (data.delta.x > 0)
                    ScrollToPrevious();
                else if (data.delta.x < 0)
                    ScrollToNext();
            }
            else 
                ScrollToSpecificIndex(currentIndex);
        }

    }

}