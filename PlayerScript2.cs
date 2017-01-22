using UnityEngine;
using System.Collections;

public class PlayerScript2 : MonoBehaviour {

	public float moveSpeed;

	private Rigidbody2D myRigidbody;

	FrameAnimate myFrameAnimate;

	private bool facingRight;
	public float jumpForce = 100;
	public bool canMove = true;
	public bool canAttack = true;
	public bool isAttacking = false;
	public GameObject waveProjectile;
	public AudioSource audioSource;
	public bool lightAttackQueued = false;
	public bool heavyAttackQueued = false;
	public bool dashAttackQueued = false;
	public bool unqueuedThisAnim = false;
	public bool alreadyPerformedAttack = false;
	public bool grounded = false;
	public bool isJumping = false;
	public GameObject groundCheck;
	public float dashCooler = .75f;
	public int buttonCount = 0;
	public float dashSpeed;
	public bool isDashing;
	public bool isDashAttacking;


	public AudioClip heavyAttack1SoundClip;
	public AudioClip heavyAttack2SoundClip;
	public AudioClip heavyAttack3SoundClip;
	public AudioClip heavyAttack4SoundClip;
	public AudioClip heavyAttack5SoundClip;
	public AudioClip lightAttack1SoundClip;
	public AudioClip lightAttack2SoundClip;
	public AudioClip lightAttack3SoundClip;
	public AudioClip dashAttackSoundClup;

	// Use this for initialization
	void Start () {
		facingRight = true;
		myRigidbody = GetComponent<Rigidbody2D> ();
		myFrameAnimate = myRigidbody.GetComponent<FrameAnimate> ();
		dashCooler = 0f;
	}

	void Update(){
		grounded = Physics2D.Linecast (transform.position, groundCheck.transform.position, 1 << LayerMask.NameToLayer ("ground"));

		if (Input.GetButtonDown ("Jump") && grounded) {
			isJumping = true;
		}
	}

