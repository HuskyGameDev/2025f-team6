using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Drawing;

public class PlayerCutscene: MonoBehaviour
{
    // Rendering components
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store original color
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = VehicleManager.instance.GetVehicle();
        }

    }


}