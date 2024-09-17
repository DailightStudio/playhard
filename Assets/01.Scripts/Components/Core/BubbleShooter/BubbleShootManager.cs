using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleShoot : MonoBehaviour
{
    [SerializeField] GameObject bubblePrefab;  // 발사할 구슬 프리팹
    [SerializeField] RectTransform shootPoint; // 발사 시작 위치 (UI 요소로 설정)
    float shootSpeed = 10f;   // 발사 속도
    [SerializeField] PhysicsMaterial2D bounceMaterial;  // 벽에 튕기기 위한 물리 소재
    [SerializeField] LineRenderer lineRenderer; // 드래그 경로를 표시할 LineRenderer
    float minDragAngle = 30f; // 최소 드래그 각도
    float maxDragAngle = 150f; // 최대 드래그 각도
    LayerMask colLayers => LayerMask.GetMask("Wall", "Bubble");
    int simulationSteps = 15; // 경로 예측을 위한 시뮬레이션 단계 수
    float simulationTimeStep = 0.1f; // 시뮬레이션 시간 간격

    Vector3 dragStartPoint;
    Vector3 dragEndPoint;

    private List<Vector3> trajectoryPoints = new List<Vector3>();

    void Start()
    {
        // LineRenderer 설정
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void Update()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragStartPoint.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }

        // 드래그 중
        if (Input.GetMouseButton(0))
        {
            dragEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPoint.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

            // 드래그 방향이 제한 각도를 초과하는지 확인
            Vector2 dragVector = dragEndPoint - dragStartPoint;
            float angle = Vector2.Angle(Vector2.right, dragVector);

            if (angle < minDragAngle || angle > maxDragAngle)
            {
                // 제한 각도를 초과하면 LineRenderer 비활성화
                lineRenderer.enabled = false;
                return;
            }
            else
            {
                // 드래그 방향이 범위 내에 있는 경우 LineRenderer 업데이트
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    UpdateLineRenderer();
                }
            }
        }

        // 드래그 끝
        if (Input.GetMouseButtonUp(0))
        {
            dragEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPoint.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

            // 드래그 방향이 제한 각도를 초과하는지 확인
            Vector2 dragVector = dragEndPoint - dragStartPoint;
            float angle = Vector2.Angle(Vector2.right, dragVector);

            if (angle < minDragAngle || angle > maxDragAngle)
            {
                // 제한 각도를 초과하면 LineRenderer 비활성화
                lineRenderer.enabled = false;
                return;
            }

            // 총알 발사
            ShootBubble();
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0; // 드래그 끝나면 LineRenderer 비우기
            }
        }
    }

    Bubble lastBubble = null;
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        trajectoryPoints.Clear();

        Vector2 direction = (dragStartPoint - dragEndPoint).normalized;
        Vector2 velocity = direction * shootSpeed;

        // 시뮬레이션을 통해 경로 계산
        Vector3 startPosition = Camera.main.ScreenToWorldPoint(shootPoint.position);
        startPosition.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

        Vector2 position = startPosition;
        Vector2 currentVelocity = velocity;

        int maxSteps = simulationSteps;
        lineRenderer.positionCount = maxSteps;

        lineRenderer.SetPosition(0, startPosition);
        trajectoryPoints.Add(startPosition);

        int positionIndex = 1;
        Vector2 collisionPoint = Vector2.zero;

        for (int i = 0; i < maxSteps - 1; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, currentVelocity.normalized, currentVelocity.magnitude * simulationTimeStep, colLayers);

            if (hit.collider != null)
            {
                position = hit.point;
                collisionPoint = position;
                if (positionIndex < lineRenderer.positionCount)
                {
                    lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                    positionIndex++;
                }

                // 충돌한 레이어 확인
                int hitLayer = hit.collider.gameObject.layer;
                Vector2 collisionNormal = hit.normal;

                if (hitLayer == LayerMask.NameToLayer("Wall"))
                {
                    // 반사 벡터 계산
                    Vector2 reflectedDirection = Vector2.Reflect(currentVelocity, collisionNormal);
                    currentVelocity = reflectedDirection; // 반사된 속도로 갱신

                    // 위치가 충돌 이후 변경됨
                    position += currentVelocity * simulationTimeStep; // 반사 이후 이동
                }
                if (hitLayer == LayerMask.NameToLayer("Bubble"))
                {
                    lastBubble = hit.collider.GetComponent<Bubble>();
                    Debug.Log(lastBubble.transform.position);
                }
            }
            else // 충돌이 발생하지 않으면
            {
                position += currentVelocity * simulationTimeStep; // 속도에 따른 위치 갱신
            }

            if (positionIndex < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                trajectoryPoints.Add(position);
                positionIndex++;
            }
        }

        // 마지막 지점 설정
        if (positionIndex < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(positionIndex, new Vector3(collisionPoint.x, collisionPoint.y, 0));
            trajectoryPoints.Add(collisionPoint); // 최종 지점 추가
        }
        lineRenderer.positionCount = positionIndex;
    }

    void ShootBubble()
    {
        Vector3 shootPointWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(shootPoint, shootPoint.position, Camera.main, out shootPointWorldPosition);
        shootPointWorldPosition.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

        Vector2 shootDirection = (dragStartPoint - dragEndPoint).normalized;

        // 구슬 생성
        GameObject _bubbleObj = Instantiate(bubblePrefab, shootPointWorldPosition, Quaternion.identity);
        Bubble _bubble = _bubbleObj.GetComponent<Bubble>();

        StartCoroutine(MoveBubbleAlongPath(_bubble));
    }

    IEnumerator MoveBubbleAlongPath(Bubble _bubble)
    {
        if (trajectoryPoints.Count < 2) yield break; // 경로가 없는 경우

        // 초기 위치
        Vector3 currentPosition = _bubble.transform.position;
        int currentPointIndex = 0;

        // 구슬이 경로를 따라 이동하도록 함
        while (currentPointIndex < trajectoryPoints.Count)
        {
            Vector3 targetPoint = trajectoryPoints[currentPointIndex];
            float distance = Vector3.Distance(currentPosition, targetPoint);

            // 목표 지점으로 이동
            while (distance > 0.1f)
            {
                currentPosition = Vector3.MoveTowards(currentPosition, targetPoint, shootSpeed * Time.deltaTime);
                _bubble.transform.position = currentPosition;
                distance = Vector3.Distance(currentPosition, targetPoint);
                yield return null;
            }
            // 다음 지점으로 이동
            currentPointIndex++;
        }

        // 최종 위치 보정
        Vector2 _bubblePos = _bubble.transform.position;
        Vector2 _lastBubblePos = lastBubble.transform.position;

        Vector2 lastBubbleDirection = (_lastBubblePos - _bubblePos).normalized;

        MovementFlag closestDirectionFlag = MovementFlag.Down; // 기본값 설정
        float minAngle = float.MaxValue; // 최소 각도 초기화

        foreach (var direction in StageManager.Instance.directionsDic)
        {
            float angle = Vector2.Angle(lastBubbleDirection, direction.Value);
            if (angle < minAngle)
            {
                minAngle = angle;
                closestDirectionFlag = direction.Key;
            }
        }

        Vector2 closestDirection = StageManager.Instance.directionsDic[closestDirectionFlag];

        Vector2 correctedPosition = HexagonGridManager.Instance.hexaGridDatasDic[lastBubble.currentXY + closestDirection].slot.transform.position;
        _bubble.transform.position = new Vector3(correctedPosition.x, correctedPosition.y, -2f);

    }
}