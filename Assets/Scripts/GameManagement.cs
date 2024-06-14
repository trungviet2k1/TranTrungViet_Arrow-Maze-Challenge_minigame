using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance { get; set; }

    [Header("GameUI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    public GameObject winPanel;
    public GameObject losePanel;
    public Button resetBtn;
    public Button homeBtn;
    public Button nextBtn;

    [Header("Title")]
    public Transform gridContainer;
    public GameObject arrowTilePrefab;

    private GameObject arrowTile;
    private int arrowPositionIndex;
    private float timeRemaining;
    private bool timerRunning;
    private bool gameWon;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    void Start()
    {
        SetupGame();
        arrowTile.GetComponent<Button>().onClick.AddListener(MoveArrowTile);
    }

    void SetupGame()
    {
        arrowPositionIndex = -1;
        timeRemaining = 45f;
        timerRunning = true;
        gameWon = false;

        if (arrowTile != null)
        {
            Destroy(arrowTile);
        }

        arrowTile = Instantiate(arrowTilePrefab, gridContainer.GetChild(arrowPositionIndex + 1).position, Quaternion.identity, gridContainer);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        levelText.text = "Level " + currentLevel.ToString();
    }

    void Update()
    {
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;

                if (!gameWon)
                {
                    ShowLosePanel();
                }
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void ShowWinPanel()
    {
        gameWon = true;
        winPanel.SetActive(true);
        timerRunning = false;

        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt("Level" + nextLevel + "Unlocked", 1);
        PlayerPrefs.Save();
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }

    public void MenuUI()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MoveArrowTile()
    {
        if (arrowPositionIndex < gridContainer.childCount - 1)
        {
            Transform targetTransform = gridContainer.GetChild(arrowPositionIndex + 1);
            if (targetTransform.position.y > arrowTile.transform.position.y)
            {
                arrowPositionIndex++;
                StartCoroutine(MoveArrowCoroutine(targetTransform.position, () =>
                {
                    if (targetTransform.GetComponent<GridManager>().type == GridManager.TileType.Destination)
                    {
                        ShowWinPanel();
                    }
                }));
            }
        }
    }

    IEnumerator MoveArrowCoroutine(Vector3 targetPosition, System.Action onMoveComplete)
    {
        float duration = 0.5f;
        Vector3 startPosition = arrowTile.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            arrowTile.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        arrowTile.transform.position = targetPosition;
        onMoveComplete?.Invoke();
    }

    public void ResetGame()
    {
        SetupGame();
        arrowTile.GetComponent<Button>().onClick.AddListener(MoveArrowTile);
    }

    public void NextLevel(int levelId)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelId);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level " + levelId);
    }
}