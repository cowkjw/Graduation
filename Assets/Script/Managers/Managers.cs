using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_Instance; // ½Ì±ÛÅæ
    static Managers Instance { get { Init(); return s_Instance; } } // ÇÁ·ÎÆÛÆ¼ »ç¿ë

    InputManager _input = new InputManager();
    GameManagerExt _game = new GameManagerExt();
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    MySceneManager _scene = new MySceneManager();

    public static GameManagerExt Game { get => Instance._game; }
    public static InputManager Input { get => Instance._input; }
    public static DataManager Data { get => Instance._data; }
    public static PoolManager Pool { get => Instance._pool; }
    public static MySceneManager Scene { get => Instance._scene; }


    void Start()
    {
        Init();
    }


    void Update()
    { 
        _input.MouseUpdate(); 
        _input.KeyboardUpdate();
        //Clear();
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
        }

    }


    public static void Clear()
    {
        Input.Clear(); 
    }
}
