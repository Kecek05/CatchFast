using NUnit.Framework;
using QFSW.QC;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameControllerMultiplayer : NetworkBehaviour
{
    
    public static GameControllerMultiplayer Instance { get; private set; }

    [SerializeField] private SpawnableObjectListSO spawnableObjectListSO;

    public SpawnableObjectSO silverCoinSODEBUG;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            //SpawnableObject.SpawnSilverCoinAtCursorDebug();
        }
    }

    #region Spawn and Destroy SpawnableObjects Methods

    //Spawn

    public void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position, Vector2 targetPosition)
    {
        SpawnSpawnableObjectServerRpc(GetSpawnableObjectSOIndex(spawnableObjectSO), position, targetPosition);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SpawnSpawnableObjectServerRpc(int spawnableObjectSOIndex, Vector2 position, Vector2 targetPosition)
    {
        SpawnableObjectSO spawnableObjectSO = GetSpawnableObjectSOFromIndex(spawnableObjectSOIndex);

        GameObject spawnableObjectGameObject = Instantiate(spawnableObjectSO.prefab, position, Quaternion.identity);

        NetworkObject spawnableObjectNetworkObject = spawnableObjectGameObject.GetComponent<NetworkObject>();
        spawnableObjectNetworkObject.Spawn(true);

        SpawnableObject spawnableObject = spawnableObjectGameObject.GetComponent<SpawnableObject>();

        spawnableObject.SetSpawnableObjectTargetPosition(targetPosition);


    }

    //Destroy

    public void DestroySpawnableObject(SpawnableObject spawnableObject)
    {
        DestroySpawnableObjectServerRpc(spawnableObject.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroySpawnableObjectServerRpc(NetworkObjectReference spawnableObjectNetworkObjectReference)
    {
        spawnableObjectNetworkObjectReference.TryGet(out NetworkObject spawnableObjectNetworkObject);

        if (spawnableObjectNetworkObject == null) return;

        SpawnableObject spawnableObject = spawnableObjectNetworkObject.GetComponent<SpawnableObject>();
        spawnableObjectNetworkObject.Despawn(true);
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

    public List<SpawnableObjectSO> GetSpawnableObjectSOList()
    {
        return spawnableObjectListSO.spawnableObjectSOList;
    }


    #endregion






}
