using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    public TextMeshProUGUI TextMeshPro;

    void Update()
    {
        TextMeshPro.text = "";
        // Update the TextMeshPro text with the variable value
        TextMeshPro.text += "Money: " + MainManager.Instance.MoneyToString(PlayerManager.Instance.Money) + "\n";
        TextMeshPro.text += "Day:" + MainManager.Instance.Day.ToString() + "\n";
        TextMeshPro.text += "hour:" + MainManager.Instance.Hour.ToString() + "\n"; ;
        TextMeshPro.text += "Stamina: " + PlayerManager.Instance.MaxStamina.ToString() + "/" + PlayerManager.Instance.Stamina.ToString() + "\n";
    }
}