using UnityEngine;
using System.IO;
using System.Text;

public class PositionRecorder : MonoBehaviour
{
    private string dynamicPath;

    void Start()
    {
        dynamicPath = Path.Combine(Application.persistentDataPath, "AppleSpawnLog_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");

        using (StreamWriter writer = new StreamWriter(dynamicPath, false, Encoding.UTF8))
        {
            writer.WriteLine("trialID,appleIndex,x,y,z");
        }
    }

    public void LogAppleSpawn(int trialID, int appleIndex, Vector3 position)
    {
        using (StreamWriter writer = new StreamWriter(dynamicPath, true, Encoding.UTF8))
        {
            writer.WriteLine($"{trialID},{appleIndex},{position.x:F4},{position.y:F4},{position.z:F4}");
        }
    }
}
