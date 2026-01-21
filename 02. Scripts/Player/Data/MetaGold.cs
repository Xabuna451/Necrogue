using UnityEngine;

public class MetaGold : MonoBehaviour
{
    public static MetaGold Instance;
    private int totalGold = 0;

    void Awake() { Instance = this; LoadGold(); }
    public void AddGold(int amount) { totalGold += amount; SaveGold(); }
    //public bool SpendGold(int amount) { /* 소비 로직 */ }
    private void SaveGold() { PlayerPrefs.SetInt("PersistentGold", totalGold); }
    private void LoadGold() { totalGold = PlayerPrefs.GetInt("PersistentGold", 0); }
}
