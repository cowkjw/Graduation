using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController:MonoBehaviour
{

    Define.Scene _currentScene;

    void Awake()
    {
        _currentScene = (Define.Scene)SceneManager.GetActiveScene().buildIndex;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 3) // 플레이어가 아니라면
        {
            return;
        }

        switch(_currentScene)
        {
            case Define.Scene.Town:
                SceneManager.LoadScene((int)Define.Scene.Dungeon);
                break;
            case Define.Scene.Dungeon:
                if(this.gameObject.layer==LayerMask.NameToLayer("Town"))
                {
                    SceneManager.LoadScene((int)Define.Scene.Town);
                }
                else 
                {
                  SceneManager.LoadScene((int)Define.Scene.BossDungeon);
                }
                break;
            case Define.Scene.BossDungeon:
                break;
        }
        Managers.Data.PlayerDataChange();
        _currentScene = (Define.Scene)SceneManager.GetActiveScene().buildIndex;
    }
}
