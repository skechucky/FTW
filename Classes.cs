using UnityEngine;


[System.Serializable]
public class AnimalAI
{
	public Movement animalMovement;
	public FlipCharacter flipCharacter;
	public Transform myAnimalTransform;
	public Transform playerTransform;
	public float targetPosition;
	public float startPosition;
	public Animator animalAnimator;
	public Vector3 velocityDirection;
	public float currentDistance; // current distance of player,animal Vector2.Distance
	public float rangeFallowPlayer; // at wich range can fallow
	public float rangeAtack; // at wich range can attack



	public void IdleFallowAtack()
	{
		flipCharacter.FlipVerification(velocityDirection.x);
		if (currentDistance <= rangeAtack) // A in B area
		{
			Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeAtack, Color.blue);
			velocityDirection.x = 0; // don't move

			animalAnimator.SetBool("Atack", true); //Atack player
			animalAnimator.SetBool("Walk", false);
		}
		else if (currentDistance <= rangeFallowPlayer) // A in B area
		{
			Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.red);
			FallowPlayerDirection(myAnimalTransform.position.x, playerTransform.position.x); //Fallow Player

			animalAnimator.SetBool("Walk", true); // Walk
			animalAnimator.SetBool("Idle", false);
		}
		else // A out of B area
		{
			velocityDirection.x = 0; // don't move

			animalAnimator.SetBool("Idle", true); // Stay Iddle
			animalAnimator.SetBool("Walk", false);
		}

	}

	public void WalkFallowAtak()
	{
		flipCharacter.FlipVerification(velocityDirection.x);

		if (currentDistance <= rangeAtack)
		{
			Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeAtack, Color.blue);

			animalAnimator.SetBool("Atack", true); //Atack player
			animalAnimator.SetBool("Walk", false);
		}
		else if (currentDistance < rangeFallowPlayer)//
		{
			Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.yellow);

			FallowPlayerDirection(myAnimalTransform.position.x, playerTransform.position.x); //Fallow Player
			animalAnimator.SetBool("Walk", true);
			animalAnimator.SetBool("Atack", false);
		}
		else
		{
			Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.blue);

			WalkDirection();
			animalAnimator.SetBool("Walk", true); // Walk
			animalAnimator.SetBool("Atack", false);
		}

	}

	void FallowPlayerDirection(float myAnimalPosition, float playerPosition) // used in IdleFallowAtack() and PatrolFallowAtak()
	{
		velocityDirection.x = (myAnimalPosition >= playerPosition) ? -1 : 1; //Player-Left & Animal-Right  else  //Animal-Left & Player-Right
		//Debug.Log("FallowPlayerDirection");
	}

	void WalkDirection() //used in PatrolFallowAtak()
	{
		if (myAnimalTransform.position.x >= targetPosition && velocityDirection.x != -1)
		{
			velocityDirection.x = -1; //Target-Left & Animal-Right
		}
		else if (myAnimalTransform.position.x <= startPosition && velocityDirection.x != 1)//
		{
			velocityDirection.x = 1; //Animal - Left & Player - Right
		}
	}

}

[System.Serializable]
public class Movement
{
	public bool rightRay, leftRay, topRay, bottomRay;
	public Vector2 originCornerTL, originCornerTR, originCornerBL, originCornerBR;
	public int xRaysNumber, yRaysNumber;                                              // define how many rays will have x,y of BoxCollider
	private float xDistBetweenRay, yDistBetweenRay;                                  //horizontalRaySpacing
	float rayExtend = 0.15f;                                                                  //taie din coluri pt a nu avea probleme daca un box e mai inalt decat celalt(cazul horizontal != 0)
	public BoxCollider2D box;                                                      //need ref where is called
	public LayerMask collisionTag;
	public FlipCharacter flipCharacter;

	public Vector3 Move(Vector3 velocity)
	{
		SetOriginRay();
		ResetBooleanRay();

		if(velocity.x != 0)
		{
			XRayDetection(ref velocity);
		}
		if(velocity.y != 0)
		{
			YRayDetection(ref velocity);
		}

		return velocity;
	}

