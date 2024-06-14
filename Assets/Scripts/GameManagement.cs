using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance { get; private set; }

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
    private bool isMoving = false;
    private bool canMoveHorizontally = false;
    private bool isChangingDirection = false;

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
    }

    public void SetupGame()
    {
        arrowPositionIndex = -1;
        timeRemaining = 45f;
        timerRunning = true;
        gameWon = false;
        canMoveHorizontally = false;
        isChangingDirection = false;

        if (arrowTile != null)
        {
            Destroy(arrowTile);
        }

        if (gridContainer.childCount > 0)
        {
            arrowTile = Instantiate(arrowTilePrefab, gridContainer.GetChild(arrowPositionIndex + 1).position, Quaternion.identity, gridContainer);
            arrowTile.GetComponent<Button>().onClick.AddListener(CheckArrowMovement);
        }

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

    public void CanChangeDirection()
    {
        canMoveHorizontally = true;
        isChangingDirection = false;

        Transform arrowImageTransform = arrowTile.transform.Find("Arrow");

        if (arrowImageTransform != null)
        {
            arrowImageTransform.rotation = Quaternion.Euler(0, 0, -180);
        }
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
        if (isMoving || isChangingDirection)
            return;

        int nextPositionIndex = arrowPositionIndex + 1;
        if (nextPositionIndex >= gridContainer.childCount)
            return;

        Transform nextPositionTransform = gridContainer.GetChild(nextPositionIndex);

        if (nextPositionTransform.TryGetComponent<GridManager>(out var gridManager))
        {
            Vector3 targetPosition = nextPositionTransform.position;
            int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

            if (currentLevel == 1 || currentLevel == 2)
            {
                if (Mathf.Approximately(targetPosition.x, arrowTile.transform.position.x) && targetPosition.y > arrowTile.transform.position.y)
                {
                    MoveArrowToPosition(nextPositionIndex, targetPosition, gridManager, 0.5f);
                }
            }
            else if (currentLevel == 3)
            {
                if (canMoveHorizontally)
                {
                    if (Mathf.Approximately(targetPosition.y, arrowTile.transform.position.y) && targetPosition.x > arrowTile.transform.position.x)
                    {
                        MoveArrowToPosition(nextPositionIndex, targetPosition, gridManager, 0.3f); // faster speed
                    }
                }
                else if (Mathf.Approximately(arrowTile.transform.position.x, gridContainer.GetChild(0).position.x))
                {
                    if (Mathf.Approximately(targetPosition.x, arrowTile.transform.position.x) && targetPosition.y > arrowTile.transform.position.y)
                    {
                        MoveArrowToPosition(nextPositionIndex, targetPosition, gridManager, 0.5f);
                    }
                }
            }
        }
    }

    private void MoveArrowToPosition(int nextPositionIndex, Vector3 targetPosition, GridManager gridManager, float duration)
    {
        if (gridManager.type == GridManager.TileType.Destination)
        {
            StartCoroutine(MoveArrowCoroutine(targetPosition, ShowWinPanel, duration));
            arrowPositionIndex = nextPositionIndex;
        }
        else if (gridManager.type == GridManager.TileType.Obstacle)
        {
            Debug.Log("Encountered an obstacle, stopping movement.");
        }
        else if (gridManager.type == GridManager.TileType.Step)
        {
            arrowPositionIndex++;
            StartCoroutine(MoveArrowCoroutine(targetPosition, MoveArrowTile, duration));
        }
        else if (gridManager.type == GridManager.TileType.ChangeDirection)
        {
            arrowPositionIndex++;
            isChangingDirection = true;
            StartCoroutine(MoveArrowCoroutine(targetPosition, CanChangeDirection, 1f));
        }
    }

    IEnumerator MoveArrowCoroutine(Vector3 targetPosition, System.Action onMoveComplete, float duration)
    {
        isMoving = true;
        Vector3 startPosition = arrowTile.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            arrowTile.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        arrowTile.transform.position = targetPosition;
        isMoving = false;
        onMoveComplete?.Invoke();
    }

    public void CheckArrowMovement()
    {
        MoveArrowTile();
    }

    public void ResetGame()
    {
        SetupGame();
    }

    public void NextLevel(int levelId)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelId);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level " + levelId);
    }
}