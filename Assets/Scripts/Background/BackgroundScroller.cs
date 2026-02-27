using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField]
    private bool isScrolling = true;

    [Tooltip("Multiplier relative to obstacle speed (1 = same speed)")]
    [SerializeField]
    private float speedMultiplier = 1f;

    [SerializeField]
    private new Camera camera;

    [Header("Panels")]
    [SerializeField]
    private List<BackgroundPanelVisual> panels = new List<BackgroundPanelVisual>();

    [SerializeField]
    private bool autoFindPanelsIfEmpty = true;

    public event Action<BackgroundPanelVisual> PanelWrapped;

    private void Reset()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        if (autoFindPanelsIfEmpty && (panels == null || panels.Count == 0))
        {
            panels = GetComponentsInChildren<BackgroundPanelVisual>(true).ToList();
        }
    }

    private void Awake()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        if (autoFindPanelsIfEmpty && (panels == null || panels.Count == 0))
        {
            panels = GetComponentsInChildren<BackgroundPanelVisual>(true).ToList();
        }
    }

    private void Update()
    {
        if (!isScrolling)
            return;

        if (camera == null)
            return;

        if (panels == null || panels.Count == 0)
            return;

        GameSpeedController speedController = GameSpeedController.GetOrCreate();
        float scrollSpeed = speedController.CurrentSpeed * speedMultiplier;
        float deltaY = scrollSpeed * Time.deltaTime;

        MovePanels(deltaY);
        WrapPanelsThatLeftView();
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

    private void MovePanels(float deltaY)
    {
        foreach (BackgroundPanelVisual panel in panels)
        {
            if (panel == null)
                continue;

            panel.transform.position -= Vector3.up * deltaY;
        }
    }

    private void WrapPanelsThatLeftView()
    {
        float cameraBottom = camera.transform.position.y - camera.orthographicSize;
        List<BackgroundPanelVisual> panelsToWrap = new List<BackgroundPanelVisual>();

        foreach (BackgroundPanelVisual panel in panels)
        {
            if (panel == null)
                continue;

            float panelHalfHeight = panel.GetWorldHeight() * 0.5f;
            float panelTopY = panel.transform.position.y + panelHalfHeight;

            if (panelTopY <= cameraBottom)
            {
                panelsToWrap.Add(panel);
            }
        }

        if (panelsToWrap.Count == 0)
            return;

        panelsToWrap = panelsToWrap.OrderBy(p => p.transform.position.y).ToList();

        foreach (BackgroundPanelVisual panel in panelsToWrap)
        {
            WrapSinglePanel(panel);
        }
    }

    private void WrapSinglePanel(BackgroundPanelVisual panel)
    {
        if (panel == null)
            return;

        BackgroundPanelVisual currentTopmost = FindTopmostPanelExcluding(panel);
        float wrappedHeight = panel.GetWorldHeight();
        Vector3 position = panel.transform.position;

        if (currentTopmost != null)
        {
            float topHeight = currentTopmost.GetWorldHeight();
            float newY =
                currentTopmost.transform.position.y + (topHeight * 0.5f) + (wrappedHeight * 0.5f);

            position.y = newY;
        }
        else
        {
            float cameraTop = camera.transform.position.y + camera.orthographicSize;
            position.y = cameraTop + (wrappedHeight * 0.5f);
        }

        panel.transform.position = position;
        PanelWrapped?.Invoke(panel);
    }

    private BackgroundPanelVisual FindTopmostPanelExcluding(BackgroundPanelVisual excludedPanel)
    {
        BackgroundPanelVisual topmost = null;
        float topY = float.NegativeInfinity;

        foreach (BackgroundPanelVisual panel in panels)
        {
            if (panel == null || panel == excludedPanel)
                continue;

            if (panel.transform.position.y > topY)
            {
                topmost = panel;
                topY = panel.transform.position.y;
            }
        }

        return topmost;
    }
}
