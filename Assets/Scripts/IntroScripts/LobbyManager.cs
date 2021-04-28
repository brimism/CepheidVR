using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{

    enum Steps {
        start,//receptionist talks, wait a few seconds then highlight chart
        grabChart, // player interacts with chart and UI prompts them to move on
        readChart,
        nextRoom
    };

    string[] uiText = {
        "Pick up the clipboard",
        "Take a moment to read its contents",
        "Go to exam room?"
    };

    public GameObject rightHand, clipboard;
    public TextMeshProUGUI ui;
    public GameObject confirmButton;
    public Animator blackPanel;
    
    
    private Steps currentStep;

    private float waitTime = 4, lastTimeChecked;

    private void Start()
    {
        currentStep = Steps.start;
        confirmButton.SetActive(false);
    }

    private void Update()
    {
        switch (currentStep)
        {
            case Steps.start:
                ui.text = uiText[0];
                clipboard.GetComponent<Outline>().enabled = true;
                break;
            case Steps.grabChart:
                ui.text = uiText[1];
                lastTimeChecked = Time.time;
                clipboard.GetComponent<Outline>().enabled = false;
                currentStep = Steps.readChart;
                break;
            case Steps.readChart:
                TimeUp();
                break;
            case Steps.nextRoom:
                ui.text = uiText[2];
                confirmButton.SetActive(true);
                break;
        }
    }

    public void grabbedBoard()
    {
        if(currentStep == Steps.start)
        {
            currentStep = Steps.grabChart;
            clipboard.transform.position = rightHand.transform.position;
            clipboard.transform.rotation = rightHand.transform.rotation;
            clipboard.transform.parent = rightHand.transform;
            clipboard.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    public IEnumerator nextRoom()
    {
        if (currentStep == Steps.nextRoom)
        {
            blackPanel.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.1f);
            SceneManager.LoadScene("Rig_NewSampleRoom");
        }
    }

    private void TimeUp()
    {
        if (Time.time - lastTimeChecked > waitTime)
            currentStep = Steps.nextRoom;
    }

}
