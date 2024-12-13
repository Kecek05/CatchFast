using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableObjectSO", menuName = "Scriptable Objects/SpawnableObjectSO")]
public class SpawnableObjectSO : ScriptableObject
{
    public GameObject prefab;
    public int value;
    public float speed;

}
