using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public GameObject basketApplePrefab;
    public Transform basketDropPoint; // ���ɵ�λ�ã�Ӧ���������Ϸ���

    public void PlaceAppleInBasket()
    {
        Instantiate(basketApplePrefab, basketDropPoint.position, Quaternion.identity);
    }
}
