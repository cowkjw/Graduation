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
        currentWeaponID =Managers.Data.PlayerData.equippedWeapon; // 하드코딩 고치기 씬마다 고정되게 인스턴스화됨
        weapons[currentWeaponID].SetActive(true);
    }

    public void ChangeWeapon(int weaponID)
    {
        Debug.Log("무기 변경");
       if (weapons.TryGetValue(weaponID, out GameObject newWeapon))
        {
            weapons[currentWeaponID].SetActive(false);
            newWeapon.SetActive(true);
            currentWeaponID = weaponID;
            Managers.Data.PlayerData.equippedWeapon = currentWeaponID; // 아이템 장착 저장
            Managers.Data.PlayerDataChange();
        }
        else
        {
            return;
        }
    }
}
