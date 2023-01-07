using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform CharPos; 
    public float speed; 
    // Start is called before the first frame update
    void Start()
    {
        Vector3 Cam = CharPos.position; 
        Cam.y = transform.position.y; 
        transform.position = Cam; 
    }

    // Update is called once per frame
    void Update()   
    {
        Vector3 Cam = transform.position; 
        
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical"); 
        if(horizontal > 0 ){
            Cam.x += Time.deltaTime*speed; 
        }
        else if (horizontal < 0){
            Cam.x -= Time.deltaTime*speed; 
        }
        if(vertical > 0 ){
            Cam.z += Time.deltaTime*speed; 
        }
        else if (vertical < 0){
            Cam.z -= Time.deltaTime*speed; 
        }
        if(Input.GetKey(KeyCode.Q)){
            Cam.y -= Time.deltaTime*speed; 
        }
        else if(Input.GetKey(KeyCode.E)){
            Cam.y += Time.deltaTime*speed; 
        }
        
        transform.position = Cam; 
        
    }
}
