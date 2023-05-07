using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField]
    Define.Coin CoinType;

    int _coinValue=0;

    AudioSource coinPickUp;

    public int CoinValue { get { return _coinValue; } }
    private void Start()
    {
        coinPickUp = GetComponent<AudioSource>();
        if (CoinType == Define.Coin.Bronze)
        {
            _coinValue = 100;
        }
        else if (CoinType == Define.Coin.Sliver)
        {
            _coinValue = 200;
        }
        else if (CoinType == Define.Coin.Gold)
        {
            _coinValue = 300;
        }

        transform.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 200f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Managers.Data.Gold += CoinValue;
            coinPickUp.Play();
            //Managers.Data.PlayerDataChange(); // �̷��� ���� �־ �ǳ�??
            Destroy(gameObject,0.5f);
        }
    }

}
