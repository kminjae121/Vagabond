using System;
using _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower;
using UnityEngine;

public class PlayerDeadZone : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    private void OnCollisionEnter(Collision other)
    {
        if (((1 << other.gameObject.layer) & whatIsPlayer) != 0)
        {
            other.gameObject.GetComponent<BloodFlowerSystem>().GetDamage(99999);
        }
    }
}
