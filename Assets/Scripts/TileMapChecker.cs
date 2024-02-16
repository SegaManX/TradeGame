using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TMPro;


public class TileMapChecker : MonoBehaviour
{
    // Collision /w player
    public Tilemap Tilemap;
    public GameObject PlayerObject;

    // City Menu Update
    public GameObject CityMenuObject;

    public TMP_Text NameText;
    public TMP_Text PopText;
    public TMP_Text WealthText;
    public TMP_Text FoodText;

    public TextMeshProUGUI PricesText;

    public GameObject InventoryEntryPrefab;
    public Transform ContentPanel;
    public float VerticalSpacing = 20f;

    public Transform PlayerContentPanel;
    public GameObject PlayerInventoryEntryPrefab;


    // Logic
    private bool EnteredCity;
    private bool WasSetActive = true;


    void Start()
    {
        if(MainManager.Instance.GameLoaded == false)
        {
            MainManager.Instance.Hour = 0;
            MainManager.Instance.Day = 1;
            MainManager.Instance.TileIDMap = new Dictionary<Vector3Int, int>();
            MainManager.Instance.TileInfoMap = new Dictionary<int, TileInfo>();
            MainManager.Instance.GenerateTileIDs(Tilemap);
        }
        else
        {
            PlayerObject.transform.position = PlayerManager.Instance.PlayerPosition;
        }
    }

    private void Update()
    {
        UpdatePlayerMovementState();;
    }

    private void UpdatePlayerMovementState()
    {
        if (CityMenuObject.activeSelf != WasSetActive)
        {
            PlayerManager.Instance.CanMove = !CityMenuObject.activeSelf;
            WasSetActive = CityMenuObject.activeSelf;
        }
    }

    public void UpdateCityMenu(int tileID)
    {
        TileInfo tileInfo = MainManager.Instance.TileInfoMap[tileID];

        // Set TextMeshPro component values
        NameText.text = tileInfo.TileName;
        PopText.text = "Pop: " + tileInfo.TilePop.ToString();
        WealthText.text = "Wealth: " + tileInfo.TileWealth.ToString();
        FoodText.text = "Food: " + tileInfo.TileFood.ToString();

        // Activate the CityMenu object to show the value
        CityMenuObject.SetActive(true);

        DisplayInventory(tileID);
        DisplayPrices(tileID);
    }

    public void DisplayInventory(int tileID)
    {
        ClearInventory();
        float entryHeight = InventoryEntryPrefab.GetComponent<RectTransform>().rect.height;
        float currentYPos = 250f;

        var tileInfo = MainManager.Instance.TileInfoMap[tileID];

        foreach (var entry in tileInfo.TileInventory)
        {
            GameObject newEntry = Instantiate(InventoryEntryPrefab, ContentPanel);
            RectTransform rectTransform = newEntry.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-180f, currentYPos);
            currentYPos -= entryHeight + VerticalSpacing;

            UpdateInventoryEntry(newEntry, entry.Key, entry.Value, tileInfo.TilePrices[entry.Key], tileID);
        }
    }

    public void ClearInventory()
    {
        foreach (Transform child in ContentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateInventoryEntry(GameObject entry, string itemName, int itemData, int itemPrice, int tileID)
    {
        // Update UI elements within the inventory entry prefab using inventoryData
        TextMeshProUGUI itemText = entry.transform.Find("Inventory").GetComponent<TextMeshProUGUI>();
        itemText.text = itemName + ": " + itemData.ToString() + "x - " + MainManager.Instance.MoneyToString(itemPrice);

        // Add functionality to buy and sell buttons
        Button buyButton = entry.transform.Find("BuyBtn").GetComponent<Button>();
        buyButton.onClick.AddListener(() => BuyAction(itemName, tileID));

        Button sellButton = entry.transform.Find("SellBtn").GetComponent<Button>();
        sellButton.onClick.AddListener(() => SellAction(itemName, tileID));
    }

    void BuyAction(string itemName, int tileID)
    {
        TileInfo tile = MainManager.Instance.TileInfoMap[tileID];

        if (tile.TileInventory.ContainsKey(itemName) && tile.TileInventory[itemName] > 0)
        {
            if (PlayerManager.Instance.Money >= tile.TilePrices[itemName])
            {
                PlayerManager.Instance.Money -= tile.TilePrices[itemName];
                PlayerManager.Instance.Inventory[itemName] += 1;
                tile.TileInventory[itemName] -= 1;
                tile.TileWealth += tile.TilePrices[itemName];

                MainManager.Instance.TileInfoMap[tileID] = tile;

                DisplayInventory(tileID);
                UpdateCityMenu(tileID);
            }
        }
    }

    void SellAction(string itemName, int tileID)
    {
        TileInfo tile = MainManager.Instance.TileInfoMap[tileID];

        if (PlayerManager.Instance.Inventory[itemName] > 0)
        {
            PlayerManager.Instance.Money += tile.TilePrices[itemName];
            PlayerManager.Instance.Inventory[itemName] -= 1;
            tile.TileInventory[itemName] += 1;
            tile.TileWealth -= tile.TilePrices[itemName];

            MainManager.Instance.TileInfoMap[tileID] = tile;
            DisplayInventory(tileID);
            UpdateCityMenu(tileID);
        }
    }

    public void DisplayPrices(int tileID)
    {
        if (MainManager.Instance.TileInfoMap.ContainsKey(tileID))
        {
            var tileInfo = MainManager.Instance.TileInfoMap[tileID];
            string pricesDisplay = "Prices:\n";

            foreach (var item in tileInfo.TilePrices)
            {
                pricesDisplay += $"{item.Key}: {MainManager.Instance.MoneyToString(item.Value)}\n";
            }

            PricesText.text = pricesDisplay;
        }
        else
        {
            PricesText.text = "No inventory available for this city.";
        }
    }


    public void DisplayPlayerInventory()
    {
        ClearPlayerInventory();
        float entryHeight = PlayerInventoryEntryPrefab.GetComponent<RectTransform>().rect.height;
        float currentYPos = 250f;

        foreach (var entry in PlayerManager.Instance.Inventory)
        {
            if (entry.Value > 0)
            {
                GameObject newEntry = Instantiate(PlayerInventoryEntryPrefab, PlayerContentPanel);
                RectTransform rectTransform = newEntry.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(-180f, currentYPos);
                currentYPos -= entryHeight + VerticalSpacing;

                UpdatePlayerInventoryEntry(newEntry, entry.Key, entry.Value);
            }
        }
    }
    public void ClearPlayerInventory()
    {
        foreach (Transform child in PlayerContentPanel)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdatePlayerInventoryEntry(GameObject entry, string itemName, int itemData)
    {
        // Update UI elements within the inventory entry prefab using inventoryData
        TextMeshProUGUI itemText = entry.transform.Find("Inventory").GetComponent<TextMeshProUGUI>();
        itemText.text = itemName + ": " + itemData.ToString() + "x";
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerObject)
        {
            Vector3Int playerTilePosition = Tilemap.WorldToCell(PlayerObject.transform.position);
            TileBase tile = Tilemap.GetTile(playerTilePosition);

            if (tile != null && tile.name.Contains("City"))
            {
                if (!EnteredCity)
                {
                    UpdateCityMenu(MainManager.Instance.GetTileID(playerTilePosition));
                    EnteredCity = true;
                }
            }
            else
            {
                EnteredCity = false;
            }
        }
    }


}