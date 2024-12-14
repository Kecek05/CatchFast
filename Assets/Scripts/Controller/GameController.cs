using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    private const float SPAWNABLE_OBJECTS_SPAWN_OFFSET = 5f;
    private const float SPAWNABLE_SCREEN_EDGE_OFFSET = 35f;

    public event Action<int> OnPlayer1ScoreChanged;
    public event Action<int> OnPlayer2ScoreChanged;


    private NetworkVariable<int> player1Score = new(0);
    private NetworkVariable<int> player2Score = new(0);


    public GameObject debugCircleYellow;
    public GameObject debugCircleRed;

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
        print("All clients have loaded the scene");

        StartCoroutine(SpawnCoinsCoroutine());
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

    public IEnumerator SpawnCoinsCoroutine()
    {

        while(true)
        {
            yield return new WaitForSeconds(2f);
            (Vector2 randomPosition, Vector2 oppositePosition) = GetRandomEdgePositionWithOpposite();
            SpawnableObject.SpawnSpawnableObject(GameControllerMultiplayer.Instance.GetRandomSpawnableObjectSO(), randomPosition, oppositePosition);
            print("Spawned coin at " + randomPosition);
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

        screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        screenY = Screen.height + SPAWNABLE_SCREEN_EDGE_OFFSET;
        position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));

        screenXOpposite = UnityEngine.Random.Range(0, Screen.width);
        screenYOpposite = -SPAWNABLE_SCREEN_EDGE_OFFSET;

        oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(screenXOpposite, screenYOpposite, Camera.main.nearClipPlane));

        Instantiate(debugCircleYellow, position, Quaternion.identity);
        Instantiate(debugCircleRed, oppositePosition, Quaternion.identity);

        print("ScreenX: " + screenX + " ScreenY: " +  screenY + " Position: " + position);
        print("ScreenXOpposite: " + screenXOpposite + " ScreenYOpposite: " + screenYOpposite + " Opposite Position: " + oppositePosition);

        //switch (side)
        //{
        //    case 0: // Top side
        //        screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        //        screenY = Screen.height + SPAWNABLE_OBJECTS_SPAWN_OFFSET;
        //        position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));
        //        oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET), SPAWNABLE_OBJECTS_SPAWN_OFFSET, Camera.main.nearClipPlane));
        //        break;
        //    case 1: // Bottom side
        //        screenX = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        //        screenY = SPAWNABLE_OBJECTS_SPAWN_OFFSET;
        //        position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));
        //        oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET), Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET, Camera.main.nearClipPlane));
        //        break;
        //    case 2: // Left side
        //        screenX = SPAWNABLE_OBJECTS_SPAWN_OFFSET;
        //        screenY = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        //        position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));
        //        oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET, UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET), Camera.main.nearClipPlane));
        //        break;
        //    case 3: // Right side
        //        screenX = Screen.width - SPAWNABLE_OBJECTS_SPAWN_OFFSET;
        //        screenY = UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET);
        //        position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, Camera.main.nearClipPlane));
        //        oppositePosition = Camera.main.ScreenToWorldPoint(new Vector3(SPAWNABLE_OBJECTS_SPAWN_OFFSET, UnityEngine.Random.Range(SPAWNABLE_OBJECTS_SPAWN_OFFSET, Screen.height - SPAWNABLE_OBJECTS_SPAWN_OFFSET), Camera.main.nearClipPlane));
        //        break;
        //    default:
        //        position = Vector2.zero;
        //        oppositePosition = Vector2.zero;
        //        break;
        //}


        return (position, oppositePosition);
    }


    #endregion

}
