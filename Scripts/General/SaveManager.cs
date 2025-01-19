using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private int level, berries;
    private float volumeLevel;
    public static SaveManager instance;

    private bool[] level1Dialogues = { false, false, false, false, false, false, false, false, };
    private bool[] level2Dialogues = { false, false, false, false, false, false };
    private bool[] level3Dialogues = { false, false, false, false };

    private Vector2 spawnPoint;

    private bool[] berriesLvl1 = { false, false, false, false, false };
    private bool[] berriesLvl2 = { false, false, false, false, false };
    private bool[] berriesLvl3 = { false, false, false, false, false };

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        LoadEveryThing();
    }   
    public void SaveGame()
    {
        PlayerPrefs.SetInt("BerryCount", berries);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetFloat("VolumeLevel", volumeLevel);

        PlayerPrefs.SetInt("SpawnPointX", (int)spawnPoint.x);
        PlayerPrefs.SetInt("SpawnPointY", (int)spawnPoint.y);
    }

    private void LoadEveryThing()
    {
        berries = LoadBerryCount();
        spawnPoint = LoadSpawnPoint();
        volumeLevel = LoadVolumeLevel();

        LoadDiasOnAwake();

        LoadPickedBerries();
    }

    public void DeleteData()
    {
        level = 0;
        berries = 0;

        for(int i = 0; i < berriesLvl1.Length; i++)
        {
            if (berriesLvl1[i])
                berriesLvl1[i] = false;
        }
        for (int i = 0; i < berriesLvl2.Length; i++)
        {
            if (berriesLvl1[i])
                berriesLvl1[i] = false;
        }
        for (int i = 0; i < berriesLvl3.Length; i++)
        {
            if (berriesLvl1[i])
                berriesLvl1[i] = false;
        }
    }

    //berry
    public int GetBerryCount()
    {
        return berries;
    }
    public void SetBerryCount(int berryCount)
    {
        berries = berryCount;
        PlayerPrefs.SetInt("BerryCount", berryCount);
    }
    public int LoadBerryCount()
    {
        return PlayerPrefs.GetInt("BerryCount");
    }
    public bool IsBerryPicked(int level, int berryIndex)
    {
        switch(level)
        {
            case 0:
                return berriesLvl1[berryIndex];

            case 1:
                return berriesLvl2[berryIndex];

            case 2:
                return berriesLvl3[berryIndex];

            default: return false;
        }
    }
    public void SetPickedBerry(int level, int index)
    {
        switch (level)
        {
            case 0:
                berriesLvl1[index] = true;
                break;
            case 1:
                berriesLvl2[index] = true;
                break;
            case 2:
                berriesLvl3[index] = true;
                break;
            default: break;
        }
        SaveBerryLists(level);
    }
    public void LoadPickedBerries()
    {
        for (int i = 0; i < berriesLvl1.Length; i++)
        {
            if (PlayerPrefs.GetInt("BerriesLvl1" + i) == 1)
                berriesLvl1[i] = true;
            else
                berriesLvl1[i] = false;
        }
        for (int i = 0; i < berriesLvl2.Length; i++)
        {
            if (PlayerPrefs.GetInt("BerriesLvl2" + i) == 1)
                berriesLvl2[i] = true;
            else
                berriesLvl2[i] = false;
        }
        for (int i = 0; i < berriesLvl3.Length; i++)
        {
            if (PlayerPrefs.GetInt("BerriesLvl3" + i) == 1)
                berriesLvl3[i] = true;
            else
                berriesLvl3[i] = false;
        }
    }
    public void SaveBerryLists(int level)
    {
        switch(level)
        {
            case 0:
                for(int i = 0; i < berriesLvl1.Length; i++)
                {
                    if (berriesLvl1[i])
                        PlayerPrefs.SetInt("BerriesLvl1" + i, 1);  
                    else
                        PlayerPrefs.SetInt("BerriesLvl1" + i, 0);
                }
                break;
            case 1:
                for (int i = 0; i < berriesLvl2.Length; i++)
                {
                    if (berriesLvl1[i])
                        PlayerPrefs.SetInt("BerriesLvl2" + i, 1);
                    else
                        PlayerPrefs.SetInt("BerriesLvl2" + i, 0);
                }
                break;
            case 2:
                for (int i = 0; i < berriesLvl3.Length; i++)
                {
                    if (berriesLvl1[i])
                        PlayerPrefs.SetInt("BerriesLvl3" + i, 1);
                    else
                        PlayerPrefs.SetInt("BerriesLvl3" + i, 0);
                }
                break;
        }
    }


    //volume
    public float GetVolumeLevel()
    {
        return volumeLevel;
    }
    public void SetVolumeLevel(float volume)
    {
        volumeLevel = volume;
        PlayerPrefs.SetFloat("VolumeLevel", volumeLevel);
    }
    public float LoadVolumeLevel()
    {
        return PlayerPrefs.GetFloat("VolumeLevel");
    }

    //dialogue
    public void SetUsedDialogue(int level, int diaIndex)
    {
        switch(level)
        {
            case 0:
                level1Dialogues[diaIndex] = true;
                PlayerPrefs.SetInt("level1Dialogue" + diaIndex, 1);
                break;

            case 1:
                level2Dialogues[diaIndex] = true;
                PlayerPrefs.SetInt("level2Dialogue" + diaIndex, 1);
                break;

            case 2:
                level3Dialogues[diaIndex] = true;
                PlayerPrefs.SetInt("level3Dialogue" + diaIndex, 1);
                break;

            default: break;
        }
    }
    public bool LoadUsedDialogue(int level, int diaIndex)
    {
        switch (level)
        {
            case 0:
                if (PlayerPrefs.GetInt("level1Dialogue" + diaIndex) == 1)
                    return true;
                else
                    return false;

            case 1:
                if (PlayerPrefs.GetInt("level2Dialogue" + diaIndex) == 1)
                    return true;
                else
                    return false;
            case 2:
                if (PlayerPrefs.GetInt("level3Dialogue" + diaIndex) == 1)
                    return true;
                else
                    return false;
        }
        return true;
    }
    private void LoadDiasOnAwake()
    {
        for (int i = 0; i < level1Dialogues.Length; i++)
        {
            level1Dialogues[i] = LoadUsedDialogue(0, i);
        }
        for (int i = 0; i < level2Dialogues.Length; i++)
        {
            level2Dialogues[i] = LoadUsedDialogue(1, i);
        }
        for (int i = 0; i < level3Dialogues.Length; i++)
        {
            level3Dialogues[i] = LoadUsedDialogue(2, i);
        }
    }

    //spawnpoints
    public void SetSpawnPoint(int level, int index)
    {
        spawnPoint = new Vector2(level, index);
        PlayerPrefs.SetInt("SpawnPointX", level);
        PlayerPrefs.SetInt("SpawnPointY", index);
    }
    public Vector2 LoadSpawnPoint()
    {
        return new Vector2(PlayerPrefs.GetInt("SpawnPointX"), PlayerPrefs.GetInt("SpawnPointY"));
    }
}
