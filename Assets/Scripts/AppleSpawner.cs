using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AppleSpawner : MonoBehaviour
{
    public List<GameObject> applePrefabs;
    public Transform spawnPoint;

    [Header("Amplification Targets")]
    public List<HandAmplifier> handAmplifiers;  // 引用左右手的 HandAmplifier 脚本

    private bool positionFileWritten = false;

    public void SpawnApple()
    {
        if (applePrefabs.Count == 0) return;

        GameObject prefab = null;
        int safetyCounter = 10;
        while (prefab == null && safetyCounter-- > 0)
        {
            prefab = applePrefabs[Random.Range(0, applePrefabs.Count)];
        }

        if (prefab == null)
        {
            Debug.LogWarning("No valid apple prefab found!");
            return;
        }

        GameObject newApple = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // 设置放大目标
        foreach (var amplifier in handAmplifiers)
        {
            if (amplifier != null)
                amplifier.SetTarget(newApple.transform);
        }

        // 记录事件
        TrialManager tm = FindObjectOfType<TrialManager>();
        EventLogger logger = FindObjectOfType<EventLogger>();
        if (tm != null && logger != null)
        {
            logger.LogEvent("AppleSpawned", tm.GetCurrentTrialID());
        }

        // 写入位置信息（只执行一次）
        if (!positionFileWritten)
        {
            WritePositionFile();
            positionFileWritten = true;
        }
    }

    private void WritePositionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "AppleSpawnPositions.csv");
        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.WriteLine("Type,X,Y,Z");
            writer.WriteLine($"SpawnPoint,{spawnPoint.position.x:F4},{spawnPoint.position.y:F4},{spawnPoint.position.z:F4}");

            // 记录 halo 位置
            HaloChecker halo = FindObjectOfType<HaloChecker>();
            if (halo != null)
            {
                writer.WriteLine($"HaloLeft,{halo.leftHand.position.x:F4},{halo.leftHand.position.y:F4},{halo.leftHand.position.z:F4}");
                writer.WriteLine($"HaloRight,{halo.rightHand.position.x:F4},{halo.rightHand.position.y:F4},{halo.rightHand.position.z:F4}");
            }
        }

        Debug.Log("Spawn position file written to: " + path);
    }
}
