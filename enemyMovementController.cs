using UnityEngine;
using System.Collections;

public class enemyMovementController : MonoBehaviour {


	//Animator enemyAnimator;

	//facing
	public GameObject enemyGraphic;
	bool facingRight = false;

	//attacking
	public bool canMove = true;
	public bool canAttack = true;
	private bool isFalling = false;
	private Rigidbody2D myRigidbody;
	public float moveSpeed;
	FrameAnimate myFrameAnimate;
	BoxCollider2D boxCol;
	public float stoppingDistance;
	public float timeLeft = 2f;

	// Use this for initialization
	void Start () {
		//enemyAnimator = GetComponentInChildren<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D> ();
		myFrameAnimate = myRigidbody.GetComponent<FrameAnimate> ();
		float x = Random.Range(0.75f, 1.2f);
		boxCol = GetComponent<BoxCollider2D> (); 
		boxCol.size = new Vector2 (boxCol.size.x, boxCol.size.y * x);  
	}

	void FixedUpdate() {

	}
	
	// Update is called once per frame
	void Update () {
		timeLeft = Time.deltaTime;
		if (timeLeft < 0) {
			timeLeft = 2f;
		}
	}


	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			//If the player is to the left, horizontal = -1
			//If the player is to the right, horizontal = 1
			//When enemy is less than or equal to stopping distance, stop movement
			if (!(Mathf.Abs (other.transform.position.x - gameObject.transform.position.x) <= stoppingDistance && canMove)) {
				if (other.transform.position.x < gameObject.transform.position.x) {
					Move (-1);
				}
				if (other.transform.position.x > gameObject.transform.position.x) {
					Move (1);
				}
			} else if (Mathf.Abs (other.transform.position.x - gameObject.transform.position.x) <= stoppingDistance && timeLeft < .5f) {
				myFrameAnimate.SetAnimation ("attack");
			} else
				myFrameAnimate.SetAnimation ("idle");
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
		}
	}

	private void Move (float horizontal){
		if (canMove) {
			myRigidbody.velocity = new Vector2 (horizontal * moveSpeed, myRigidbody.velocity.y);
			if (!isFalling && horizontal > 0 || horizontal < 0)
				myFrameAnimate.SetAnimation ("run");
			else {
				myFrameAnimate.SetAnimation ("idle");
			}
		}
		Flip (horizontal);
	}



}
