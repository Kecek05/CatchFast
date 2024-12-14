using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectSO", menuName = "Scriptable Objects/SpawnableObjectSO")]
public class SpawnableObjectSO : ScriptableObject
{
    public GameObject prefab;
    public int pointsValue;
    public float speed;

    public SpawnableObjectRarity rarity;

    public enum SpawnableObjectRarity
    {
        //individual percentage
        Common = 100,
        Epic = 30,
        Legendary = 10,
    }
}
