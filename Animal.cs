using UnityEngine;

public class Animal : MonoBehaviour {

	public AnimalAI animalAI;
	public PlayerAbilities playerAbilities;
    public PlayerController playerController;
	public Transform target;
	public bool friendlyAnimal; //true - can control it //false - is a enemy
	public bool idleFallowAtack;
	public bool walkFallowAtak;

	void Start ()
	{
		animalAI.flipCharacter.myCharacterTransform = GetComponent<Transform>();
		animalAI.myAnimalTransform = GetComponent<Transform>();
		animalAI.animalAnimator = GetComponent<Animator>();	
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
                playerController.isDead = true;
			}
		}

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (friendlyAnimal)
		{
			if (collision.tag == "Animal" || collision.tag == "Trap")
			{
                playerController.isDead = true;
                playerAbilities.isControled = false;
			}

		}
	}

}
