public abstract class Enemy{
	public int damage;
	public int health;

	public abstract void ReceiveDamage(int damage);

	public abstract void Die();
}