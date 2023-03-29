using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeController : MonoBehaviour
{

    [SerializeField]
    Dictionary<int,GameObject> weapons = new();

    int currentWeaponID;
    private void Start()
    {
        for(int i = 0;i<4;i++)
        {
            weapons.Add(int.Parse(transform.GetChild(i).name), transform.GetChild(i).gameObject);
        }
        currentWeaponID =Managers.Data.PlayerData.equippedWeapon; // �ϵ��ڵ� ��ġ�� ������ �����ǰ� �ν��Ͻ�ȭ��
        weapons[currentWeaponID].SetActive(true);
    }

    public void ChangeWeapon(int weaponID)
    {
        Debug.Log("���� ����");
       if (weapons.TryGetValue(weaponID, out GameObject newWeapon))
        {
            weapons[currentWeaponID].SetActive(false);
            newWeapon.SetActive(true);
            currentWeaponID = weaponID;
            Managers.Data.PlayerData.equippedWeapon = currentWeaponID; // ������ ���� ����
            Managers.Data.PlayerDataChange();
        }
        else
        {
            return;
        }
    }
}
