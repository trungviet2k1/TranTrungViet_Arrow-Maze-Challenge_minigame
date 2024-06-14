using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum TileType { Obstacle, Step, Destination, Move, ChangeDirection }
    public TileType type;
}