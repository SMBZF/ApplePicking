using UnityEngine;

public class HandAmplifier : MonoBehaviour
{
    public Transform realHand;       // XR 控制器节点
    public Transform virtualHand;    // XR Rig 中的虚拟手
    [Header("Amplification")]
    [Range(1f, 3f)] public float gain = 1.4f;
    [Range(0f, 1f)] public float helpFactor = 0.7f;
    public float returnSpeed = 5f;              // 回位速度（m/s）
    public float rotationSmoothSpeed = 10f;     // 角速度（deg/s 的近似）
    public bool isLeftHand = false;

    [Header("Reach & Safety")]
    public float extendThreshold = 0.1f;        // 虚实手距离阈值（m）
    public float maxMoveSpeed = 1.5f;           // 本帧最大位移速度（m/s），代替固定Clamp 0.02
    public Transform commonRoot;                // 计算所用的共同父节点；为空则使用 virtualHand.parent

    private Vector3 lastRealLocalPos;
    private Transform target;
    private bool isAmplifying = true;
    private bool otherHandHidden = false;

    void Start()
    {
        if (realHand == null || virtualHand == null)
        {
            Debug.LogError("HandAmplifier: Please assign realHand and virtualHand.");
            enabled = false;
            return;
        }
        if (commonRoot == null) commonRoot = virtualHand.parent;

        // 初始记录在“共同父节点”的局部位置
        lastRealLocalPos = ToLocal(realHand.position);
    }

    void Update()
    {
        // 统一在局部空间做所有计算，避免父节点差异导致的“方向怪、漂移”
        Vector3 currentRealLocal = ToLocal(realHand.position);
        Vector3 meLocal = currentRealLocal - lastRealLocalPos;   // 本帧真实手位移（局部）

        if (isAmplifying && target != null)
        {
            if (meLocal.sqrMagnitude > 1e-8f)
            {
                Vector3 toTargetLocal = ToLocal(target.position) - ToLocal(virtualHand.position);
                float dot = Vector3.Dot(toTargetLocal.normalized, meLocal.normalized);

                // 仅当朝向目标移动时进行“助推放大”
                if (dot > 0.1f)
                {
                    // 用“投影”得到朝目标方向的真实分量，避免生硬的 normalized*magnitude
                    Vector3 alongTarget = Vector3.Project(meLocal, toTargetLocal.normalized);

                    float distanceToTarget = toTargetLocal.magnitude;
                    const float maxDistance = 1.0f; // 可按需要调
                    float alpha = Mathf.Clamp01(Mathf.InverseLerp(0f, maxDistance, distanceToTarget)) * helpFactor;

                    // 融合：alpha 越大越沿目标方向放大，越小越贴近真实手
                    Vector3 amplifiedLocal = alpha * alongTarget * gain + (1f - alpha) * meLocal;

                    // 速度上限（随帧率变化稳定）
                    float stepLimit = maxMoveSpeed * Time.deltaTime;
                    if (amplifiedLocal.magnitude > stepLimit) amplifiedLocal = amplifiedLocal.normalized * stepLimit;

                    // 应用到虚拟手（回到世界坐标）
                    virtualHand.position = FromLocal(ToLocal(virtualHand.position) + amplifiedLocal);
                }
                else
                {
                    // 朝相反或侧向移动：不放大，直接跟随真实手位移
                    Vector3 stepLocal = meLocal;
                    float stepLimit = maxMoveSpeed * Time.deltaTime;
                    if (stepLocal.magnitude > stepLimit) stepLocal = stepLocal.normalized * stepLimit;
                    virtualHand.position = FromLocal(ToLocal(virtualHand.position) + stepLocal);
                }
            }

            // 伸出判定（世界空间距离即可）
            if (!otherHandHidden)
            {
                float extendDistance = Vector3.Distance(virtualHand.position, realHand.position);
                if (extendDistance > extendThreshold)
                {
                    // 保护：AppleGameManager 可能不存在
                    if (AppleGameManager.Instance != null)
                        AppleGameManager.Instance.NotifyHandExtended(this);
                    otherHandHidden = true;
                }
            }
        }
        else
        {
            // 回位（用速度而不是Lerp因子，体验更一致）
            Vector3 backDir = realHand.position - virtualHand.position;
            float maxBack = returnSpeed * Time.deltaTime;
            if (backDir.magnitude <= maxBack) virtualHand.position = realHand.position;
            else virtualHand.position += backDir.normalized * maxBack;
        }

        // 旋转对齐
        Quaternion desiredRotation = realHand.rotation;
        if (isLeftHand) desiredRotation *= Quaternion.AngleAxis(180f, Vector3.forward);
        virtualHand.rotation = Quaternion.Slerp(virtualHand.rotation, desiredRotation, Mathf.Clamp01(rotationSmoothSpeed * Time.deltaTime));

        lastRealLocalPos = currentRealLocal;
    }

    // —— 公共 API（保持不变） ——
    public void SetTarget(Transform newTarget) { target = newTarget; isAmplifying = true; }
    public void StopAmplification() { isAmplifying = false; }
    public void SetHandVisible(bool visible) { if (virtualHand) virtualHand.gameObject.SetActive(visible); }
    public void ResetHandState() { otherHandHidden = false; }
    public void SetAmplification(float gain) { this.gain = gain; }

    // —— 工具：世界<->局部（以 commonRoot 为参考） ——
    private Vector3 ToLocal(Vector3 worldPos)
    {
        return commonRoot ? commonRoot.InverseTransformPoint(worldPos) : worldPos;
    }
    private Vector3 FromLocal(Vector3 localPos)
    {
        return commonRoot ? commonRoot.TransformPoint(localPos) : localPos;
    }
}
