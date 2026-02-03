using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>Dictionary를 JSON으로 직렬화하기 위한 래퍼 클래스</summary>
[Serializable]
public class IntIntDictionaryEntry
{
    public int key;
    public int value;

    public IntIntDictionaryEntry() { }
    public IntIntDictionaryEntry(int k, int v) { key = k; value = v; }
}

[Serializable]
public class GameSaveData
{
    public int deathCount = 0;
    public int metaGold = 0;
    public int perkBonus = 0;
    
    // JSON 직렬화용
    public IntIntDictionaryEntry[] haveItemArray = new IntIntDictionaryEntry[0];

    // 런타임용 (직렬화 안 됨)
    [System.NonSerialized]
    public Dictionary<int, int> haveItem = new();

    /// <summary>JSON 저장 전에 Dictionary → Array로 변환</summary>
    public void PrepareSerialization()
    {
        if (haveItem != null)
            haveItemArray = haveItem.Select(kvp => new IntIntDictionaryEntry(kvp.Key, kvp.Value)).ToArray();
    }

    /// <summary>JSON 로드 후 Array → Dictionary로 변환</summary>
    public void PostDeserialization()
    {
        haveItem = new Dictionary<int, int>();
        if (haveItemArray != null)
        {
            foreach (var entry in haveItemArray)
                haveItem[entry.key] = entry.value;
        }
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    /// <summary>런타임에서 사용하는 실제 데이터</summary>
    public GameSaveData Data { get; private set; }

    /// <summary>UI 갱신용</summary>
    public event Action<GameSaveData> OnChanged;

    // 파일 경로
    string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    // =========================
    // Inspector Read-Only Preview
    // =========================
    [Header("Read Only (Preview)")]
    [SerializeField] private GameSaveData preview;     // 인스펙터에서 보는 값(복사본)
    [SerializeField] private bool saveFileExists;       // 파일 존재 여부

    void Awake()
    {
        var root = transform.root.gameObject;

        if (Instance != null && Instance != this)
        {
            Destroy(root);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(root);

        Load();
    }

    // =========================
    // Core Load/Save
    // =========================
    public void Load()
    {
        Data = ReadFromDiskOrNew();
        SyncPreviewFrom(Data);
        NotifyChanged();
    }

    public void Save()
    {
        if (Data == null) Data = new GameSaveData();

        try
        {
            // JSON 저장 전 Dictionary를 Array로 변환
            Data.PrepareSerialization();

            var dir = Path.GetDirectoryName(SavePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패. path={SavePath}\n{e}");
        }

        // 저장 후에도 preview 갱신
        SyncPreviewFrom(Data);
    }

    void Commit()
    {
        Save();
        NotifyChanged();
    }

    void NotifyChanged()
    {
        OnChanged?.Invoke(Data);
    }

    GameSaveData ReadFromDiskOrNew()
    {
        saveFileExists = File.Exists(SavePath);

        if (!saveFileExists)
            return new GameSaveData();

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();
            
            // JSON 로드 후 Array를 Dictionary로 변환
            data.PostDeserialization();
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveManager] 로드 실패 → 새 데이터 생성\n{e}");
            return new GameSaveData();
        }
    }

    void SyncPreviewFrom(GameSaveData src)
    {
        if (src == null)
        {
            preview = null;
            return;
        }

        // “읽기 전용” 보장을 위해 복사본으로 유지
        preview ??= new GameSaveData();
        preview.deathCount = src.deathCount;
        preview.metaGold = src.metaGold;
        preview.perkBonus = src.perkBonus;
        
        // Array도 복사
        preview.haveItemArray = src.haveItemArray != null 
            ? (IntIntDictionaryEntry[])src.haveItemArray.Clone() 
            : new IntIntDictionaryEntry[0];
    }

    // =========================
    // Death
    // =========================
    public int DeathCount => Data?.deathCount ?? 0;

    public void AddDeath(int amount = 1)
    {
        if (amount <= 0) return;
        Data ??= new GameSaveData();

        Data.deathCount = Mathf.Max(0, Data.deathCount + amount);
        Commit();
    }

    public void SetDeathCount(int value)
    {
        Data ??= new GameSaveData();

        Data.deathCount = Mathf.Max(0, value);
        Commit();
    }

    public void ResetDeathCount() => SetDeathCount(0);

    // =========================
    // Gold
    // =========================
    public int MetaGold => Data?.metaGold ?? 0;

    public bool CanSpendGold(int amount) => amount >= 0 && MetaGold >= amount;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        Data ??= new GameSaveData();

        Data.metaGold = Mathf.Max(0, Data.metaGold + amount);
        Commit();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        Data ??= new GameSaveData();

        if (!CanSpendGold(amount))
            return false;

        Data.metaGold = Mathf.Max(0, Data.metaGold - amount);
        Commit();
        return true;
    }

    public void SetGold(int value)
    {
        Data ??= new GameSaveData();

        Data.metaGold = Mathf.Max(0, value);
        Commit();
    }

    public void ResetGold() => SetGold(0);

    // =========================
    // Have Item
    // =========================

    public Dictionary<int, int> HaveItem => Data?.haveItem ?? new Dictionary<int, int>();

    public void AddHaveItem(int itemId, int amount)
    {
        if (amount <= 0) return;
        Data ??= new GameSaveData();

        if (Data.haveItem.ContainsKey(itemId))
            Data.haveItem[itemId] += amount;
        else
            Data.haveItem[itemId] = amount;

        Commit();
    }

    public int GetHaveItemCount(int itemId)
    {
        if (Data == null || Data.haveItem == null) return 0;

        if (Data.haveItem.TryGetValue(itemId, out int count))
            return count;
        return 0;
    }


    // =========================
    // Perk Bonus
    // =========================
    public int PerkBonus => Data?.perkBonus ?? 0;

    public void AddPerkBonus(int amount)
    {
        if (amount == 0) return;
        Data ??= new GameSaveData();

        Data.perkBonus = Mathf.Max(0, Data.perkBonus + amount);
        Commit();
    }

    public void SetPerkBonus(int value)
    {
        Data ??= new GameSaveData();

        Data.perkBonus = Mathf.Max(0, value);
        Commit();
    }

    public void ResetPerkBonus() => SetPerkBonus(0);

    // =========================
    // Debug
    // =========================
    [ContextMenu("Delete All SaveData")]
    public void DeleteAllData()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);

        Load();
    }

    [ContextMenu("Force Save Now")]
    public void ForceSaveNow()
    {
        Data ??= new GameSaveData();
        Save();
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서도(플레이 전) 현재 save.json을 읽어서 preview 갱신
    /// </summary>
    void OnValidate()
    {
        // 플레이 중엔 런타임 흐름(Load/Commit)으로 갱신되므로 건드리지 않음
        if (Application.isPlaying) return;

        var temp = ReadFromDiskOrNew();
        SyncPreviewFrom(temp);

        // 인스펙터 즉시 반영
        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Open Save Folder")]
    void OpenSaveFolder()
    {
        UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
#endif
}
