#nullable enable

namespace SpaceFighters.Server
{
    public interface IDamagable
    {
        void ReceiveDamage(DamageInfo damage);
    }
}
