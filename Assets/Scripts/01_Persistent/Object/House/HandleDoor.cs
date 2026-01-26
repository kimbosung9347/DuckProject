using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
using UnityEngine.AI;
public enum EDoorState
{
    Closed,     // 완전히 닫힘
    Opening,    // 여는 중
    Opened,     // 완전히 열림
    Closing,    // 닫는 중
} 
public class HandleDoor : HandleInteractionBase
{
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 0.35f;

    private Collider doorCollider;
    private Coroutine routine;

    private EDoorState state = EDoorState.Closed;

    protected override void Awake()
    {
        doorCollider = doorPivot.GetComponent<BoxCollider>();
        CloseInstant();
    }

    // =========================
    // Interaction
    // =========================
    public override void DoInteractionToPlayer(PlayerInteraction _)
    {
        if (IsBusy())
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(
            state == EDoorState.Closed ? OpenRoutine() : CloseRoutine()
        );
    }

    public override void EndInteractionToPlayer() { }

    // =========================
    // Query (외부용)
    // =========================
    public bool IsClose()
    {
        return state == EDoorState.Closed;
    }

    public bool IsOpened()
    {
        return state == EDoorState.Opened;
    }

    public bool IsBusy()
    {
        return state == EDoorState.Opening || state == EDoorState.Closing;
    }

    // =========================
    // Routines
    // =========================
    private IEnumerator OpenRoutine()
    {
        state = EDoorState.Opening;
        doorCollider.enabled = false;

        yield return RotateDoor(0f, openAngle);

        doorCollider.enabled = true;
        state = EDoorState.Opened;
    }  

    private IEnumerator CloseRoutine()
    {
        state = EDoorState.Closing;
        doorCollider.enabled = false;

        yield return RotateDoor(openAngle, 0f);

        doorCollider.enabled = true;
        state = EDoorState.Closed;
    }

    private IEnumerator RotateDoor(float fromY, float toY)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float y = Mathf.Lerp(fromY, toY, Mathf.SmoothStep(0f, 1f, t));
            doorPivot.localRotation = Quaternion.Euler(0f, y, 0f);
            yield return null;
        }

        doorPivot.localRotation = Quaternion.Euler(0f, toY, 0f);
    }

    private void CloseInstant()
    {
        state = EDoorState.Closed;
        doorPivot.localRotation = Quaternion.identity;
        doorCollider.enabled = true;
    }
}
 