using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] protected int coinValue;
    [SerializeField] protected float coinSpeed;



    protected void OnMouseDown()
    {
        Destroy(gameObject);
    }
}

