using NUnit.Framework;
using QFSW.QC;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public static SpawnController Instance { get; private set; }

    [SerializeField] private List<GameObject> objectsToSpawn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }



    [Command]
    public static void Spawn(GameObject prefab, Vector2 position)
    {
        GameControllerMultiplayer.Instance.SpawnNetworkObject(prefab, position);
    }


}
