using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockbackForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            Rigidbody playerRigidBody = collision.gameObject.GetComponent<Rigidbody>();

            Vector3 knockbackDirection = (collision.gameObject.transform.position - transform.position).normalized;

            playerRigidBody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

            Debug.Log("Knockback");
        }
    }
}
