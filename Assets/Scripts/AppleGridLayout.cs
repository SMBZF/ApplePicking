using UnityEngine;
using System.Collections.Generic;

public class AppleGridLayout : MonoBehaviour
{
    public GameObject applePrefab;
    public int rows = 2;
    public int columns = 6;
    public float spacingX = 0.22f; // 原本是 0.25，调小让苹果更靠近
    public float spacingY = 0.25f;

    public Transform leftHalo;
    public Transform rightHalo;
    public float forwardOffset = 1.2f;

    public float verticalOffset = 0.4f; // 整体高度向上偏移

    public List<GameObject> generatedApples = new List<GameObject>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Vector3 haloMid = (leftHalo.position + rightHalo.position) * 0.5f;
        Vector3 forward = (-(leftHalo.up + rightHalo.up) * 0.5f).normalized;
        Vector3 rightDir = (rightHalo.position - leftHalo.position).normalized;

        Vector3 center = haloMid + forward * forwardOffset + Vector3.up * verticalOffset;

        // 如果想再往中间收紧，可以加个压缩比例，例如 0.85
        float compressionFactor = 0.85f;
        float startX = -(columns - 1) * spacingX * 0.5f * compressionFactor;
        float startY = (rows - 1) * spacingY * 0.5f;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 offset = new Vector3(
                    startX + col * spacingX * compressionFactor,
                    startY - row * spacingY,
                    0f
                );

                Vector3 worldPos = center + rightDir * offset.x + Vector3.up * offset.y;

                GameObject apple = Instantiate(applePrefab, worldPos, Quaternion.identity, this.transform);
                apple.SetActive(false); // 初始隐藏，交给 GameManager 控制
                generatedApples.Add(apple);
            }
        }

        if (AppleGameManager.Instance != null)
        {
            AppleGameManager.Instance.appleObjects = generatedApples;
        }
    }
}
