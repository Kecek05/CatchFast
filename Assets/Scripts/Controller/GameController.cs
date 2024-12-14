using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{

    private Dictionary<ulong, NetworkVariable<int>> playerScore;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerScore = new Dictionary<ulong, NetworkVariable<int>>();
    }

    #region Spawn/ Destroy Methods

    public static void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position)
    {
        GameControllerMultiplayer.Instance.SpawnSpawnableObject(spawnableObjectSO, position);
    }

    public static void DestroySpawanableObject(SpawnableObject spawnableObject)
    {
        GameControllerMultiplayer.Instance.DestroySpawnableObject(spawnableObject);
    }

    #endregion

    #region Score Point Methods

    public void HitSpawnableObject(SpawnableObject spawnableObject)
    {

        ScorePointServerRpc(spawnableObject.GetSpawnableObjectSO().pointsValue);



        GameControllerMultiplayer.Instance.DestroySpawnableObject(spawnableObject);
    }



    [ServerRpc(RequireOwnership = false)]
    private void ScorePointServerRpc(int pointsToScore, ServerRpcParams serverRpcParams = default)
    {
        playerScore[serverRpcParams.Receive.SenderClientId].Value += pointsToScore;

        Debug.Log(playerScore[serverRpcParams.Receive.SenderClientId].Value + " of player " + serverRpcParams.Receive.SenderClientId);
    }

    #endregion
}
