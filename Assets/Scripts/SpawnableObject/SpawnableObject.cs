using Unity.Netcode;
using UnityEngine;

public class SpawnableObject : NetworkBehaviour
{
    [SerializeField] protected SpawnableObjectSO spawnableObjectSO;






    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public SpawnableObjectSO GetSpawnableObjectSO()
    {
        return spawnableObjectSO;
    }

    protected void OnMouseDown()
    {
        GameController.Instance.HitSpawnableObject(this);
    }
}

