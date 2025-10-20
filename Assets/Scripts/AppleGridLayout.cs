using UnityEngine;
using System.Collections.Generic;

public class AppleGridLayout : MonoBehaviour
{
    public GameObject applePrefab;
    public int rows = 2;
    public int columns = 6;
    public float spacingX = 0.22f; // ԭ���� 0.25����С��ƻ��������
    public float spacingY = 0.25f;

    public Transform leftHalo;
    public Transform rightHalo;
    public float forwardOffset = 1.2f;

    public float verticalOffset = 0.4f; // ����߶�����ƫ��

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

        // ����������м��ս������ԼӸ�ѹ������������ 0.85
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
                apple.SetActive(false); // ��ʼ���أ����� GameManager ����
                generatedApples.Add(apple);
            }
        }

        if (AppleGameManager.Instance != null)
        {
            AppleGameManager.Instance.appleObjects = generatedApples;
        }
    }
}
