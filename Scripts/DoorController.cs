using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;

    public Collider2D doorCollider;
    public Collider2D triggerCollider;

    private SpriteRenderer doorSprite;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        doorSprite = gameObject.GetComponent<SpriteRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            doorCollider.enabled = false;
            doorSprite.sprite = openSprite;
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            doorCollider.enabled = true;
            doorSprite.sprite = closedSprite;
            audioSource.Play();
        }
    }
}
