using UnityEngine;

public interface IDamageable<T>
{
    void Damage(T amount);
}

public interface IKillable
{
    void Kill();    
}