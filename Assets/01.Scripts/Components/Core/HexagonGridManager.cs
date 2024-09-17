using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI.Table;

public class HexaGridData
{
    public Bubble bubble;
    public GridSlot slot;
    public Vector2 gridXY;

    public HexaGridData(GridSlot _slot, Bubble _bubble, Vector2 _pos)
    {
        this.bubble = _bubble;
        this.slot = _slot;
        this.gridXY = _pos;
    }
}

public class HexagonGridManager : Singleton<HexagonGridManager>
{
    [HideInInspector] public Dictionary<Vector2, HexaGridData> hexaGridDatasDic = new Dictionary<Vector2, HexaGridData>();

    public GameObject bubblePrafab;
    [SerializeField] GameObject hexaGridPrafab;

    public Stage stage;

    public Sprite movementDown, movementRight, movementLeft;
    public Dictionary<MovementFlag, Sprite> movementSprDic => new Dictionary<MovementFlag, Sprite>()
    {
        { MovementFlag.Down, movementDown},
        { MovementFlag.Left, movementLeft},
        { MovementFlag.Right, movementRight}
    };

    [HideInInspector] public BubbleColor selectColorEditor;
    [HideInInspector] public MovementFlag selectMovementEditor;

    public float radius { get; set; } = 0.4f; // 구슬 반지름

    [HideInInspector] public int row = 6; // 행
    [HideInInspector] public int column = 12; // 열
    [HideInInspector] public int startColumn = 6; // 시작 열


    new void Awake()
    {
        RemoveStageAll();
    }

    public void SetGrid()
    {
        hexaGridDatasDic.Clear();
        Transform _stageGrid = StageManager.Instance.currentStage.gridParent;
        for (int i = 0; i < _stageGrid.childCount; i++)
        {
            GridSlot _slot = _stageGrid.GetChild(i).GetComponent<GridSlot>();
            _slot.sprRenderer.sprite = null;
            HexaGridData _gridData = new HexaGridData(_slot, null, _slot.gridXY);
            hexaGridDatasDic.Add(_slot.gridXY, _gridData);
        }
    }

    public void SetBubbles()
    {
        Transform _stageBubble = StageManager.Instance.currentStage.bubblesParent;
        for (int i = 0; i < _stageBubble.childCount; i++)
        {
            Bubble _bubble = _stageBubble.GetChild(i).GetComponent<Bubble>();
            hexaGridDatasDic[_bubble.currentXY].bubble = _bubble;
        }
    }

    public void CreateGrid()
    {
        if (stage.gridParent.childCount > 0)
        {
            Debug.LogError("이미 스테이지가 생성되어 있습니다.");
            return;
        }

        float _horizontalSpacing = radius * Mathf.Sqrt(3f); // 수평 간격
        float _verticalSpacing = 1.5f * radius; // 수직 간격

        // 전체 그리드의 너비 계산
        float _gridWidth = (row - 1) * _horizontalSpacing;

        // 화면 좌측 하단 좌표를 가져옴 (화면 하단 기준)
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);

        // 가로는 중앙, 세로는 화면 바닥에 맞춤
        Vector2 startPos = new Vector2(-_gridWidth / 2, bottomLeft.y);

        hexaGridDatasDic.Clear();

        // 그리드 생성 및 배치
        for (int _col = 0; _col < column + startColumn; _col++)
        {
            for (int _row = 0; _row < row; _row++)
            {
                float _xOffset = _row * _horizontalSpacing;
                float _yOffset = _col * _verticalSpacing;

                // 짝수 열의 경우,
                if (_col % 2 == 0)
                {
                    _xOffset += _horizontalSpacing / 2;
                }

                Vector2 _pos = startPos + new Vector2(_xOffset, _yOffset);
                GameObject _obj = Instantiate(hexaGridPrafab, _pos, Quaternion.identity);
                _obj.transform.SetParent(stage.gridParent);

                HexaGridData _gridData = new HexaGridData(_obj.GetComponent<GridSlot>(), null, new Vector2(_row, _col));
                Vector2 _gridXY = new Vector2(_row, _col);
                hexaGridDatasDic.Add(_gridXY, _gridData);
                _obj.GetComponent<GridSlot>().gridXY = _gridXY;
            }
        }
        CreateBubbles();
    }

    void CreateBubbles()
    {
        for (int _col = startColumn; _col < column + startColumn; _col++)
        {
            for (int _row = 0; _row < row; _row++)
            {
                Vector2 _key = new Vector2(_row, _col);
                HexaGridData _data = hexaGridDatasDic[_key];
                GameObject _obj = Instantiate(bubblePrafab, new Vector3(_data.slot.transform.position.x, _data.slot.transform.position.y, -0.1f), Quaternion.identity);
                _obj.transform.SetParent(stage.bubblesParent);
                Bubble _bubble = _obj.GetComponent<Bubble>();
                _bubble.currentXY = _key;
                _data.bubble = _bubble;
            }
        }
    }

    public void RemoveStageAll()
    {
        while (stage.gridParent.childCount > 0)
        {
            DestroyImmediate(stage.gridParent.transform.GetChild(0).gameObject);
        }

        while (stage.bubblesParent.childCount > 0)
        {
            DestroyImmediate(stage.bubblesParent.transform.GetChild(0).gameObject);
        }

    }
}