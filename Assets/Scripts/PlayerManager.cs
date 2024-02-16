using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
    public int Money;
    public Dictionary<string, int> Inventory;
    public int MaxStamina;
    public int Stamina;
    public SerializableVector3 PlayerPosition;
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public GameObject player;
    public bool CanMove = true;
    private PlayerData playerData;


    // Saveable
    public int Money;
    public Dictionary<string, int> Inventory = new Dictionary<string, int>(); // Item name : Quantity
    public int MaxStamina;
    public int Stamina;
    public Vector3 PlayerPosition;

    public void SavePlayerData(string saveName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saves/player" + saveName + ".sav");

        SerializableVector3 serializablePlayerPosition = new SerializableVector3(PlayerPosition);
        

        PlayerData playerData = new PlayerData
        {
            Money = Money,
            Inventory = Inventory,
            MaxStamina = MaxStamina,
            Stamina = Stamina,
            PlayerPosition = serializablePlayerPosition
        };

        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Saved Player");
    }

    // Function to load player data from a file
    public bool LoadPlayerData(string saveName)
    {
        string path = Application.persistentDataPath + "/saves/player" + saveName + ".sav";
        if (File.Exists(path))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saves/player" + saveName + ".sav", FileMode.Open);
            PlayerData playerData = (PlayerData)bf.Deserialize(file);
            file.Close();

            // Convert PlayerPosition keys back to Vector3Int
            Vector3 deserializedPlayerPosition = playerData.PlayerPosition.ToVector3();
    
            Money = playerData.Money;
            Inventory = playerData.Inventory;
            MaxStamina = playerData.MaxStamina;
            Stamina = playerData.Stamina;
            PlayerPosition = deserializedPlayerPosition;

            player.transform.position = PlayerPosition;
            Debug.Log("Loaded Player");
            return true;
        }
        else
        {
            Debug.Log("Not Loaded Player");
            return false;
        }
    }

    private Dictionary<string, int> InitializeEmptyInventory()
    {
        Dictionary<string, int> inventory = new Dictionary<string, int>();

        foreach (string item in MainManager.Instance.Items)
        {
            if (!inventory.ContainsKey(item))
            {
                inventory[item] = 0;
            }
        }

        return inventory;
    }

    public void InitStats()
    {
        MaxStamina = 30;
        Stamina = 30;

        Inventory = InitializeEmptyInventory();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void UpdateStamina()
    {
        if (Stamina <= 0)
        {
            Stamina = MaxStamina;
            MainManager.Instance.Hour += 1;
        }
    }

    private void Update()
    {
        UpdateStamina();
        if (player != null)
        {
            PlayerPosition = player.transform.position;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}