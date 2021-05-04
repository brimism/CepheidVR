using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SampleRoomScript : MonoBehaviour
{
    /*As the experience progresses, the state of the step completion is kept organized using the enum
    The basic structure consists of:
    1. Initialize values (renderers, colliders, outlines, etc) appropriately when entering a new step
    2. Receive feedback on a user action (a separate Interaction script broadcasts a message upwards on a trigger) that triggers a function
    3. The triggered function takes care of any one-off event that needs to occur and updates the current state to the next step in the enum
    4. Repeat
    */
    public enum Steps 
    {
        start,
        openPack, openSwab, pickUpSwab, insertSwabLeft, applyPressureLeft, rotateSwabLeft, swapSwab,//taking the sample
        insertSwabRight, applyPressureRight, rotateSwabRight, //second nostril
        pickupTube, breakSwab, disposeSwab, pickUpTubeCap, closeTube, //depositing the sample
        disposeGloves, door, //move to lab

        //for returning to this room
        readResults, giveResults,
        end
    };
    [Header("State")]
    public Steps current;//keeps track of the current step in the switch statement

    [Header("SceneManager")]
    public SceneScriptableObj sceneOS;

    [Header("NonDiegeticUI")]

    //transform of instruction canvas
    public Canvas instructionsCanvas;//canvas of instructions over the counter in the sample taking room
    public TextMeshProUGUI instructionsText;//text element of instructionsCanvas
    public GameObject confirmButton;//to teleport to next room

    private string[] instructions = {//array of instructions given to the player
        "Please put on gloves.",//0
        "Open the vacuum-sealed pack and unwrap the swab.",//1
        "Unwrap the swab",//2
        "Pick up the swab with your right hand, at the middle of the shaft on the scoreline. Take care to not touch the tip of the swab to any surface.",//3
        "Insert the swab into the patient's nostril, no more than 1-1.5cm.",//4
        "Apply pressure with a finger to the outside of the nostril.",//5
        "Rotate swab against the inside of the nostril for 3 seconds.",//6
        "Swap hands.",//7
        "Repeat: Insert the swab into the patient's nostril, no more than 1-1.5cm.",//8
        "Repeat: Apply pressure with a finger to the outside of the nostril.",//9
        "Repeat: Rotate swab against the inside of the nostril for 3 seconds.",//10
        "Pick up the test tube.",//11
        "Insert the swab into the transport medium, and break the swab against the side of the tube at the scoreline.",//12
        "Dispose the swab.",//13
        "Replace the cap on the tube and close tightly.",//14
        "Please remove your gloves.",//15
        "Bring your sample to the lab.",//16

        "Take a moment to read the patient's results.",//17
        "Give the perscription to the patient.",//18
        "End training experience?"//19
    };
    
    public AudioSource instructionSource;//audio source that plays instruction audio
    public AudioClip[] instructionAudio;//array of instruction audio clips 

    public enum instructionText
    {
        start,
        openPack, pickUpSwab, insertSwabLeft, applyPressureLeft, rotateSwabLeft, swapSwab,//taking the sample
        insertSwabRight, applyPressureRight, rotateSwabRight, //second nostril
        pickupTube, breakSwab, disposeSwab, pickUpTubeCap, closeTube, //depositing the sample
        disposeGloves, door, //move to lab
        
        //for giving results
        readResults, giveResults,
        end
    }

    public instructionText currentInstruction;

    
    [Header("Interactibles")]   
    public GameObject gloveBox;//glove box in the sample room
    public GameObject vacuumPack;//pack carrying the test tube and swab

    public GameObject swabPack;//individual wrapper on the swab
    public GameObject swabTop;
    public GameObject swabBottom;

    public GameObject leftNostril;//"left" nostril (player's left, not the patient's actual left nostril)
    public GameObject rightNostril;//likewise "right" nostril
    public GameObject leftPressure;//the place where you must put pressure to swab the left nostril
    public GameObject rightPressure;//likewise where you must put pressure to swab the right nostril

    public GameObject testTube;//tube for placing sample
    public GameObject tubeCap;//cap of the test tube
    public GameObject tubeSample;//broken swab sample inside the testTube

    public GameObject trashCan;//trash can in sample room
    public GameObject trashCanLid;//lid of the trash can
    public GameObject giveToPatientCollider;//radius surrounding patient to give Rx
    public GameObject testResults;

    [Header("PCElements")]
    public GameObject pc;//player controller
    public GameObject rightHand;//player's right hand transform
    public GameObject leftHand;//player's left hand transform
    public GameObject rightHandModel;//player's visible right hand
    public GameObject leftHandModel;//player's visible left hand

    public Mesh defaultHand;//default open hand
    public Mesh swabHand;//hand position while holding the swab
    public Mesh closedHand;//hand position while holding the test tube
    public Mesh pointHand;//hand position while pointing at a screen

    [Header("Animation")]
    public Animator characterAnimator;
    public Animator bigPacketAnimator;
    public Animator swabPacketAnimator;

    [Header("SnapTransforms")]
    public Transform leftSwabTransform;//transform that the test swab snaps to when swabbing the left nostril
    public Transform leftPressureTransform;//transform that the finger snaps to when swabbing the left nostril
    public Transform rightSwabTransform;//transform that the test swab snaps to when swabbing the right nostril
    public Transform rightPressureTransform;//transform that the finger snaps to when swabbing the right nostril

    public Transform sampleRoomTransform;//starting position in the sample-taking room
 
    [Header("DiegeticSounds")]
    public AudioSource audioSource;//the sound source, part of the player
    public AudioClip glovesOn;//sound of putting gloves on
    public AudioClip glovesOff;//sound of taking gloves off
    public AudioClip vacuumPackSound;//sound of opening the vacuum pack
    public AudioClip tubeOpenSound;//sound of opening the test tube
    public AudioClip swabSnap;//sound of swab snapping in half
    public AudioClip tubeCloseSound;//sound of closing the test tube
    public AudioClip trashSound;//sound of something going into the trash

    [Header("Materials")]

    public Material blue;
    public Material glass;
    public Material pipetteColor;

    [Header("BooleansAndMisc")]
    public int gloved; //how many times the hands have been gloved or ungloved in the current state
    public Image panel;//the black panel of the fade out canvas
    
        //this manager uses a switch statement to navigate between states. When the hands in the scene interact with objects, they send broadcasts up to the manager,
    //which triggers the methods below that update the states
    void Start()
    {
        pc.transform.position = sampleRoomTransform.position;
        pc.transform.rotation = sampleRoomTransform.rotation;
        gloved = 0;
        //set fade canvas to black and invisible
        panel.color = new Color(0, 0, 0, 1);
        panel.CrossFadeAlpha(0.0f, 0f, true);

        //all colliders disabled (will enable relevant colliders in Steps.start)
        Collider[] colliders = this.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        //all outlines disabled (will enable relevant outlines in Steps.start)
        Outline[] outlines = this.GetComponentsInChildren<Outline>();
        foreach (Outline o in outlines)
            o.enabled = false;

        //check what state the room is going to be in
        switch (sceneOS.examRoomState)
        {

            //swab patient
            case 0:
                current = Steps.start; //assign current step to first
                currentInstruction = instructionText.start;
                instructionSource.PlayDelayed(1.5f);
                break;

            //give results
            case 1:
                current = Steps.readResults;
                currentInstruction = instructionText.readResults;
                testResults.SetActive(true);
                //enable clipboard, Rx, etc
                break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (current)
        {
            case Steps.start: //player must put on gloves

                //some componenents not visible
                testTube.GetComponent<MeshRenderer>().enabled = true;
                tubeSample.GetComponent<MeshRenderer>().enabled = false;
                rightPressure.GetComponent<MeshRenderer>().enabled = false;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                //enable glove box and hands
                gloveBox.GetComponent<Collider>().enabled = true;
                gloveBox.GetComponent<Outline>().enabled = true;

                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;


                instructionsText.text = instructions[0];

                break;
            case Steps.openPack://User has put on gloves, player must open the vacuum pack to move to next step
                gloveBox.GetComponent<Collider>().enabled = false;
                gloveBox.GetComponent<Outline>().enabled = false;

                vacuumPack.GetComponent<Collider>().enabled = true;
                vacuumPack.GetComponentInChildren<Outline>().enabled = true;

                instructionsText.text = instructions[1];

                break;
            case Steps.openSwab://need to unwrap the swab
                vacuumPack.GetComponent<Collider>().enabled = false;

                swabPack.GetComponent<Collider>().enabled = true;
                swabPack.GetComponentInChildren<Outline>().enabled = true;

                instructionsText.text = instructions[2];
                break;
            case Steps.pickUpSwab://pack now opened, player must pick up the swab with right hand

                swabPack.GetComponent<Collider>().enabled = false;
                swabPack.GetComponentInChildren<Outline>().enabled = false;

                testTube.GetComponent<MeshRenderer>().enabled = true;

                swabBottom.GetComponent<CapsuleCollider>().enabled = true;
                swabBottom.GetComponent<Outline>().enabled = true;

                instructionsText.text = instructions[3];

                leftHand.GetComponent<Collider>().enabled = false;
                break;
            case Steps.insertSwabLeft://player has picked up swab, must insert swab into patient's left nostril

                swabBottom.GetComponent<CapsuleCollider>().enabled = false;
                swabBottom.GetComponent<Outline>().enabled = false;

                swabTop.GetComponent<CapsuleCollider>().enabled = true;
                swabTop.GetComponentInChildren<Outline>().enabled = true;

                leftNostril.GetComponent<Outline>().enabled = true;
                leftNostril.GetComponent<Collider>().enabled = true;

                instructionsText.text = instructions[4];

                rightHand.GetComponent<Collider>().enabled = false;
                rightHandModel.GetComponent<MeshFilter>().mesh = swabHand;
                break;
            case Steps.applyPressureLeft://player has inserted swab into nostril left, swab is now frozen(?), player must apply pressure on the nostril with their other hand

                swabTop.GetComponent<CapsuleCollider>().enabled = false;
                swabTop.GetComponentInChildren<Outline>().enabled = false;

                leftNostril.GetComponent<Outline>().enabled = false;

                rightHand.transform.position = leftSwabTransform.position;
                rightHand.transform.rotation = leftSwabTransform.rotation * Quaternion.Euler(0, 180, 0);

                leftHand.GetComponent<Collider>().enabled = true;
                leftHandModel.GetComponent<MeshFilter>().mesh = pointHand;

                leftPressure.GetComponent<Collider>().enabled = true;
                leftPressure.GetComponent<MeshRenderer>().enabled = true;
                leftPressure.GetComponent<Outline>().enabled = true;

                instructionsText.text = instructions[5];
                break;
            case Steps.rotateSwabLeft://player has applied pressure, must rotate swab in nostril for 3 seconds
                rightHand.GetComponent<Outline>().enabled = true;

                leftHand.transform.position = leftPressureTransform.position;
                leftHand.transform.rotation = leftPressureTransform.rotation;

                leftPressure.GetComponent<Outline>().enabled = false;

                rightHand.transform.position = leftSwabTransform.position;
                rightHand.transform.eulerAngles = new Vector3(leftSwabTransform.rotation.x, rightHand.transform.eulerAngles.y, leftSwabTransform.rotation.z);

                leftPressure.GetComponent<Outline>().enabled = false;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                break;
            case Steps.swapSwab://player has taken the sample from the first nostril, must swap hands to swab the second nostril
                leftHand.GetComponent<Outline>().enabled = true;
                leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                rightHand.GetComponent<Outline>().enabled = false;
                rightHand.GetComponent<Collider>().enabled = false;

                swabBottom.GetComponent<CapsuleCollider>().enabled = true;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                instructionsText.text = instructions[7];
                break;
            case Steps.insertSwabRight://player has swapped hands, must insert swab into the second nostril

                swabBottom.GetComponent<CapsuleCollider>().enabled = false;
                swabBottom.GetComponent<Outline>().enabled = false;

                swabTop.GetComponent<CapsuleCollider>().enabled = true;
                swabTop.GetComponentInChildren<Outline>().enabled = true;

                leftHand.GetComponent<Outline>().enabled = false;
                leftHandModel.GetComponent<MeshFilter>().mesh = swabHand;

                rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                rightNostril.GetComponent<Outline>().enabled = true;
                rightNostril.GetComponent<Collider>().enabled = true;

                instructionsText.text = instructions[8];
                break;
            case Steps.applyPressureRight://player has inserted swab, must apply pressure outside the nostril

                swabTop.GetComponent<CapsuleCollider>().enabled = false;

                rightNostril.GetComponent<Outline>().enabled = false;

                leftHand.GetComponent<Collider>().enabled = false;
                leftHand.transform.position = rightSwabTransform.position;
                leftHand.transform.rotation = rightSwabTransform.rotation;// *Quaternion.Euler(0,180,0);

                rightHand.GetComponent<Collider>().enabled = true;
                rightHandModel.GetComponent<MeshFilter>().mesh = pointHand;

                rightPressure.GetComponent<Collider>().enabled = true;
                rightPressure.GetComponent<MeshRenderer>().enabled = true;
                rightPressure.GetComponent<Outline>().enabled = true;

                instructionsText.text = instructions[9];
                break;
            case Steps.rotateSwabRight://player has applied pressure, must rotate the swab for 3 seconds
                leftHand.GetComponent<Outline>().enabled = true;
                leftHand.GetComponent<Collider>().enabled = false;
                leftHand.transform.eulerAngles = new Vector3(rightSwabTransform.rotation.x, leftHand.transform.eulerAngles.y, rightSwabTransform.rotation.z);
                leftHand.transform.position = rightSwabTransform.position;

                rightHand.transform.position = rightPressureTransform.position;
                rightHand.transform.rotation = rightPressureTransform.rotation;

                rightPressure.GetComponent<Outline>().enabled = false;
                rightPressure.GetComponent<MeshRenderer>().enabled = false;

                //pc.GetComponent<PlayerController>().SendHaptics(true, 0.75f, .1f);

                break;
            case Steps.pickupTube://player has successfully gathered the sample, must pick up the test tube
                instructionsText.text = instructions[11];

                swabTop.GetComponentInChildren<Outline>().enabled = false;
                swabTop.GetComponent<CapsuleCollider>().enabled = false;

                testTube.GetComponent<Outline>().enabled = true;
                testTube.GetComponent<CapsuleCollider>().enabled = true;

                leftHand.GetComponent<Outline>().enabled = false;
                leftHand.GetComponent<Collider>().enabled = false;

                rightHand.GetComponent<Collider>().enabled = true;
                rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
                break;
            case Steps.breakSwab://player has picked up the test tube (which automatically opens), must break the swab inside the tube
                //swab.GetComponent<Outline>().enabled = true;
                //swab.GetComponent<BoxCollider>().enabled = true;

                swabTop.GetComponentInChildren<Outline>().enabled = true;
                swabTop.GetComponent<CapsuleCollider>().enabled = true;

                rightHand.GetComponent<Collider>().enabled = false;
                rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;

                instructionsText.text = instructions[12];
                break;
            case Steps.disposeSwab://player has broken the swab in the tube, must dispose of the swab
                instructionsText.text = instructions[13];
                testTube.GetComponent<CapsuleCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = false;
                tubeSample.GetComponent<MeshRenderer>().enabled = true;//broken swab tip appears in the tube

                //swab.GetComponent<BoxCollider>().enabled = false;
                //swab.GetComponent<CapsuleCollider>().enabled = true;

                swabTop.GetComponentInChildren<MeshRenderer>().enabled = false;
                swabBottom.GetComponent<CapsuleCollider>().enabled = true;

                trashCan.GetComponent<Collider>().enabled = true;
                trashCan.GetComponent<Outline>().enabled = true;

                trashCanLid.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                break;
            case Steps.pickUpTubeCap://player has disposed of the swab, must pick up the cap
                trashCan.GetComponent<Collider>().enabled = false;
                trashCan.GetComponent<Outline>().enabled = false;

                trashCanLid.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                testTube.GetComponent<CapsuleCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = true;

                tubeCap.GetComponent<Collider>().enabled = true;
                tubeCap.GetComponent<Outline>().enabled = true;

                leftHand.GetComponent<Collider>().enabled = true;
                leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                instructionsText.text = instructions[14];
                break;
            case Steps.closeTube://player has picked up the cap, must close the tube

                leftHand.GetComponent<Collider>().enabled = false;
                leftHandModel.GetComponent<MeshFilter>().mesh = closedHand;
                testTube.GetComponent<BoxCollider>().enabled = true;
                break;
            case Steps.disposeGloves://player has closed the tube, must dispose of the gloves
                //Debug.Log("glove1");
                trashCan.GetComponent<Collider>().enabled = true;
                trashCan.GetComponent<Outline>().enabled = true;

                trashCanLid.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;

                testTube.GetComponent<CapsuleCollider>().enabled = true;
                testTube.GetComponent<BoxCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = false;
                tubeCap.GetComponent<Collider>().enabled = false;
                tubeCap.GetComponent<Outline>().enabled = false;

                instructionsText.text = instructions[15];
                break;
            case Steps.door:
                trashCan.GetComponent<Collider>().enabled = false;
                trashCan.GetComponent<Outline>().enabled = false;

                trashCanLid.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                //doorKnob.GetComponent<Collider>().enabled = true;
                //doorKnob.GetComponent<Outline>().enabled = true;
                confirmButton.SetActive(true);

                instructionsText.text = instructions[16];
                break;

            case Steps.readResults:
                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;
                instructionsText.text = instructions[17];
                StartCoroutine(WaitToRead());
                break;
            case Steps.giveResults:
                //activate collider to give patient stuff, then switch to end
                giveToPatientCollider.GetComponent<Collider>().enabled = true;
                instructionsText.text = instructions[18];
                break;
            case Steps.end:
                instructionsText.text = instructions[19];
                break;

            default:
                //Debug.Log("NOTHING");
                break;
        }
    }

    public void Gloves()//called by the hands when they contact a glove box or a trash can
    {
        //Debug.LogWarning("Called Gloves method");
        if (gloved == 0)//neither hands gloved/ungloved
        {
            gloved++;//add counter
            if (current == Steps.start)
            {
                audioSource.PlayOneShot(glovesOn);
            }
            else if (current == Steps.disposeGloves)
            {
                audioSource.PlayOneShot(glovesOff);
            }
        }
        else if (gloved == 1)//one hand gloved
        {
            gloved = 0;
            //next step
            if (current == Steps.start)
            {
                audioSource.PlayOneShot(glovesOn);

                instructionSource.Stop();
                instructionSource.PlayOneShot(instructionAudio[1]);

                current = Steps.openPack;
            }
            else if (current == Steps.disposeGloves)
            {
                audioSource.PlayOneShot(glovesOff);
                instructionSource.Stop();
                instructionSource.PlayOneShot(instructionAudio[15]);
                current = Steps.door;
            }
        }
    }

        public void pickUpSwab()
    {
        if (current == Steps.pickUpSwab)
        {
            GameObject swabParent = swabBottom.transform.parent.gameObject;
            swabParent.transform.parent = rightHand.transform;
            swabParent.transform.position = rightHand.transform.position;
            swabParent.transform.localPosition += new Vector3(0.0f, 0.07f, 0f);
            swabParent.transform.rotation = rightHand.transform.rotation * Quaternion.Euler(15, 0, 180);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[3]);

            characterAnimator.SetTrigger("Transition");
            current = Steps.insertSwabLeft;
        }
        else if (current == Steps.swapSwab)
        {
            GameObject swabParent = swabBottom.transform.parent.gameObject;
            swabParent.transform.parent = leftHand.transform;
            swabParent.transform.position = leftHand.transform.position;
            swabParent.transform.localPosition += new Vector3(0.0f, 0.07f, 0f);
            swabParent.transform.rotation = leftHand.transform.rotation * Quaternion.Euler(15, 0, 180);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[7]);

            current = Steps.insertSwabRight;
        }
    }

    public void vacuumPackOpen()
    {
        audioSource.PlayOneShot(vacuumPackSound);
        bigPacketAnimator.SetTrigger("touch");
        instructionSource.Stop();
        instructionSource.PlayOneShot(instructionAudio[2]);

        current = Steps.openSwab;
    }

    public void swabPackOpen()
    {
        audioSource.PlayOneShot(vacuumPackSound);
        swabPacketAnimator.SetTrigger("touch");
        instructionSource.Stop();
        //instructionSource.PlayOneShot(instructionAudio[2]);

        current = Steps.pickUpSwab;
    }

    public void insertSwab()
    {
        if (current == Steps.insertSwabLeft)
        {
            //leftSwabTransform = leftNostril.transform;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[4]);

            current = Steps.applyPressureLeft;
        }
        else if (current == Steps.insertSwabRight)
        {
            //rightSwabTransform = rightNostril.transform;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[8]);

            current = Steps.applyPressureRight;
        }
    }

    public void rotateSwab()
    {
        if (current == Steps.applyPressureLeft)
        {
            //leftPressureTransform = leftPressure.transform;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[5]);

            current = Steps.rotateSwabLeft;
            Rotate();
        }
        else if (current == Steps.applyPressureRight)
        {
            //rightPressureTransform = rightPressure.transform;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[9]);

            current = Steps.rotateSwabRight;
            Rotate();
        }
    }

    public void pickUpTube(string handtag)
    {
        if (current == Steps.pickupTube)
        {
            audioSource.PlayOneShot(tubeOpenSound);
            testTube.transform.position = rightHand.transform.position;
            testTube.transform.rotation = rightHand.transform.rotation;
            testTube.transform.parent = rightHand.transform;
            testTube.transform.localPosition += new Vector3(0f, 0.07f, 0f);

            rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[11]);

            current = Steps.breakSwab;
        }
        else
        {
            if (handtag == "LeftHand")
            {
                testTube.transform.position = leftHand.transform.position;
                testTube.transform.rotation = leftHand.transform.rotation;
                testTube.transform.parent = leftHand.transform;
                testTube.transform.localPosition += new Vector3(0f, 0.07f, 0f);

                leftHandModel.GetComponent<MeshFilter>().mesh = closedHand;
                rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
            }
            else//right
            {
                testTube.transform.position = rightHand.transform.position;
                testTube.transform.rotation = rightHand.transform.rotation;
                testTube.transform.parent = rightHand.transform;
                testTube.transform.localPosition += new Vector3(0f, 0.07f, 0f);
                //testTube.transform.rotation = Quaternion.Euler(0, 0, 0);

                rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;
                leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
            }
        }
    }

    public void breakSwab()
    {
        pc.GetComponent<AudioSource>().PlayOneShot(swabSnap);

        swabTop.SetActive(false);

        instructionSource.Stop();
        instructionSource.PlayOneShot(instructionAudio[12]);

        current = Steps.disposeSwab;
    }

    public void disposeSwab()
    {
        pc.GetComponent<AudioSource>().PlayOneShot(trashSound);
        leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

        instructionSource.Stop();
        instructionSource.PlayOneShot(instructionAudio[13]);

        current = Steps.pickUpTubeCap;
    }

    public void pickUpTubeCap()
    {
        if (current == Steps.pickUpTubeCap)
        {
            tubeCap.transform.position = leftHand.transform.position;
            tubeCap.transform.rotation = leftHand.transform.rotation;
            tubeCap.transform.parent = leftHand.transform;

            current = Steps.closeTube;
        }
    }

    public void closeTube()
    {
        if (current == Steps.closeTube)
        {
            audioSource.PlayOneShot(tubeCloseSound);
            tubeCap.transform.position = testTube.transform.position;
            tubeCap.transform.rotation = testTube.transform.rotation;
            tubeCap.gameObject.transform.parent = testTube.gameObject.transform;
            //tubeCap.transform.localPosition = new Vector3(0f, 1f, 0f);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[14]);

            current = Steps.disposeGloves;
        }
    }
    IEnumerator Rotate()
    {
        Debug.Log("Coroutine Started");
        yield return new WaitForSeconds(0.3f);
        if (current == Steps.rotateSwabLeft)
        {
            instructionsText.text = "Rotate swab against the inside of the nostril for 3 seconds.";
            yield return new WaitForSeconds(3f);
            instructionsText.text = "Rotate swab against the inside of the nostril for 2 seconds.";
            yield return new WaitForSeconds(1f);
            instructionsText.text = "Rotate swab against the inside of the nostril for 1 seconds.";
            yield return new WaitForSeconds(1f);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[6]);

            current = Steps.swapSwab;
        }
        else if (current == Steps.rotateSwabRight)
        {
            instructionsText.text = "Repeat: Rotate swab against the inside of the nostril for 3 seconds.";
            yield return new WaitForSeconds(3f);
            instructionsText.text = "Repeat: Rotate swab against the inside of the nostril for 2 seconds.";
            yield return new WaitForSeconds(1f);
            instructionsText.text = "Repeat: Rotate swab against the inside of the nostril for 1 seconds.";
            yield return new WaitForSeconds(1f);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[10]);

            current = Steps.pickupTube;
        }

    }

    public void giveResults()
    {
        if (current == Steps.giveResults)
        {
            testResults.SetActive(false);
            confirmButton.SetActive(true);
            current = Steps.end;
        }
    }

    IEnumerator FadeOut()
    {
        Debug.Log("Coroutine Started");
        //if (current == Steps.door)
        //{
        //doorKnob.GetComponent<Outline>().enabled = false;
        panel.CrossFadeAlpha(1.0f, 2.5f, true); //fade canvas to black
        yield return new WaitForSeconds(2.5f);//delay
        //pc.transform.SetPositionAndRotation(testRoomTransform.position, testRoomTransform.rotation);
        yield return new WaitForSeconds(1.5f);//delay
        panel.CrossFadeAlpha(0.0f, 2.5f, true);//fade in view

        if (sceneOS.examRoomState == 0)
            SceneManager.LoadScene(3);
        else if (sceneOS.examRoomState == 1)
            SceneManager.LoadScene("IntroScene");
        //instructionSource.Stop();
        //instructionSource.PlayOneShot(instructionAudio[16]);
        //current = Steps.labGloves;//"please put on gloves" (again)
        //}
    }

    IEnumerator PlayCutscene()
    {
        Debug.Log("Cutscene Started");
        yield return new WaitForSeconds(2.5f);
        Debug.Log("Cutscene Ended");
;    }
    


    IEnumerator WaitToRead()
    {
        yield return new WaitForSeconds(3f);
        current = Steps.giveResults;
    }
}
