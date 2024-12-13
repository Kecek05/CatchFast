using UnityEngine;

public class GameControllerMultiplayer : MonoBehaviour
{
    
    public static GameControllerMultiplayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void SpawnNetworkObject(GameObject prefab, Vector2 position)
    {

    }
}
