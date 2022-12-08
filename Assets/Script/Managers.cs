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

    public static GameManagerExt game { get { return Instance._game; } }
    public static InputManager Input { get { return Instance._input; } }
    public static DataManager Data { get { return Instance._data; } }


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

        }
    }


    public static void Clear()
    {
        Input.Clear();
    }
}
