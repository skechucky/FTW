using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public LevelDetails[] levelDetails;
	public Vector2 buttonSize; 
	private float currentPage;
	public Vector2 blockedOffset; 	   
	public Texture blockedSprite;
	public Vector2 blockedSize;    //lock picture size
	public Canvas exitGroup;
	public Canvas resetGroup;
	public Canvas optionsGroup;
	public Button startButton;
	public Button optionButton;
	public Button exitButton;
	public Button resetButton;
	public Slider volumeSlider;
	private float volume;
	private bool canDisplayBar;
	public AudioInfo[] audioInfo;//array of audio information	
	
	AudioSource audioSource;
	void Start()
	{
		exitGroup.enabled = false;
		optionsGroup.enabled = false;

		if (!PlayerPrefs.HasKey("CompletedLevels"))
		{
			PlayerPrefs.SetFloat("CompletedLevels", 0);
		}
		if (!PlayerPrefs.HasKey("Volume"))
		{
			PlayerPrefs.SetFloat("Volume", 0.5f);
		}
		audioSource = GetComponent<AudioSource>(); // add reference

		volumeSlider.value = PlayerPrefs.GetFloat("Volume"); //display current volume
		audioInfo[0].LoopPlayAudio(audioSource);
	}

	void OnGUI()
	{
		if (canDisplayBar)
		{
			if (levelDetails.Length > 0)
			{
				float width = 5 * buttonSize.x + 30; 
				GUI.BeginGroup(new Rect(Screen.width / 2 - width / 2, 250, width, buttonSize.y + 10), "", "Box"); //Levels Bar

				for (var i = 0; i < levelDetails.Length; i++)
				{
					Vector2 rectVector = new Vector2(buttonSize.x * i + 5 + i * 5, 5);
					float rectOffset = width * currentPage - currentPage * 5; 
					Rect rect = new Rect(rectVector.x - rectOffset, rectVector.y, buttonSize.x, buttonSize.y); 

					if (levelDetails[i].LevelPicture)
					{
						if (GUI.Button(rect, levelDetails[i].LevelPicture) && levelDetails[i].unlockedLevels <= PlayerPrefs.GetInt("CompletedLevels"))
						{
							SceneManager.LoadScene(levelDetails[i].sceneName); 
						}
					}

					if (levelDetails[i].unlockedLevels > PlayerPrefs.GetInt("CompletedLevels")) 
					{
						GUI.Label(new Rect(rectVector.x - rectOffset + blockedOffset.x, rectVector.y + blockedOffset.y, blockedSize.x, blockedSize.y), blockedSprite);
					}

				}

				GUI.EndGroup();
			}

			
			if (GUI.Button(new Rect(350, 245, 40, buttonSize.y + 20), "<"))
			{
				if (currentPage > 0)
				{
					currentPage -= 1;
				}
			}

			
			if (GUI.Button(new Rect(Screen.width - 400, 245, 40, buttonSize.y + 20), ">"))
			{
				if (currentPage < 1)
				{
					currentPage += 1;
				}
			}
		}

	}
	public void StartLevelMap()
	{
		canDisplayBar = canDisplayBar ? false : true;

		exitGroup.enabled = false;
		optionsGroup.enabled = false;
	}

	public void Resett()
	{
		resetGroup.enabled = true;
		//resetButton.enabled = false;
	}
	public void Options()
	{
		optionsGroup.enabled = true;

		//Disable others
		canDisplayBar = false;
		exitGroup.enabled = false;
		resetGroup.enabled = false;
	}

	public void Exit()
	{
		exitGroup.enabled = true;

		canDisplayBar = false;
		optionsGroup.enabled = false;
	}

	public void SetVolume(float value)
	{
		PlayerPrefs.SetFloat("Volume", value);
		audioSource.volume = value; //set volume
	}

	public void NoExit()
	{
		exitGroup.enabled = false;
	}

	public void NoReset()
	{
		resetGroup.enabled = false;
	}

	public void YesReset()
	{
		PlayerPrefs.SetInt("CompletedLevels", 0);
		optionsGroup.enabled = false;
	}

	public void YesExit()
	{
		Application.Quit();
	}

}