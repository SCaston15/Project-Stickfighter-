using UnityEngine;
using System.Collections;

public class Impact : MonoBehaviour {

	public Animator anim;
	public bool hit = false; 
 

	// Use this for initialization
	void Start () {


		anim.enabled = false; 

	}
	void FixedUpdate() {

		hit = false; 
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.tag == "HitBox") {
			hit = true; 

		} 
	}


	void Update(){

		//SpriteRenderer sr = GameObject.Find("Impact").GetComponent<SpriteRenderer> ();
		SpriteRenderer sr = GetComponentsInChildren<SpriteRenderer>()[1]; 
		if(hit){
			anim.enabled = true; 
			sr.enabled = true; 
		}
		else{
			anim.enabled = false; 
			sr.enabled = false; 
			}
		}
}
