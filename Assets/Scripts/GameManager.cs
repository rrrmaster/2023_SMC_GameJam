using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Header("View")] [SerializeField] private GameView _gameView;

    [Header("TileMap")] public Tilemap mapTilemap;
    public Tilemap fillTilemap;

    public TileBase redFIleTile;
    public TileBase blueFillTile;
    public TileBase grayFillTile;

    public int[] redUnitCount;
    public int[] blueUnitCount;

    public int[] cursorRedUnitCount;
    public int[] cursorBlueUnitCount;


    [Header("")] public GameObject redUnitGameObject;
    public GameObject blueUnitGameObject;
    public Transform createdPositionTransform;

    public int turn;

    public Unit unit;
    public Unit[] units;


    public int redUnitIndex;
    public int blueUnitIndex;

    private List<AttackEvent> _units = new();
    private Vector2Int? cursorPos = null;
    private Dictionary<Vector2Int, int> power;
    private Dictionary<Vector2Int, Team> teams;

    private readonly List<Vector2Int> attackPosts = new()
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

    public RectInt size;

    private void Awake()
    {
        GetTiles();
        power = new Dictionary<Vector2Int, int>();
        teams = new Dictionary<Vector2Int, Team>();
        var vector2Int = new Vector2Int(size.xMax, size.yMin);
        teams.Add(vector2Int, Team.Blue);
        AttackTile(vector2Int, Team.Blue);
        teams.Add(new Vector2Int(size.xMin, size.yMax), Team.Red);
        AttackTile(new Vector2Int(size.xMin, size.yMax), Team.Red);
        cursorRedUnitCount = redUnitCount.ToArray();
        cursorBlueUnitCount = blueUnitCount.ToArray();

        for (int i = 0; i < redUnitCount.Length; i++)
            _gameView.SetRedTeamUnitCount(i, redUnitCount[i], cursorRedUnitCount[i]);
        for (int i = 0; i < blueUnitCount.Length; i++)
            _gameView.SetBlueTeamUnitCount(i, blueUnitCount[i], cursorBlueUnitCount[i]);

        _gameView.SetTurnChanged();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var cellPos = (Vector2Int)mapTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            cursorPos = cellPos;
            createdPositionTransform.gameObject.SetActive(true);
            createdPositionTransform.position = mapTilemap.CellToWorld((Vector3Int)cursorPos);
            createdPositionTransform.GetComponent<SpriteRenderer>().color =
                DOGetValue(cellPos) ? allowColor : notAllowColor;
            Debug.Log(cursorPos);
        }
    }

    public Color allowColor;
    public Color notAllowColor;

    public void ComplateTurn()
    {
        if (!cursorPos.HasValue)
        {
            return;
        }

        DONewMethod(cursorPos.Value);
    }

    private void DONewMethod(Vector2Int cellPos)
    {
        var teamTurn = turn % 2 == 0 ? Team.Red : Team.Blue;
        unit = units[teamTurn == Team.Red ? redUnitIndex : blueUnitIndex];

        if (!DOGetValue(cellPos))
            return;

        GameUpdate(teamTurn, new Vector2Int(cellPos.x, cellPos.y));
        turn += 1;

        createdPositionTransform.gameObject.SetActive(false);

        for (int i = 0; i < redUnitCount.Length; i++)
            _gameView.SetRedTeamUnitCount(i, redUnitCount[i], cursorRedUnitCount[i]);
        for (int i = 0; i < blueUnitCount.Length; i++)
            _gameView.SetBlueTeamUnitCount(i, blueUnitCount[i], cursorBlueUnitCount[i]);

        _gameView.SetTurnText(turn);
        _gameView.SetTurnChanged();
    }

    private bool DOGetValue(Vector2Int cellPos)
    {
        var count = attackPosts
            .Select(p => p + cellPos)
            .Count(p => fillTilemap.GetTile((Vector3Int)p) && ((TeamTurn == Team.Red && power[p] > 0) ||
                                                               (TeamTurn == Team.Blue && power[p] < 0)));
        var vector3Int = (Vector3Int)cellPos;
        var i = (TeamTurn == Team.Red ? cursorRedUnitCount[redUnitIndex] : cursorBlueUnitCount[blueUnitIndex]);

        if (i > 0 & mapTilemap.GetTile(vector3Int) != null && !teams.ContainsKey(cellPos) && count != 0 &&
            fillTilemap.GetTile(vector3Int) != grayFillTile)
        {
            return true;
        }

        return false;
    }


    private bool IsGameMapRange(Vector2Int pos) => mapTilemap.GetTile((Vector3Int)pos) != null;
    public Team TeamTurn => turn % 2 == 0 ? Team.Red : Team.Blue;


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

        if (teamTurn == Team.Red) redUnitCount[redUnitIndex] -= 1;
        else blueUnitCount[blueUnitIndex] -= 1;

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

    public GameObject rock;

    private void GetTiles()
    {
        var rect = size;
        rect.x += 2;
        rect.y += 2;
        rect.width -= 2;
        rect.height -= 2;
        for (int i = 0; i < 6; i++)
        {
            var vector3Int = new Vector3Int(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax), 0);
            mapTilemap.SetTile(vector3Int, null);
            Instantiate(rock, mapTilemap.CellToWorld(vector3Int), Quaternion.identity);
        }
    }
}