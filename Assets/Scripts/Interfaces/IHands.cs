using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHands : IWeapon
{
    Item heldItem { get; set; }
}
