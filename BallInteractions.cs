﻿using UnityEngine;

public class BallInteractions : MonoBehaviour
{
	public PlayerAbilities playerAbilities;
	public Animator auraAnimation;
	public bool ballSignalCanBeTaken; //ball aura
	public bool ballOnGround; ////ball is on ground
    public PlayerController playerController;
    void Update()
	{
		if(ballSignalCanBeTaken && ballOnGround)
		{
			auraAnimation.SetBool("AuraAnim", true);
		}
		else
		{
			auraAnimation.SetBool("AuraAnim", false);
		}
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player" && playerAbilities.currentAbility == 0)
		{
			ballSignalCanBeTaken = true; //player in the ball area
			
		}
		if (collision.tag == "Animal" || collision.tag == "Trap")
		{
			playerAbilities.canThrow = false; // Stop using throw bu
            playerController.isDead = true;
			Destroy(gameObject);
		}
		if (collision.tag == "Ground") //ball is on ground
		{
			ballOnGround = true;
			if (playerAbilities.currentAbility == 1) //if abiliy 2 lanced
			{
				
				playerAbilities.returnBoomerang = true; //return boomerang because hit something
				Debug.Log("hitted");
			}
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player" && playerAbilities.currentAbility == 0)
		{
			ballSignalCanBeTaken = false; // player out of the ball area
		}

		if (collision.tag == "Ground") //ball exit ground
		{
			ballOnGround = false;
		}

	}

}
