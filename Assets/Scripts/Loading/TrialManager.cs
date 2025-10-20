using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;

public class TrialManager : MonoBehaviour
{
    [System.Serializable]
    public class TrialConfig
    {
        public int trialID;
        public float leftGain;
        public float rightGain;
        public float duration;
    }

    public string csvFileName = "TrialConfig.csv";
    public HandAmplifier leftHandAmplifier;
    public HandAmplifier rightHandAmplifier;
    public UIManager uiManager;
    public HaloChecker haloChecker;
    public EventLogger eventLogger;  // New field
    public bool IsTrialRunning => isTrialRunning;

    private List<TrialConfig> trials = new List<TrialConfig>();
    private int currentTrialIndex = 0;
    private bool isTrialRunning = false;

    void Start()
    {
        // 注意：Android 下 StreamingAssets 不能用 File.ReadAllLines 直接读
        StartCoroutine(LoadCSV_AndroidSafeThenRun());
    }

    IEnumerator LoadCSV_AndroidSafeThenRun()
    {
        yield return StartCoroutine(LoadCSV_AndroidSafe());
        StartCoroutine(RunTrialSequence());
    }

    // 兼容 Android 与编辑器/PC 的 CSV 读取
    IEnumerator LoadCSV_AndroidSafe()
    {
        trials.Clear();
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CSV load failed: " + www.error + " | " + path);
                yield break;
            }
            ParseCsvFromString(www.downloadHandler.text);
        }
#else
        if (!File.Exists(path))
        {
            Debug.LogError("CSV file not found: " + path);
            yield break;
        }
        ParseCsvFromString(File.ReadAllText(path));
        yield return null;
#endif
        Debug.Log("CSV loaded: " + trials.Count + " trials read.");
    }

    void ParseCsvFromString(string csv)
    {
        var lines = csv.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var values = line.Split(',');
            if (values.Length < 4) continue;

            TrialConfig config = new TrialConfig();
            config.trialID = int.Parse(values[0]);
            // 使用 InvariantCulture，避免部分设备小数点解析失败
            config.leftGain = float.Parse(values[1], CultureInfo.InvariantCulture);
            config.rightGain = float.Parse(values[2], CultureInfo.InvariantCulture);
            config.duration = float.Parse(values[3], CultureInfo.InvariantCulture);

            trials.Add(config);
        }
    }

    IEnumerator RunTrialSequence()
    {
        while (currentTrialIndex < trials.Count)
        {
            TrialConfig trial = trials[currentTrialIndex];

            leftHandAmplifier.SetAmplification(trial.leftGain);
            rightHandAmplifier.SetAmplification(trial.rightGain);

            Debug.Log("Waiting for Trial to start: " + trial.trialID);
            isTrialRunning = false;

            if (haloChecker != null)
            {
                haloChecker.SetHalosVisible(true);
            }

            if (uiManager != null)
            {
                uiManager.SetTrialInfo(trial.trialID, trial.leftGain, trial.rightGain);
                uiManager.ShowInstruction("Place both hands into the halos to start");
            }

            yield return new WaitUntil(() => haloChecker.AreHandsInHalos());

            Debug.Log("Starting Trial: " + trial.trialID);
            isTrialRunning = true;

            if (haloChecker != null)
            {
                haloChecker.SetHalosVisible(false);
            }

            FindObjectOfType<MovementLogger>().StartLogging();
            eventLogger?.LogEvent("TrialStart", trial.trialID);

            if (uiManager != null)
            {
                uiManager.StartCountdown(trial.duration);
                uiManager.ShowInstruction("");
            }

            float timer = 0f;
            while (timer < trial.duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (uiManager != null)
            {
                uiManager.StopCountdown();
            }

            FindObjectOfType<MovementLogger>().StopLogging();
            eventLogger?.LogEvent("TrialEnd", trial.trialID);

            Debug.Log("Trial ended: " + trial.trialID);

            currentTrialIndex++;
            isTrialRunning = false;

            if (currentTrialIndex < trials.Count)
            {
                if (haloChecker != null)
                {
                    haloChecker.SetHalosVisible(true);
                }

                if (uiManager != null)
                {
                    uiManager.ShowInstruction("Place hands back into the halos to begin the next trial");
                }

                yield return new WaitUntil(() => haloChecker.AreHandsInHalos());
            }
        }

        Debug.Log("All trials completed.");

        if (haloChecker != null)
        {
            haloChecker.SetHalosVisible(false);
        }

        if (uiManager != null)
        {
            uiManager.ShowInstruction("<b>You have completed the test. Please remove the headset.</b>");
        }
    }

    public int GetCurrentTrialID()
    {
        if (currentTrialIndex < trials.Count)
            return trials[currentTrialIndex].trialID;
        else
            return -1;
    }

    public float GetLeftGain()
    {
        if (currentTrialIndex < trials.Count)
            return trials[currentTrialIndex].leftGain;
        else
            return 0f;
    }

    public float GetRightGain()
    {
        if (currentTrialIndex < trials.Count)
            return trials[currentTrialIndex].rightGain;
        else
            return 0f;
    }
}
