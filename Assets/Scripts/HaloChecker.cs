using UnityEngine;

[DefaultExecutionOrder(-100)] // �����ȱ�Ĺ�������������
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

        // ���ף����ûָ�����Ӷ��󣬾�����ײ�屾��
        if (!leftHaloVisual && leftHalo) leftHaloVisual = leftHalo.gameObject;
        if (!rightHaloVisual && rightHalo) rightHaloVisual = rightHalo.gameObject;
    }

    void Start()
    {
        SetHalosVisible(true); // ����ʱ������ʾ�����ⱻ����˳�򡰳Ե���
        Debug.Log($"[HaloChecker] LVis:{(leftHaloVisual ? leftHaloVisual.name : "NULL")} " +
                  $"RVis:{(rightHaloVisual ? rightHaloVisual.name : "NULL")} " +
                  $"LCol:{(leftHalo ? leftHalo.name : "NULL")} RCol:{(rightHalo ? rightHalo.name : "NULL")}");
    }

    public bool AreHandsInHalos()
    {
        if (!leftHalo || !rightHalo || !leftHand || !rightHand) return false;
        // ע�⣺bounds.Contains �õ���AABB���㹻�����������򡱵Ĵ��ж�
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
