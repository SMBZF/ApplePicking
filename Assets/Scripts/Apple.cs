using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class AppleGrabHandler : MonoBehaviour
{
    private XRGrabInteractable interactable;
    private bool hasBeenGrabbed = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnGrab);
            interactable.selectExited.RemoveListener(OnRelease);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        hasBeenGrabbed = true;

        string handUsed = GetHandName(args);

        EventLogger logger = FindObjectOfType<EventLogger>();
        TrialManager tm = FindObjectOfType<TrialManager>();
        if (logger != null && tm != null)
        {
            logger.LogEvent("AppleGrabbed", tm.GetCurrentTrialID(), handUsed);
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (!hasBeenGrabbed) return;

        string handUsed = GetHandName(args);

        EventLogger logger = FindObjectOfType<EventLogger>();
        TrialManager tm = FindObjectOfType<TrialManager>();
        if (logger != null && tm != null)
        {
            logger.LogEvent("AppleReleased", tm.GetCurrentTrialID(), handUsed);
        }

        AppleGameManager.Instance.OnAppleReleased(gameObject);
    }

    private string GetHandName(BaseInteractionEventArgs args)
    {
        // ³£¼ûÃüÃû: "LeftHand Controller" / "RightHand Controller"
        string interactorName = args.interactorObject.transform.name.ToLower();
        if (interactorName.Contains("left")) return "Left";
        if (interactorName.Contains("right")) return "Right";
        return "Unknown";
    }

    public void ResetPosition()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
        hasBeenGrabbed = false;
    }
}
