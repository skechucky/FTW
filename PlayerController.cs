using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    float moveAcc = 10;
    float maxSpeed = 20;
    float gravityAcc = 9.8f;
    float dragAcc = 20;
    float jumpSpeed = 20;
    float inputDirection;
    Vector2 moveVec;
    float moveSpeed;
    bool isMoving = false;
    public float jumpInput = 0;
    Vector2 gravityDir = new Vector2(0, -1);
    public Vector3 velocity;
    private float currentSmooth;
    public bool isJump;  // set from ui button
    Animal animal;
    private Animator playerAnimation;
    public bool isDead;
    public Vector3 input;
    public BoxCollider2D currentCollider2D; //selected player
    public GameObject playerMonk;
    public GameObject playerAnimal;
    Dictionary<GameObject, Vector2> onGoingCollisions = new Dictionary<GameObject, Vector2>();
    float ySpeed = 0;
    bool isFlippedX = false;

    public enum JumpState
    {
        InAir,
        Grounded
    }
    public JumpState jumpState = JumpState.InAir;

    void Start()
    {
        animal = playerAnimal.GetComponent<Animal>();
        playerAnimation = GetComponent<Animator>();

    }

    void Update()
    {
        if (!HasGroundCollisionInProgress())
            jumpState = JumpState.InAir;

        UpdateInput();
        UpdateJump();

        float dragSpeed = 0;
        if (!isMoving && jumpState == JumpState.Grounded && moveSpeed != 0)
        {
            dragSpeed = -Mathf.Sign(moveSpeed) * dragAcc * Time.fixedDeltaTime;
            if (Mathf.Sign(moveSpeed + dragSpeed) != Mathf.Sign(moveSpeed))
            {
                dragSpeed = -moveSpeed;
            }
        }

        float gravityEnabled = jumpState == JumpState.Grounded ? 0 : 1;

        moveSpeed = Mathf.Clamp(moveSpeed + inputDirection * moveAcc * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
        moveSpeed = moveSpeed + gravityEnabled * gravityDir.x * gravityAcc * Time.fixedDeltaTime;
        moveSpeed = moveSpeed + dragSpeed;

        ySpeed = ySpeed + gravityEnabled * gravityDir.y * gravityAcc * Time.fixedDeltaTime;

        transform.position = transform.position + new Vector3(moveSpeed, ySpeed, 0) * Time.fixedDeltaTime;

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Collider")
        {
            Vector2 normal = col.contacts[0].normal;
            if (!onGoingCollisions.ContainsKey(col.gameObject))
                onGoingCollisions.Add(col.gameObject, normal);
            if (IsGround(normal))
            {
                jumpState = JumpState.Grounded;
                Debug.Log(jumpState.ToString());
                ySpeed = 0;
            }
        }

        Debug.Log("Enter " + col.gameObject.name + " " + onGoingCollisions.Count);
    }
    private bool HasGroundCollisionInProgress()
    {
        foreach (var collision in onGoingCollisions)
        {
            if (IsGround(collision.Value))
            {
                return true;
            }
        }
        return false;
    }
    void UpdateInput()
    {
        inputDirection = 0;
        if (input.x == -1)
            inputDirection += -1;
        if (input.x == 1)
            inputDirection += 1;

        if (inputDirection != 0 && Mathf.Sign(inputDirection) != Mathf.Sign(moveSpeed))
            moveSpeed = 0;
        isMoving = inputDirection != 0;
    }

    void UpdateJump()
    {
        jumpInput = 0;
        if (input.x == 1)
        {
            if (jumpState == JumpState.Grounded)
                jumpInput = 1;
        }
        else
        {
            if (input.x == -1 && jumpState == JumpState.Grounded)
                jumpInput = 1;
        }

        if (jumpInput == 0)
            ySpeed = jumpInput * jumpSpeed * -gravityDir.y;
    }


    private bool IsGround(Vector2 normal)
    {
        return Vector3.Angle(normal, -gravityDir) < 15;
    }

    void FlipVerificationX()
    {
        if (inputDirection < 0 && isFlippedX == false)
        {
            FlipX();
            isFlippedX = true;
        }
        else if (inputDirection > 0 && isFlippedX == true)
        {
            FlipX();
            isFlippedX = false;
        }
    }

    void FlipX()
    {
        Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
    }

    void AnimalAnimations()
    {
        if (input.x == 0)
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
        if (input.x == 0)
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

        if (jumpState == JumpState.InAir)
        {
            if (input.x == 0)
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