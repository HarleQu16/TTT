using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Setting")]
public class Setting : ScriptableObject
{
    public bool isSinglePlayer = false;
}
