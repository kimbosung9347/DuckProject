using UnityEngine;

public class SallyDuckAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct AxisAnim
    {
        public float min;
        public float max;
        public float speed;
    }

    [Header("Target")]
    [SerializeField] private Transform duckTp;

    [Header("Rotation Offset")]
    [SerializeField] private AxisAnim rotX; 
    [SerializeField] private AxisAnim rotY;
    [SerializeField] private AxisAnim rotZ;

    [Header("Position Offset")]
    [SerializeField] private AxisAnim posX;
    [SerializeField] private AxisAnim posY;
    [SerializeField] private AxisAnim posZ;

    private Vector3 baseLocalPos;
    private Vector3 baseLocalEuler;

    private void Awake()
    {
        baseLocalPos = duckTp.localPosition;
        baseLocalEuler = duckTp.localEulerAngles;
    }
     
    private void Update()
    {
        var euler = baseLocalEuler;
        euler.x += Eval(rotX);
        euler.y += Eval(rotY);
        euler.z += Eval(rotZ);
        duckTp.localEulerAngles = euler;

        var pos = baseLocalPos;
        pos.x += Eval(posX);
        pos.y += Eval(posY);
        pos.z += Eval(posZ);
        duckTp.localPosition = pos;
    }

    private float Eval(AxisAnim anim)
    {
        float t = (Mathf.Sin(Time.time * anim.speed) + 1f) * 0.5f;
        return Mathf.Lerp(anim.min, anim.max, t);
    }
}
