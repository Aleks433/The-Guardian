using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectItemButton : MonoBehaviour
{
    public ItemSelector itemSelector;
    public BaseUseableItem item;
    public Color selectedColor;
    public Color notSelectedColor;
    public Image backgroundImage;
    public TextMeshProUGUI itemText;
    public bool selected = false;

    private void Start()
    {
        //button = GetComponent<Button>();
        backgroundImage = GetComponent<Image>();
    }

    public void DeselectItem()
    {
        SetBackgroundColor(notSelectedColor);
        selected = false;
    }

    private void SetBackgroundColor(Color color)
    {
        backgroundImage.color = color;
    }

    public void SelectItem()
    {
        if(selected)
        {
            itemSelector.DeselectItem();
            DeselectItem();
            return;
        }
        selected = true;
        itemSelector.SelectItem(item);
        SetBackgroundColor(selectedColor);
    }


}
