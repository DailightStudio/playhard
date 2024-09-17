using UnityEngine;
using System.Collections;
using Unity.Burst.CompilerServices;

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
    LayerMask gridLayers => LayerMask.GetMask("Grid");
    int simulationSteps = 15; // ��� ������ ���� �ùķ��̼� �ܰ� ��
    float simulationTimeStep = 0.1f; // �ùķ��̼� �ð� ����

    Vector3 dragStartPoint;
    Vector3 dragEndPoint;

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

    Vector2 collisionPoint;
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;

        Vector2 direction = (dragStartPoint - dragEndPoint).normalized;
        Vector2 velocity = direction * shootSpeed;

        // �ùķ��̼��� ���� ��� ���
        Vector3 startPosition = Camera.main.ScreenToWorldPoint(shootPoint.position);
        startPosition.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

        Vector2 position = startPosition;
        Vector2 currentVelocity = velocity;

        // LineRenderer�� ��ġ ���� �ùķ��̼� �ܰ� ���� ���߱�
        int maxSteps = simulationSteps;
        lineRenderer.positionCount = maxSteps; // ��ġ �� ����

        lineRenderer.SetPosition(0, startPosition); // ������ ����

        int positionIndex = 1; // LineRenderer�� ��ġ �ε��� (ù ��°�� shootPoint)
        collisionPoint = Vector2.zero; // �浹 ����

        for (int i = 0; i < maxSteps - 1; i++) // ������ ����Ʈ �����ϰ� �ݺ�
        {
            // ���� �浹 �˻�
            RaycastHit2D hit = Physics2D.Raycast(position, currentVelocity.normalized, currentVelocity.magnitude * simulationTimeStep, colLayers);

            if (hit.collider != null) // �浹�� �߻��ϸ�
            {
                // �浹 ���� ����
                position = hit.point;
                collisionPoint = position; // �浹 ���� ����
                if (positionIndex < lineRenderer.positionCount)
                {
                    lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                    positionIndex++;
                }

                // �浹�� ���̾� Ȯ��
                int hitLayer = hit.collider.gameObject.layer;
                Vector2 collisionNormal = hit.normal; // �浹 ���� (����ȭ�� ����)

                if (hitLayer == LayerMask.NameToLayer("Wall"))
                {
                    // �ݻ� ���� ���
                    Vector2 reflectedDirection = Vector2.Reflect(currentVelocity, collisionNormal);
                    currentVelocity = reflectedDirection; // �ݻ�� �ӵ��� ����

                    // ��ġ�� �浹 ���� �����
                    position += currentVelocity * simulationTimeStep; // �ݻ� ���� �̵�
                }
            }
            else // �浹�� �߻����� ������
            {
                position += currentVelocity * simulationTimeStep; // �ӵ��� ���� ��ġ ����
            }

            if (positionIndex < lineRenderer.positionCount)
            {
                lineRenderer.SetPosition(positionIndex, new Vector3(position.x, position.y, 0));
                positionIndex++;
            }
        }

        // ������ ���� ����
        if (positionIndex < lineRenderer.positionCount)
        {
            lineRenderer.SetPosition(positionIndex, new Vector3(collisionPoint.x, collisionPoint.y, 0));
        }

        // LineRenderer�� ������ ���� ���� �ε��� ������ ����
        lineRenderer.positionCount = positionIndex;
    }

    private void ShootBubble()
    {
        // UI�� RectTransform�� ���� ��ǥ�� ��ȯ
        Vector3 shootPointWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(shootPoint, shootPoint.position, Camera.main, out shootPointWorldPosition);
        shootPointWorldPosition.z = 0; // Z���� 0���� �����Ͽ� 2D ��鿡 ���߱�

        // ���� ����
        GameObject bubble = Instantiate(bubblePrefab, shootPointWorldPosition, Quaternion.identity);
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // ������ ���� ���� ���� (�ݻ� ó��)
            CircleCollider2D collider = bubble.GetComponent<CircleCollider2D>();
            if (collider != null && bounceMaterial != null)
            {
                collider.sharedMaterial = bounceMaterial;
            }

            // �巡�� ���� ��� (�巡�� ���ۿ��� ���� �� ���͸� ����)
            Vector2 shootDirection = (dragStartPoint - dragEndPoint).normalized;

            // ���� �ӵ� ����
            rb.velocity = shootDirection * shootSpeed;

            // ������ LineRenderer�� ��θ� ���� �̵��ϵ��� ����
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