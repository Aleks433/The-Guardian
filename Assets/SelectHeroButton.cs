using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : MonoBehaviour
{
    public BaseHero hero;
    public ItemSelector itemSelector;
    public Button button;
    public Image backgroundImage;
    public Color notSelectedColor;
    public Color selectedColor;
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroHPText;
    public TextMeshProUGUI heroMPText;
    public bool selected = false;

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    public void SelectHero()
    {

        if(selected)
        {
            itemSelector.DeselectHero();
            DeselectHero();
            return;
        }
        selected = true;
        itemSelector.SelectHero(hero);
        SetBackgroundColor(selectedColor);
    }

    public void DeselectHero()
    {
        SetBackgroundColor(notSelectedColor);
        selected = false;
    }

    private void SetBackgroundColor(Color color)
    {
        backgroundImage.color = color;
    }

}
