using UnityEngine;

public class HeroKnight : MonoBehaviour{
	[SerializeField] private float m_speed = 4.0f;
	[SerializeField] private float m_jumpForce = 7.5f;
	[SerializeField] private float m_rollForce = 6.0f;
	[SerializeField] private bool m_noBlood;

	[SerializeField] private GameObject m_slideDust;

	// Variables creadas por mi
	[Header("Whip")] [SerializeField] private float m_distanceWhip = 6;

	[Header("Health")] [SerializeField] private int health = 4;

	[SerializeField] private int maxHealth = 4;

	[Header("Invincible")] [SerializeField]
	private float timeInvincible = 2.0f;

	[SerializeField] private bool isInvincible;

	[Header("Death")] [SerializeField] private bool isDead;

	[Header("Fall Damage")] [SerializeField]
	private float maxDistanceYFallDamage = 10.0f;

	[SerializeField] private float distanceYFall;
	[SerializeField] private bool doDamageFall;
	private readonly float m_rollDuration = 8.0f / 14.0f;

	private Animator m_animator;
	private Rigidbody2D m_body2d;
	private int m_currentAttack;
	private float m_delayToIdle;
	private int m_facingDirection = 1;
	private bool m_grounded;
	private Sensor_HeroKnight m_groundSensor;
	private bool m_isWallSliding;
	private float m_rollCurrentTime;
	private bool m_rolling;
	private float m_timeSinceAttack;
	private Sensor_HeroKnight m_wallSensorL1;
	private Sensor_HeroKnight m_wallSensorL2;
	private Sensor_HeroKnight m_wallSensorR1;
	private Sensor_HeroKnight m_wallSensorR2;
	[Header("Inventory")] public Inventory inventory{ get; set; } = new();

	// Use this for initialization
	private void Start(){
		health = maxHealth;
		m_animator = GetComponent<Animator>();
		m_body2d = GetComponent<Rigidbody2D>();
		m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
		m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
		m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
		m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
		m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
	}

	// Update is called once per frame
	private void Update(){
		if (isDead) return;
		if (health <= 0){
			Death();
		}
		// Increase timer that controls attack combo
		m_timeSinceAttack += Time.deltaTime;

		// Increase timer that checks roll duration
		if (m_rolling)
			m_rollCurrentTime += Time.deltaTime;

		// Disable rolling if timer extends duration
		if (m_rollCurrentTime > m_rollDuration)
			m_rolling = false;

		//Check if character just landed on the ground
		if (!m_grounded && m_groundSensor.State()){
			m_grounded = true;
			m_animator.SetBool("Grounded", m_grounded);
		}

		//Check if character just started falling
		if (m_grounded && !m_groundSensor.State()){
			m_grounded = false;
			m_animator.SetBool("Grounded", m_grounded);
		}

		// -- Handle input and movement --
		var inputX = Input.GetAxis("Horizontal");

		// Swap direction of sprite depending on walk direction
		if (inputX > 0){
			GetComponent<SpriteRenderer>().flipX = false;
			m_facingDirection = 1;
		}

		else if (inputX < 0){
			GetComponent<SpriteRenderer>().flipX = true;
			m_facingDirection = -1;
		}

		// Move
		if (!m_rolling)
			m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

		//Set AirSpeed in animator
		m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

		// -- Handle Animations --
		//Wall Slide
		m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) ||
		                  (m_wallSensorL1.State() && m_wallSensorL2.State());
		m_animator.SetBool("WallSlide", m_isWallSliding);

