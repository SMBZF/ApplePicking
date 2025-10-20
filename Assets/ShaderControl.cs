using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderControl : MonoBehaviour
{
    public Texture2D[] alphaTextures; // 9张图片
    private int currentIndex = 0;
    private Renderer rend;
    private Material mat;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
        }
        else
        {
            Debug.LogError("No Renderer found on object: " + gameObject.name);
        }

        UpdateAlphaTexture();
    }

    // 这个方法会被其他脚本调用来触发切换
    public void NextAlphaTexture()
    {
        if (alphaTextures == null || alphaTextures.Length == 0)
        {
            Debug.LogError("Alpha textures array is empty!");
            return;
        }

        // 切换索引
        currentIndex = (currentIndex + 1) % alphaTextures.Length;

        // 更新贴图
        UpdateAlphaTexture();
    }

    private void UpdateAlphaTexture()
    {
        if (mat != null)
        {
            mat.SetTexture("_Alpha", alphaTextures[currentIndex]);
        }
    }
}