	void FixedUpdate () {

		//Horizontal movement system.
		float horizontal = Input.GetAxis ("Horizontal");
		if (!grounded && canMove) {
			myFrameAnimate.SetAnimation ("jump");
		}
		/*
		if (canMove) {
			myRigidbody.velocity = new Vector2 (horizontal * moveSpeed, myRigidbody.velocity.y);
			if (grounded) {
				if (horizontal > 0 || horizontal < 0)
					myFrameAnimate.SetAnimation ("walk");
				else {
					myFrameAnimate.SetAnimation ("idle");
				}
			}
		}*/

		if (canMove && !isDashing) {
			myRigidbody.velocity = new Vector2 (horizontal * moveSpeed, myRigidbody.velocity.y);
			if (grounded) {
				if (horizontal > 0 || horizontal < 0)
					myFrameAnimate.SetAnimation ("walk");
				else {
					myFrameAnimate.SetAnimation ("idle");
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){

			if ( dashCooler > 0 && buttonCount == 1/*Number of Taps you want Minus One*/){
				//Has double tapped
				myFrameAnimate.SetAnimation("Dash");
				myRigidbody.AddForce (new Vector2 (dashSpeed, 0));
				isDashing = true;
			}else{
				
				dashCooler = 0.75f ; 
				buttonCount += 1 ;
			}
		}

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){

			if ( dashCooler > 0 && buttonCount == 1/*Number of Taps you want Minus One*/){
				//Has double tapped
				myFrameAnimate.SetAnimation("Dash");
				myRigidbody.AddForce (new Vector2 (dashSpeed * -1, 0));
				isDashing = true;
			}else{

				dashCooler = 0.75f ; 
				buttonCount += 1 ;
			}
		}

		if ( dashCooler > 0 )
		{

			dashCooler -= 1 * Time.deltaTime ;

		}else{
			if (!isDashAttacking) {
				isDashing = false;
			}
			buttonCount = 0 ;
		}


		Flip (horizontal);

		Combo ();

		if (isJumping) {
			myRigidbody.AddForce (new Vector2(myRigidbody.velocity.x, jumpForce));
			isJumping = false;
		}
	}

	private void Flip(float horizontal)
	{
		if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
		{
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
			myFrameAnimate.Flip ();
		}
	}

	void OnCollisionStay(){
	}

	// C-C-C-COMBO SYSTEM!

		private bool ActivateTimerToReset = false;
		private float comboTimer;
		public int currentComboState = 0;

	void Combo(){
		NewCombo ();
		ResetComboState (ActivateTimerToReset);
	}

	void ResetComboState(bool reset){
		if (reset) {
			comboTimer -= Time.deltaTime;
//			Debug.Log ("comboTimer: " + comboTimer);
			if (comboTimer <= 0) {
				currentComboState = 0;
				ActivateTimerToReset = false;
				myFrameAnimate.SetAnimation ("idle");
				canMove = true;
				unqueuedThisAnim = false;
				alreadyPerformedAttack = false;
			}
		}
	}

	void NewCombo(){
		if (!lightAttackQueued && !isDashing) {
			if (Input.GetKeyDown (KeyCode.Mouse0))
				lightAttackQueued = true;
		}

		if (!heavyAttackQueued && !isDashing) {
			if (Input.GetKeyUp (KeyCode.Mouse1))
				heavyAttackQueued = true;
		}

		if (isDashing && (Input.GetKeyDown (KeyCode.Mouse0) || Input.GetKeyDown (KeyCode.Mouse1)))
			dashAttackQueued = true;
		//DASH ATTACK

		if (dashAttackQueued) {
			isDashAttacking = true;
			if (!unqueuedThisAnim) {
				unqueuedThisAnim = true;
			}
			if (!alreadyPerformedAttack) {
				myFrameAnimate.SetAnimation ("dashAttack");
				ActivateTimerToReset = true;
				comboTimer = myFrameAnimate.GetAnimationTime ();
				audioSource.clip = dashAttackSoundClup;
				audioSource.Play ();
				alreadyPerformedAttack = true;
			}
			if (dashAttackQueued && myFrameAnimate.IsLastFrame ()) {
				dashAttackQueued = false;
				isDashAttacking = false;
				isDashing = false;
				unqueuedThisAnim = false;
				alreadyPerformedAttack = false;

			}
		}
		


		//LIGHT ATTACK LIST

		if (lightAttackQueued) {
			canMove = false;
			myRigidbody.velocity = new Vector2 (0, 0);
			switch (currentComboState) {
			case 0:
				if (!unqueuedThisAnim) {
					lightAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("lightAttack1");
					ActivateTimerToReset = true;
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = lightAttack1SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if (lightAttackQueued && myFrameAnimate.IsLastFrame ()) {
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 1:
				if (!unqueuedThisAnim) {
					lightAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("lightAttack2");
					ActivateTimerToReset = true;
					comboTimer = myFrameAnimate.GetAnimationTime ();
					myRigidbody.velocity = new Vector2 (1, 0);
					audioSource.clip = lightAttack2SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if (lightAttackQueued && myFrameAnimate.IsLastFrame ()) {
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 2:
				if (!unqueuedThisAnim) {
					lightAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("lightAttack3");
					ActivateTimerToReset = true;
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = lightAttack3SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if (lightAttackQueued && myFrameAnimate.IsLastFrame ()) {
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;
			}
		}

		//HEAVY ATTACK LIST

		if(heavyAttackQueued) {
			canMove = false;
			myRigidbody.velocity = new Vector2 (0, 0);
			switch (currentComboState) {
			case 0:
				if (!unqueuedThisAnim) {
					heavyAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("heavyAttack1");
					ActivateTimerToReset = true;
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = heavyAttack1SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if(heavyAttackQueued && myFrameAnimate.IsLastFrame()){
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;
			
			case 1:
				if (!unqueuedThisAnim) {
					heavyAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("HeavyAttack2");
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = heavyAttack2SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if(heavyAttackQueued && myFrameAnimate.IsLastFrame()){
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 2:
				if (!unqueuedThisAnim) {
					heavyAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("HeavyAttack3");
					comboTimer = myFrameAnimate.GetAnimationTime ();
					myRigidbody.velocity = new Vector2 (5, 5);
					audioSource.clip = heavyAttack3SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if(heavyAttackQueued && myFrameAnimate.IsLastFrame()){
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 3:
				if (!unqueuedThisAnim) {
					heavyAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("HeavyAttack4");
					Rigidbody2D waveInstance = Instantiate (waveProjectile, gameObject.transform.position, gameObject.transform.rotation) as Rigidbody2D;
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = heavyAttack4SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if(heavyAttackQueued && myFrameAnimate.IsLastFrame()){
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 4:
				if (!unqueuedThisAnim) {
					heavyAttackQueued = false;
					unqueuedThisAnim = true;
				}
				if (!alreadyPerformedAttack) {
					myFrameAnimate.SetAnimation ("HeavyAttack5");
					comboTimer = myFrameAnimate.GetAnimationTime ();
					audioSource.clip = heavyAttack5SoundClip;
					audioSource.Play ();
					alreadyPerformedAttack = true;
				}

				if(heavyAttackQueued && myFrameAnimate.IsLastFrame()){
					currentComboState++;
					unqueuedThisAnim = false;
					alreadyPerformedAttack = false;
				}
				break;

			case 5:
				comboTimer = 0f;
				break;
			}
		}
	}
}

// ALL ACTIONS STATE MACHINE, DO NOT USE THIS.
/*
 * 
	CharacterState currentState;
	enum CharacterState {Idling, Walking, Dashing, Attacking, Jumping, Hurt}

	float lastStateChange = 0f;

	void SetCurrentState(CharacterState state){
		currentState = state;
		lastStateChange = Time.time;
	}

	float GetStateElapsed(){
		return Time.time - lastStateChange;
	}
	
		// Begin finite state machine.
	switch(currentState){

	// Idling
	case CharacterState.Idling:
		myFrameAnimate.SetAnimation ("idle");
		if (Input.GetButtonDown ("Fire1")) {
			Debug.Log ("Fire1 pressed.");
			SetCurrentState (CharacterState.Attacking);
		}
		break;
		// Walking
	case CharacterState.Walking:
		myFrameAnimate.SetAnimation ("walk");
		if (horizontal == 0 && !isAttacking)
			SetCurrentState (CharacterState.Idling);
		break;
		// Dashing
	case CharacterState.Dashing:
		myFrameAnimate.SetAnimation ("dash");
		break;
		// Attacking
	case CharacterState.Attacking:
		isAttacking = true;
		if (attackCombo == 1) {
			myFrameAnimate.SetAnimation ("heavyAttack1");
			attackCombo++;
		} else if (attackCombo == 2 && GetStateElapsed () > 1) {
			myFrameAnimate.SetAnimation ("heavyAttack2");
			attackCombo++;
		} else if (GetStateElapsed () > 2) {
			isAttacking = false;
			attackCombo = 1;
		}
		break;
		// Jumping
	case CharacterState.Jumping:
		myFrameAnimate.SetAnimation ("jump");
		if (!isFalling)
			SetCurrentState (CharacterState.Idling);
		break;
		// Hurt
	case CharacterState.Hurt:
	
		break;
	
	}
*/