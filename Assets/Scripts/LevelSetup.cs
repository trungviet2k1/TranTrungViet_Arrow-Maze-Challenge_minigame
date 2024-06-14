using UnityEngine;
using UnityEngine.UI;

public class LevelSetup : MonoBehaviour
{
    public static LevelSetup Instance { get; set; }

    public GridLayoutGroup gridLayout;
    public GameObject destinationPrefab;
    public GameObject stepPrefab;
    public GameObject obstaclePrefab;
    public GameObject changeDirectionPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel == 1)
        {
            CreateGridLevel1();
        }
        else if (currentLevel == 2)
        {
            CreateGridLevel2();
        }
        else if (currentLevel == 3)
        {
            CreateGridLevel3();
        }

        GameManagement.Instance.SetupGame();
    }

    public void CreateGridLevel1()
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                Instantiate(destinationPrefab, gridLayout.transform);
            }
            else
            {
                Instantiate(stepPrefab, gridLayout.transform);
            }
        }
    }

    public void CreateGridLevel2()
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                Instantiate(destinationPrefab, gridLayout.transform);
            }

            if (i == 2)
            {
                Instantiate(obstaclePrefab, gridLayout.transform);
            }
            else
            {
                Instantiate(stepPrefab, gridLayout.transform);
            }
        }
    }

    public void CreateGridLevel3()
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 5;

        for (int i = 0; i < 24; i++)
        {
            if (i == 4)
            {
                Instantiate(destinationPrefab, gridLayout.transform);
            }
            if (i == 0)
            {
                Instantiate(changeDirectionPrefab, gridLayout.transform);
            }
            else
            {
                Instantiate(stepPrefab, gridLayout.transform);
            }
        }
    }
}