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

    #region Network

    public override void OnNetworkSpawn()
    {
        player1Score.OnValueChanged += Player1Score_OnValueChanged;
        player2Score.OnValueChanged += Player2Score_OnValueChanged;


        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SpawnCoins();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        print("Player " + clientId + " has joined the game");
    }


    #endregion

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

    #region Score Point Methods

    public void HitSpawnableObject(SpawnableObject spawnableObject)
    {
        ScorePointServerRpc(spawnableObject.GetSpawnableObjectSO().pointsValue);

        SpawnableObject.DestroySpawanableObject(spawnableObject);
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


    #region Spawn Controller 

    public void SpawnCoins()
    {
        print("Spawn Coins");
    }


    #endregion

}
