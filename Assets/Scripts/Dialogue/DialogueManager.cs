using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class DialogueManager : MonoBehaviour
{
    public Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTitleText;
    public TextMeshProUGUI dialogueContentText;
    public Dialogue currentDialogue;

    private void Start()
    {
        dialoguePanel.SetActive(false); 
    }

    public void AddDialogue(Dialogue dialogue)
    {
        dialogueQueue.Enqueue(dialogue);
    }
    private void Update()
    {
        if (currentDialogue == null)
        {
            if(dialogueQueue.Count == 0)
            {
                CloseDialogue();
                return;
            }
            currentDialogue = dialogueQueue.Dequeue();
            AdvanceDialogue();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AdvanceDialogue();
        }
       
    }
    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }


    IEnumerator TypeDialogue(string dialogueContent)
    {
        dialogueTitleText.text = currentDialogue.dialogueTitle;
        dialogueContentText.text = "";
        foreach(char dialogue_chr in dialogueContent.ToCharArray())
        {
            dialogueContentText.text += dialogue_chr;
            yield return new WaitForSeconds(currentDialogue.dialogueSpeed);
        }
    }
    public void AdvanceDialogue()
    {
        dialoguePanel.SetActive(true);
        if(currentDialogue.dialogueContent.Count == 0)
        {
            StopAllCoroutines();
            currentDialogue = null;
            return;
        }
        dialogueTitleText.text = currentDialogue.dialogueTitle;
        StopAllCoroutines();
        StartCoroutine(TypeDialogue(currentDialogue.dialogueContent.Dequeue()));
    }
}