		//Attack
		if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling){
			m_currentAttack++;

			// Loop back to one after third attack
			if (m_currentAttack > 3)
				m_currentAttack = 1;

			// Reset Attack combo if time since last attack is too large
			if (m_timeSinceAttack > 1.0f)
				m_currentAttack = 1;

			// Call one of three attack animations "Attack1", "Attack2", "Attack3"
			m_animator.SetTrigger("Attack" + m_currentAttack);

			// Reset timer
			m_timeSinceAttack = 0.0f;

			// Attack logic
			Vector3 offset = GetComponent<SpriteRenderer>().flipX ? Vector3.left : Vector3.right;
			Vector3 start = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z) + offset;
			Debug.DrawRay(start, GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right * 2f, Color.red, 1.0f);
			var hit = Physics2D.Raycast(start, GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right, 2f);

			if (hit.collider != null){
				Debug.Log("Hit: " + hit.collider.name);
				if (hit.collider.CompareTag("Enemy")){
					Debug.Log("Enemy hit");
					hit.collider.GetComponent<Slime>().ReceiveDamage(1);
				}
				else{
					Debug.Log("Hit, but not an enemy");
				}
			}
			else{
				Debug.Log("No hit detected");
			}
		}

		// Block
		else if (Input.GetMouseButtonDown(1) && !m_rolling){
			m_animator.SetTrigger("Block");
			m_animator.SetBool("IdleBlock", true);
		}

		else if (Input.GetMouseButtonUp(1)){
			m_animator.SetBool("IdleBlock", false);
		}

		// Roll
		else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding){
			m_rolling = true;
			m_animator.SetTrigger("Roll");
			m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
		}

		//Jump
		else if (Input.GetKeyDown("space") && m_grounded && !m_rolling){
			m_animator.SetTrigger("Jump");
			m_grounded = false;
			m_animator.SetBool("Grounded", m_grounded);
			m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);
			m_groundSensor.Disable(0.2f);
		}

		//Run
		else if (Mathf.Abs(inputX) > Mathf.Epsilon){
			// Reset timer
			m_delayToIdle = 0.05f;
			m_animator.SetInteger("AnimState", 1);
		}

		//Idle
		else{
			// Prevents flickering transitions to idle
			m_delayToIdle -= Time.deltaTime;
			if (m_delayToIdle < 0)
				m_animator.SetInteger("AnimState", 0);
		}

		// Codigo creado por mi
		// Whip
		if (Input.GetKeyDown("f") && !m_rolling){
			m_animator.SetTrigger("Whip");
			AE_Whip();
			//m_animator.SetBool("IdleBlock", true);
		}

		if (Input.GetKeyDown(KeyCode.P)) GameManager.instance.PlayerReceiveDamage(1);

		damageByFall();

		if (isInvincible){
			timeInvincible -= Time.deltaTime;
			if (timeInvincible < 0){
				isInvincible = false;
				timeInvincible = 2.0f;
			}
		}

		if (health <= 0) GameManager.instance.PlayerDie();
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag("Enemy") && !isInvincible){
			GameManager.instance.PlayerReceiveDamage(1);
			isInvincible = true;
		}
	}

	public void Death(){
		m_animator.SetBool("noBlood", m_noBlood);
		m_animator.SetTrigger("Death");
		isDead = true;
	}

	public void Hurt(){
		m_animator.SetTrigger("Hurt");
	}

	private void damageByFall(){
		if (m_body2d.linearVelocity.y < 0){
			distanceYFall += m_body2d.linearVelocity.y * Time.deltaTime;
			if (distanceYFall < -maxDistanceYFallDamage) doDamageFall = true;
		}
		else{
			distanceYFall = 0;
		}

		if (m_body2d.linearVelocity.y == 0 && doDamageFall){
			GameManager.instance.PlayerReceiveDamage(1);
			doDamageFall = false;
		}
	}

	// Animation Events
	// Called in slide animation.
	private void AE_SlideDust(){
		Vector3 spawnPosition;

		if (m_facingDirection == 1)
			spawnPosition = m_wallSensorR2.transform.position;
		else
			spawnPosition = m_wallSensorL2.transform.position;

		if (m_slideDust != null){
			// Set correct arrow spawn position
			var dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation);
			// Turn arrow in correct direction
			dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
		}
	}

	// Funciones creadas por mi
	private void AE_Whip(){

		var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		var direction = (mousePosition - gameObject.transform.position).normalized;

		var distanceToMouse = Vector3.Distance(gameObject.transform.position, mousePosition);

		Debug.Log("Direccion: " + direction);
		Debug.Log("Distancia al ratón: " + distanceToMouse);
		
		if (distanceToMouse <= m_distanceWhip)
			Debug.Log("El látigo llega al objetivo.");
		else
			Debug.Log("El látigo no llega al objetivo.");

		Debug.DrawLine(gameObject.transform.position, mousePosition, Color.red, 3);
	}
}