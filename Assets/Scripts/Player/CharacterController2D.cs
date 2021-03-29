using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	public Animator animator;

	float horizontalMove = 0f;

	public float runSpeed = 40f;

	bool jump = false;

	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private LayerMask m_WhatIsWall;                            // me: A mask determining what is a stickable wall
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform m_WallRightCheck;                        // me: A position marking where to check for stickable walls on the right
	[SerializeField] private Transform m_WallLeftCheck;							// me: A positon marking where to check for stickable walls on the left

	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	
	

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private float k_wallRadius = .2f;		// me: Radius of the overlap circle for determening collison (modifiy if collison not working properlly)
	private bool m_onWall;				// me: Whether or not the player is sticked to a stickable wall
	private bool m_onRightWall;			
	private bool m_onLeftWall;
	private bool m_wallSide;
	private float switchTime;

	private int extraJumps; // me: number of extra jumps ( current )
	public int extraJumpsValue; // me: number of extraJumpsValue this can be changed in unity ( this is a fixed number 1,2,3 (double,triple jump))

	public bool getFacingRight() { return m_FacingRight; }

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		switchTime = float.PositiveInfinity;
		extraJumps = extraJumpsValue;//init

		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}
	void Update()
	{
		horizontalMove = Input.GetAxisRaw("Horizontal");
		float verticalVelocityAnim = 0;
		float verticalVelocity = m_Rigidbody2D.velocity.y;
		if (verticalVelocity > 0.1f)
        {
			verticalVelocityAnim = 1;
        }
		else if (verticalVelocity < -0.1f)
        {
			verticalVelocityAnim = -1;
        }
		//Debug.Log(verticalVelocityAnim);
		animator.SetFloat("yVelocity", verticalVelocityAnim);  // knowing if it's  moving upwards or downwards
		animator.SetFloat("horizMovement",Mathf.Abs(horizontalMove));
	
		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			
		}
		//Debug.Log(m_Grounded);


	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		bool wasWall = m_onWall;
		m_onWall = false;
		m_onLeftWall = false;
		m_onRightWall = false;



		//me: Collider for checking if player sticked to stickable wall on the right
		Collider2D[] wallRightColliders = Physics2D.OverlapCircleAll(m_WallRightCheck.position, k_wallRadius, m_WhatIsWall);
		for (int i = 0; i < wallRightColliders.Length; i++)
		{
			if (wallRightColliders[i].gameObject != gameObject)
			{
				m_onWall = true;
				m_onRightWall = true;
				if (!wasWall)
					OnLandEvent.Invoke();
			}
		}

		//me: Collider for checking if player sticked to stickable wall on the left
		Collider2D[] wallLeftColliders = Physics2D.OverlapCircleAll(m_WallLeftCheck.position, k_wallRadius, m_WhatIsWall);
		for (int i = 0; i < wallLeftColliders.Length; i++)
		{
			if (wallLeftColliders[i].gameObject != gameObject)
			{
				m_onWall = true;
				m_onLeftWall = true;
				if (!wasWall)
					OnLandEvent.Invoke();
			}
		}


		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		// me: wall grabbing mechanism
		if (m_onWall && (m_onLeftWall || m_onRightWall))
        {
			m_Rigidbody2D.gravityScale = 0;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }
        else
        {
			m_Rigidbody2D.gravityScale = 3.5f;
		}

		if (float.IsPositiveInfinity(switchTime))
        {
			switchTime = Time.time + 0.1f;
		}

		
		//Debug.Log(m_Grounded);
		if (m_Grounded || m_onWall)
		{
			if (Time.time >= switchTime)
            {
				extraJumps = extraJumpsValue; // me: if grounded resets the number of possible 
				switchTime = float.PositiveInfinity;
			}

			
				
		}

		animator.SetBool("inAir", !m_Grounded); // as long as we are grounded or on a wall we are not jumping so jump bool from anim is false
		//Move character
		Move(horizontalMove * Time.fixedDeltaTime * runSpeed, false);
		//jump = false;


	}


	public void Move(float move, bool crouch)
	{
		

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
                {
                    m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
        //if (extraJumps == 0)
        //{
        //    jump = false;
        //}
        // If the player should jump...
        //if ((m_Grounded || m_onWall) && extraJumps ==2 && jump)
        //{
        //    // Add a vertical force to the player.
        //    //m_Grounded = false;
        //    //m_onWall = false;
        //    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.y, 0f);
        //    m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
        //    extraJumps=extraJumps-1; // me: -1 jump possibility
        //    Debug.Log("enetered 1");
        //}
        //else if ((!m_Grounded) && jump && extraJumps==1)
        //{
        //    //m_Grounded = false;
        //    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.y, 0f);
        //    m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
        //    extraJumps = extraJumps - 1;
        //    Debug.Log("enetered 1");
        //}
		if (jump && extraJumps > 0)
        {
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.y, 0f);
		    m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
		    extraJumps=extraJumps-1; // me: -1 jump possibility
		}
		jump = false;
		
	}


    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
		// Multiply the player's x local scale by -1.
		//Vector3 theScale = transform.localScale;
		//theScale.x *= -1;
		//transform.localScale = theScale;

	}
}
