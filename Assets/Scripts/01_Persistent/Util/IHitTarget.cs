using UnityEngine;

public interface IHitTarget
{
    // TakeDamage가 성공했으면 True
    bool TakeDamage(float _damage, DuckAttack _duckAttack);
}        