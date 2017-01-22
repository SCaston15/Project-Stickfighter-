using UnityEngine;
using System.Collections;

public class HurtBoxObject : MonoBehaviour {

	public bool isDebug = false;
	public GameObject owner;
	private Vector2 forceVector;

	// On trigger enter, if the other collider is a hitbox tag, get the damage and apply it to the owner's health.

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "HitBox") {
			Debug.Log ("Owner = " + owner);
			HealthScript myHealthScript = owner.GetComponent<HealthScript> ();
			if (!myHealthScript.GetIsInvulnerable ()){
				HitBoxObject incomingHitBox = other.GetComponent<HitBoxObject> ();
				HitStun targetHitstun = owner.GetComponent<HitStun>();
	
				float receivingStunTime = incomingHitBox.stunTime;
				float receivingForceAmount = incomingHitBox.forceAmount;	
				Vector2 forceVector = new Vector2 (incomingHitBox.forceDirectionX, incomingHitBox.forceDirectionY);
				float receivingDamage = incomingHitBox.damageAmount;
				//Debug.Log ("Damage = " + receivingDamage);
				myHealthScript.TakeDamage (receivingDamage);
				//Debug.Log ("ReceivingStunTime = " + receivingStunTime + ", forceDirectionX = " + incomingHitBox.forceDirectionX + ", forceDirectionY = " + incomingHitBox.forceDirectionY + ", receivingForceamount = " + receivingForceAmount + ", owner = " + owner);
				targetHitstun.Stun (receivingStunTime, forceVector, receivingForceAmount);
				
				//Debug.Log("Hurtbox Struck!");
			}
		}
	}
}
