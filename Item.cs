using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float itemCost = 100f;
    public int rarity = 0;
    public float duration = 0f;
    public GameObject description;

    protected bool applied;
    protected PlayerController playerController;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        applied = false;
        description.SetActive(false);
    }

    public virtual void applyItem(PlayerController playerController)
    {
        this.playerController = playerController;
        applied = true;
    }

    public virtual void removeItem()
    {
        playerController.itemManager.removeItem(gameObject);
    }

    public void hideSprite()
    {
        sprite.enabled = false;
    }
   private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag== "Player"){
            description.SetActive(true);
        }
   }
   private void OnTriggerExit2D(Collider2D other) {
        if (other.tag== "Player"){
            description.SetActive(false);
        }
   }
}
