using UnityEngine;

public class Animal : MonoBehaviour {

	public AnimalAI animalAI;
	private float gravity;
	public float jumpHeight = 2f; // 2 squares
	public float jumpTimeToRoof = .4f; // 0.4 seconds
	//private float groundTime = .1f;
	public float jumpVelocity;
	private float currentSmooth;
	PlayerAbilities playerAbilities;
	public Transform target;
	public bool friendlyAnimal; //true - can control it //false - is a enemy
	public bool idleFallowAtack;
	public bool walkFallowAtak;
	public int animalSpeed;

	void Start ()
	{
		//thisAnimal = GetComponent<GameObject>();
		animalAI.animalMovement.box = GetComponent<BoxCollider2D>();
		animalAI.flipCharacter.myCharacterTransform = GetComponent<Transform>();
		animalAI.myAnimalTransform = GetComponent<Transform>();
		animalAI.animalAnimator = GetComponent<Animator>();	
		animalAI.animalMovement.SetRaysDistance(); //set equal distance between each rays
		gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToRoof, 2);	
		playerAbilities = GameObject.FindGameObjectWithTag("PlayerAbilities").GetComponent<PlayerAbilities>();
		if (target != null)
		{
			animalAI.targetPosition = target.position.x; //used in patrol
		}
		animalAI.startPosition = transform.position.x;
	}
	
	void Update ()
	{
		if (!friendlyAnimal) //enemy animal
		{

			animalAI.currentDistance = Vector2.Distance(transform.position, animalAI.playerTransform.transform.position);

			if (animalAI.animalMovement.bottomRay || animalAI.animalMovement.topRay)
			{
				animalAI.velocityDirection.y = 0;
			}

			transform.Translate(animalAI.animalMovement.Move(animalAI.velocityDirection * animalSpeed * Time.deltaTime));
			animalAI.velocityDirection.y += gravity * Time.deltaTime;
			if (idleFallowAtack)
			{
				animalAI.IdleFallowAtack(); //set how to behave 
			}
			else if (walkFallowAtak)
			{
				animalAI.WalkFallowAtak(); //set how to behave
			}
			if (animalAI.animalAnimator.GetBool("Atack"))
			{
				playerAbilities.GetComponentInParent<PlayerController>().isDead = true;
			}
		}

		else //friendly animal
		{
			if (!playerAbilities.isControled) //is not controled yet
			{
				if (animalAI.animalMovement.bottomRay || animalAI.animalMovement.topRay)
				{
					animalAI.velocityDirection.y = 0;
				}

				transform.Translate(animalAI.animalMovement.Move(animalAI.velocityDirection * animalSpeed * Time.deltaTime));
				animalAI.velocityDirection.y += gravity * Time.deltaTime;
			}
		}

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (friendlyAnimal)
		{
			if (collision.tag == "Animal" || collision.tag == "Trap")
			{
				playerAbilities.GetComponentInParent<PlayerController>().isDead = true;
				playerAbilities.isControled = false;
			}

		}
	}

}
