using System.Collections.Generic;
using UnityEngine;

public class AppleGameManager : MonoBehaviour
{
    public static AppleGameManager Instance;

    [Header("References")]
    public List<GameObject> appleObjects;
    public HaloChecker haloChecker;
    public UIManager uiManager;
    public BasketManager basketManager;
    public HandAmplifier[] handAmplifiers;

    [Header("Settings")]
    public float gameDuration = 180f;

    [Header("Control Settings")]
    private bool handLocked = false;
    public bool autoHideOtherHand = true; // 添加的公开选项开关

    private float startTime;
    private int appleCount = 0;
    private bool isGameActive = false;
    private bool awaitingNextApple = false;
    private GameObject currentApple;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HideAllApples();
        haloChecker.SetHalosVisible(true);
        uiManager.ShowInstruction("Place both hands into the halos to start");
    }

    void Update()
    {
        if (!isGameActive)
        {
            if (haloChecker.AreHandsInHalos())
            {
                StartGame();
            }
        }
        else
        {
            float elapsed = Time.time - startTime;
            float remaining = Mathf.Max(0, gameDuration - elapsed);

            if (remaining <= 0)
            {
                uiManager.ShowInstruction($"Time's up! You picked {appleCount} apples.");
                isGameActive = false;
                HideAllApples();
                haloChecker.SetHalosVisible(true);
            }
            else if (awaitingNextApple && haloChecker.AreHandsInHalos())
            {
                awaitingNextApple = false;
                haloChecker.SetHalosVisible(false);
                ShowRandomApple();
                uiManager.ShowInstruction("Please grab the apple");
            }
        }
    }

    void StartGame()
    {
        isGameActive = true;
        startTime = Time.time;
        appleCount = 0;
        awaitingNextApple = false;
        uiManager.UpdateCount(appleCount);
        uiManager.ShowInstruction("Please grab the apple");
        haloChecker.SetHalosVisible(false);
        ShowRandomApple();
    }

    public void OnAppleReleased(GameObject apple)
    {
        if (!isGameActive) return;

        var grabHandler = apple.GetComponent<AppleGrabHandler>();
        if (grabHandler != null)
            grabHandler.ResetPosition();

        apple.SetActive(false);
        appleCount++;

        foreach (var amplifier in handAmplifiers)
        {
            amplifier.StopAmplification();
        }

        basketManager.PlaceAppleInBasket();

        uiManager.UpdateCount(appleCount);
        uiManager.ShowInstruction("Place both hands into the halos to continue");
        haloChecker.SetHalosVisible(true);
        currentApple = null;
        awaitingNextApple = true;

        foreach (var amp in handAmplifiers)
        {
            amp.SetHandVisible(true);
            amp.ResetHandState();
        }
        handLocked = false;
    }

    void ShowRandomApple()
    {
        HideAllApples();

        var candidates = appleObjects.FindAll(a => a != null);
        if (candidates.Count == 0)
        {
            Debug.LogWarning("No available apples!");
            return;
        }

        int index = Random.Range(0, candidates.Count);
        currentApple = candidates[index];
        currentApple.SetActive(true);

        foreach (var amplifier in handAmplifiers)
        {
            amplifier.SetTarget(currentApple.transform);
        }

        int trialID = FindObjectOfType<TrialManager>().GetCurrentTrialID();
        FindObjectOfType<PositionRecorder>()?.LogAppleSpawn(trialID, index, currentApple.transform.position);
    }


    void HideAllApples()
    {
        foreach (var apple in appleObjects)
        {
            if (apple != null) apple.SetActive(false);
        }
    }

    public void NotifyHandExtended(HandAmplifier activeHand)
    {
        if (handLocked || !autoHideOtherHand) return; // 添加开关判断
        handLocked = true;

        foreach (var amp in handAmplifiers)
        {
            if (amp != activeHand)
                amp.SetHandVisible(false);
        }
    }
}
