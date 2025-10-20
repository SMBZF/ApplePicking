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

        // ���ļ���д��ͷ
        writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine("Timestamp,EventName,TrialID,HandUsed");
        writer.Flush();

        Debug.Log("[EventLogger] Writing to: " + filePath);
    }

    // ����¼��
    public void LogEvent(string eventName, int trialID)
    {
        if (writer == null) return;
        string time = System.DateTime.Now.ToString("HH:mm:ss.fff"); // ��ʵʱʱ���ֱ��
        writer.WriteLine($"{time},{eventName},{trialID},");
        writer.Flush(); // ��ؼ���ÿ���¼�������
    }

    // ��¼��
    public void LogEvent(string eventName, int trialID, string handUsed)
    {
        if (writer == null) return;
        string time = System.DateTime.Now.ToString("HH:mm:ss.fff");
        writer.WriteLine($"{time},{eventName},{trialID},{handUsed}");
        writer.Flush(); // ��ؼ�
    }

    // ��׿������Ӧ�ý����̨ʱҲ��ˢ��
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
