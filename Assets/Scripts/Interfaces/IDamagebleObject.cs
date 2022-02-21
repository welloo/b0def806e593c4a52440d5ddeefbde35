public interface IDamagebleObject
{
    int Health { get; }

    public void TakeDamage(int damage);
    void SetHealth(int count);
}