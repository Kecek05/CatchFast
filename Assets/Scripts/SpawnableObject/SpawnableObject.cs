using Unity.Netcode;
using UnityEngine;

public class SpawnableObject : NetworkBehaviour
{
    [SerializeField] protected SpawnableObjectSO spawnableObjectSO;




    public static void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position)
    {
        GameControllerMultiplayer.Instance.SpawnSpawnableObject(spawnableObjectSO, position);
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    protected void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

