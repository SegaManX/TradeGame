using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapChecker : MonoBehaviour
{
    public Tilemap tilemap;
    public int jak = 0;
    public GameObject playerObject;
    public TileBase secondTypeTile;
    private bool enteredCity;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            Vector3Int playerTilePosition = tilemap.WorldToCell(playerObject.transform.position);
            TileBase tile = tilemap.GetTile(playerTilePosition);

            if (tile == secondTypeTile)
            {
                if (!enteredCity)
                {
                    jak += 1;
                    enteredCity = true;
                }
            }
            else
            {
                enteredCity = false;
            }
        }
    }
}