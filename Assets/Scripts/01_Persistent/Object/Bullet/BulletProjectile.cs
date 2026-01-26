using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : MonoBehaviour
{ 
    [SerializeField] private float speed = 70f;
    [SerializeField] private float lifeTime = 1.5f;

    private readonly List<GameObject> excludeObjects = new();

    private Vector3 dir;
    private float damage;
    private float lifeTimer;

    private DuckAttack cachedDuckAttack;
    private Rigidbody rb;
    private Collider col;
     
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        col.enabled = true;
        lifeTimer = 0f;
        excludeObjects.Clear();
    } 
     
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            ReturnToPool();
        }
    }
     
    public void Fire(Vector3 _dir, float _damage, DuckAttack _attack)
    {
        gameObject.SetActive(true);
         
        lifeTimer = 0f;
        col.enabled = true;

        dir = _dir.normalized; 
        damage = _damage;
        cachedDuckAttack = _attack;

        rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject hitGO = other.gameObject;

        for (int i = 0; i < excludeObjects.Count; i++)
        {
            if (excludeObjects[i] == hitGO)
                return;
        }

        IHitTarget hitTarget = hitGO.GetComponent<IHitTarget>();
        if (hitTarget != null)
        {
            if (!hitTarget.TakeDamage(damage, cachedDuckAttack))
                return;  

            col.enabled = false; // 추가 히트 방지
        } 

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector3.zero;
        GameInstance.Instance.POOL_Return(gameObject);
    }  

    public void AddExcludeObject(GameObject obj)
    {
        if (obj && !excludeObjects.Contains(obj))
            excludeObjects.Add(obj);
    }
}
