using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeController : MonoBehaviour
{

    [SerializeField]
    Dictionary<int,GameObject> _weapons = new();

    int currentWeaponID;
    private void Start()
    {
        for(int i = 0;i<4;i++)
        {
            _weapons.Add(int.Parse(transform.GetChild(i).name), transform.GetChild(i).gameObject);
        }
        currentWeaponID =Managers.Data.PlayerData.equippedWeapon; // �ϵ��ڵ� ��ġ�� ������ �����ǰ� �ν��Ͻ�ȭ��
        _weapons[currentWeaponID].SetActive(true);
    }

    public void ChangeWeapon(int weaponID)
    {
#if UNITY_EDITOR
        Debug.Log("���� ����");
#endif
        if (_weapons.TryGetValue(weaponID, out GameObject newWeapon))
        {
            _weapons[currentWeaponID].SetActive(false);
            newWeapon.SetActive(true);
            currentWeaponID = weaponID;
            Managers.Data.PlayerData.equippedWeapon = currentWeaponID; // ������ ���� ����
            Managers.Data.PlayerDataChange();
            
            Managers.Game.GetPlayer().GetComponent<PlayerStat>().Attack +=Managers.Data.ItemDict[currentWeaponID].Attack; // �ٲ� ���� ���� �߰� ���ݷ� ���ϱ�
        }
        else
        {
            return;
        }
    }
}
