using UnityEngine;

public class BasketManager : MonoBehaviour
{
    public GameObject basketApplePrefab;
    public Transform basketDropPoint; // 生成点位置（应在篮子正上方）

    public void PlaceAppleInBasket()
    {
        Instantiate(basketApplePrefab, basketDropPoint.position, Quaternion.identity);
    }
}
