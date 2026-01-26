using UnityEngine;

public class AiAiming : DuckAiming
{
    private Transform targetTransform;

    protected override void Update()
    {
        UpdateRecoil();
        UpdateLookTarget();
    } 
       
    private void UpdateLookTarget()
    {
        if (!targetTransform)
            return;
         
        lookDir = (targetTransform.position - transform.position).normalized;
    }

    public void SetTargetTrasform(Transform _target)
    {
        targetTransform = _target;
    } 
      
    public void SetLookDir(Vector3 _dir)
    {
        lookDir = _dir;
    } 
    public void SetTargetPoint(Vector3 _point)
    {
        targetPoint = _point;
    }

    public override Vector3 GetTargetPos()
    { 
        return base.GetTargetPos();
        // return transform.position + lookDir * 50f;
        // return targetTransform.position;
    }  
} 
