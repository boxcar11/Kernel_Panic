using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    
    private float verticalAxis;
    private float horizontalAxis;

    private Inventory inventory;
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        inventory = Inventory.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        verticalAxis = (Input.GetAxis("Vertical"));
        horizontalAxis = (Input.GetAxis("Horizontal"));

        transform.Translate(new Vector3(horizontalAxis, verticalAxis, 0)* moveSpeed * Time.deltaTime);

        // Set animation direction
        if (Mathf.Abs(verticalAxis) >= 0.5f || Mathf.Abs(horizontalAxis) >= 0.5f)
        {
            animator.SetBool("walking", true);
            if (verticalAxis > .5f)
            {
                //animator.SetInteger("Direction", 2);
                animator.SetFloat("Blend", 0.0f);
            }
            else if (verticalAxis < -0.5f)
            {
                //animator.SetInteger("Direction", 0);
                animator.SetFloat("Blend", 0.34f);
            }
            else
            {
                if (horizontalAxis > .5f)
                {
                    //animator.SetInteger("Direction", 1);
                    animator.SetFloat("Blend", 1f);
                }
                if (horizontalAxis < -.5f)
                {
                    //animator.SetInteger("Direction", 3);
                    animator.SetFloat("Blend", 0.67f);
                }
            }
        }        
        else
        {
            animator.SetBool("walking", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pickup")
        {
            Item item = collision.GetComponent<Item>();
            inventory.AddMaterial(item.materials.materialName, item.materials.count);
            Destroy(collision.gameObject);
        }
    }
}
