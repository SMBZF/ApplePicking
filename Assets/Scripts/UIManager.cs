using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text countText;
    public Text instructionText;
    public Text trialInfoText;

    private float countdownTime = 0f;
    private bool isCountingDown = false;

    private int currentTrialID;
    private float currentLeftGain;
    private float currentRightGain;

    void Update()
    {
        if (isCountingDown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime < 0f)
                countdownTime = 0f;

            UpdateTrialInfoDisplay();  // ʵʱ���� Trial ��Ϣ UI
        }
    }

    public void StartCountdown(float duration)
    {
        countdownTime = duration;
        isCountingDown = true;
    }

    public void StopCountdown()
    {
        isCountingDown = false;
        countdownTime = 0f;
        UpdateTrialInfoDisplay(); // ������ʾΪ 0
    }

    public void UpdateCount(int count)
    {
        countText.text = $"Picked: {count}";
    }

    public void ShowInstruction(string message)
    {
        instructionText.text = message;
    }

    // ���õ�ǰ trial ������TrialManager ���ã�
    public void SetTrialInfo(int trialID, float leftGain, float rightGain)
    {
        currentTrialID = trialID;
        currentLeftGain = leftGain;
        currentRightGain = rightGain;
        UpdateTrialInfoDisplay();
    }

    // UI��ʾˢ�·�������������ʱ��
    private void UpdateTrialInfoDisplay()
    {
        if (trialInfoText != null)
        {
            trialInfoText.text = $"Current Trial: {currentTrialID}\n" +
                                 //$"Left Hand Amplification: {currentLeftGain}\n" +
                                 //$"Right Hand Amplification: {currentRightGain}\n" +
                                 $"Time Remaining: {countdownTime:F1} seconds";
        }
    }

}
