using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectToOverlayUI : MonoBehaviour
{
    public Transform worldObject;  // 3D 오브젝트 (예: 아이템)
    public RectTransform targetUI; // 이동할 UI 위치
    public Camera mainCamera;      // 메인 카메라
    public Canvas canvas;          // Overlay Canvas

    public float moveDuration = 1.5f; // 이동 시간
    public AnimationCurve moveCurve;  // 부드러운 이동을 위한 애니메이션 커브

    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    public void StartMoveToUI()
    {
        if (isMoving) return;
        StartCoroutine(MoveObjectToUI());
    }

    IEnumerator MoveObjectToUI()
    {
        isMoving = true;
        float elapsedTime = 0f;

        // 3D 오브젝트의 시작 위치
        Vector3 startPosition = worldObject.position;

        // UI 위치를 스크린 좌표로 변환
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(targetUI.position);

        // UI 상의 최종 위치 설정
        Vector2 targetUIPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, targetScreenPos, null, out targetUIPos);

        // 오브젝트를 UI 내에서 움직이도록 UI 패널을 사용
        GameObject floatingObject = Instantiate(worldObject.gameObject, startPosition, Quaternion.identity);
        //floatingObject.transform.SetParent(canvas.transform, false); // Canvas에 추가
        //floatingObject.transform.localScale = Vector3.one * 0.5f; // 크기 조절
        floatingObject.GetComponent<Collider>().enabled = false; // 물리 충돌 제거

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            t = moveCurve.Evaluate(t); // 애니메이션 커브 적용

            // 현재 위치를 스크린 좌표로 변환
            Vector3 currentScreenPos = Vector3.Lerp(mainCamera.WorldToScreenPoint(startPosition), targetScreenPos, t);

            // 스크린 좌표를 UI 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, currentScreenPos, null, out Vector2 newUIPos);
            floatingObject.transform.localPosition = newUIPos;

            yield return null;
        }

        // UI에 결합 후 삭제 또는 애니메이션 효과 추가 가능
        floatingObject.transform.SetParent(targetUI);
        floatingObject.transform.localPosition = Vector3.zero;
        floatingObject.transform.localScale = Vector3.one;

        //Destroy(floatingObject, 0.5f);
        isMoving = false;
    }
}
