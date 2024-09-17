using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleShoot : MonoBehaviour
{
    [SerializeField] GameObject bubblePrefab;  // �߻��� ���� ������
    [SerializeField] RectTransform shootPoint; // �߻� ���� ��ġ (UI ��ҷ� ����)
    float shootSpeed = 10f;   // �߻� �ӵ�
    [SerializeField] PhysicsMaterial2D bounceMaterial;  // ���� ƨ��� ���� ���� ����
    [SerializeField] LineRenderer lineRenderer; // �巡�� ��θ� ǥ���� LineRenderer
    float minDragAngle = 30f; // �ּ� �巡�� ����
    float maxDragAngle = 150f; // �ִ� �巡�� ����
    LayerMask colLayers => LayerMask.GetMask("Wall", "Bubble");
    int simulationSteps = 15; // ��� ������ ���� �ùķ��̼� �ܰ� ��
    float simulationTimeStep = 0.1f; // �ùķ��̼� �ð� ����

    Vector3 dragStartPoint;
    Vector3 dragEndPoint;

    private List<Vector3> trajectoryPoints = new List<Vector3>();

    void Start()
    {
        // LineRenderer ����
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void Update()
    {
        // �巡�� ����
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragStartPoint.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }

        // �巡�� ��
        if (Input.GetMouseButton(0))
        {
            dragEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPoint.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

            // �巡�� ������ ���� ������ �ʰ��ϴ��� Ȯ��
            Vector2 dragVector = dragEndPoint - dragStartPoint;
            float angle = Vector2.Angle(Vector2.right, dragVector);

            if (angle < minDragAngle || angle > maxDragAngle)
            {
                // ���� ������ �ʰ��ϸ� LineRenderer ��Ȱ��ȭ
                lineRenderer.enabled = false;
                return;
            }
            else
            {
                // �巡�� ������ ���� ���� �ִ� ��� LineRenderer ������Ʈ
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = true;
                    UpdateLineRenderer();
                }
            }
        }

        // �巡�� ��
        if (Input.GetMouseButtonUp(0))
        {
            dragEndPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragEndPoint.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

            // �巡�� ������ ���� ������ �ʰ��ϴ��� Ȯ��
            Vector2 dragVector = dragEndPoint - dragStartPoint;
            float angle = Vector2.Angle(Vector2.right, dragVector);

            if (angle < minDragAngle || angle > maxDragAngle)
            {
                // ���� ������ �ʰ��ϸ� LineRenderer ��Ȱ��ȭ
                lineRenderer.enabled = false;
                return;
            }

            // �Ѿ� �߻�
            ShootBubble();
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0; // �巡�� ������ LineRenderer ����
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

        // �ùķ��̼��� ���� ��� ���
        Vector3 startPosition = Camera.main.ScreenToWorldPoint(shootPoint.position);
        startPosition.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

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

                // �浹�� ���̾� Ȯ��
                int hitLayer = hit.collider.gameObject.layer;
                Vector2 collisionNormal = hit.normal;

                if (hitLayer == LayerMask.NameToLayer("Wall"))
                {
                    // �ݻ� ���� ���
                    Vector2 reflectedDirection = Vector2.Reflect(currentVelocity, collisionNormal);
                    currentVelocity = reflectedDirection; // �ݻ�� �ӵ��� ����

                    // ��ġ�� �浹 ���� �����
                    position += currentVelocity * simulationTimeStep; // �ݻ� ���� �̵�
                }
                if (hitLayer == LayerMask.NameToLayer("Bubble"))
                {
                    lastBubble = hit.collider.GetComponent<Bubble>();
                    Debug.Log(lastBubble.transform.position);
                }
            }
            else // �浹�� �߻����� ������
            {
                position += currentVelocity * simulationTimeStep; // �ӵ��� ���� ��ġ ����
            }

            if (positionIndex < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                trajectoryPoints.Add(position);
                positionIndex++;
            }
        }

        // ������ ���� ����
        if (positionIndex < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(positionIndex, new Vector3(collisionPoint.x, collisionPoint.y, 0));
            trajectoryPoints.Add(collisionPoint); // ���� ���� �߰�
        }
        lineRenderer.positionCount = positionIndex;
    }

    void ShootBubble()
    {
        Vector3 shootPointWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(shootPoint, shootPoint.position, Camera.main, out shootPointWorldPosition);
        shootPointWorldPosition.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

        Vector2 shootDirection = (dragStartPoint - dragEndPoint).normalized;

        // ���� ����
        GameObject _bubbleObj = Instantiate(bubblePrefab, shootPointWorldPosition, Quaternion.identity);
        Bubble _bubble = _bubbleObj.GetComponent<Bubble>();

        StartCoroutine(MoveBubbleAlongPath(_bubble));
    }

    IEnumerator MoveBubbleAlongPath(Bubble _bubble)
    {
        if (trajectoryPoints.Count < 2) yield break; // ��ΰ� ���� ���

        // �ʱ� ��ġ
        Vector3 currentPosition = _bubble.transform.position;
        int currentPointIndex = 0;

        // ������ ��θ� ���� �̵��ϵ��� ��
        while (currentPointIndex < trajectoryPoints.Count)
        {
            Vector3 targetPoint = trajectoryPoints[currentPointIndex];
            float distance = Vector3.Distance(currentPosition, targetPoint);

            // ��ǥ �������� �̵�
            while (distance > 0.1f)
            {
                currentPosition = Vector3.MoveTowards(currentPosition, targetPoint, shootSpeed * Time.deltaTime);
                _bubble.transform.position = currentPosition;
                distance = Vector3.Distance(currentPosition, targetPoint);
                yield return null;
            }
            // ���� �������� �̵�
            currentPointIndex++;
        }

        // ���� ��ġ ����
        Vector2 _bubblePos = _bubble.transform.position;
        Vector2 _lastBubblePos = lastBubble.transform.position;

        Vector2 lastBubbleDirection = (_lastBubblePos - _bubblePos).normalized;

        MovementFlag closestDirectionFlag = MovementFlag.Down; // �⺻�� ����
        float minAngle = float.MaxValue; // �ּ� ���� �ʱ�ȭ

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