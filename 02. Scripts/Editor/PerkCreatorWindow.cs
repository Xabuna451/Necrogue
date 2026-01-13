using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 프로젝트 네임스페이스에 맞게 수정 필요할 수 있음
using Necrogue.Perk.Data;        // PerkDef, PerkCategory, PerkRarity
using Necrogue.Perk.Data.Perk;   // PerkEffect
using Necrogue.Core.Domain.Necro; // NecroParam

public class PerkCreatorWindow : EditorWindow
{
    // ─────────────────────────────────────────────
    // Paths (너가 추천한 구조 기반)
    // ─────────────────────────────────────────────
    private const string DefaultPerkDefFolder = "Assets/03. ScriptableObjects/Perk/Defs";
    private const string DefaultEffectNecroFolder = "Assets/03. ScriptableObjects/Perk/Effects/Necro";

    // ─────────────────────────────────────────────
    // PerkDef fields
    // ─────────────────────────────────────────────
    private string perkId = "necro_ally_damage_01";
    private string displayName = "언데드 강화";
    private string description = "언데드 공격력이 증가한다.";
    private Sprite icon;

    private PerkCategory category = PerkCategory.Necro;
    private PerkRarity rarity = PerkRarity.Common;
    private float weight = 1f;
    private int maxStack = 10;

    private string perkDefFolder = DefaultPerkDefFolder;

    // ─────────────────────────────────────────────
    // Effect creation (NecroStat presets)
    // ─────────────────────────────────────────────
    [System.Serializable]
    private class NecroStatPresetRow
    {
        public bool create = true;

        public NecroParam param = NecroParam.AllyDamage;

        public float addPerStack = 0f;
        public float mulPerStack = 1f;

        // 파일명 태그용
        public string nameTag = "Add1";
    }

    private string effectNecroFolder = DefaultEffectNecroFolder;
    private List<NecroStatPresetRow> presets = new List<NecroStatPresetRow>();

    // “이미 있는 Effect 에셋”도 추가로 꽂고 싶을 때
    private List<PerkEffect> extraEffects = new List<PerkEffect>();

    // PerkEffect_NecroStat 타입 문자열 (타입명 바뀌면 여기만 수정)
    private const string NecroStatEffectTypeName = "PerkEffect_NecroStat";

    [MenuItem("Necrogue/Perk Creator")]
    public static void Open()
    {
        var w = GetWindow<PerkCreatorWindow>("Perk Creator");
        w.minSize = new Vector2(560, 520);
        w.InitIfEmpty();
    }

    private void InitIfEmpty()
    {
        if (presets.Count == 0)
        {
            presets.Add(new NecroStatPresetRow
            {
                create = true,
                param = NecroParam.AllyDamage,
                addPerStack = 1f,
                mulPerStack = 1f,
                nameTag = "Add1"
            });
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);

        DrawHeader("PerkDef 생성");
        perkDefFolder = DrawFolderField("PerkDef 폴더", perkDefFolder);

        perkId = EditorGUILayout.TextField("Perk ID (고유)", perkId);
        displayName = EditorGUILayout.TextField("Display Name", displayName);
        EditorGUILayout.LabelField("Description");
        description = EditorGUILayout.TextArea(description, GUILayout.Height(54));
        icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);

        EditorGUILayout.Space(6);

        category = (PerkCategory)EditorGUILayout.EnumPopup("Category", category);
        rarity = (PerkRarity)EditorGUILayout.EnumPopup("Rarity", rarity);
        weight = EditorGUILayout.FloatField("Weight", weight);
        maxStack = EditorGUILayout.IntField("Max Stack", maxStack);

        EditorGUILayout.Space(12);

        DrawHeader("NecroStat Effect 프리셋 생성(선택)");
        effectNecroFolder = DrawFolderField("Effect(Necro) 폴더", effectNecroFolder);

        DrawPresetsUI();

        EditorGUILayout.Space(8);
        DrawHeader("추가로 꽂을 Effect (이미 존재하는 에셋)");
        DrawExtraEffectsUI();

