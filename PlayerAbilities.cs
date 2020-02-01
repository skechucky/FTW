using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
	public Rigidbody2D bulletPrefab;
	public LayerMask interactionsLayer;
	public LayerMask ballLayer;
	public LayerMask animalLayer;
	public LayerMask playerLayer;
	LayerMask newLayer;
	private Camera cam;
	public Transform shootingPont;
    PlayerController playerController;
	LineRenderer lr;
	public float velocity; //set by hit, this need be found
	public float angle;
    int segmentsLenght = 3; //manipulate in inspector, tell us how many segments have our arc
	float g; // force of gravity on the y axe
	float radianValue;//Un radian este o unitate de măsură pentru măsura unghiurilor. 
	public int currentAbility;
	public bool canThrow=true;
	public bool returnBoomerang;
	Vector2 touchPosition;
	public int boomerangSpeed;
	Rigidbody2D obj;
    BallInteractions ballInteractions;
	public bool isControled;

	void Start()
	{
		cam = Camera.main;
		lr = GetComponent<LineRenderer>();
		g = Math.Abs(Physics2D.gravity.y);
		playerController = GetComponentInParent<PlayerController>();	
	}

	void Update()
	{
		if (!playerController.isDead)
		{
			//First player ability selected
			if (currentAbility == 0) //if can throw (have balls)
			{
				if (canThrow)
				{
					Drow();  //on touch in
					Throw(); //on touch out
				}
				else
				{
					//ball in player trigger + ability0 ON + ball on ground
					if (obj.GetComponent<BallInteractions>().ballSignalCanBeTaken && obj.GetComponent<BallInteractions>().ballOnGround)
					{
						PickUpBall();
					}
				}
			}

			//Second player ability selected
			if (currentAbility == 1) //is allrdy throwed
			{
				(canThrow ? (Action)Throw : AbilityTwo)();
			}

			//Third player ability slected
			if (currentAbility == 2)
			{
				TakeControl();

			}
		}
	}

	void TakeControl()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 mouse = Input.mousePosition;
			mouse.z = -cam.transform.position.z;

			Vector2 camRay22 = cam.ScreenToWorldPoint(mouse);
			newLayer = isControled ? playerLayer : animalLayer; 
			if (Physics2D.Raycast(camRay22, Vector2.right, 100f, newLayer))
			{
				
				if (!isControled)
				{
					canThrow = false;
					isControled = true;
				}
				else
				{
					canThrow = true;
					isControled = false;
				}
			}
		}
	}

	void PickUpBall()
	{
		if (Input.GetMouseButtonUp(0)) 
		{
			Vector3 mouse =Input.mousePosition;
			mouse.z = -cam.transform.position.z;

			Vector2 camRay22 = cam.ScreenToWorldPoint(mouse);
			if (Physics2D.Raycast(camRay22, Vector2.right, 100f, ballLayer)) 
			{
				obj.GetComponent<BallInteractions>().ballSignalCanBeTaken = false; 
				canThrow = true;

				Destroy(obj.gameObject);
			}
		}

	}

	void Drow()
	{
		if (Input.GetMouseButton(0))                                                                     
		{
			Vector3 mouse = Input.mousePosition;
			mouse.z = -cam.transform.position.z;
			Vector2 camRay22 = cam.ScreenToWorldPoint(mouse);
			RaycastHit2D hit = Physics2D.Raycast(camRay22, Vector2.right, 100f, interactionsLayer);
			if (hit)
			{
				SetProjectile(hit);
				CreateSegments();        
			}
			else
			{
				lr.SetVertexCount(0); 
			}
		}
	}

	void SetProjectile(RaycastHit2D hit) 
	{
		if (hit)
		{
			Vector2 Vo = FindVelocity(hit.point, shootingPont.position, 1f);

			velocity = Vo.magnitude;
			angle = Mathf.Atan2(Vo.y, Vo.x) * Mathf.Rad2Deg;
		}
	}
	Vector2 FindVelocity(Vector2 touchPoint, Vector2 shootingPont, float time)
	{
		float x, y;
		float Vx, Vy;
		Vector2 distance, xDistance;

		distance = touchPoint - shootingPont;
		xDistance = distance;
		xDistance.y = 0;

		x = distance.x;
		y = distance.y; 

		Vx = x / time;
		Vy = y / time + 1 / 2f * Math.Abs(Physics2D.gravity.y) * time; 

		Vector2 result = xDistance.normalized; 
		result *= (Vx >= 0) ? Vx : -Vx; 
		result.y = Vy;

		return result;
	}

	void CreateSegments() 
	{
		lr.SetVertexCount(segmentsLenght + 1);
		lr.SetPositions(SetEachSegments());
	}
	Vector3[] SetEachSegments()
	{
		float distance;
		Vector3[] arcArray = new Vector3[segmentsLenght + 1];

		radianValue = Mathf.Deg2Rad * angle; 
		distance = (velocity * velocity * Mathf.Sin(2 * radianValue) / g); 
		for (int i = 0; i <= segmentsLenght; i++)
		{
			float t = (float)i / (float)segmentsLenght; 
			arcArray[i] = GetSegmentPosition(t, distance);
		}
		return arcArray;
	}


	Vector3 GetSegmentPosition(float time, float distance) 
	{
		float x, y;
		Vector3 result;
		x = time * distance; 
		y = x * Mathf.Tan(radianValue) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianValue) * Mathf.Cos(radianValue)));
		result = new Vector3(x + transform.position.x, y + transform.position.y);

		return result;
	}

	void Throw()
	{
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 mouse2 = Input.mousePosition;
			mouse2.z = -cam.transform.position.z;
			Vector2 camRay2 = cam.ScreenToWorldPoint(mouse2);
			RaycastHit2D hit = Physics2D.Raycast(camRay2, Vector2.right, 1000f, interactionsLayer);
			if (hit)
			{
				Hit(hit);
				touchPosition = hit.point;
				canThrow = false;
			}
		}
	}
	void Hit(RaycastHit2D hit)
	{
		lr.SetVertexCount(0);
		Vector2 Vo = FindVelocity(hit.point, shootingPont.position, 1f);

		obj = Instantiate(bulletPrefab, shootingPont.position, Quaternion.identity);
		obj.velocity = Vo;
	}


	void AbilityTwo()
	{
		
		if (obj.velocity.x >= 0)
		{
			if (obj.transform.position.x > touchPosition.x)
			{			
				returnBoomerang = true;
			}
		}
		else
		{
			if (obj.transform.position.x < touchPosition.x)
			{
				returnBoomerang = true;
			}
		}
		if (returnBoomerang)
		{
			obj.transform.position = Vector3.MoveTowards(obj.transform.position, shootingPont.transform.position, boomerangSpeed * Time.deltaTime);
			if (obj.transform.position == shootingPont.transform.position)
			{
				Destroy(obj.gameObject);
				returnBoomerang = false;
				canThrow = true;			
			}
		}

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Animal" || collision.tag == "Trap")
		{
			canThrow = false; // Stop using throw bu
            playerController.isDead = true;
		}

	}

}
