using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    public event Action<int> OnPlayer1ScoreChanged;
    public event Action<int> OnPlayer2ScoreChanged;


    private NetworkVariable<int> player1Score = new(0);
    private NetworkVariable<int> player2Score = new(0);


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        player1Score.OnValueChanged += Player1Score_OnValueChanged;
        player2Score.OnValueChanged += Player2Score_OnValueChanged;


        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
    }


    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        print("Player " + clientId + " has joined the game");
    }


    #region On Value Changed

    private void Player1Score_OnValueChanged(int previousValue, int newValue)
    {
        OnPlayer1ScoreChanged?.Invoke(newValue);
    }

    private void Player2Score_OnValueChanged(int previousValue, int newValue)
    {
        OnPlayer2ScoreChanged?.Invoke(newValue);
    }

    #endregion


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
        if(serverRpcParams.Receive.SenderClientId == NetworkManager.ServerClientId)
        {
            //Host score point
            player1Score.Value += pointsToScore;
        } else
        {
            //Client score point
            player2Score.Value += pointsToScore;
        }
    }

    #endregion

}
