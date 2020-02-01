using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalNextLvl : MonoBehaviour {

	public int currentLvl;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			int completedLevels = PlayerPrefs.GetInt("CompletedLevels");

			if (completedLevels < currentLvl)
			{
				PlayerPrefs.SetInt("CompletedLevels", currentLvl); //save level performance
			}

			if (currentLvl == 4)
			{
				SceneManager.LoadScene(0); // last level -> main menu
			}
			else
			{
				SceneManager.LoadScene(currentLvl + 1); // load next level
			}
		}
	}

}
