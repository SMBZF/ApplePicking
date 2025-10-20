using UnityEngine;

[DefaultExecutionOrder(-100)] // 让它比别的管理器更早运行
public class HaloChecker : MonoBehaviour
{
    public static HaloChecker Instance;

    public Transform leftHand;
    public Transform rightHand;
    public Collider leftHalo;
    public Collider rightHalo;

    public GameObject leftHaloVisual;
    public GameObject rightHaloVisual;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Debug.LogWarning("Multiple HaloChecker instances found!"); Destroy(this); }

        // 兜底：如果没指定可视对象，就用碰撞体本体
        if (!leftHaloVisual && leftHalo) leftHaloVisual = leftHalo.gameObject;
        if (!rightHaloVisual && rightHalo) rightHaloVisual = rightHalo.gameObject;
    }

    void Start()
    {
        SetHalosVisible(true); // 启动时就先显示，避免被流程顺序“吃掉”
        Debug.Log($"[HaloChecker] LVis:{(leftHaloVisual ? leftHaloVisual.name : "NULL")} " +
                  $"RVis:{(rightHaloVisual ? rightHaloVisual.name : "NULL")} " +
                  $"LCol:{(leftHalo ? leftHalo.name : "NULL")} RCol:{(rightHalo ? rightHalo.name : "NULL")}");
    }

    public bool AreHandsInHalos()
    {
        if (!leftHalo || !rightHalo || !leftHand || !rightHand) return false;
        // 注意：bounds.Contains 用的是AABB，足够做“进入区域”的粗判定
        return leftHalo.bounds.Contains(leftHand.position) &&
               rightHalo.bounds.Contains(rightHand.position);
    }

    public void SetHalosVisible(bool visible)
    {
        if (leftHaloVisual) leftHaloVisual.SetActive(visible);
        if (rightHaloVisual) rightHaloVisual.SetActive(visible);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!leftHaloVisual && leftHalo) leftHaloVisual = leftHalo.gameObject;
        if (!rightHaloVisual && rightHalo) rightHaloVisual = rightHalo.gameObject;
    }
#endif
}
