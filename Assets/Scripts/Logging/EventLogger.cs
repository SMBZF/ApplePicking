using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    private string filePath;
    private StreamWriter writer;

    void Start()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Path.Combine(Application.persistentDataPath, $"EventLog_{timestamp}.csv");

        // 打开文件并写表头
        writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine("Timestamp,EventName,TrialID,HandUsed");
        writer.Flush();

        Debug.Log("[EventLogger] Writing to: " + filePath);
    }

    // 不记录手
    public void LogEvent(string eventName, int trialID)
    {
        if (writer == null) return;
        string time = System.DateTime.Now.ToString("HH:mm:ss.fff"); // 用实时时间更直观
        writer.WriteLine($"{time},{eventName},{trialID},");
        writer.Flush(); // ★关键：每次事件都落盘
    }

    // 记录手
    public void LogEvent(string eventName, int trialID, string handUsed)
    {
        if (writer == null) return;
        string time = System.DateTime.Now.ToString("HH:mm:ss.fff");
        writer.WriteLine($"{time},{eventName},{trialID},{handUsed}");
        writer.Flush(); // ★关键
    }

    // 安卓常见：应用进入后台时也先刷盘
    void OnApplicationPause(bool paused)
    {
        if (paused) writer?.Flush();
    }

    void OnDisable()
    {
        CloseWriter();
    }

    void OnApplicationQuit()
    {
        CloseWriter();
    }

    private void CloseWriter()
    {
        try { writer?.Flush(); writer?.Close(); }
        catch { /* ignore */ }
        finally { writer = null; }
    }
}
