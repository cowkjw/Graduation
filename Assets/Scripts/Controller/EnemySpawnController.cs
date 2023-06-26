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
            while (Managers.Pool.MonsterPool.Count != 0) // ��ȯ �� �������� �������� ť�� ��������
            {
                GameObject enemy = Managers.Pool.MonsterPool.Peek();
                //_enemyStack.Push(enemy); // ������� ���ÿ� �־��
                _enemyQueue.Enqueue(enemy); // ������� ���ÿ� �־��
                Managers.Pool.MonsterPool.Dequeue();
            }

            yield return new WaitForSeconds(delay);

            while(_enemyQueue.Count!=0) // ������� �־��� ������ ���� ������ �� 
            {
                //  _enemyStack.Pop().SetActive(true);
                _enemyQueue.Dequeue().SetActive(true);
            }
        }

    }

    void InitSpawnEnemies() // ������ ����ŭ ���� ��ȯ
    {
        for (int i = 0; i < _maximumEnemy; i++)
        {
            GameObject enemy = Managers.Pool.MonsterPool.Peek();
            enemy.SetActive(true);
            Managers.Pool.MonsterPool.Dequeue();
        }
    }
}
