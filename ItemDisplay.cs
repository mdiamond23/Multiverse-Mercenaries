using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{

// https://answers.unity.com/questions/1593158/how-to-set-sprite-size-in-pixels.html

    public GameObject ItemImage;
    public GameObject Positioner;
    public ArrayList items = new ArrayList();

    public Image[] itemImages;

    public void addItem(SpriteRenderer newItemImage){
        // TODO look into this later
        // int Offset = itemImages.Count * 100;
        // Vector3 position = Positioner.transform.position;
        // position.x += Offset;
        // GameObject newItem = Instantiate(ItemImage, position, Quaternion.identity);
        // newItem.GetComponent<SpriteRenderer>().sprite = newItemImage.sprite;
        // newItem.transform.parent = Positioner.transform;
        // itemImages.Add(newItem);

        int imageNumber = items.Count; 

        itemImages[imageNumber].sprite = newItemImage.sprite;
        itemImages[imageNumber].gameObject.SetActive(true);
        items.Add(newItemImage);
    }
}
