using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager instance;
    
    [SerializeField] public Sprite playerSprite = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public Sprite GetVehicle()
    {
        return playerSprite;
    }

    public void SetVehicle(Sprite sprite)
    {
        playerSprite = sprite;
    }
}
