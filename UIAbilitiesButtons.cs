using UnityEngine;
using UnityEngine.UI;

public class UIAbilitiesButtons : MonoBehaviour
{
	PlayerAbilities playerAbilities;
    PlayerController playerController;
	public Button[] buttonsArray;
	ColorBlock[] storedColorsArray = new ColorBlock[3]; //store our colors
	ColorBlock[] newColorsArray = new ColorBlock[3]; //red mark ability is not available
	public LayerMask arrowsLayer;

	private void Start()
	{
		playerAbilities = GameObject.FindGameObjectWithTag("PlayerAbilities").GetComponent<PlayerAbilities>();
		playerController = playerAbilities.GetComponentInParent<PlayerController>();
		PopulateArrayColors();
	}
	void Update()
	{
		buttonsArray[playerAbilities.currentAbility].Select(); 

		if (!playerAbilities.canThrow)
		{
			buttonsArray[playerAbilities.currentAbility].colors = newColorsArray[playerAbilities.currentAbility];
		}
		else
		{
			buttonsArray[playerAbilities.currentAbility].colors = storedColorsArray[playerAbilities.currentAbility];

		}
	}

	void PopulateArrayColors()
	{
		for (int i = 0; i < 3; i++) // loop in abilities buttons
		{
			storedColorsArray[i] = buttonsArray[i].colors; //store colors from component
			newColorsArray[i] = storedColorsArray[i]; //store colors from component
			newColorsArray[i].highlightedColor = Color.red;  // modifie color highlt
		}
	}

	public void UI_Ability0() //Drow + Throw
	{
		playerAbilities.currentAbility = playerAbilities.canThrow ? 0 : playerAbilities.currentAbility;
	}

	public void UI_Ability1() //Throw Boomerang
	{
		playerAbilities.currentAbility = playerAbilities.canThrow ? 1 : playerAbilities.currentAbility;
	}

	public void UI_Ability2() //Take Control
	{
		playerAbilities.currentAbility = playerAbilities.canThrow ? 2 : playerAbilities.currentAbility;

	}

	public void UI_Ability3() //Jump
	{
		if (!playerController.isJump && playerController.playerMovement.bottomRay)
		{
			playerController.isJump = true;
		}
	}

	public void UI_ArrowLeft()
	{
		if (!playerController.isDead)
		{
			playerController.input.x = -1;
		}
	}

	public void UI_ArrowRight()
	{
		if (!playerController.isDead)
		{
			playerController.input.x = 1;
		}
	}

	public void ReleaseArrows() 
	{
		playerController.input.x = 0;
	}
}
