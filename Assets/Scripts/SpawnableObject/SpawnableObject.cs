using Unity.Netcode;
using UnityEngine;

public class SpawnableObject : NetworkBehaviour
{
    [SerializeField] protected SpawnableObjectSO spawnableObjectSO;



    protected void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