	void XRayDetection(ref Vector3 velocity)     // detect obstacle from X ax
	{
		float xDirection = Mathf.Sign(velocity.x);
		float rayDistance = Mathf.Abs(velocity.x) + rayExtend;

		for (int i = 0; i < xRaysNumber; i++)
		{
			Vector2 origin;
			origin = (xDirection == -1) ? originCornerBL : originCornerBR;

			origin += Vector2.up * (xDistBetweenRay * i);
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * xDirection, rayDistance, collisionTag);
			Debug.DrawRay(origin, Vector2.right * xDirection * rayDistance * 3, Color.green);
			if (hit)
			{
				velocity.x = (hit.distance - rayExtend) * xDirection;
				rayDistance = hit.distance;

				leftRay = (xDirection == -1) ? true : false;
				rightRay = (xDirection == 1) ? true : false;		
			}
		}
	}

	void YRayDetection(ref Vector3 velocity) // detect obstacle/ground from Y ax
	{
		float yDirection = Mathf.Sign(velocity.y); //where i look/move, 0 if is iddle
		float rayDistance = Mathf.Abs(velocity.y) + rayExtend;

		for (int i = 0; i < yRaysNumber; i++)
		{
			Vector2 origin;
			origin = (yDirection == -1) ? originCornerBL : originCornerTL; 
			origin += Vector2.right * (yDistBetweenRay * i + velocity.x); //+vecolity.x to be centre when i'm moving
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * yDirection, rayDistance, collisionTag);
			Debug.DrawRay(origin, Vector2.up * yDirection * rayDistance * 10, Color.green);
			if (hit)
			{
				velocity.y = (hit.distance - rayExtend) * yDirection;
				rayDistance = hit.distance;

				bottomRay = (yDirection == -1) ? true : false;
				topRay = (yDirection == 1) ? true : false;
			}
		}

	}
	 
	void SetOriginRay()                                               
	{ 
		Bounds bounds = box.bounds;
		bounds.Expand(-rayExtend );                                     //cut a little bit

		originCornerTL = new Vector2(bounds.min.x, bounds.max.y);
		originCornerTR = new Vector2(bounds.max.x, bounds.max.y);
		originCornerBL = new Vector2(bounds.min.x, bounds.min.y);
		originCornerBR = new Vector2(bounds.max.x, bounds.min.y);
	}

	public void SetRaysDistance()                                             // set equal distance between each rays
	{
		Bounds bounds = box.bounds;
		bounds.Expand(-rayExtend);                                         //cut a little bit

		                                                                               
		xRaysNumber = Mathf.Clamp(xRaysNumber, 2, int.MaxValue);                 //centrare
		yRaysNumber = Mathf.Clamp(yRaysNumber, 2, int.MaxValue);

		xDistBetweenRay = bounds.size.x / (xRaysNumber-1);                     // distance between each ray
		yDistBetweenRay = bounds.size.y / (yRaysNumber-1);                             // -1 because ex.: 4 rays => 3spaces |-|-|-|
	
	}

	public void ResetBooleanRay()
	{
		rightRay = false;
		leftRay = false;
		topRay = false;
		bottomRay = false;
	}

}

[System.Serializable]
public class LevelDetails
{
	public Texture LevelPicture;
	public string sceneName;
	public int unlockedLevels;   //which is locked
}

[System.Serializable]
public class AudioInfo
{
	public int ID;
	public AudioClip audioClip;

	public void LoopPlayAudio(AudioSource audioSource)
	{
		audioSource.volume = PlayerPrefs.GetFloat("Volume"); //set volume
		audioSource.loop = true;
		audioSource.clip = audioClip;
		audioSource.Play();
	}
	public void OncePlayAudio(AudioSource audioSource)
	{
		audioSource.volume = PlayerPrefs.GetFloat("Volume"); //set volume
		audioSource.PlayOneShot(audioClip);
	}

}

[System.Serializable]
public class FlipCharacter
{
	public Transform myCharacterTransform;
	bool sampleFlip;

	public void FlipVerification(float velocity)
	{
		if (velocity < 0 && !sampleFlip)
		{
			Flip();
			sampleFlip = true;
		}
		else if (velocity > 0 && sampleFlip)
		{
			Flip();
			sampleFlip = false;
		}
	}
	public void Flip() 
	{
		Vector3 theScale = myCharacterTransform.localScale;
		theScale.x = -theScale.x;
		myCharacterTransform.localScale = theScale;
	}
}

[System.Serializable]
public class MoveObjectInteraction
{
	public float X;                                                                    //x axe target
	public float Y;                                                                               //y axe target
	Vector3 targetArrive; 
	Vector3 targetStart;
	public int speed;

	public void Init(Transform obj)                                                        //constructor
	{
		targetArrive = new Vector3(obj.position.x + X, obj.position.y + Y);
		targetStart = obj.position;
	}

	public Vector3 MoveAtoB(Transform obj)
	{
		return Vector3.MoveTowards(obj.position, targetArrive, speed * Time.deltaTime);
	}
	public Vector3 MoveBtoA(Transform obj)
	{
		return Vector3.MoveTowards(obj.position, targetStart, speed * Time.deltaTime);
	}

	public Vector3 PushButton(Transform obj, bool btnPressed)
	{
		if (btnPressed)
		{
			return MoveAtoB(obj);
		}
		else
		{
			return MoveBtoA(obj);
		}
	}

}

