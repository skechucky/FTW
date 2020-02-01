using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
	public MoveObjectInteraction moveObjectInteraction;

    public bool isPushButton;// set in inspector
	public bool objHit; //set in Ball Interactions
	public bool isPressed;
	Animator auraAnimator;
	public bool existAnimator; //aura
	//public bool withoutMoveObj;
	private bool sampleOnce;
	public Transform obj;

	void Start()
	{
		if (existAnimator)
		{
			auraAnimator = GetComponent<Animator>();
			auraAnimator.SetBool("AuraAnim", true);
		}
		//obj = obj.GetComponent<Transform>();
		//if (!withoutMoveObj)
		//{
			moveObjectInteraction.Init(obj);
		//}

	}

	void Update()
	{
		if (objHit) // object is hit by the ball
		{
			//if (!withoutMoveObj)
			//{
			if (isPushButton)
			{
				obj.position = moveObjectInteraction.PushButton(obj, isPressed);
			}
			else
			{
				obj.position = moveObjectInteraction.MoveAtoB(obj);
			}
			//}
			if (!sampleOnce && existAnimator)
			{
				auraAnimator.SetBool("AuraAnim", false);
				GetComponent<SpriteRenderer>().enabled = false;
				sampleOnce = false;
			}			
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Ball" || collision.tag == "FriendlyAnimal" || collision.tag == "Player") //ball is in object zone
		{
			objHit = true;
		}
		if (isPushButton)
		{
			isPressed = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (isPushButton)
		{
			if (collision.tag == "Ball" || collision.tag == "FriendlyAnimal" || collision.tag == "Player") //ball is in object zone
			{
				isPressed = false;
			}
		}
	}
}
