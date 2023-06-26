using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    int _maximumEnemy = 9;
    float _spawnDelay = 5.0f;
   // Stack<GameObject> _enemyStack = new Stack<GameObject>();
    Queue<GameObject> _enemyQueue = new Queue<GameObject>();
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
                //_enemyStack.Push(enemy); // 순서대로 스택에 넣어둠
                _enemyQueue.Enqueue(enemy); // 순서대로 스택에 넣어둠
                Managers.Pool.MonsterPool.Dequeue();
            }

            yield return new WaitForSeconds(delay);

            while(_enemyQueue.Count!=0) // 순서대로 넣었기 때문에 죽은 순서로 들어감 
            {
                //  _enemyStack.Pop().SetActive(true);
                _enemyQueue.Dequeue().SetActive(true);
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
}
