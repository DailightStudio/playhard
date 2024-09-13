#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;
using System;
using static System.Collections.Specialized.BitVector32;

public class HexaGridData
{
    public float x;
    public float y;

    public HexaGridData(Vector2 _pos)
    {
        this.x = _pos.x;
        this.y = _pos.y;
    }
}

public class HexagonGridManager : Singleton<HexagonGridManager>
{
    [HideInInspector] public Dictionary<Vector2, HexaGridData> hexaGridDatasDic = new Dictionary<Vector2, HexaGridData>();

    [SerializeField] GameObject bubblePrafab;
    [SerializeField] GameObject hexaGridPrafab;
    public GameObject bezierCurvePrafab;
    [SerializeField] Transform stageParent;
    public Transform bezierParent;
    public Transform gridParent;
    [HideInInspector] public BubbleColor selectColorEditor;
    [HideInInspector] public string saveFilePath = "Assets/03.Prafabs/Stage"; // 스테이지를 저장할 파일 경로
    float radius = 0.4f; // 구슬 반지름

    [HideInInspector] public string stageFilePath = "Stage_00";
    [HideInInspector] public int row = 6; // 행
    [HideInInspector] public int column = 12; // 열
    [HideInInspector] public int startColumn = 6; // 시작 열

    private void Start()
    {
        CreateGrid();
    }
    public void CreateGrid()
    {
        if (gridParent.childCount > 0)
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
        BubbleManager.Instance.bubbleDatasDic.Clear();

        // 그리드 생성 및 배치
        for (int _row = 0; _row < column + startColumn; _row++)
        {
            for (int _col = 0; _col < row; _col++)
            {
                float _xOffset = _col * _horizontalSpacing;
                float _yOffset = _row * _verticalSpacing;

                // 짝수 행의 경우 x 오프셋 추가
                if (_row % 2 == 0)
                {
                    _xOffset += _horizontalSpacing / 2;
                }
                Vector2 _pos = startPos + new Vector2(_xOffset, _yOffset);
                GameObject _obj = Instantiate(hexaGridPrafab, _pos, Quaternion.identity);
                _obj.transform.SetParent(gridParent);
                HexaGridData _gridData = new HexaGridData(_pos);
                hexaGridDatasDic.Add(new Vector2(_row, _col), _gridData);
            }
        }
        CreateBubbles();
    }

    void CreateBubbles()
    {
        for (int _row = startColumn; _row < column + startColumn; _row++)
        {
            for (int _col = 0; _col < row; _col++)
            {
                Vector2 _key = new Vector2(_row, _col);
                HexaGridData _data = hexaGridDatasDic[_key];
                Vector2 _pos = new Vector2(_data.x, _data.y);
                GameObject _obj = Instantiate(bubblePrafab, _pos, Quaternion.identity);
                _obj.transform.SetParent(stageParent);
                Bubble _bubble = _obj.GetComponent<Bubble>();
                BubbleData _bubbleData = new BubbleData(_bubble, _pos);
                BubbleManager.Instance.bubbleDatasDic.Add(new Vector2(_row, _col), _bubbleData);
            }
        }
    }


#if UNITY_EDITOR
    public void RemoveStageAll()
    {
        while (stageParent.childCount > 0)
        {
            DestroyImmediate(stageParent.transform.GetChild(0).gameObject);
        }
        while (gridParent.childCount > 0)
        {
            DestroyImmediate(gridParent.transform.GetChild(0).gameObject);
        } 
        while (bezierParent.childCount > 0)
        {
            DestroyImmediate(bezierParent.transform.GetChild(0).gameObject);
        }
    }

    public void SaveStage()
    {
        // 프리팹 저장 경로 설정
        string prefabName = $"{stageFilePath}.prefab";
        string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // 슬래시 통일

        // 프리팹 저장
        PrefabUtility.SaveAsPrefabAsset(stageParent.gameObject, savePath);

        // 에셋 데이터베이스를 갱신
        AssetDatabase.SaveAssets();
    }
#endif
}