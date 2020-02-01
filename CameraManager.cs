using UnityEngine;

public class CameraManager : MonoBehaviour
{

	public Transform playerTransform;
	public Transform animalTransform;
	public Vector3 offset;
	PlayerAbilities playerAbilities;
	public float velocity = 2.8f;

	void Start()
	{
		playerAbilities = GameObject.FindGameObjectWithTag("PlayerAbilities").GetComponent<PlayerAbilities>();
		Vector3 willBeAt = playerTransform.position + offset;
		willBeAt.z = transform.position.z;
		Vector3 lerpPos = Vector3.Lerp(transform.position, willBeAt, velocity * Time.deltaTime);		transform.position = lerpPos;

	}

	void Update()
	{
		Vector3 willBeAt = (!playerAbilities.isControled) ? (playerTransform.position + offset) : (animalTransform.position);
		willBeAt.z = transform.position.z;
		Vector3 lerpPos = Vector3.Lerp(transform.position, willBeAt, velocity * Time.deltaTime);
		transform.position = lerpPos;
	}

}
