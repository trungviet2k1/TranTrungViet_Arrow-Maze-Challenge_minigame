using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            Image lockImage = levelButtons[i].GetComponentInChildren<Image>();
            TextMeshProUGUI levelText = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (levelText != null && lockImage != null)
            {
                if (levelNumber == 1)
                {
                    levelText.text = levelNumber.ToString();
                }
                else
                {
                    bool isUnlocked = PlayerPrefs.GetInt("Level" + levelNumber + "Unlocked", 0) == 1;

                    if (isUnlocked)
                    {
                        lockImage.gameObject.SetActive(false);
                        levelText.gameObject.SetActive(true);
                        levelText.text = levelNumber.ToString();
                    }
                    else
                    {
                        lockImage.gameObject.SetActive(true);
                        levelText.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void SelectLevel(int levelIndex)
    {
        int selectedLevel = levelIndex + 1;

        bool isUnlocked = PlayerPrefs.GetInt("Level" + selectedLevel + "Unlocked", selectedLevel == 1 ? 1 : 0) == 1;

        if (isUnlocked)
        {
            PlayerPrefs.SetInt("CurrentLevel", selectedLevel);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Level " + selectedLevel);
        }
    }
}