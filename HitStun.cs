using UnityEngine;
using System.Collections;

public class HitStun : MonoBehaviour {

	/* 
	 * This script determines whether the character is stunned or not. 
	 * Stunned objects cannot move. They also cannot attack.
	 * They are invulnerable for the duration of half of the stun. 
	 * They have their hit animations called.
	 * They have a certain amount of force applied to them in a certain direction.
	 * After the stun duration, movement and attack controls are enabled, and their idle state is called.
	 */
	

	// Declare variables.
	private bool stunned = false;			// Stunned state.
	private bool invulnerable = false;		// Invulnerability state.
	private float stunTime;					// Amount of time the object is stunned.
	private float timeStunned = 0f;			// Time that the stun occurred.

	private Rigidbody2D myRigidbody;
	private enemyMovementController emc;
	private HealthScript hScript;
	FrameAnimate myFrameAnimate;

	// Use this for initialization
	void Start () {
		
		stunned = false;
		invulnerable = false;
		myRigidbody = gameObject.GetComponent<Rigidbody2D> ();
		myFrameAnimate = myRigidbody.GetComponent<FrameAnimate> ();
		emc = gameObject.GetComponent<enemyMovementController> ();
		hScript = gameObject.GetComponent<HealthScript> ();
	}
	
	void FixedUpdate () {
		// we want the object to stay stunned until the stun time ends. We also want the invulnerability to end halfway between the stun time so the player can combo the enemy.
		if (stunned) {
			if (GetTimeElapsed () <= stunTime / 2 && invulnerable == true) {
				Debug.Log ("Vulnerable!");
				StopInvulnerability ();
			}
			if (GetTimeElapsed () <= stunTime) {
				StopStun ();
				Debug.Log ("StunStopped.");
			} 
		}
	}

	float GetTimeElapsed(){
		Debug.Log ("Time elapsed = " + ((Time.time + 10) - timeStunned));
		return Time.time - timeStunned;
	}

	// Meant to be called externally by the hurtbox. Stuns the object. Disables movement. 
	// Disables actions. Calls hit anim. Takes stun time and force as parameters.
	public void Stun(float sTime, Vector2 forceDirection, float forceAmount){
		// if the object isn't stunned, stun it, apply invulnerability, force, disable movement, and call struck anim.
		if (!stunned) {
			// apply stun
			stunned = true;
			stunTime = sTime;
			timeStunned = Time.time;
			Debug.Log("Stunned!");
			// apply invulnerability
			invulnerable = true;
			hScript.invulnerable = true;
			Debug.Log ("Invulnerable!");
			// apply force
			myRigidbody.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse);
			// disable movement
			emc.canMove = false;
			// disable actions
			emc.canAttack = false;
			// call stuck animation
			myFrameAnimate.SetAnimation ("hurt");

		}
		// if the object is stunned and this method is called, it's because the hitbox has struck other hurtboxes.
		// however, we need to watch our times and turn off stun and invulnerability at the appropriate times. See FixedUpdate()

	}

	// remove stun effects.
	void StopStun(){
		stunned = false;
		stunTime = 0f;
		timeStunned = 0f;
		emc.canMove = true;
		emc.canAttack = true;

		Debug.Log("Unstunned!");
		myFrameAnimate.SetAnimation ("idle");
	}

	// remove invulnerability.
	void StopInvulnerability(){
		invulnerable = false;
		hScript.invulnerable = false;

		Debug.Log("Vulnerable!");
	}
}