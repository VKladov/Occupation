using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    Transform transform { get; }
    WeaponType Type { get; }
}

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private WeaponType _type;

    public Gun Weapon { get; private set; }
    public bool IsEmpty => Weapon == null;
    public WeaponType Type => _type;

    public void Put(Gun weapon)
    {
        weapon.transform.SetParent(transform, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        Weapon = weapon;
    }

    public Gun Take()
    {
        var weapon = Weapon;
        Weapon.transform.SetParent(null);
        Weapon = null;
        return weapon;
    }
}
