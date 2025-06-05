using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour
{
    public BaseUseableItem selectedItem;
    bool isItemSelected;
    public BaseHero selectedHero;
    bool isHeroSelected;
    public Button useButton;
    public List<SelectHeroButton> heroButtons = new List<SelectHeroButton>();
    public List<SelectItemButton> itemButtons = new List<SelectItemButton>();

    private void Start()
    {
    }
    private void Update()
    {
        if (isItemSelected && isHeroSelected)
        {
            useButton.interactable = true;
        }
        else
        {
            useButton.interactable = false;
        }
    }

    public void SelectHero(BaseHero hero)
    {
        selectedHero = hero;
        isHeroSelected = true;
        foreach (SelectHeroButton button in heroButtons)
        {
            if(button.hero != hero)
            {
                button.DeselectHero();
            }
        }
    }
    public void DeselectHero()
    {
        selectedHero = null;
        isHeroSelected = false;
    }

    public void SelectItem(BaseUseableItem item)
    {
        selectedItem = item;
        isItemSelected = true;
        foreach (SelectItemButton button in itemButtons)
        {
            if(button.item != item)
            {
                button.DeselectItem();
            }

        }
    }
    public void DeselectItem()
    {
        selectedItem = null;
        isItemSelected = false;
    }
    public void UseItemOnHero()
    {
        itemButtons.Clear();
        heroButtons.Clear();
        GameObject.Find("GameManager").GetComponent<GameManager>().UseItemOnHero(selectedItem, selectedHero);
    }
}
