using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    public Button obstacleButton;
    public Transform gridContainer;
    public float moveRange = 1.0f;

    private void Start()
    {
        if (obstacleButton != null)
        {
            obstacleButton.onClick.AddListener(OnObstacleClick);
        }
    }

    private void OnObstacleClick()
    {
        ShowMovementOptions();
    }

    private void ShowMovementOptions()
    {
        ClearMovementOptions();

        Vector3 obstaclePosition = transform.position;
        Vector3[] directions = {
            Vector3.up * moveRange,
            Vector3.down * moveRange,
            Vector3.left * moveRange,
            Vector3.right * moveRange
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 targetPosition = obstaclePosition + direction;
            Collider2D hit = Physics2D.OverlapPoint(targetPosition);

            if (hit == null || hit.transform.CompareTag("Step"))
            {
                GameObject movementButton = new GameObject("MovementButton");
                movementButton.transform.SetParent(gridContainer);
                movementButton.transform.position = targetPosition;

                Button button = movementButton.AddComponent<Button>();
                Text buttonText = movementButton.AddComponent<Text>();
                buttonText.text = "Move";

                button.onClick.AddListener(() => MoveObstacle(targetPosition));
            }
        }
    }

    private void ClearMovementOptions()
    {
        foreach (Transform child in gridContainer)
        {
            if (child.name == "MovementButton")
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void MoveObstacle(Vector3 newPosition)
    {
        transform.position = newPosition;
        GameManagement.Instance.CheckArrowMovement();
    }
}