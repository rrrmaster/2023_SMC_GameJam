using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class Unit : ScriptableObject
{
    public int Turn;
    public Vector2Int[] AttackPos;

    public List<Vector2Int> Attack(Vector2Int pos)
    {
        return AttackPos.Select(i => pos + i).ToList();
    }
}