using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CapsuleCollider body;
    SphereCollider head;
    CharacterController controller;
    Camera camera;
    bool falling = false;
    //float fallTime = 0.0f;
    float verticalVelocity = 0.0f;
    static float gravity = 9.81f;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<CapsuleCollider>();
        transform.Find("Head").GetComponent<SphereCollider>();
        controller = GetComponent<CharacterController>();
        camera = FindObjectOfType<Camera>();
        


    }

    // Update is called once per frame
    void Update()
    {
        float r = Input.GetAxis("Horizontal");
        float f = Input.GetAxis("Vertical");
        float u = Input.GetAxis("Jump");
        bool vert = Input.GetKeyDown(KeyCode.Space);
        
        Vector3 delta = new Vector3(r, 0, f);
        Debug.DrawLine(transform.position, transform.position - new Vector3(0, 1.0f, 0), Color.red, Time.deltaTime, false);
        bool isGrounded = Physics.SphereCast(transform.position, 1.0f, Vector3.down, out _, 0.5f);
        
        if (!isGrounded) { falling = true; }
        else { falling = false; verticalVelocity = 0.0f; }
        if (falling)
        {
            verticalVelocity += -gravity * Time.deltaTime;
            //fallTime += Time.deltaTime;
        }
        if (vert && isGrounded)
        {
            verticalVelocity = 2.5f;
        }

        Debug.Log(isGrounded);
        delta.y = verticalVelocity;

        controller.Move(delta * 10 * Time.deltaTime);
        
        
        
    }
}
