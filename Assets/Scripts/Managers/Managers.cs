using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour // ½Ì±ÛÅæ
{
    static Managers s_Instance;
    static Managers Instance { get { Init(); return s_Instance; } } // ÇÁ·ÎÆÛÆ¼ »ç¿ë

    InputManager _input = new InputManager();
    GameManagerExt _game = new GameManagerExt();
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    UIManager _ui = new UIManager();

    public static GameManagerExt Game { get => Instance._game; }
    public static InputManager Input { get => Instance._input; }
    public static DataManager Data { get => Instance._data; }
    public static PoolManager Pool { get => Instance._pool; }
    public static UIManager UI { get => Instance._ui; }
 
    void Start()
    {
        Init();
    }

    void Update()
    { 
        _input.MouseUpdate(); 
        _input.KeyboardUpdate();
    }

    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Managers>();

            s_Instance._data.Init();
            s_Instance._pool.LoadTheLastPosition();
            s_Instance._ui.Init();
        }
    }

    public static void Clear()
    {
        Input.Clear();
        UI.Clear();
    }
}
