using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    public Camera mainCam;
    public bool isHoldingObj {
        get {
            return (curHeld != null);
        }
    }

    private Transform cachedTrans;
    private Rigidbody curHover = null;
    private Rigidbody curHeld = null;
    private RaycastHit rHit;
    private bool curHeldHasGravity;
    private Vector3 mouseVelo;
    private Vector3 lastMousePos;
    private Vector3 mouseHeldOffset;
    private float heldDistance = 3f;
    private bool wireframe = false;

    private const float REACH_DISTANCE = 10f;
    private const float MOUSE_VELO_SCALING = 0.1f;

    void Awake() {
        cachedTrans = GetComponent<Transform>();
    }

    void OnPreRender() {
        GL.wireframe = wireframe;
    }

    void OnPostRender() {
        GL.wireframe = false;
    }
 
    void Update() {
        if(Input.GetKeyDown(KeyCode.W)) {
            wireframe = !wireframe;

            if(wireframe) {
                mainCam.clearFlags = CameraClearFlags.Nothing;
            } else {
                mainCam.clearFlags = CameraClearFlags.Skybox;
            }
        }

        if(!isHoldingObj) {
            HandleObjectSelection();

            if(Input.GetMouseButtonDown(0)) {
                curHeld = curHover;
                if(curHeld != null) {
                    curHeldHasGravity = curHeld.useGravity;
                    curHeld.useGravity = false;
                    curHeld.velocity = Vector3.zero;

                    Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
                    Vector3 targetMousePos = cachedTrans.position + camRay.direction * heldDistance;
                    mouseHeldOffset = curHeld.GetComponent<Transform>().position - targetMousePos;
                }

                curHover = null;
                return;
            }
        } 
    }

    void FixedUpdate() {
        if(isHoldingObj) {
            HandleHeldObject();

            if(!Input.GetMouseButton(0)) {
                curHeld.useGravity = curHeldHasGravity;
                curHeld.AddForceAtPosition(mouseVelo * MOUSE_VELO_SCALING, curHeld.GetComponent<Transform>().position - mouseHeldOffset, ForceMode.Impulse);
                Debug.DrawLine(curHeld.GetComponent<Transform>().position + mouseHeldOffset, curHeld.GetComponent<Transform>().position + mouseHeldOffset + Vector3.one, Color.red, 10f);
                curHeld = null;
                return;
            }
        }
    }

    private void HandleHeldObject() {
        Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPos = cachedTrans.position + camRay.direction * heldDistance;
        curHeld.MovePosition(targetPos + mouseHeldOffset);

        // Calculate current mouse velocity
        mouseVelo = (Input.mousePosition - lastMousePos);
        lastMousePos = Input.mousePosition;

        heldDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 15f;
    }

    private void HandleObjectSelection() {
        curHover = null;

        Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(camRay, out rHit, REACH_DISTANCE)) {
            if(rHit.collider.CompareTag("pickup")) {
                curHover = rHit.collider.GetComponent<Rigidbody>();
            }
        }
    }
	
}
