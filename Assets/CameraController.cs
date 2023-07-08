using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject gridController;

    public float strength = 0.4f;
    public float size;
    public float radius;
    // Start is called before the first frame update
    void Start() {
        size = gridController.GetComponent<Controller>().size/2;
        radius = transform.GetComponent<SphereCollider>().radius = size*1.6f;
        transform.position = new Vector3(0, size, 0);
        transform.GetChild(0).transform.Translate(Vector3.back*radius, Space.Self);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(Vector3.down*strength);
        else
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(Vector3.up * strength);
        } 
    }

}
