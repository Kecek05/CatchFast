using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class SpawnableObject : NetworkBehaviour
{
    [SerializeField] protected SpawnableObjectSO spawnableObjectSO;

    private void Start()
    {
        if (IsServer)
        {
            //transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        // Move the object up based on its rotation
        transform.position += transform.up * spawnableObjectSO.speed;
    }




    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public SpawnableObjectSO GetSpawnableObjectSO()
    {
        return spawnableObjectSO;
    }

    public void SetSpawnableObjectTargetPosition(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        Instantiate(GameController.Instance.debugCircleRed, direction, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

    }


    #region Spawn/ Destroy Methods

    public static void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position, Vector2 targetPosition)
    {
        GameControllerMultiplayer.Instance.SpawnSpawnableObject(spawnableObjectSO, position, targetPosition);
    }

    public static void DestroySpawanableObject(SpawnableObject spawnableObject)
    {
        GameControllerMultiplayer.Instance.DestroySpawnableObject(spawnableObject);
    }


    #region DEBUG

    //[Command]
    //public static void SpawnSilverCoinAtCursorDebug()
    //{
    //    Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    cursorPosition.z = 0; // Ensure the z-coordinate is set to 0 for 2D games

    //    SpawnSpawnableObject(GameControllerMultiplayer.Instance.silverCoinSODEBUG, new Vector2(cursorPosition.x, cursorPosition.y));
    //    print("Spawned at " + cursorPosition);
    //}

    //[Command]
    //public static void SpawnSilverCoinAtMiddleDebug()
    //{
    //    SpawnSpawnableObject(GameControllerMultiplayer.Instance.silverCoinSODEBUG, new Vector2(0, 3));
    //    print("Spawned at " + new Vector2(0, 3));
    //}


    #endregion

    #endregion





    protected void OnMouseDown()
    {
        GameController.Instance.HitSpawnableObject(this);
    }
}

