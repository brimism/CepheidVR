using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    public enum Steps
    {
        start,// intro statement
        explain,
        lookAtUI, // have user press button on UI
        teachSwipeAway, // have user swipe right or left to move UI
        teachSwipeBack,
        showObj,
        pickUpObject,
        placeObject,
        
        nextRoom
    };

    string[] tutorial = {
        "Welcome to the Cepheid GeneXpert training simulation.",
        "Before beginning, I will explain the controls you will use.",
        "In front of you you will see a small panel with text on it. This will provide instructions throughout the simulation.",
        "To move the panel out of the way, hold the trigger on one of your controllers and swipe left or right before releasing.",
        "To return it to it's default state, repeat the same motion but in the opposite direction.",
        "Highlighted objects are items you can interact with. Try grabbing the object in front of you by touching it.",
        "Some objects have certain places they must be placed. For this object, place it on the shadowed area.",
        "The tutorial is complete. Please refer to your assistant panel for all future instruction."
    };

    string[] uiText =
    {
        "Have you read this?",
        "Swipe me away",
        "Swipe me back",
        "Grab the object",
        "Put the object down",
        "Continue to simulation?"
    };

    public SceneScriptableObj sceneSO;

    public Steps currentStep;

    public TextMeshProUGUI tutorialText, assistText;
    public GameObject rightHand;
    public GameObject assistUI;
    public GameObject testObj, placeholder;
    public GameObject confirmButton;
    public Animator blackPanel;

    private int stepIndex = 0;

    private IEnumerator Start()
    {
        currentStep = Steps.start;
        assistUI.SetActive(false);
        testObj.SetActive(false);
        placeholder.SetActive(false);
        tutorialText.text = tutorial[stepIndex];
        yield return new WaitForSeconds(4);
        currentStep = Steps.explain;
        tutorialText.text = tutorial[1];
        yield return new WaitForSeconds(4);
        currentStep = Steps.lookAtUI;

        sceneSO.ResetAll();
    }

    private void Update()
    {
        switch(currentStep)
        {
            case Steps.start:
                //StartCoroutine(WaitForTime(Steps.lookAtUI));
                //StartCoroutine("WaitForTime", Steps.explain);
                break;
            case Steps.explain:
                //tutorialText.text = tutorial[1];
                //StartCoroutine("WaitForTime", Steps.lookAtUI);
                break;
            case Steps.lookAtUI:
                tutorialText.text = tutorial[2];
                assistText.text = uiText[0];
                assistUI.SetActive(true);
                confirmButton.SetActive(true);
                break;
            case Steps.teachSwipeAway:
                tutorialText.text = tutorial[3];
                assistText.text = uiText[1];
                confirmButton.SetActive(false);
                break;
            case Steps.teachSwipeBack:
                tutorialText.text = tutorial[4];
                assistText.text = uiText[2];
                break;
            case Steps.showObj:
                tutorialText.text = tutorial[5];
                assistText.text = uiText[3];
                confirmButton.SetActive(false);
                testObj.SetActive(true);
                testObj.GetComponent<Outline>().enabled = true;
                break;
            case Steps.pickUpObject:
                tutorialText.text = tutorial[6];
                assistText.text = uiText[4];
                placeholder.SetActive(true);
                testObj.GetComponent<Outline>().enabled = false;
                //testObj.SetActive(true);
                break;
            case Steps.placeObject:
                placeholder.GetComponent<MeshRenderer>().enabled = false;
                currentStep = Steps.nextRoom;
                //testObj.GetComponent<Outline>().enabled = false;
                break;
            case Steps.nextRoom:
                tutorialText.text = tutorial[7];
                assistText.text = uiText[5];
                confirmButton.SetActive(true);
                break;
        }
    }

    

    private IEnumerable WaitForTime(Steps step)
    {
        while (true)
        {
            yield return new WaitForSeconds(4);
            currentStep = step;
            stepIndex++;
        }
    }

    public void teachSwipeAway()
    //temporarily skipping the swiping instructions, as the gesture control has been unreliable and anchoring the InstructionsKeeper to a single point is preferable
    {
        if (currentStep == Steps.lookAtUI)
            // currentStep = Steps.teachSwipeAway; 
            currentStep = Steps.showObj;
    }

    public void teachSwipeBack()
    {
        if (currentStep == Steps.teachSwipeAway)
            currentStep = Steps.teachSwipeBack;
    }

    public void showObj()
    {
        if (currentStep == Steps.teachSwipeBack)
            currentStep = Steps.showObj;
    }

    public void pickUpObject()
    {
        if (currentStep == Steps.showObj)
        {
            currentStep = Steps.pickUpObject;
            testObj.transform.position = rightHand.transform.position;
            testObj.transform.rotation = rightHand.transform.rotation;
            testObj.transform.parent = rightHand.transform;
            testObj.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    public void placeObject()
    {
        if (currentStep == Steps.pickUpObject)
        {
            currentStep = Steps.placeObject;
            testObj.transform.position = placeholder.transform.position;
            testObj.transform.rotation = placeholder.transform.rotation;
            testObj.transform.parent = placeholder.transform;
            testObj.transform.rotation = Quaternion.Euler(0, 90, 0);
            testObj.GetComponent<Collider>().enabled = false;
        }
    }

    public IEnumerator nextRoom()
    {
        if (currentStep == Steps.nextRoom)
        {
            blackPanel.SetTrigger("FadeOut");
            yield return new WaitForSeconds(1.1f);
            SceneManager.LoadScene("01_Lobby");
        }
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene2");

        while (!asyncLoad.isDone)
            yield return null;
    }
}
