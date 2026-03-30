using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public static VehicleManager instance;
    
    [SerializeField] public Sprite playerSprite = null;
    [SerializeField] public int stage = 0;

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

    public int GetStage()
    {
        return stage;
    }

    public void SetStage(int stageNum)
    {
        stage = stageNum;
    }
}
