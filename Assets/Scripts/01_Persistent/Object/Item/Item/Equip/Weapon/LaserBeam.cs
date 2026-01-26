using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private LineRenderer line;

    [Header("Option")]
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Material laserMat;

    private DuckAiming cachedAiming;
    private Transform firePoint;
    private float maxDistance = 100f;
    private bool isActive = false;
     
    private void Awake()
    {
        gameObject.SetActive(false);
    }  

    public void Active(Transform _firePoint, float _maxDistance, DuckAiming _aiming)
    {
        gameObject.SetActive(true);

        firePoint = _firePoint;
        cachedAiming = _aiming; 
        transform.SetParent(firePoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; 
        
        isActive = true;
        line.enabled = true;
        maxDistance = _maxDistance;  
    }

    public void Disable(Transform _parent)
    {
        isActive = false;
        line.enabled = false;
        if (_parent) 
            transform.SetParent(_parent, false);
        gameObject.SetActive(false);
        firePoint = null;
        cachedAiming = null;
    }  

    public void SetMaxDistance(float _distance)
    {
        maxDistance = _distance; 
    }
    public void ActiveRaiser(bool _isActive)
    {
        isActive = _isActive;
          
        if (!isActive)
        {
            line.enabled = false;
        }

        else 
        {
            line.enabled = true;
        }
    } 

    private void Update()
    {
        if (!cachedAiming || !isActive || firePoint == null)
            return;

        Vector3 start = firePoint.position;
        Vector3 target = cachedAiming.GetTargetPos();
        Vector3 dir = (target - start).normalized;

        line.SetPosition(0, start);

        if (Physics.Raycast(start, dir, out RaycastHit hit, maxDistance, hitMask))
        {
            // 히트 시: 거기까지
            line.SetPosition(1, hit.point);
        }
        else
        {
            // 미스 시: 최대 거리
            line.SetPosition(1, start + dir * maxDistance);
        } 
    }
} 
