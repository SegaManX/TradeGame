using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class TileInfo
{
    public string TileName { get; set; }
    public int TilePop { get; set; }
    public int TileWealth { get; set; }
    public int TileFood { get; set; }
    public int TileMoveCost { get; set; }
    public Dictionary<string, int> TileInventory { get; set; }
    public Dictionary<string, int> TilePrices { get; set; }

    public TileInfo(string name, int pop, int wealth, int food, int tileMoveCost, Dictionary<string, int> tileInventory, Dictionary<string, int> tilePrices)
    {
        TileName = name;
        TilePop = pop;
        TileWealth = wealth;
        TileFood = food;
        TileMoveCost = tileMoveCost;
        TileInventory = tileInventory;
        TilePrices = tilePrices;
    }
}

public class TileGenerator
{
    public string[] TileNames { get; set; }
    public int TilePopMin { get; set; }
    public int TilePopMax { get; set; }
    public int TileWealthMin { get; set; }
    public int TileWealthMax { get; set; }
    public int TileFoodMin { get; set; }
    public int TileFoodMax { get; set; }
    public int TileMoveCost { get; set; }
    public Dictionary<string, int> TileInventoryMin { get; set; }
    public Dictionary<string, int> TileInventoryMax { get; set; }
    public Dictionary<string, int> TilePricesMin { get; set; }
    public Dictionary<string, int> TilePricesMax { get; set; }

    public TileGenerator(
        string[] tileNames,
        int tilePopMin, int tilePopMax,
        int tileWealthMin, int tileWealthMax,
        int tileFoodMin, int tileFoodMax,
        int tileMoveCost,
        Dictionary<string, int> tileInventoryMin, Dictionary<string, int> tileInventoryMax,
        Dictionary<string, int> tilePricesMin, Dictionary<string, int> tilePricesMax
    )
    {
        TileNames = tileNames;
        TilePopMin = tilePopMin;
        TilePopMax = tilePopMax;
        TileWealthMin = tileWealthMin;
        TileWealthMax = tileWealthMax;
        TileFoodMin = tileFoodMin;
        TileFoodMax = tileFoodMax;
        TileMoveCost = tileMoveCost;
        TileInventoryMin = tileInventoryMin;
        TileInventoryMax = tileInventoryMax;
        TilePricesMin = tilePricesMin;
        TilePricesMax = tilePricesMax;
    }
}

[System.Serializable]
public class SerializableVector3Int
{
    public int x;
    public int y;
    public int z;

