using UnityEngine;

public class CameraSway : MonoBehaviour {
    public float focalDistance = 2f;
    public float sensitivityX = 2f;
    public float sensitivityY = 1.5f;
    public float smoothing = 5f;
    public HandController handC;

    private Transform tr;
    private Vector3 defaultPos;
    private Vector3 focalPoint;
    private Vector2 posOffset;

    private void Awake() {
        tr = transform;
        focalPoint = tr.position + (tr.forward * focalDistance);
        defaultPos = tr.localPosition;
    }

    private void Update() {
        //if(handC.isHoldingObj)
        //    return;

        float xOffset = Mathf.Clamp((Input.mousePosition.x - (Screen.width * 0.5f)) / (Screen.width * 0.5f), -1f, 1f);
        float yOffset = Mathf.Clamp((Input.mousePosition.y - (Screen.height * 0.5f)) / (Screen.height * 0.5f), -1f, 1f);
        posOffset = Vector2.Lerp(posOffset, new Vector2(xOffset * sensitivityX, yOffset * sensitivityY) * 5f, Time.deltaTime * smoothing);

        tr.localPosition = defaultPos + new Vector3(posOffset.x, posOffset.y, 0f);
        tr.localRotation = Quaternion.LookRotation(focalPoint - tr.position);
    }
}