using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    public Transform followObject;

    public float speed;
    Vector2 direction;

    const float EPSILON = 0.1f;

    void Update(){
        
        direction = new Vector3(followObject.position.x - transform.position.x, 0, 0);

        if((transform.position - followObject.position).magnitude > EPSILON){
            transform.Translate(direction * Time.deltaTime * speed);
        }
    }
}
