using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.CullingGroup;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    private const float SPAWNABLE_OBJECTS_SPAWN_OFFSET = 40f;
    private const float SPAWNABLE_SCREEN_EDGE_OFFSET = 200f;

    public event Action<int> OnPlayer1ScoreChanged;
    public event Action<int> OnPlayer2ScoreChanged;
    public event Action<int> OnPlayerWin;
    public event Action OnGameStateChanged;

    private NetworkVariable<int> player1Score = new(0);
    private NetworkVariable<int> player2Score = new(0);

    //Game States
    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    private NetworkVariable<GameState> gameState = new(GameState.WaitingToStart);

    private List<SpawnableObjectSO> spawnableObjectSOCommonList = new List<SpawnableObjectSO>();
    private List<SpawnableObjectSO> spawnableObjectSOEpicList = new List<SpawnableObjectSO>();
    private List<SpawnableObjectSO> spawnableObjectSOLegendaryList = new List<SpawnableObjectSO>();

    public GameObject debugCircleYellow;
    public GameObject debugCircleRed;

    [SerializeField] private float delayToSpawnCoins = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #region GameState Control

    private void ChangeGameState(GameState newState)
    {
        if(newState != gameState.Value)
        {
            switch (newState)
            {
                case GameState.WaitingToStart:
                    //Waiting for all players to connect
                    break;
                case GameState.CountdownToStart:
                    //Countdown to start the game
                    break;
                case GameState.GamePlaying:
                    //Game On
                    StartCoroutine(SpawnCoinsCoroutine());
                    break;
                case GameState.GameOver:
                    //Game Over, show win screen
                    break;
            }
        }
    }

    #endregion


    #region Network

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += State_OnValueChanged;
        player1Score.OnValueChanged += Player1Score_OnValueChanged;
        player2Score.OnValueChanged += Player2Score_OnValueChanged;


        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }


        foreach (SpawnableObjectSO spawnableObjectSO in GameControllerMultiplayer.Instance.GetSpawnableObjectSOList())
        {
            if (spawnableObjectSO.rarity == SpawnableObjectSO.SpawnableObjectRarity.Common)
            {
                spawnableObjectSOCommonList.Add(spawnableObjectSO);
            }
            else if (spawnableObjectSO.rarity == SpawnableObjectSO.SpawnableObjectRarity.Epic)
            {
                spawnableObjectSOEpicList.Add(spawnableObjectSO);
            }
            else if (spawnableObjectSO.rarity == SpawnableObjectSO.SpawnableObjectRarity.Legendary)
            {
                spawnableObjectSOLegendaryList.Add(spawnableObjectSO);
            }
        }
    }

    private void State_OnValueChanged(GameState previousValue, GameState newValue)
    {
        
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        print("All clients have loaded the scene");

        ChangeGameState(GameState.CountdownToStart);
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
        CheckScoreToWinServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckScoreToWinServerRpc()
    {
        if (player1Score.Value >= 10)
        {
            print("Player 1 Wins");
            CastPlayerWinClientRpc(1);
        }
        else if (player2Score.Value >= 10)
        {
            print("Player 2 Wins");
            CastPlayerWinClientRpc(2);
        }
    }



    [ClientRpc]
    private void CastPlayerWinClientRpc(int playerNumber)
    {
        OnPlayerWin?.Invoke(playerNumber);
    }


    #endregion


    #region Spawn Controller 

    public IEnumerator SpawnCoinsCoroutine()
    {

        while(true)
        {
            yield return new WaitForSeconds(delayToSpawnCoins);
            if (!IsInGamePlayingState()) break;

            (Vector2 randomPosition, Vector2 oppositePosition) = GetRandomEdgePositionWithOpposite();
            SpawnableObject.SpawnSpawnableObject(GetRandomSpawnableObjectSO(), randomPosition, oppositePosition);
        }
    }


    private Vector2 GetRandomPositionInScreen()
    {
        float screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        float screenY = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));
        return new Vector2(worldPosition.x, worldPosition.y);
    }

    private (Vector2, Vector2) GetRandomEdgePositionWithOpposite()
    {
        float screenX, screenY, screenXOpposite, screenYOpposite;
        Vector2 position, oppositePosition;

        // Determine which side to spawn on
        int side = UnityEngine.Random.Range(0, 4);


        switch(side)
        {
            case 0: // Top Side
                screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                screenY = Screen.height + SPAWNABLE_SCREEN_EDGE_OFFSET;
                position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));

                screenXOpposite = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                screenYOpposite = -SPAWNABLE_SCREEN_EDGE_OFFSET;

                oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenXOpposite, screenYOpposite, Camera.main.nearClipPlane));

                break;
            case 1: // Bottom Side
                screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                screenY = -SPAWNABLE_SCREEN_EDGE_OFFSET;
                position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));

                screenXOpposite = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                screenYOpposite = Screen.height + SPAWNABLE_SCREEN_EDGE_OFFSET;

                oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenXOpposite, screenYOpposite, Camera.main.nearClipPlane));

                break;
            case 2: // Left Side
                screenX = -SPAWNABLE_SCREEN_EDGE_OFFSET;
                screenY = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));

                screenXOpposite = Screen.width + SPAWNABLE_SCREEN_EDGE_OFFSET;
                screenYOpposite = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);

                oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenXOpposite, screenYOpposite, Camera.main.nearClipPlane));
                break;
            case 3: // Right Side
                screenX = Screen.width + SPAWNABLE_SCREEN_EDGE_OFFSET;
                screenY = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
                position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));

                screenXOpposite = -SPAWNABLE_SCREEN_EDGE_OFFSET;
                screenYOpposite = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);

                oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenXOpposite, screenYOpposite, Camera.main.nearClipPlane));
                break;
            default:
                position = Vector2.zero;
                oppositePosition = Vector2.one;
                break;
        }


        Instantiate(debugCircleYellow, position, Quaternion.identity);
        Instantiate(debugCircleRed, oppositePosition, Quaternion.identity);
        return (position, oppositePosition);

    }



    public SpawnableObjectSO GetRandomSpawnableObjectSO()
    {
        while(true)
        {
            int randomNumber = UnityEngine.Random.Range(1, 101); // 1 - 100

            //rarest to common
            if (randomNumber <= (int)SpawnableObjectSO.SpawnableObjectRarity.Legendary && spawnableObjectSOLegendaryList.Count > 0)
            {
                return spawnableObjectSOLegendaryList[UnityEngine.Random.Range(0, spawnableObjectSOLegendaryList.Count)];
            }
            else if (randomNumber <= (int)SpawnableObjectSO.SpawnableObjectRarity.Epic && spawnableObjectSOEpicList.Count > 0)
            {
                return spawnableObjectSOEpicList[UnityEngine.Random.Range(0, spawnableObjectSOEpicList.Count)];
            }
            else if (randomNumber <= (int)SpawnableObjectSO.SpawnableObjectRarity.Common && spawnableObjectSOCommonList.Count > 0)
            {
                return spawnableObjectSOCommonList[UnityEngine.Random.Range(0, spawnableObjectSOCommonList.Count)];
            }

        }
    }


    #endregion

    #region GameState Checks

    public bool IsInGamePlayingState()
    {
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsInCountdownToStartState()
    {
        return gameState.Value == GameState.CountdownToStart;
    }

    #endregion

}
