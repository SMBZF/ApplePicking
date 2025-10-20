using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 引入 UI 组件

public class CodeCheck : MonoBehaviour
{
    public Renderer[] targetObjects; // 四个目标物体（挂Renderer的对象）
    public Texture2D[] alphaTextures; // 9张Alpha贴图，按顺序编号1到9
    public string correctCode = "1234"; // 预定正确的4位密码
    public Animator targetAnimator1;
    public Animator targetAnimator2;

    public GameObject uiPanel; // UI 面板（包含文本）
    public Text uiText; // UI 显示的文本
    public float uiDisplayTime = 5f; // UI 显示的时间（3秒后自动隐藏）

    private Dictionary<Texture, int> textureToNumber; // 贴图到编号的映射
    private bool hasUnlocked = false; // 防止重复触发 UI

    void Start()
    {
        // 初始化贴图编号映射
        textureToNumber = new Dictionary<Texture, int>();
        for (int i = 0; i < alphaTextures.Length; i++)
        {
            textureToNumber[alphaTextures[i]] = i + 1; // 贴图1对应编号1，以此类推
        }

        // 确保 UI 默认隐藏
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    public void CheckCode()
    {
        if (targetObjects.Length != 4)
        {
            Debug.LogError("必须有4个目标物体！");
            return;
        }

        string currentCode = "";

        foreach (Renderer renderer in targetObjects)
        {
            Material mat = renderer.material;
            Texture currentAlphaTex = mat.GetTexture("_Alpha");

            if (textureToNumber.TryGetValue(currentAlphaTex, out int number))
            {
                currentCode += number.ToString();
            }
            else
            {
                Debug.LogError($"物体 {renderer.gameObject.name} 使用的Alpha贴图不在预设列表里！");
                return;
            }
        }

        //Debug.Log("当前组合编号：" + currentCode);

        if (currentCode == correctCode && !hasUnlocked)
        {
            Debug.Log("Access Granted");
            targetAnimator1.enabled = true;
            targetAnimator2.enabled = true;

            // 触发 UI 显示
            ShowSuccessUI();

            hasUnlocked = true; // 防止重复触发
        }
        //else
        //{
        //   Debug.Log("密码错误！");
        // }
    }

    private void ShowSuccessUI()
    {
        if (uiPanel != null && uiText != null)
        {
            uiPanel.SetActive(true);
            uiText.text = "Access Granted"; // 显示 "密码正确"
            Debug.Log("密码正确 - UI 显示");

            // 3 秒后隐藏 UI
            StartCoroutine(HideUIAfterDelay());
        }
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(uiDisplayTime);
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            Debug.Log("密码正确 - UI 隐藏");
        }
    }

    private void Update()
    {
        CheckCode();
    }
}
