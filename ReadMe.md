# Treasure Hunter


## 게임 개요

* 개발도구 : Unity 3D
* 플랫폼 : PC
* 시점 : 쿼터뷰
* 장르 : 3D RPG

## 게임 소개
도굴꾼이 된 검사가 몬스터를 사냥하며 금화와 경험치를 모아 성장하고 새로운 무기를 얻어 최종 보스 몬스터를 처치하는 것이 목적인 쿼터뷰 시점 3D RPG 게임

## 플레이 영상
 
[![Alt text](https://img.youtube.com/vi/-05XPydsDnY/0.jpg)](https://www.youtube.com/watch?v=-05XPydsDnY)

## 게임 시스템
* 레벨 시스템
* 스텟 시스템
* 스킬 시스템
* 인벤토리 시스템
* 상점 시스템

## 주요 구현 특징
* Singleton
* Object Pooling
* Newtonsoft.Json 라이브러리 활용
* BehaviorTree 구현을 통한 Boss AI



# Singleton
* 싱글톤 패턴을 사용하여 데이터의 중복 생성을 방지로 메모리 낭비를 막고, 데이터 간 공유를 쉽도록 구현
<details>
<summary>Singleton 코드</summary>
<div markdown="1">

```C#
public class Managers : MonoBehaviour // 싱글톤
{
    static Managers s_Instance;
    static Managers Instance { get { Init(); return s_Instance; } } // 프로퍼티 사용

    InputManager _input = new InputManager();
    GameManagerExt _game = new GameManagerExt();
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();

    public static GameManagerExt Game { get => Instance._game; }
    public static InputManager Input { get => Instance._input; }
    public static DataManager Data { get => Instance._data; }
    public static PoolManager Pool { get => Instance._pool; }
 
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
        }
    }
}
```
</div>
</details>    
    
    
# BehavoirTree
* 직접 BehaviorTree의 간단한 노드들을 구현하고 이를 통해 Boss AI에 적용
![mermaid-diagram-2023-05-08-183754](https://github.com/cowkjw/Graduation/assets/83215829/13cbb036-3474-477a-9540-da655a2f122b)
<details>
<summary>BehaviorTree</summary>
<div markdown="1">
    
```C#
public interface INode
{
    bool Execute();
}

public class BehaviorTree
{
    INode rootNode;

    public void SetRootNode(INode rootNode)
    {
        this.rootNode = rootNode;
    }

    public void Update()
    {
        rootNode.Execute();
    }
}

````
</div>
</details>

<details>
<summary>Sequence Node</summary>
<div markdown="1">
    
```C#
public class Sequence : INode
{
    List<INode> children = new List<INode>();

    public void AddChild(INode child)
    {
        children.Add(child);
    }

    public bool Execute()
    {
        foreach (INode child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }
}
````
</div>
</details>
    
<details>
<summary>Selector Node</summary>
<div markdown="1">
    
```C#
public class Selector : INode
{
    List<INode> children = new List<INode>();

    public void AddChild(INode child)
    {
        children.Add(child);
    }

    public bool Execute()
    {
        foreach (INode child in children)
        {
            if (child.Execute())
            {
                return true;
            }
        }
        return false;
    }
}
````
</div>
</details>

<details>
<summary>Condition Node</summary>
<div markdown="1">
    
```C#
public class ConditionNode : INode
{
    Func<bool> condition; // 참인지 확인

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public bool Execute()
    {
        return condition();
    }
}
````
</div>
</details>

<details>
<summary>Action Node</summary>
<div markdown="1">
    
```C#
public class ActionNode : INode
{
     Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public bool Execute()
    {
        action();
        return true;
    }
}
````
</div>
</details>  
 

   

# Object Pooling
* EnemyDict의 데이터를 Queue에 삽입하고 리스폰 시에는 Stack을 활용하여 죽은 상태인 몬스터부터 스폰하도록 구현
* 몬스터의 위치는 json에 x,y,z로 저장하고 좌표 사용시에는 VectorConverter를 통하여 transform으로 변경
<details>
<summary>VectorConverter</summary>
<div markdown="1">
    
```C#
[Serializable]
public class VectorConverter // 몬스터 위치 변환
{
    public float x;
    public float y;
    public float z;

    public VectorConverter(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 ToVecotr3()
    {
        return new Vector3(this.x, this.y, this.z);
    }
}
```
</div>
</details>

<details>
<summary>Pool Manager</summary>
<div markdown="1">    
    
```C#
  public void Init()
    {
        if(GameObject.FindObjectOfType<DungeonScene>() is BossDungeonScene) // 던전이 보스 던전이라면
        {
            return; 
        }
        _monsterPrefab = Resources.Load<GameObject>("Prefabs/Skelton");
        _poolManagers = new GameObject { name = "@PoolManagers" };
        MonsterPool = new Queue<GameObject>();
        foreach (var data in Managers.Data.EnemyDict)
        {
            if (_monsterPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("몬스터 프리팹 NULL");
#endif
                return;
            }
            GameObject monster = GameObject.Instantiate(_monsterPrefab, data.Value.ToVecotr3(), Quaternion.identity);
            monster.name = data.Key;
            monster.SetActive(false);
            MonsterPool.Enqueue(monster);
            monster.transform.SetParent(_poolManagers.transform);
        }

    }
```
</div>
</details>    

<details>
<summary>EnemySpawnController</summary>
<div markdown="1">    
    
```C#
    int _maximumEnemy = 9;
    float _spawnDelay = 5.0f;
    Stack<GameObject> _enemyStack = new Stack<GameObject>();
    public void Start()
    {
        InitSpawnEnemies();
        StartCoroutine(SpawnEnemiesCoroutine(_spawnDelay));
    }

    IEnumerator SpawnEnemiesCoroutine(float delay)
    {
        while (true)
        {
            while (Managers.Pool.MonsterPool.Count != 0) // 소환 후 리스폰을 위함으로 큐가 빌때까지
            {
                GameObject enemy = Managers.Pool.MonsterPool.Peek();
                _enemyStack.Push(enemy); // 순서대로 스택에 넣어둠
                Managers.Pool.MonsterPool.Dequeue();
            }

            yield return new WaitForSeconds(delay);

            while(_enemyStack.Count!=0) // 순서대로 넣었기 때문에 죽은 순서로 들어감 
            {
                _enemyStack.Pop().SetActive(true);
            }
        }

    }

    void InitSpawnEnemies() // 설정한 수만큼 몬스터 소환
    {
        for (int i = 0; i < _maximumEnemy; i++)
        {
            GameObject enemy = Managers.Pool.MonsterPool.Peek();
            enemy.SetActive(true);
            Managers.Pool.MonsterPool.Dequeue();
        }
    }
```
</div>
</details>  

# Newtonsoft.Json
* Newtonsoft.Json 라이브러리를 사용하여 JsonUtility에서 사용할 수 없는 non-array 들을 직렬화 역직렬화 할 수 있다. 

