using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Header("View")] [SerializeField] private GameView _gameView;

    [Header("TileMap")] public Tilemap mapTilemap;
    public Tilemap fillTilemap;

    public TileBase redFIleTile;
    public TileBase blueFillTile;
    public TileBase grayFillTile;

    [Header("")] public GameObject redUnitGameObject;
    public GameObject blueUnitGameObject;

    public int turn;
    public Dictionary<Vector2Int, int> power;
    public Dictionary<Vector2Int, Team> teams;

    public Unit unit;

    public Unit[] units;
    private List<AttackEvent> _units = new();
    public int redUnitIndex;
    public int blueUnitIndex;


    private void Awake()
    {
        power = new Dictionary<Vector2Int, int>();
        teams = new Dictionary<Vector2Int, Team>();
        teams.Add(new Vector2Int(7, -7), Team.Blue);
        AttackTile(new Vector2Int(7, -7), Team.Blue);

        teams.Add(new Vector2Int(-8, 8), Team.Red);
        AttackTile(new Vector2Int(-8, 8), Team.Red);
    }

    private readonly List<Vector2Int> attackPosts = new List<Vector2Int>()
    {
        Vector2Int.down,
        Vector2Int.up,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down + Vector2Int.right,
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up + Vector2Int.right,
    };

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var cellPos = mapTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            DONewMethod(cellPos);
        }
    }

    private void DONewMethod(Vector3Int cellPos)
    {
        var teamTurn = turn % 2 == 0 ? Team.Red : Team.Blue;
        unit = units[teamTurn == Team.Red ? redUnitIndex : blueUnitIndex];

        var count = attackPosts
            .Select(p => p + (Vector2Int)cellPos)
            .Count(p => fillTilemap.GetTile((Vector3Int)p) && ((teamTurn == Team.Red && power[p] > 0) ||
                                                               (teamTurn == Team.Blue && power[p] < 0)));

        if (mapTilemap.GetTile(cellPos) == null || teams.ContainsKey((Vector2Int)cellPos) || count == 0 ||
            fillTilemap.GetTile(cellPos) == grayFillTile)
        {
            return;
        }

        GameUpdate(teamTurn, new Vector2Int(cellPos.x, cellPos.y));
        turn += 1;
        _gameView.SetTurnText(turn);
    }


    private bool IsGameMapRange(Vector2Int pos) => mapTilemap.GetTile((Vector3Int)pos) != null;

    private void GameUpdate(Team teamTurn, Vector2Int pos)
    {
        foreach (var attackEvent in _units)
        {
            attackEvent.Turn -= 1;
            if (attackEvent.Turn > 0) continue;

            foreach (var attackPos in attackEvent.AttackPos.Select(i => attackEvent.Pos + i))
            {
                AttackTile(attackPos, attackEvent.Team);
            }
        }

        _units = _units.Where(p => p.Turn > 0).ToList();
        Instantiate(teamTurn == Team.Red ? redUnitGameObject : blueUnitGameObject,
            mapTilemap.GetCellCenterWorld((Vector3Int)pos),
            Quaternion.identity
        ).transform.DOScaleY(1, 0.65f).From(0).SetEase(Ease.OutQuint);
        _units.Add(new AttackEvent { AttackPos = unit.AttackPos, Pos = pos, Turn = unit.Turn, Team = teamTurn });
        teams.Add(pos, teamTurn);

        Debug.Log($"레드 : ${power.Count(p => p.Value > 0)} 블루 : ${power.Count(p => p.Value < 0)}");
    }

    private void AttackTile(Vector2Int attackPos, Team team)
    {
        if (!IsGameMapRange(attackPos))
        {
            return;
        }

        if (!power.ContainsKey(attackPos))
        {
            power.Add(attackPos, 0);
        }

        power[attackPos] += team == Team.Red ? 1 : -1;

        if (teams.ContainsKey(attackPos))
        {
            fillTilemap.SetTile((Vector3Int)attackPos, teams[attackPos] == Team.Red ? redFIleTile : blueFillTile);
            return;
        }

        if (power[attackPos] != 0)
        {
            fillTilemap.SetTile((Vector3Int)attackPos, power[attackPos] > 0 ? redFIleTile : blueFillTile);
        }
        else
        {
            fillTilemap.SetTile((Vector3Int)attackPos, grayFillTile);
        }
    }
}

public class AttackEvent
{
    public Vector2Int Pos;
    public Vector2Int[] AttackPos;
    public int Turn;
    public Team Team;
}

public enum Team
{
    Red,
    Blue
}