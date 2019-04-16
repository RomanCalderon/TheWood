using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    /// <summary>
    /// Loads the data of specified type T from path
    /// </summary>
    /// <typeparam name="T">Save data</typeparam>
    /// <param name="path">Path of serialized saved data</param>
    /// <returns></returns>
    public static T LoadData<T>(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            T data = (T)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return default;
        }
    }

    // Save Player
    public static void SavePlayer(Player player, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerSaveData data = new PlayerSaveData(player);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved Player data.");
    }

    // Save DayNightCycle
    public static void SaveDayNightCycle(DayNightCycle dayNightCycle, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        DayNightCycleSaveData data = new DayNightCycleSaveData(dayNightCycle);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved DayNightCycle data.");
    }

    // Save InventoryManager
    public static void SaveInventoryManager(InventoryManager inventoryManager, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        InventoryManagerSaveData data = new InventoryManagerSaveData(inventoryManager);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved InventoryManager data.");
    }

    // Save BuildingManager
    public static void SaveBuildingManager(BuildingManager buildingManager, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        BuildingManagerSaveData data = new BuildingManagerSaveData(buildingManager);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved BuildingManager data.");
    }

    // Save ItemStorage
    public static void SaveItemStorage(ItemStorage itemStorage, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        ItemStorageSaveData data = new ItemStorageSaveData(itemStorage);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("Saved ItemStorage data.");
    }

}
