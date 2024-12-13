using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class GameControllerMultiplayer : MonoBehaviour
{
    
    public static GameControllerMultiplayer Instance { get; private set; }

    [SerializeField] private SpawnableObjectListSO spawnableObjectListSO;

    [SerializeField] private SpawnableObjectSO silverCoinSODEBUG;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region Spawn SpawnableObjects Methods

    public void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position)
    {
        SpawnSpawnableObjectServerRpc(GetSpawnableObjectSOIndex(spawnableObjectSO), position);
    }



    [Command]
    private void SpawnSilverCoinDebug(Vector2 position)
    {
        SpawnSpawnableObjectServerRpc(GetSpawnableObjectSOIndex(silverCoinSODEBUG), position);
        print("Spawned at " + position);
    }

    [Rpc(SendTo.Server)]
    public void SpawnSpawnableObjectServerRpc(int spawnableObjectSOIndex, Vector2 position)
    {
        SpawnableObjectSO spawnableObjectSO = GetSpawnableObjectSOFromIndex(spawnableObjectSOIndex);

        GameObject spawnableObjectGameObject = Instantiate(spawnableObjectSO.prefab, position, Quaternion.identity);

        NetworkObject spawnableObjectNetworkObject = spawnableObjectGameObject.GetComponent<NetworkObject>();
        spawnableObjectNetworkObject.Spawn(true);

    }


    #endregion

    #region SpawnableObjectSOList Methods
    public int GetSpawnableObjectSOIndex(SpawnableObjectSO spawnableObjectSO)
    {
        return spawnableObjectListSO.spawnableObjectSOList.IndexOf(spawnableObjectSO);
    }

    public SpawnableObjectSO GetSpawnableObjectSOFromIndex(int spawnableObjectSOIndex)
    {
        return spawnableObjectListSO.spawnableObjectSOList[spawnableObjectSOIndex];
    }

    #endregion
}
