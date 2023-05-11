using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int CoinValue { get { return _coinValue; } }
    
    [SerializeField]
    Define.Coin CoinType;
    AudioSource _coinPickUp;
    int _coinValue=0;

    private void Start()
    {
        _coinPickUp = GetComponent<AudioSource>();
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
            _coinPickUp.Play();
            //Managers.Data.PlayerDataChange(); // 이렇게 자주 넣어도 되나??
            Destroy(gameObject,0.5f);
        }
    }

}
