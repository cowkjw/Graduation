using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    int maximumEnemy = 9;

    float spawnDelay = 8.0f;


    Stack<GameObject> enemyStack = new Stack<GameObject>();


    public void Start()
    {
        InitSpawnEnemies();
        StartCoroutine(SpawnEnemiesCoroutine(spawnDelay));
    }


    IEnumerator SpawnEnemiesCoroutine(float delay)
    {

        while (true)
        {
            while (Managers.Pool.monsterPool.Count != 0) // 소환 후 리스폰을 위함으로 큐가 빌때까지
            {
                GameObject enemy = Managers.Pool.monsterPool.Peek();
                enemyStack.Push(enemy); // 순서대로 스택에 넣어둠
                Managers.Pool.monsterPool.Dequeue();
            }

            yield return new WaitForSeconds(delay);

            while(enemyStack.Count!=0) // 순서대로 넣었기 때문에 죽은 순서로 들어감 
            {
                enemyStack.Pop().SetActive(true);
            }
        }

    }

    void InitSpawnEnemies() // 설정한 수만큼 몬스터 소환
    {
        for (int i = 0; i < maximumEnemy; i++)
        {
            GameObject enemy = Managers.Pool.monsterPool.Peek();
            enemy.SetActive(true);
            Managers.Pool.monsterPool.Dequeue();
        }

    }
}
