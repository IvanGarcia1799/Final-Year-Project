using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public bool lockCursor;
    public GameObject temp;
    public float yaw;
    public float pitch;
    public float mouseSensitivity = 10;
    public Vector2 boundaries = new Vector2(-40, 85);
    public float dst = 2;
    public float rotTime = .13f;
    public Vector3 rotSpeed;
    public Vector3 currentRot;
    public bool lockOn = false;
    public RaycastHit hit;
    public float distance;
    public float attackRadius = 2f;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        temp = GameObject.Find("Follower");
        target= temp.transform;
        cam = Camera.main;

        if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, boundaries.x, boundaries.y);
        bool backing = Input.GetKey(KeyCode.S);

        if (Input.GetMouseButtonDown(0) && !lockOn)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.name == "Enemy")
                {
                    lockOn = true;
                }
            }
        }

        if( Input.GetKey(KeyCode.Space) && lockOn)
        {
            distance = Vector3.Distance(hit.transform.position, target.position);

            if(distance > attackRadius)
            {
                lockOn = false;
            }
            
        }

        currentRot = Vector3.SmoothDamp(currentRot, new Vector3(pitch, yaw), ref rotSpeed, rotTime);

        transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * dst, Time.deltaTime*6f);

        if(!lockOn || !hit.transform)
        {
            transform.eulerAngles = currentRot;
            lockOn = false;
        } else
        {
            cam.transform.LookAt(hit.collider.transform.position);
        }

        if(backing && !lockOn){
            currentRot = Vector3.SmoothDamp(currentRot, new Vector3(pitch, yaw), ref rotSpeed, 0.001f);
            transform.eulerAngles = currentRot;
        }
    }
}