        EditorGUILayout.Space(14);

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(perkId)))
        {
            if (GUILayout.Button("Create PerkDef + Effects", GUILayout.Height(40)))
            {
                CreateAll();
            }
        }

        EditorGUILayout.Space(6);
        DrawValidationHints();
    }

    private void DrawHeader(string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        var r = GUILayoutUtility.GetRect(1, 1);
        EditorGUI.DrawRect(new Rect(r.x, r.y, position.width, 1), new Color(1, 1, 1, 0.08f));
    }

    private string DrawFolderField(string label, string path)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(130));
        EditorGUILayout.SelectableLabel(path, EditorStyles.textField, GUILayout.Height(18));
        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            var selected = EditorUtility.OpenFolderPanel(label, "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (!selected.Contains("/Assets"))
                {
                    EditorUtility.DisplayDialog("오류", "프로젝트 Assets 폴더 안을 선택해야 함", "OK");
                }
                else
                {
                    var idx = selected.IndexOf("/Assets");
                    path = selected.Substring(idx + 1); // "Assets/..."
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        return path;
    }

    private void DrawPresetsUI()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            for (int i = 0; i < presets.Count; i++)
            {
                var p = presets[i];

                EditorGUILayout.BeginVertical("helpbox");
                EditorGUILayout.BeginHorizontal();

                p.create = EditorGUILayout.Toggle(p.create, GUILayout.Width(18));
                EditorGUILayout.LabelField($"Preset #{i}", EditorStyles.boldLabel);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    presets.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();

                p.param = (NecroParam)EditorGUILayout.EnumPopup("Param", p.param);
                p.addPerStack = EditorGUILayout.FloatField("Add / stack", p.addPerStack);
                p.mulPerStack = EditorGUILayout.FloatField("Mul / stack", p.mulPerStack);
                p.nameTag = EditorGUILayout.TextField("NameTag", p.nameTag);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Preset"))
            {
                presets.Add(new NecroStatPresetRow
                {
                    create = true,
                    param = NecroParam.AllyDamage,
                    addPerStack = 0f,
                    mulPerStack = 1f,
                    nameTag = "New"
                });
            }

            if (GUILayout.Button("Add HP↑ + Dmg↓ 템플릿"))
            {
                presets.Add(new NecroStatPresetRow
                {
                    create = true,
                    param = NecroParam.AllyHp,
                    addPerStack = 5f,
                    mulPerStack = 1f,
                    nameTag = "HpAdd5"
                });
                presets.Add(new NecroStatPresetRow
                {
                    create = true,
                    param = NecroParam.AllyDamage,
                    addPerStack = 0f,
                    mulPerStack = 0.97f,
                    nameTag = "DmgMul0p97"
                });
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawExtraEffectsUI()
    {
        using (new EditorGUILayout.VerticalScope("box"))
        {
            int removeIdx = -1;
            for (int i = 0; i < extraEffects.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                extraEffects[i] = (PerkEffect)EditorGUILayout.ObjectField(extraEffects[i], typeof(PerkEffect), false);
                if (GUILayout.Button("X", GUILayout.Width(24))) removeIdx = i;
                EditorGUILayout.EndHorizontal();
            }
            if (removeIdx >= 0) extraEffects.RemoveAt(removeIdx);

            if (GUILayout.Button("+ Add Existing Effect"))
                extraEffects.Add(null);
        }
    }

    private void DrawValidationHints()
    {
        // perkId 중복 체크
        bool idDup = IsPerkIdDuplicate(perkId);

        if (idDup)
            EditorGUILayout.HelpBox("perkId가 이미 존재함. 중복이면 저장/세이브/밸런스 관리에서 터짐.", MessageType.Error);
        else
            EditorGUILayout.HelpBox("perkId 중복 없음.", MessageType.Info);

        if (perkId.Any(char.IsWhiteSpace))
            EditorGUILayout.HelpBox("perkId에 공백 넣지 마라. (추천: snake_case)", MessageType.Warning);

        if (maxStack < 1)
            EditorGUILayout.HelpBox("MaxStack은 1 이상.", MessageType.Warning);
    }

    private bool IsPerkIdDuplicate(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        var guids = AssetDatabase.FindAssets("t:PerkDef");
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var def = AssetDatabase.LoadAssetAtPath<PerkDef>(path);
            if (def != null && def.perkId == id) return true;
        }
        return false;
    }

    private void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        // "Assets/a/b/c"를 단계적으로 생성
        var parts = folderPath.Split('/');
        string cur = parts[0]; // "Assets"
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{cur}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }

    private void CreateAll()
    {
        // ─────────────────────────────────────────────
        // Validation
        // ─────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(perkId))
        {
            EditorUtility.DisplayDialog("오류", "perkId 비었음", "OK");
            return;
        }
        if (IsPerkIdDuplicate(perkId))
        {
            EditorUtility.DisplayDialog("오류", "perkId 중복. 다른 값으로 바꿔라.", "OK");
            return;
        }

        EnsureFolder(perkDefFolder);
        EnsureFolder(effectNecroFolder);

        // ─────────────────────────────────────────────
        // Create PerkDef asset
        // ─────────────────────────────────────────────
        var def = ScriptableObject.CreateInstance<PerkDef>();
        def.perkId = perkId;
        def.displayName = displayName;
        def.description = description;
        def.icon = icon;
        def.category = category;
        def.rarity = rarity;
        def.weight = Mathf.Max(0f, weight);
        def.maxStack = Mathf.Max(1, maxStack);

        string defAssetPath = $"{perkDefFolder}/Perk_{perkId}.asset";
        defAssetPath = AssetDatabase.GenerateUniqueAssetPath(defAssetPath);
        AssetDatabase.CreateAsset(def, defAssetPath);

        // ─────────────────────────────────────────────
        // Create Effect assets (optional)
        // ─────────────────────────────────────────────
        var createdEffects = new List<PerkEffect>();

        // NecroStat Effect 타입 찾기(리플렉션)
        var effectType = GetAllTypes().FirstOrDefault(t => t.Name == NecroStatEffectTypeName);
        if (effectType == null)
        {
            // NecroStatEffect 없이도 PerkDef는 만들 수 있게 해둠
            Debug.LogWarning($"[PerkCreator] {NecroStatEffectTypeName} 타입을 못 찾음. Effect 생성은 스킵됨.");
        }
        else
        {
            foreach (var p in presets)
            {
                if (!p.create) continue;

                var eff = ScriptableObject.CreateInstance(effectType) as PerkEffect;
                if (eff == null) continue;

                // SerializeField 값 세팅: SerializedObject로 안전하게
                var so = new SerializedObject(eff);

                // 필드명은 PerkEffect_NecroStat 코드와 동일해야 함
                so.FindProperty("param").enumValueIndex = (int)p.param;
                so.FindProperty("addPerStack").floatValue = p.addPerStack;
                so.FindProperty("mulPerStack").floatValue = p.mulPerStack;
                so.ApplyModifiedPropertiesWithoutUndo();

                string tag = string.IsNullOrWhiteSpace(p.nameTag) ? "Preset" : p.nameTag.Trim();
                string effPath = $"{effectNecroFolder}/E_Necro_{p.param}_{tag}.asset";
                effPath = AssetDatabase.GenerateUniqueAssetPath(effPath);

                AssetDatabase.CreateAsset(eff, effPath);
                createdEffects.Add(eff);
            }
        }

        // ─────────────────────────────────────────────
        // Attach effects into PerkDef.effects[]
        // ─────────────────────────────────────────────
        var finalEffects = new List<PerkEffect>();
        finalEffects.AddRange(createdEffects);
        finalEffects.AddRange(extraEffects.Where(e => e != null));

        def.effects = finalEffects.ToArray();
        EditorUtility.SetDirty(def);

        // Save
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Ping
        Selection.activeObject = def;
        EditorGUIUtility.PingObject(def);

        Debug.Log($"[PerkCreator] Created: {defAssetPath} (effects: {def.effects?.Length ?? 0})");
    }

    private static IEnumerable<System.Type> GetAllTypes()
    {
        // 에디터에서만: 모든 어셈블리 타입 스캔
        var asms = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in asms)
        {
            System.Type[] types = null;
            try { types = a.GetTypes(); }
            catch { continue; }

            foreach (var t in types)
                yield return t;
        }
    }
}
