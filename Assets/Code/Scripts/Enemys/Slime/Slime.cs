using UnityEngine;

public class Slime : MonoBehaviour{
	[Header("Current State")] [SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField] private Animator _animator;

	[Header("Slime Stats")] [SerializeField]
	private int _health;

	[SerializeField] private int points;
	[SerializeField] private bool isDead;
	[SerializeField] private int _damage;
	[SerializeField] private float _jump;
	[SerializeField] private float _cooldownJump = 2f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start(){
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
	}
	
	private void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.CompareTag("Player")){
			GameManager.instance.PlayerReceiveDamage(_damage);
		}
	}

	private void Death(){
		Destroy(gameObject, 1f);
		Destroy(this, 1f);
	}

	public void ReceiveDamage(int damage){
		if (isDead) return;
		_health -= damage;
		if (_health <= 0){
			_animator.SetTrigger("Death");
			GameManager.instance.addPoints(points);
			isDead = true;
			Death();
		}else{
			_animator.SetTrigger("Hurt");
		}
	}
}