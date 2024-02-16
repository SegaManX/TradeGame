using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public TileMapChecker tileMapChecker;

    void Update()
    {
        // Update the TextMeshPro text with the variable value
        textMeshPro.text = "Gold: " + tileMapChecker.jak.ToString();
    }
}