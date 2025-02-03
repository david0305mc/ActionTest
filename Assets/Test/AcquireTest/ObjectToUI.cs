using UnityEngine;
using System.Collections;

public class ObjectToUI : MonoBehaviour
{
    public Transform worldObject;  // 3D 오브젝트
    public RectTransform targetUI; // 이동할 UI 위치
    public Camera mainCamera;      // 메인 카메라 (UI와 3D를 같이 렌더링하는 카메라)
    public Canvas canvas;          // UI가 속한 캔버스

    public float moveDuration = 1.5f; // 이동 시간
    public AnimationCurve moveCurve;  // 움직임을 조절하는 애니메이션 커브

    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void StartMoveToUI()
    {
        if (isMoving) return;
        StartCoroutine(MoveObjectToUIBezier());
    }

    IEnumerator MoveObjectToUI()
    {
        isMoving = true;
        float elapsedTime = 0f;

        // 3D 오브젝트의 시작 위치 저장
        Vector3 startPosition = worldObject.position;

        // UI 위치를 월드 공간으로 변환
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(targetUI.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, targetScreenPos, mainCamera, out Vector3 targetWorldPos);

        GameObject floatingObject = Instantiate(worldObject.gameObject, startPosition, Quaternion.identity);
        floatingObject.transform.localScale = worldObject.localScale;
        floatingObject.GetComponent<Collider>().enabled = false; // 충돌 제거

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            t = moveCurve.Evaluate(t); // 애니메이션 커브 적용

            // 선형 보간 이동 (Lerp)
            floatingObject.transform.position = Vector3.Lerp(startPosition, targetWorldPos, t);
            yield return null;
        }

        // UI에 결합 (선택 사항)
        floatingObject.transform.SetParent(targetUI);
        floatingObject.transform.localPosition = Vector3.zero;
        floatingObject.transform.localScale = Vector3.one;

        Destroy(floatingObject, 0.5f); // 최종적으로 제거
        isMoving = false;
    }
    IEnumerator MoveObjectToUIBezier()
    {
        isMoving = true;
        float elapsedTime = 0f;

        Vector3 startPosition = worldObject.position;

        // UI 위치를 월드 공간으로 변환
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(targetUI.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, targetScreenPos, mainCamera, out Vector3 targetWorldPos);

        // 중간 곡선 포인트 설정 (출발점과 도착점의 중간 높이를 올려서 포물선 연출)
        Vector3 controlPoint = (startPosition + targetWorldPos) / 2 + Vector3.left * 50f;

        GameObject floatingObject = Instantiate(worldObject.gameObject, startPosition, Quaternion.identity);
        floatingObject.transform.localScale = worldObject.localScale;
        floatingObject.GetComponent<Collider>().enabled = false;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            t = moveCurve.Evaluate(t); // 곡선 적용

            // Bezier 곡선 공식
            Vector3 newPosition = Mathf.Pow(1 - t, 2) * startPosition +
                                  2 * (1 - t) * t * controlPoint +
                                  Mathf.Pow(t, 2) * targetWorldPos;

            floatingObject.transform.position = newPosition;
            yield return null;
        }

        // UI에 결합
        floatingObject.transform.SetParent(targetUI);
        floatingObject.transform.localPosition = Vector3.zero;
        floatingObject.transform.localScale = Vector3.one;

        Destroy(floatingObject, 0.5f);
        isMoving = false;
    }
}
