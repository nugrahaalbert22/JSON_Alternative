using UnityEngine;
using UnityEditor;
using Photon.Pun;
using System.Collections.Generic;
using System.IO;

// ---------------- RUNTIME ----------------
public class AutoGrantByName : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class PlayerUnlockConfig
    {
        public string playerName;
        public List<GameObject> items = new List<GameObject>();
    }

    [System.Serializable]
    public class SaveData
    {
        public List<string> unlockedItemNames = new List<string>();
    }

    private static string saveDir => Path.Combine(Application.persistentDataPath, "PlayerUnlocks");
    public List<PlayerUnlockConfig> playerConfigs = new List<PlayerUnlockConfig>();

    private void Awake()
    {
        if (!Directory.Exists(saveDir))
            Directory.CreateDirectory(saveDir);
    }

    void Start()
    {
        TryGrantItems();
    }

    public override void OnJoinedRoom()
    {
        TryGrantItems();
    }

    private void TryGrantItems()
    {
        string playerName = PhotonNetwork.NickName;
        PlayerUnlockConfig config = playerConfigs.Find(c => c.playerName == playerName);

        if (config == null) return;

        SaveData saveData = LoadPlayerData(playerName);

        foreach (var item in config.items)
        {
            if (item == null) continue;

            if (!saveData.unlockedItemNames.Contains(item.name))
            {
                saveData.unlockedItemNames.Add(item.name);
                SavePlayerData(playerName, saveData);
            }

            if (saveData.unlockedItemNames.Contains(item.name))
                item.SetActive(true);
        }
    }

    private void SavePlayerData(string playerName, SaveData data)
    {
        string path = Path.Combine(saveDir, playerName + ".json");
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    private SaveData LoadPlayerData(string playerName)
    {
        string path = Path.Combine(saveDir, playerName + ".json");
        if (File.Exists(path))
        {
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }
        return new SaveData();
    }
}

// ---------------- EDITOR ----------------
public class AutoGrantByNameWindow : EditorWindow
{
    private AutoGrantByName grantManager;
    private Vector2 scroll;

    [MenuItem("JSON_Alternative/Auto_Grant_By_Name")]
    public static void ShowWindow()
    {
        GetWindow<AutoGrantByNameWindow>("Grant_Items");
    }

    void OnGUI()
    {
        GUILayout.Label("Auto Grant Config", EditorStyles.boldLabel);

        if (grantManager == null)
        {
            grantManager = FindObjectOfType<AutoGrantByName>();
            if (grantManager == null)
            {
                if (GUILayout.Button("Create Grant Manager"))
                {
                    GameObject go = new GameObject("AutoGrantManager");
                    grantManager = go.AddComponent<AutoGrantByName>();
                }
                return;
            }
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        SerializedObject so = new SerializedObject(grantManager);
        SerializedProperty configsProp = so.FindProperty("playerConfigs");

        for (int i = 0; i < configsProp.arraySize; i++)
        {
            SerializedProperty element = configsProp.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = element.FindPropertyRelative("playerName");
            SerializedProperty itemsProp = element.FindPropertyRelative("items");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            nameProp.stringValue = EditorGUILayout.TextField("Player Name", nameProp.stringValue);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                configsProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(itemsProp, new GUIContent("Items"), true);
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Player Config"))
        {
            configsProp.InsertArrayElementAtIndex(configsProp.arraySize);
        }

        so.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Save Config"))
        {
            EditorUtility.SetDirty(grantManager);
            Debug.Log("AutoGrant config saved.");
        }
    }
}
