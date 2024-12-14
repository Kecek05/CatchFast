using QFSW.QC;
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


    #region Spawn/ Destroy Methods

    public static void SpawnSpawnableObject(SpawnableObjectSO spawnableObjectSO, Vector2 position)
    {
        GameControllerMultiplayer.Instance.SpawnSpawnableObject(spawnableObjectSO, position);
    }

    public static void DestroySpawanableObject(SpawnableObject spawnableObject)
    {
        GameControllerMultiplayer.Instance.DestroySpawnableObject(spawnableObject);
    }


    #region DEBUG

    [Command]
    public static void SpawnSilverCoinAtCursorDebug()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0; // Ensure the z-coordinate is set to 0 for 2D games

        SpawnSpawnableObject(GameControllerMultiplayer.Instance.silverCoinSODEBUG, new Vector2(cursorPosition.x, cursorPosition.y));
        print("Spawned at " + cursorPosition);
    }

    [Command]
    public static void SpawnSilverCoinAtMiddleDebug()
    {
        SpawnSpawnableObject(GameControllerMultiplayer.Instance.silverCoinSODEBUG, new Vector2(0, 3));
        print("Spawned at " + new Vector2(0, 3));
    }


    #endregion

    #endregion
}

