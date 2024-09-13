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
    [HideInInspector] public string saveFilePath = "Assets/03.Prafabs/Stage"; // ���������� ������ ���� ���
    float radius = 0.4f; // ���� ������

    [HideInInspector] public string stageFilePath = "Stage_00";
    [HideInInspector] public int row = 6; // ��
    [HideInInspector] public int column = 12; // ��
    [HideInInspector] public int startColumn = 6; // ���� ��

    private void Start()
    {
        CreateGrid();
    }
    public void CreateGrid()
    {
        if (gridParent.childCount > 0)
        {
            Debug.LogError("�̹� ���������� �����Ǿ� �ֽ��ϴ�.");
            return;
        }

        float _horizontalSpacing = radius * Mathf.Sqrt(3f); // ���� ����
        float _verticalSpacing = 1.5f * radius; // ���� ����

        // ��ü �׸����� �ʺ� ���
        float _gridWidth = (row - 1) * _horizontalSpacing;

        // ȭ�� ���� �ϴ� ��ǥ�� ������ (ȭ�� �ϴ� ����)
        Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);

        // ���δ� �߾�, ���δ� ȭ�� �ٴڿ� ����
        Vector2 startPos = new Vector2(-_gridWidth / 2, bottomLeft.y);

        hexaGridDatasDic.Clear();
        BubbleManager.Instance.bubbleDatasDic.Clear();

        // �׸��� ���� �� ��ġ
        for (int _row = 0; _row < column + startColumn; _row++)
        {
            for (int _col = 0; _col < row; _col++)
            {
                float _xOffset = _col * _horizontalSpacing;
                float _yOffset = _row * _verticalSpacing;

                // ¦�� ���� ��� x ������ �߰�
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
        // ������ ���� ��� ����
        string prefabName = $"{stageFilePath}.prefab";
        string savePath = Path.Combine(saveFilePath, prefabName).Replace("\\", "/"); // ������ ����

        // ������ ����
        PrefabUtility.SaveAsPrefabAsset(stageParent.gameObject, savePath);

        // ���� �����ͺ��̽��� ����
        AssetDatabase.SaveAssets();
    }
#endif
}