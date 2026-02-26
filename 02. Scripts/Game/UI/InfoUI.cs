using System.Collections;
using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] TMP_Text goldTmp;
    [SerializeField] TMP_Text deathCountTmp;

    SaveManager saveManager;

    void OnEnable()
    {
        StartCoroutine(BindWhenReady());
    }

    IEnumerator BindWhenReady()
    {
        if (saveManager != null) yield break;

        while (SaveManager.Instance == null)
            yield return null;

        saveManager = SaveManager.Instance;

        saveManager.OnChanged += OnSaveChanged;

        OnSaveChanged(saveManager.Data);
    }

    void OnDisable()
    {
        if (saveManager != null)
            saveManager.OnChanged -= OnSaveChanged;

        saveManager = null;
    }

    void OnSaveChanged(GameSaveData data)
    {
        if (data == null) return;
        goldTmp.text = $"Gold: {data.metaGold}";
        deathCountTmp.text = $"Death: {data.deathCount}";
    }
}