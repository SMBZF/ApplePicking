using UnityEngine;
using System.IO;
using System.Text;

public class MovementLogger : MonoBehaviour
{
    public Transform leftRealHand;
    public Transform leftVirtualHand;
    public Transform rightRealHand;
    public Transform rightVirtualHand;

    public TrialManager trialManager; // 用于获取当前 Trial 的 ID 和增益参数

    private StreamWriter writer;
    private string filePath;
    private bool isLogging = false;

    void Start()
    {
        string filename = "MovementLog_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        filePath = Path.Combine(Application.persistentDataPath, filename);
        writer = new StreamWriter(filePath, false, Encoding.UTF8);

        writer.WriteLine("trialID,timestamp,leftRealX,leftRealY,leftRealZ,leftVirtualX,leftVirtualY,leftVirtualZ,rightRealX,rightRealY,rightRealZ,rightVirtualX,rightVirtualY,rightVirtualZ,leftGain,rightGain");
    }

    void Update()
    {
        if (!isLogging || trialManager == null) return;

        float time = Time.time;
        int trialID = trialManager.GetCurrentTrialID();
        float leftGain = trialManager.GetLeftGain();
        float rightGain = trialManager.GetRightGain();

        string line = string.Format("{0},{1:F3},{2:F4},{3:F4},{4:F4},{5:F4},{6:F4},{7:F4},{8:F4},{9:F4},{10:F4},{11:F4},{12:F4},{13:F4},{14:F4},{15:F4}",
            trialID, time,
            leftRealHand.position.x, leftRealHand.position.y, leftRealHand.position.z,
            leftVirtualHand.position.x, leftVirtualHand.position.y, leftVirtualHand.position.z,
            rightRealHand.position.x, rightRealHand.position.y, rightRealHand.position.z,
            rightVirtualHand.position.x, rightVirtualHand.position.y, rightVirtualHand.position.z,
            leftGain, rightGain
        );

        writer.WriteLine(line);
    }

    public void StartLogging()
    {
        isLogging = true;
    }

    public void StopLogging()
    {
        isLogging = false;
    }

    void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }
}