    public SerializableVector3Int(Vector3Int vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
}

[System.Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class MapData
{
    public int Hour;
    public int Day;
    public Dictionary<SerializableVector3Int, int> TileIDMap;
    public Dictionary<int, TileInfo> TileInfoMap;
}

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public AudioSource audioSource;
    public string VolumePrefsKey = "VolumeLevel"; // Key for PlayerPrefs
    public Dictionary<string, TileGenerator> TileGeneratorMap = new Dictionary<string, TileGenerator>();
    public string[] Items = { "Wood", "Iron", "Coal", "Leather", "Crystals", "Cloth", "Weapons", "Potions" };
    public bool GameLoaded = false;

    // Saveable
    public int Hour = 0;
    public int Day = 1;
    public Dictionary<Vector3Int, int> TileIDMap = new Dictionary<Vector3Int, int>();
    public Dictionary<int, TileInfo> TileInfoMap = new Dictionary<int, TileInfo>();

    public void SaveMapData(string saveName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/saves/map" + saveName + ".sav");

        // Convert TileIDMap keys to SerializableVector3Int
        Dictionary<SerializableVector3Int, int> serializableTileIDMap = new Dictionary<SerializableVector3Int, int>();
        foreach (var kvp in TileIDMap)
        {
            serializableTileIDMap[new SerializableVector3Int(kvp.Key)] = kvp.Value;
        }

        MapData mapData = new MapData
        {
            Hour = Hour,
            Day = Day,
            TileIDMap = serializableTileIDMap,
            TileInfoMap = TileInfoMap
        };

        bf.Serialize(file, mapData);
        file.Close();
        Debug.Log("Saved Map");
    }

    // Function to load player data from a file
    public bool LoadMapData(string saveName)
    {
        string path = Application.persistentDataPath + "/saves/map" + saveName + ".sav";
        if (File.Exists(path))
        {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saves/map" + saveName + ".sav", FileMode.Open);
            MapData mapData = (MapData)bf.Deserialize(file);
            file.Close();

            // Convert TileIDMap keys back to Vector3Int
            Dictionary<Vector3Int, int> deserializedTileIDMap = new Dictionary<Vector3Int, int>();
            foreach (var kvp in mapData.TileIDMap)
            {
                deserializedTileIDMap[kvp.Key.ToVector3Int()] = kvp.Value;
            }

            Hour = mapData.Hour;
            Day = mapData.Day;
            TileIDMap = deserializedTileIDMap;
            TileInfoMap = mapData.TileInfoMap;
            Debug.Log("Loaded Map");
            return true;
        }
        else
        {
            Debug.Log("Not Loaded Map");
            return false;
        }
    }

    public void NewGame()
    {
        PlayerManager.Instance.InitStats();
    }

    public void SaveGame()
    {
        PlayerManager.Instance.SavePlayerData("Test0");
        SaveMapData("Test0");
    }

    public void LoadGame()
    {
        GameLoaded = true;
        PlayerManager.Instance.LoadPlayerData("Test0");
        LoadMapData("Test0");
    }

    private void Init()
    {
        GenerateTileGeneratorMap();
        if (PlayerPrefs.HasKey(VolumePrefsKey))
        {
            audioSource.volume = PlayerPrefs.GetFloat(VolumePrefsKey);
        }
    }

    private void GenerateTileGeneratorMap()
    {
        TileGeneratorMap["Tile_Grass_Empty_Plains"] = new TileGenerator(
            new string[] { "Grass", "Grass" },
            0,
            0,
            0,
            0,
            0,
            0,
            10,
            new Dictionary<string, int> {},
            new Dictionary<string, int> {},
            new Dictionary<string, int> {},
            new Dictionary<string, int> {}
        );
        TileGeneratorMap["Tile_Grass_City_2-2-Dirt"] = new TileGenerator(
            new string[] { "Aldermoor","Briarwick","Crestmoor","Dunmere","Elderford","Fairholm","Goldfield","Hawthorne","Ivyport","Juniper's Crossing","Kestrel's Reach","Larkspur","Meadowbrook","Nightingale","Oakhaven","Pinecrest","Quail's Rest","Rosewater","Sagefield","Thistlebrook","Umberwood","Valleyview","Willowgate","Yarrowfell","Zephyr's Hollow","Auburnstead","Bramblebury","Copperwood","Dewhurst","Embermoor","Fernwood","Glenhaven","Hazelridge","Irisvale","Jade Hollow","Kingsford","Lilyvale","Mossridge","Noble's Landing","Oakenshade","Primrose Hill","Quartzvale","Raven's Nest","Silverfall","Tanglewood","Umberton","Verdant Hollow","Wispwater","Xanadu's Rest"},
            100,
            9999,
            1000,
            99999,
            10,
            100,
            5,
            new Dictionary<string, int> { { "Wood", 5 },{ "Iron", 5 },{ "Coal", 5 },{ "Leather", 5 },{ "Crystals", 0 },{ "Cloth", 5 },{ "Weapons", 0 },{ "Potions", 0 }},
            new Dictionary<string, int> { { "Wood", 100 },{ "Iron", 100 },{ "Coal", 100 },{ "Leather", 100 },{ "Crystals", 25 },{ "Cloth", 100 },{ "Weapons", 50 },{ "Potions", 50 } },
            new Dictionary<string, int> { { "Wood", 10 },{ "Iron", 15 },{ "Coal", 12 },{ "Leather", 7 },{ "Crystals", 40 },{ "Cloth", 3 },{ "Weapons", 20 },{ "Potions", 100 } },
            new Dictionary<string, int> { { "Wood", 20 },{ "Iron", 30 },{ "Coal", 40 },{ "Leather", 25 },{ "Crystals", 100 },{ "Cloth", 10 },{ "Weapons", 500 },{ "Potions", 2400 } }
        );

    }
    public void GenerateTileIDs(Tilemap tilemap)
    {
        int currentID = 1;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                TileIDMap[pos] = currentID;
                TileBase currentTile = tilemap.GetTile(pos);
                string tileName = currentTile.name;
                TileGenerator currentTileGenerator = TileGeneratorMap[tileName];
                string randomName = GenerateRandomName(tileName);
                int tileMoveCost = currentTileGenerator.TileMoveCost;

                int randomPop = 0, randomWealth = 0, randomFood = 0;
                Dictionary<string, int> randomInventory = null, randomPriceList = null;

                if (currentTile != null && currentTile.name.Contains("City"))
                {
                    randomPop = UnityEngine.Random.Range(currentTileGenerator.TilePopMin, currentTileGenerator.TilePopMax);
                    randomWealth = UnityEngine.Random.Range(currentTileGenerator.TileWealthMin, currentTileGenerator.TileWealthMax);
                    randomFood = UnityEngine.Random.Range(currentTileGenerator.TileFoodMin, currentTileGenerator.TileFoodMax);
                    randomInventory = GenerateRandomInventory(tileName);
                    randomPriceList = GenerateRandomPriceList(tileName);
                }

                TileInfo tileInfo = new TileInfo(randomName, randomPop, randomWealth, randomFood, tileMoveCost, randomInventory ?? new Dictionary<string, int>(), randomPriceList ?? new Dictionary<string, int>());
                TileInfoMap[currentID] = tileInfo;
                currentID++;
            }
        }
    }

    public string GenerateRandomName(string tileName)
    {
        string[] tileNames = TileGeneratorMap[tileName].TileNames;
        return tileNames[UnityEngine.Random.Range(0, tileNames.Length)];
    }

    public Dictionary<string, int> GenerateRandomInventory(string tileName)
    {
        TileGenerator curTileGen = TileGeneratorMap[tileName];
        Dictionary<string, int> inventory = new Dictionary<string, int>();

        foreach (string item in Items)
        {
            inventory[item] = UnityEngine.Random.Range(curTileGen.TileInventoryMin[item], curTileGen.TileInventoryMax[item]);
        }

        return inventory;
    }

    public Dictionary<string, int> GenerateRandomPriceList(string tileName)
    {
        TileGenerator curTileGen = TileGeneratorMap[tileName];
        Dictionary<string, int> priceList = new Dictionary<string, int>();

        foreach (string item in Items)
        {
            priceList[item] = UnityEngine.Random.Range(curTileGen.TilePricesMin[item], curTileGen.TilePricesMax[item]);
        }

        return priceList;
    }

    public string MoneyToString(int money)
    {
        int moneyToSilver = 99;
        int silverToGold = 100;
        int goldToPlatinum = 100;

        int bronze = money % moneyToSilver;
        int silver = (money / moneyToSilver) % silverToGold;
        int gold = (money / (moneyToSilver * silverToGold)) % goldToPlatinum;
        int platinum = money / (moneyToSilver * silverToGold * goldToPlatinum);

        string result = "";

        if (platinum > 0)
            result += platinum + "P ";
        if (gold > 0)
            result += gold + "G ";
        if (silver > 0)
            result += silver + "S ";
        if (bronze > 0 || result == "")
            result += bronze + "B";

        return result.Trim();
    }

    public int GetTileID(Vector3Int position)
    {
        if (TileIDMap.ContainsKey(position))
        {
            return TileIDMap[position];
        }
        return -1;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Init();

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void UpdateTime()
    {
        if(Hour > 23)
        {
            Hour = 0;
            Day += 1;
        }
    }

    private void Update()
    {
        UpdateTime();
    }
}
