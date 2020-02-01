using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public Movement playerMovement;
	public Vector3 velocity;
	private float gravity;
	public float jumpHeight = 2f; // 2 squares
	public float jumpTimeToRoof = .4f; // 0.4 seconds
	private float jumpTime = .2f; 
	private float groundTime = .1f; 
	public float jumpVelocity;
	public float moveSpeed = 3f;
	private float currentSmooth;
	private int horizontal;
	public bool isJump;  // set from ui button
	PlayerAbilities playerAbilities;
    Animal animal;
	private Animator playerAnimation;
	public bool isDead;
	public Vector3 input;
	public BoxCollider2D currentCollider2D; //selected player
	public GameObject playerMonk;
	public GameObject playerAnimal;

	void Start()
	{
		animal = playerAnimal.GetComponent<Animal>();

		playerAbilities = GetComponentInChildren<PlayerAbilities>();
		playerMovement.box = GetComponent<BoxCollider2D>();
		playerMovement.SetRaysDistance(); 


		gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToRoof, 2);
		jumpVelocity = Mathf.Abs(gravity) * jumpTimeToRoof; 


		playerAnimation = GetComponent<Animator>();
		playerMovement.flipCharacter.myCharacterTransform = GetComponent<Transform>();
	}

	void Update()
	{
		HorizontalSet(); // -1/0/1


		if ((playerMovement.bottomRay || playerMovement.topRay) && !isJump)
		{
			velocity.y = 0;
		}
		velocity.y += gravity * Time.deltaTime;

		//Windows
		//input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		//if ((Input.GetKeyDown(KeyCode.W) && playerMovement.bottomRay) || (Input.GetAxisRaw("Vertical") > 0.4f && playerMovement.bottomRay))  //Jump
		//{
		//	//PlayerAnimation.AnimationName = "jump";
		//	velocity.y = jumpVelocity;
		//}

		Jump();

		float xTarget = input.x * moveSpeed;

		if (playerMovement.bottomRay) 
		{
			velocity.x = Mathf.SmoothDamp(velocity.x, xTarget, ref currentSmooth, groundTime);
		}
		else 
		{
			velocity.x = Mathf.SmoothDamp(velocity.x, xTarget, ref currentSmooth, jumpTime);
		}
		


		if (!playerAbilities.isControled)
		{
			if (!isDead)
			{
				playerMonk.transform.Translate(playerMovement.Move(velocity * Time.deltaTime));
				playerMovement.flipCharacter.FlipVerification(velocity.x);

			}
			Animations();
		}
		else
		{
			if (!isDead)
			{
				playerAnimal.transform.Translate(playerMovement.Move(velocity * Time.deltaTime)); 
				animal.animalAI.flipCharacter.FlipVerification(velocity.x);
			}
			AnimalAnimations();
		}

		if (Input.GetKeyDown(KeyCode.R)) // dont forget just for test!!
		{
			isDead = false;
			GetComponentInChildren<PlayerAbilities>().canThrow = true;
		}

	}

	public void Jump() 
	{
		if (playerMovement.bottomRay && isJump && !isDead)
		{
			velocity.y = jumpVelocity;
			isJump = false;
		}
	}

	void HorizontalSet()
	{
		horizontal = (input.x < 0) ? -1 : 1;            //mobile UI input // dont forget
		if (input.x == 0)                                  //mobile UI input  //dont forget
		{
			horizontal = 0;
		}
	}



	void AnimalAnimations()
	{
		if (horizontal == 0)
		{
			animal.animalAI.animalAnimator.SetBool("Idle", true);
			animal.animalAI.animalAnimator.SetBool("Walk", false);
		}
		else
		{
			Debug.Log("(Walk,true);");
			animal.animalAI.animalAnimator.SetBool("Walk", true);
			animal.animalAI.animalAnimator.SetBool("Idle", false);
		}
	}

	void Animations()
	{
		if (horizontal == 0)
		{
			playerAnimation.SetBool("Idle", true); //
			playerAnimation.SetBool("Walk", false);
			playerAnimation.SetBool("Jump", false);
			playerAnimation.SetBool("JumpWalk", false);
		}
		else
		{
			playerAnimation.SetBool("Idle", false);
			playerAnimation.SetBool("Walk", true); //
			playerAnimation.SetBool("Jump", false);
			playerAnimation.SetBool("JumpWalk", false);
		}

		if (playerMovement.bottomRay == false)
		{
			if (horizontal == 0)
			{
				playerAnimation.SetBool("Idle", false);
				playerAnimation.SetBool("Walk", false);
				playerAnimation.SetBool("Jump", true); //
			}
			else
			{
				playerAnimation.SetBool("Idle", false);
				playerAnimation.SetBool("Walk", false);
				playerAnimation.SetBool("Jump", false);
				playerAnimation.SetBool("JumpWalk", true); //
			}
		}

		if (isDead == true)
		{
			//cantMoveOnDeath = true;
			playerAnimation.SetBool("Dead", true); //
			playerAnimation.SetBool("Throw", false);
			playerAnimation.SetBool("Walk", false);
			playerAnimation.SetBool("Jump", false);
			playerAnimation.SetBool("Idle", false);
			playerAnimation.SetBool("JumpWalk", false);

			if (playerAnimation.GetCurrentAnimatorStateInfo(0).IsName("death"))
			{
				if (playerAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 2.5f)
				{
					SceneManager.LoadScene(GameObject.FindGameObjectWithTag("PortalNextLvl").GetComponent<PortalNextLvl>().currentLvl);
				}
			}
		}

	}

}
