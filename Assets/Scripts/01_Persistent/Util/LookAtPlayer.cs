using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float rotateSpeed = 10f;

    private Transform player;
    private Quaternion defaultRotation;
    private float sqrDistance;
     
    private void Awake()
    {
        player = GameInstance.Instance.PLAYER_GetPlayerTransform();
        defaultRotation = transform.rotation;
        sqrDistance = distance * distance;
    }
    private void Update()
    {
        if (!player)
            return;

        Vector3 diff = player.position - transform.position;
        diff.y = 0f;

        float sqrMag = diff.sqrMagnitude;
        Quaternion targetRot;

        if (sqrMag <= sqrDistance && sqrMag > 0.0001f)
        {
            targetRot = Quaternion.LookRotation(diff);
        }
        else
        {
            targetRot = defaultRotation;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}
