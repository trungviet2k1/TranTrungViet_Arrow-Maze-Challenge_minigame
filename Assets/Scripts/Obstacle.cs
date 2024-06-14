using UnityEngine;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour
{
    public Button obstacleButton;
    public Transform gridContainer;

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
        // Hiển thị các tùy chọn di chuyển xung quanh vật cản
        // Triển khai logic để hiển thị các button di chuyển tại đây
    }

    public void MoveObstacle(Vector3 newPosition)
    {
        transform.position = newPosition;
        GameManagement.Instance.CheckArrowMovement();
    }
}