using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCotroller : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = this.GetComponent<Camera>();
        camera.orthographicSize = 4;
        transform.parent = player.transform;
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
       // this.transform.position = player.transform.position;
    }
}
