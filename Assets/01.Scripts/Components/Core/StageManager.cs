using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class StageManager : Singleton<StageManager>
{
    public List<GameObject> stageList { get; set; } = new List<GameObject>();

    public int stageLevel { get; set; } = 0;
    public Stage currentStage { get; set; }

    public Dictionary<MovementFlag, Vector2> directionsDic = new Dictionary<MovementFlag, Vector2>()
    {
        { MovementFlag.Down, new Vector2(0, -1) },
        { MovementFlag.Up, new Vector2(0, 1) },
        { MovementFlag.Right, new Vector2(1, 0) },
        { MovementFlag.Left, new Vector2(-1, 0) },
        { MovementFlag.LeftUp, new Vector2(-1, 1) },
        { MovementFlag.RightUp, new Vector2(1, 1) },
        { MovementFlag.LeftDown, new Vector2(-1, -1) },
        { MovementFlag.RightDown, new Vector2(1, -1) },
    };

    new void Awake()
    {
        LoadStages();
        CreateStage();

        InitStageDatas();


        StartCoroutine(ie());
    }

    void CreateStage()
    {
        if (currentStage != null) DestroyImmediate(currentStage);
        currentStage = Instantiate(stageList[stageLevel], Vector2.zero, Quaternion.identity).GetComponent<Stage>();
    }

    public void LoadStages()
    {
        GameObject[] _stages = Resources.LoadAll<GameObject>("Stage");
        foreach (GameObject _prefab in _stages)
        {
            stageList.Add(_prefab);
        }
    }

    IEnumerator ie()
    {
        while (count < 15)
        {
            yield return new WaitForSeconds(0.2f);
            NextStep();
        }
    }

    int count = 0;
    public void NextStep()
    {
        count++;

        // 변경 데이터 리스트
        List<(HexaGridData oldData, HexaGridData newData)> updates = new List<(HexaGridData, HexaGridData)>();

        foreach (var item in HexagonGridManager.Instance.hexaGridDatasDic.ToList())
        {
            HexaGridData _data = item.Value;
            if (_data.bubble != null)
            {
                Bubble _bubble = _data.bubble;
                HexaGridData _value = HexagonGridManager.Instance.hexaGridDatasDic[_bubble.nextStep];
                Vector2 _nextStep = _value.gridXY;

                _bubble.ChangeRigidBody(RigidbodyType2D.Kinematic);
                _bubble.transform.DOMove(_value.slot.transform.position, 0.1f).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        updates.Add((oldData: _data, newData: _value));
                        _bubble.ChangeRigidBody(RigidbodyType2D.Dynamic);
                    });
            }
        }

        foreach (var update in updates)
        {
            update.newData.bubble = update.oldData.bubble;
            update.oldData.bubble = null;
        }
    }

    public void NextStage()
    {
        if (stageList.Count - 1 > stageLevel)
        {
            Debug.Log("다음 스테이지");
            stageLevel++;

            InitStageDatas();
            CreateStage();
        }
        else Debug.Log("모든 스테이지 클리어");
    }

    void InitStageDatas()
    {
        HexagonGridManager.Instance.SetGrid();
        HexagonGridManager.Instance.SetBubbles();
    }

    public bool IsCloseBubble(GridSlot _slot)
    {
        bool _isClose = false;

        foreach (var _dir in directionsDic) // 각 방향에 대해 검사
        {
            Vector2 _dirPos = _slot.gridXY + _dir.Value;
            _dirPos.x = Mathf.Max(0, _dirPos.x);
            _dirPos.y = Mathf.Max(0, _dirPos.y);

            if (HexagonGridManager.Instance.hexaGridDatasDic[_dirPos].bubble != null)
            {
                _isClose = true;
                break;  // 하나라도 발견되면 종료
            }
        }
        return _isClose;
    }

}
