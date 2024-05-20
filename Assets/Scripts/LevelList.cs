using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels")]
public class LevelList : ScriptableObject
{
    public GameObject[] levels;
}