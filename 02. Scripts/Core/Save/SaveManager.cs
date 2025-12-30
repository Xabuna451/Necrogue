using UnityEngine;
using System.IO;
using System.Collections.Generic;
[System.Serializable]
public class GameSaveData
{
    public int deathCount = 0;

    // 여기부터 추가!
    public int persistentGold = 0;
    public System.Collections.Generic.List<string> unlockedCharacters = new System.Collections.Generic.List<string>();

    // TODO: 나중에 스킨, 최고 점수 등 추가
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public GameSaveData Data { get; private set; }

    [TextArea(1, 10)]
    public string FileData = "C:\\Users\\M\\AppData\\LocalLow\\DefaultCompany\\OverTail"; // 디버그용 예시 경로

    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            try
            {
                Data = JsonUtility.FromJson<GameSaveData>(json);
                if (Data == null) Data = new GameSaveData();
            }
            catch
            {
                Debug.LogWarning("[SaveManager] 세이브 파일 파싱 실패. 새로 생성.");
                Data = new GameSaveData();
            }
        }
        else
        {
            Data = new GameSaveData();
        }

        // 기본 캐릭터 하나는 처음부터 해금되게 하고 싶으면 여기서 추가
        if (Data.unlockedCharacters.Count == 0)
        {
            Data.unlockedCharacters.Add("DefaultSnake"); // 예시: 기본 뱀은 무조건 해금
        }

        Debug.Log($"[SaveManager] 로드 완료. deathCount = {Data.deathCount}, persistentGold = {Data.persistentGold}");
    }

    public void Save()
    {
        if (Data == null) Data = new GameSaveData();

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] 저장 완료. path = {SavePath}");
    }

    // ==== death 관련 편의 함수 ====
    public void AddDeath()
    {
        if (Data == null) Data = new GameSaveData();

        Data.deathCount++;
        Debug.Log($"[SaveManager] Death +1 → {Data.deathCount}");
        Save();
    }

    public int GetDeathCount()
    {
        return Data != null ? Data.deathCount : 0;
    }

    // ==== 영구 골드 관련 편의 함수 ====
    public void AddPersistentGold(int amount)
    {
        if (Data == null) Data = new GameSaveData();

        Data.persistentGold += amount;
        Debug.Log($"[SaveManager] PersistentGold +{amount} → {Data.persistentGold}");
        Save();
    }

    public bool SpendPersistentGold(int amount)
    {
        if (Data == null) Data = new GameSaveData();

        if (Data.persistentGold >= amount)
        {
            Data.persistentGold -= amount;
            Debug.Log($"[SaveManager] PersistentGold -{amount} → {Data.persistentGold}");
            Save();
            return true;
        }

        Debug.Log("[SaveManager] 골드 부족!");
        return false;
    }

    public int GetPersistentGold()
    {
        return Data != null ? Data.persistentGold : 0;
    }

    // ==== 캐릭터 해금 관련 편의 함수 ====
    public bool UnlockCharacter(string characterId)
    {
        if (Data == null) Data = new GameSaveData();

        if (Data.unlockedCharacters.Contains(characterId))
        {
            Debug.Log($"[SaveManager] 이미 해금된 캐릭터: {characterId}");
            return false;
        }

        Data.unlockedCharacters.Add(characterId);
        Save();
        Debug.Log($"[SaveManager] 캐릭터 해금 완료: {characterId}");
        return true;
    }

    public bool IsCharacterUnlocked(string characterId)
    {
        return Data != null && Data.unlockedCharacters.Contains(characterId);
    }

    public List<string> GetAllUnlockedCharacters()
    {
        return Data != null ? Data.unlockedCharacters : new List<string>();
    }

    // 디버그용: 에디터에서 테스트할 때 골드 초기화하고 싶으면 우클릭 → Reset All Data
    [ContextMenu("Reset All Data")]
    public void ResetAllData()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveManager] 모든 세이브 데이터 삭제 및 초기화");
        }
        Load(); // 새로 생성
    }
}
