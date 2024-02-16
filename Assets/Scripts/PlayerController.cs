using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public Tilemap TileMap;
    public float MoveSpeed = 5f;
    private bool IsMoving = false;
    public bool MoveAfterLoad = false;

    private Dictionary<KeyCode, Vector3Int> DirectionMapping = new Dictionary<KeyCode, Vector3Int>
    {
        {KeyCode.Keypad7, new Vector3Int(-1, 1, 0)},  // NW
        {KeyCode.Keypad8, new Vector3Int(0, 1, 0)},   // N
        {KeyCode.UpArrow, new Vector3Int(0, 1, 0)},   // N
        {KeyCode.Keypad9, new Vector3Int(1, 1, 0)},   // NE
        {KeyCode.Keypad4, new Vector3Int(-1, 0, 0)},  // W
        {KeyCode.LeftArrow, new Vector3Int(-1, 0, 0)}, // W
        {KeyCode.Keypad6, new Vector3Int(1, 0, 0)},   // E
        {KeyCode.RightArrow, new Vector3Int(1, 0, 0)},// E
        {KeyCode.Keypad1, new Vector3Int(-1, -1, 0)}, // SW
        {KeyCode.Keypad2, new Vector3Int(0, -1, 0)},  // S
        {KeyCode.DownArrow, new Vector3Int(0, -1, 0)}, // S
        {KeyCode.Keypad3, new Vector3Int(1, -1, 0)}   // SE
    };

    private HashSet<Vector3Int> UniqueDirections = new HashSet<Vector3Int>();


    void Update()
    {
        if (!MoveAfterLoad)
        {
            transform.position = PlayerManager.Instance.PlayerPosition;
            MoveAfterLoad = true;
        }
        if (!IsMoving && PlayerManager.Instance.CanMove)
        {
            UniqueDirections.Clear();

            foreach (var kvp in DirectionMapping)
            {
                if (Input.GetKey(kvp.Key))
                {
                    UniqueDirections.Add(kvp.Value);
                }
            }

            if (UniqueDirections.Count > 0)
            {
                Vector3Int combinedDirection = Vector3Int.zero;
                foreach (Vector3Int dir in UniqueDirections)
                {
                    combinedDirection += dir;
                }

                Vector3Int targetTile = TileMap.WorldToCell(transform.position) + combinedDirection;
                if (TileMap.GetTile(targetTile) != null)
                {
                    StartCoroutine(MovePlayerSingle(targetTile));
                }
            }
        }
    }


    IEnumerator MovePlayerSingle(Vector3Int targetTile)
    {
        IsMoving = true;
        Vector3 targetPosition = TileMap.GetCellCenterWorld(targetTile);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            float step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            yield return null;
        }

        transform.position = targetPosition;
        IsMoving = false;

        int tileCost = MainManager.Instance.TileInfoMap[MainManager.Instance.GetTileID(targetTile)].TileMoveCost;

        if(MainManager.Instance.Hour >= 19 || MainManager.Instance.Hour <= 5)
        {
            tileCost = Mathf.RoundToInt(tileCost * 1.5f);
        }

        PlayerManager.Instance.Stamina -= tileCost;
    }

}
