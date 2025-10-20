using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class AppleSpawner : MonoBehaviour
{
    public List<GameObject> applePrefabs;
    public Transform spawnPoint;

    [Header("Amplification Targets")]
    public List<HandAmplifier> handAmplifiers;  // ���������ֵ� HandAmplifier �ű�

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

        // ���÷Ŵ�Ŀ��
        foreach (var amplifier in handAmplifiers)
        {
            if (amplifier != null)
                amplifier.SetTarget(newApple.transform);
        }

        // ��¼�¼�
        TrialManager tm = FindObjectOfType<TrialManager>();
        EventLogger logger = FindObjectOfType<EventLogger>();
        if (tm != null && logger != null)
        {
            logger.LogEvent("AppleSpawned", tm.GetCurrentTrialID());
        }

        // д��λ����Ϣ��ִֻ��һ�Σ�
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

            // ��¼ halo λ��
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
