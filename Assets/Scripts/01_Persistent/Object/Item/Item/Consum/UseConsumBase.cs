using UnityEngine;

public abstract class UseConsumBase : ConsumBase
{
    private bool activeATryUse = false; 
    private float time = 0f; 
      
    private void Update()
    {
        if (!activeATryUse)
            return;

        time += Time.deltaTime;
        if (time >= GetRecoverTime())
        {
            Detach();
            OnUseCompleted();
        }
    }

    public override void Drop()
    {
        base.Drop();
        cachedDuckStorage = null;
    }
    public override void Attach()
    {
        base.Attach();
        time = 0f;
        activeATryUse = true;

    }
    public override void Detach()
    {
        base.Detach();
        activeATryUse = false;
    }
    public override float GetUseRatio()
    {
        return (time / GetRecoverTime());
    } 

    protected abstract float GetRecoverTime();
    protected abstract void OnUseCompleted();


}
