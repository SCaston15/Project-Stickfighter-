using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class FrameAnimate : MonoBehaviour {

	// generates lists for hurt boxes and hit boxes.
	List<GameObject> hurtBoxObjectArray = new List<GameObject>();
	List<GameObject> hitBoxObjectArray = new List<GameObject>();

	[Header("References")]
	//public Canvas debugGUIReference;
	public GameObject hurtBoxObjectReference;
	public GameObject hitBoxObjectReference;

	[Header("Animation Settings")]
	public bool debugAnimation = true;				// while enabled, displays hitboxes in game.
	public string startAnimation = "Jab";			// animation the object starts in.
	public float animationFramesPerSecond = 15f;		// how quickly the animations change frames
	int currentAnimationFrame = 0; //hide later		// displays the current frame number being played

	[Header("Animation Array")]
	public Animations[] animationList;				// array that stores animations, animations must be hard coded in the inspector.
													
	[HideInInspector]
	public Animations currentAnimation;				

	//[HideInInspector]
	public int xFacing = 1;							// determines direction facing.

	[HideInInspector]
	public float frameCurrentTime = 0;				// tracks the current frame time, used to determine when to end the frame and move on to the next frame.
	[HideInInspector]
	public float frameEndTime = 0;					// tracks the frame end time.

	// Class stores animations, including the animation name and the frames array.
	[System.Serializable]
	public class Animations
	{
		public string animationName;
		public FrameInfo[] frames; 
		public string transitionTo = null;
	}

	// Class stores frame information, including the particular sprite for the frame, the hitbox and hurtbox arrays.
	[System.Serializable]
	public class FrameInfo
	{
		public Sprite frameSprite;
		public HitBox[] HitBoxes;
		public HurtBox[] HurtBoxes;
	}

	// Class stores information about a particular hitbox's attributes, including the dimensions and the amount of damage it deals.
	[System.Serializable]
	public class HitBox {
		public Rect hitBoxDimensions;

		[Range(0.0f, 100.0f)]
		public float damageAmount;

		public float stunTime;
		public float FDX;
		public float FDY;
		public float forceAmount;
	}

	// Class stores information about a particular hurtbox's attributes, mainly just the dimensions.
	[System.Serializable]
	public class HurtBox
	{
		public Rect hurtBoxDimensions;
	}

	// Starts on enable, sets the animation to the startAnimation set in the inspector.
	void Start () {
		SetAnimation(startAnimation);
	}

	// This update runs a certain fixed number of times per second, sets frameCurrentTime and calls the AnimateFrames method every run of this update method.
	void FixedUpdate () {
		
		frameCurrentTime = Time.fixedTime * 1000;

		AnimateFrames();

		if (currentAnimation.transitionTo != null && currentAnimationFrame == currentAnimation.frames.Length - 1) {
			SetAnimation (currentAnimation.transitionTo);
		}

		//debugGUIReference.transform.FindChild("txt_frameinfo").GetComponent<Text>().text = "Frame: " + GetFrame().ToString() + "/" + (NumberOfFrames() - 1).ToString();
		//debugGUIReference.transform.FindChild("txt_animationname").GetComponent<Text>().text = "Animation Name: " + currentAnimation.animationName;
	}

	void AnimateFrames()
	{
		// if the current frame time is greater than or equal to the frame's end time.
		if(frameCurrentTime >= frameEndTime)
		{
			// incriment the currentAnimationFrame by one.
			currentAnimationFrame++;
			// set frameEndTime equal to the current frame time, plus the ceiling value of (1/animationsFPS * 1000).
			frameEndTime = frameCurrentTime + Mathf.CeilToInt((1f / animationFramesPerSecond) * 1000);
		} else
		{
			return; //make sure we are only removing and generating new boxes once per ||AUTHOR COMMENT||
		}

		// if the current animation frame is greater than the current animation's frames length - 1...
		if(currentAnimationFrame > currentAnimation.frames.Length - 1)
		{
			// ...set the currentAnimationFrame to 0, thus setting the animation back to the beginning.
			currentAnimationFrame = 0;
		}

		// regardless of which if statement occurs, set the game object's sprite renderer to the current frame's sprite.
		this.GetComponent<SpriteRenderer>().sprite = currentAnimation.frames[currentAnimationFrame].frameSprite;

		// call in the clean-up crew to kill the previous boxes and...
		CleanUpBoxes();
		// ...generate new boxes in their place.
		GenerateBoxes();

	}

	// Meant to be called externally to force the game object to a particular frame, then update the sprite.
	public void SetFrame(int frame)
	{
		if (frame > currentAnimation.frames.Length - 1)
			frame = currentAnimation.frames.Length - 1;
		if (frame < 0)
		{
			frame = 0;
		}

		currentAnimationFrame = frame;
		frameEndTime = frameCurrentTime + Mathf.CeilToInt((1f / animationFramesPerSecond) * 1000);
		this.GetComponent<SpriteRenderer>().sprite = currentAnimation.frames[frame].frameSprite;
	}

	// Meant to be called externally to get the current animation frame.
	public int GetFrame()
	{
		return currentAnimationFrame;
	}

	// Meant to be called externally to get the amount of frames in the current animation.
	public int GetNumberOfFrames()
	{
		return currentAnimation.frames.Length;
	}

	public float GetAnimationTime(){
		float result = currentAnimation.frames.Length / animationFramesPerSecond;
		return result;
	}

	// Meant to be called externally to obtain all data in the animationList array.
	public Animations GetAnimationInfo(string animationName) //returns current animation if error ||AUTHOR COMMENT||
	{
		for (int i = 0; i < animationList.Length; i++)
		{
			if (animationList[i].animationName.ToLower() == animationName.ToLower())
			{
				return animationList[i];
			}
		}

		return currentAnimation;
	}

	public bool IsLastFrame(){
		if (currentAnimationFrame == currentAnimation.frames.Length - 1)
			return true;
		else
			return false;
	}

	public void Flip(){
		xFacing *= -1;
	}

	public bool SetAnimation(string animationName, int startFrame = 0)
	{
		//Is our current animation the same as the one we are setting?
		if (currentAnimation.animationName.ToLower() == animationName.ToLower())
			return true;

		for(int i = 0; i < animationList.Length; i++)
		{
			if(animationList[i].animationName.ToLower() == animationName.ToLower())
			{
				if(startFrame > animationList[i].frames.Length - 1)
					startFrame = animationList[i].frames.Length - 1;
				if(startFrame < 0)
				{
					startFrame = 0;
				}

				currentAnimationFrame = startFrame;
				currentAnimation = animationList[i];

				//You need this to make sure we are restarting the first frame of animation timer.
				frameEndTime = frameCurrentTime + Mathf.CeilToInt((1f / animationFramesPerSecond) * 1000);
				this.GetComponent<SpriteRenderer>().sprite = currentAnimation.frames[startFrame].frameSprite;

				CleanUpBoxes();
				GenerateBoxes();

				return true;
			}
		}

		//Debug.Log("Error, animation not registered.");
		return false;
	}

	void GenerateBoxes() //generated once per frame
	{
		Vector2 mePosition = new Vector2(this.transform.position.x, this.transform.position.y);

		if (currentAnimation.frames[currentAnimationFrame].HurtBoxes.Length != 0)
		{

			//hurtboxes
			for (int i = 0; i < currentAnimation.frames[currentAnimationFrame].HurtBoxes.Length; i++)
			{
				Rect hurtBoxPosition = new Rect(new Vector2(currentAnimation.frames[currentAnimationFrame].HurtBoxes[i].hurtBoxDimensions.position.x * xFacing, currentAnimation.frames[currentAnimationFrame].HurtBoxes[i].hurtBoxDimensions.position.y) + mePosition, currentAnimation.frames[currentAnimationFrame].HurtBoxes[i].hurtBoxDimensions.size);

				GameObject hurtBoxTemp = Instantiate(hurtBoxObjectReference) as GameObject;
				hurtBoxTemp.GetComponent<HurtBoxObject>().owner = this.transform.gameObject;
				hurtBoxTemp.transform.position = new Vector3(hurtBoxPosition.x, hurtBoxPosition.y, this.transform.position.z);
				hurtBoxTemp.transform.localScale = new Vector3(hurtBoxPosition.width, hurtBoxPosition.height, 0);
				hurtBoxTemp.transform.parent = this.transform;
				hurtBoxTemp.GetComponent<HurtBoxObject>().isDebug = debugAnimation;
				hurtBoxTemp.GetComponent<SpriteRenderer>().sortingLayerName = "foreground";

				hurtBoxObjectArray.Add(hurtBoxTemp);
			}
		}

		if (currentAnimation.frames[currentAnimationFrame].HitBoxes.Length != 0)
		{
			//hitboxes
			for (int i = 0; i < currentAnimation.frames[currentAnimationFrame].HitBoxes.Length; i++)
			{
				Rect hitBoxPosition = new Rect(new Vector2(currentAnimation.frames[currentAnimationFrame].HitBoxes[i].hitBoxDimensions.position.x * xFacing, currentAnimation.frames[currentAnimationFrame].HitBoxes[i].hitBoxDimensions.position.y) + mePosition, currentAnimation.frames[currentAnimationFrame].HitBoxes[i].hitBoxDimensions.size);

				GameObject hitBoxTemp = Instantiate(hitBoxObjectReference) as GameObject;
				hitBoxTemp.GetComponent<HitBoxObject>().owner = this.transform.gameObject;
				hitBoxTemp.transform.position = new Vector3(hitBoxPosition.x, hitBoxPosition.y, this.transform.position.z);
				hitBoxTemp.transform.localScale = new Vector3(hitBoxPosition.width, hitBoxPosition.height, 0);
				hitBoxTemp.transform.parent = this.transform;
				hitBoxTemp.GetComponent<HitBoxObject>().isDebug = debugAnimation;
				hitBoxTemp.GetComponent<HitBoxObject>().damageAmount = currentAnimation.frames[currentAnimationFrame].HitBoxes[i].damageAmount;
				hitBoxTemp.GetComponent<HitBoxObject> ().stunTime = currentAnimation.frames [currentAnimationFrame].HitBoxes [i].stunTime;
				hitBoxTemp.GetComponent<HitBoxObject> ().forceDirectionX = currentAnimation.frames [currentAnimationFrame].HitBoxes [i].FDX;
				hitBoxTemp.GetComponent<HitBoxObject> ().forceDirectionY = currentAnimation.frames [currentAnimationFrame].HitBoxes [i].FDY;
				hitBoxTemp.GetComponent<HitBoxObject> ().forceAmount = currentAnimation.frames [currentAnimationFrame].HitBoxes [i].forceAmount;
				hitBoxTemp.GetComponent<SpriteRenderer>().sortingLayerName = "foreground";

				hitBoxObjectArray.Add(hitBoxTemp);
			}
		}
	}

	void CleanUpBoxes()
	{
		//find all boxes created by this schteyuck and delete them
		foreach(GameObject hb in hurtBoxObjectArray)
		{
			Destroy(hb);
		}
		hurtBoxObjectArray.Clear();

		foreach(GameObject hb in hitBoxObjectArray)
		{
			Destroy(hb);
		}
	}


	//end
}