using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject gridController;

    public float strengthX = 0.4f,strengthZ=0.1f;
    public float size;
    public float radius;
    // Start is called before the first frame update
    void Start() {
        size = gridController.GetComponent<MainController>().size/2;
        radius = transform.GetComponent<SphereCollider>().radius = size*1.6f;
        transform.position = new Vector3(0, size, 0);
        transform.GetChild(0).transform.Translate(Vector3.back*radius, Space.Self);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(Vector3.down * strengthX);
        else if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.up * strengthX);
        else if (Input.GetKey(KeyCode.UpArrow))
            transform.GetChild(0).transform.Translate(Vector3.forward * strengthZ, Space.Self);    
        else if (Input.GetKey(KeyCode.DownArrow))
            transform.GetChild(0).transform.Translate(Vector3.back * strengthZ, Space.Self);


    }

}
