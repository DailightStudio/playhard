using UnityEngine;
using System.Collections;
using Unity.Burst.CompilerServices;

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
    LayerMask gridLayers => LayerMask.GetMask("Grid");
    int simulationSteps = 15; // 경로 예측을 위한 시뮬레이션 단계 수
    float simulationTimeStep = 0.1f; // 시뮬레이션 시간 간격

    Vector3 dragStartPoint;
    Vector3 dragEndPoint;

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

    Vector2 collisionPoint;
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        Vector2 direction = (dragStartPoint - dragEndPoint).normalized;
        Vector2 velocity = direction * shootSpeed;

        // 시뮬레이션을 통해 경로 계산
        Vector3 startPosition = Camera.main.ScreenToWorldPoint(shootPoint.position);
        startPosition.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

        Vector2 position = startPosition;
        Vector2 currentVelocity = velocity;

        // LineRenderer의 위치 수를 시뮬레이션 단계 수에 맞추기
        int maxSteps = simulationSteps;
        lineRenderer.positionCount = maxSteps; // 위치 수 설정

        lineRenderer.SetPosition(0, startPosition); // 시작점 설정

        int positionIndex = 1; // LineRenderer의 위치 인덱스 (첫 번째는 shootPoint)
        collisionPoint = Vector2.zero; // 충돌 지점

        for (int i = 0; i < maxSteps - 1; i++) // 마지막 포인트 제외하고 반복
        {
            // 벽과 충돌 검사
            RaycastHit2D hit = Physics2D.Raycast(position, currentVelocity.normalized, currentVelocity.magnitude * simulationTimeStep, colLayers);

            if (hit.collider != null) // 충돌이 발생하면
            {
                // 충돌 지점 설정
                position = hit.point;
                collisionPoint = position; // 충돌 지점 저장
                if (positionIndex < lineRenderer.positionCount)
                {
                    lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                    positionIndex++;
                }

                // 충돌한 레이어 확인
                int hitLayer = hit.collider.gameObject.layer;
                Vector2 collisionNormal = hit.normal; // 충돌 방향 (정규화된 벡터)

                if (hitLayer == LayerMask.NameToLayer("Wall"))
                {
                    // 반사 벡터 계산
                    Vector2 reflectedDirection = Vector2.Reflect(currentVelocity, collisionNormal);
                    currentVelocity = reflectedDirection; // 반사된 속도로 갱신

                    // 위치가 충돌 이후 변경됨
                    position += currentVelocity * simulationTimeStep; // 반사 이후 이동
                }
            }
            else // 충돌이 발생하지 않으면
            {
                position += currentVelocity * simulationTimeStep; // 속도에 따른 위치 갱신
            }

            if (positionIndex < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                positionIndex++;
            }
        }

        // 마지막 지점 설정
        if (positionIndex < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(positionIndex, new Vector3(collisionPoint.x, collisionPoint.y, 0));
        }

        // LineRenderer의 포지션 수를 현재 인덱스 값으로 설정
        lineRenderer.positionCount = positionIndex;
    }

    private void ShootBubble()
    {
        // UI의 RectTransform을 월드 좌표로 변환
        Vector3 shootPointWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(shootPoint, shootPoint.position, Camera.main, out shootPointWorldPosition);
        shootPointWorldPosition.z = 0; // Z값을 0으로 설정하여 2D 평면에 맞추기

        // 구슬 생성
        GameObject bubble = Instantiate(bubblePrefab, shootPointWorldPosition, Quaternion.identity);
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 구슬에 물리 소재 적용 (반사 처리)
            CircleCollider2D collider = bubble.GetComponent<CircleCollider2D>();
            if (collider != null && bounceMaterial != null)
            {
                collider.sharedMaterial = bounceMaterial;
            }

            // 드래그 방향 계산 (드래그 시작에서 끝을 뺀 벡터를 반전)
            Vector2 shootDirection = (dragStartPoint - dragEndPoint).normalized;

            // 구슬 속도 설정
            rb.velocity = shootDirection * shootSpeed;

            // 구슬이 LineRenderer의 경로를 따라 이동하도록 시작
            StartCoroutine(FollowTrajectory(bubble, rb));
        }
        else
        {
            Debug.LogError("Bubble prefab must have a Rigidbody2D component.");
        }
    }

    private IEnumerator FollowTrajectory(GameObject bubble, Rigidbody2D rb)
    {
        bool _isClose = false;
        float radius = HexagonGridManager.Instance.radius;

        while (_isClose == false)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(bubble.transform.position, radius, gridLayers);
            if (colliders.Length > 0)
            {
                foreach (var _item in colliders)
                {
                    GridSlot _slot = _item.GetComponent<GridSlot>();
                    _isClose = StageManager.Instance.IsCloseBubble(_slot);
                    if (_isClose == true)
                    {
                        rb.velocity = Vector2.zero;
                        bubble.transform.position = HexagonGridManager.Instance.hexaGridDatasDic[_slot.gridXY].slot.transform.position;
                        break;
                    }
                }
            }
            yield return null;
        }
    }
}