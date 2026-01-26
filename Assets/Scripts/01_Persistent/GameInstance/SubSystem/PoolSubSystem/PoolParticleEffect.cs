using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PoolParticleEffect : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }
     
    private void OnEnable()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }
     
    private void OnParticleSystemStopped()
    {
        GameInstance.Instance.POOL_Return(gameObject);
    }
}
 