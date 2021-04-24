using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LabManager : MonoBehaviour
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
        labGloves,//putting on gloves
        holdTubeLeft, welcomeScreen, startNewTest, scanTube, confirmSpecimen,//starting the assay and confirming the specimen
        pickUpCartridge, scanCartridge, confirmCartridge, selectAssay, confirmAssay,//confirming the specimen and selecting the correct assay
        putDownCartridge,
        invertTube1, invertTube2, openCartridge, openTube, pickUpPipette, collectSample, depositSample,//preparing the sample for placement in the cartridge
        disposePipette, closeCartridge, pickUpCap2, closeTube2, disposeTube,//preparing the cartridge
        pickUpCartridge2, openGeneXpert, insertCartridge, closeGeneXpert,//starting the test
        startTest, openGeneXpert2, pickUpCartridge3, disposeCartridge, closeGeneXpert2, openResults, chooseResult,leaveRoom
    };
    [Header("State")]
    public Steps current;//keeps track of the current step in the switch statement

    [Header("NonDiegeticUI")]

    //transform of instruction canvas
    public Canvas tableInstructions;//canvas of instructions over the counter in the sample taking room
    public TextMeshProUGUI tableText;//text element of tableInstructions
    public Canvas patientInstructions;//canvas of instructions next to the patient
    public TextMeshProUGUI patientText;//text element of patientInstructions
    public TextMeshProUGUI testText;//instructions in the testing room
    public GameObject confirmButton;//to teleport to next room

    private string[] instructions = {//array of instructions given to the player
        "Please put on gloves.",//0
        "Open the vacuum-sealed pack.",//1
        "Unwrap the swab",//2
        "Pick up the swab with your right hand, at the middle of the shaft on the scoreline. Take care to not touch the tip of the swab to any surface.",//3
        "Apply pressure with a finger to the outside of the nostril.",//4
        "Rotate swab against the inside of the nostril for 3 seconds.",//5
        "Swap hands.",//6
        "Repeat: Insert the swab into the patient's nostril, no more than 1-1.5cm.",//7
        "Repeat: Apply pressure with a finger to the outside of the nostril.",//8
        "Repeat: Rotate swab against the inside of the nostril for 3 seconds.",//9
        "Pick up the test tube.",//10
        //"Remove the cap from the tube.",//
        "Insert the swab into the transport medium, and break the swab against the side of the tube at the scoreline.",//11
        "Dispose the swab.",//12
        "Replace the cap on the tube and close tightly.",//13
        "Please remove your gloves.",//14
        "Bring your sample to the lab.",//15


        "Please put on gloves.",//16
        "Start the GeneXpert testing procedure.",//17
        "Scan the test tube specimen.",//18
        "Confirm the test tube specimen.",//19
        "Scan the Xpert cartridge.",//20
        "Confirm the cartridge number.",//21
        "Select assay.",//22
        "Place the cartridge on the table and open it.",//23
        "Invert the test tube 5 times.",//24
        "Hold the tube in your left hand and open it.",//25
        "Pick up the pipette and take the sample from the tube.",//26
        "Deposit the sample into the prepared chamber of the cartridge.",//27
        "Dispose the pipette.",//28
        "Close the cartridge.",//29
        "Close the test tube.",//30
        "Dispose the test tube.",//31
        "Open the GeneXpert door.",//32
        "Place the cartridge in the GeneXpert with the barcode facing you.",//33
        "Close the GeneXpert and wait for the test to finish.",//34
        "Remove the cartridge from the GeneXpert",//35
        "Dispose the cartrdige.",//36
        "Close the GeneXpert door.",//37
        "View the test results."//38
    };

    public AudioSource instructionSource;//audio source that plays instruction audio
    public AudioClip[] instructionAudio;//array of instruction audio clips 

    public enum instructionText
    {
        labGloves,//putting on gloves
        holdTubeLeft, welcomeScreen, startNewTest, scanTube, confirmSpecimen,
        pickUpCartridge, scanCartridge, confirmCartridge, selectAssay, confirmAssay,
        putDownCartridge,
        invertTube1, invertTube2, openCartridge, openTube, pickUpPipette, collectSample, depositSample,
        disposePipette, closeCartridge, pickUpCap2, closeTube2, disposeTube,
        pickUpCartridge2, openGeneXpert
    }

    public instructionText currentInstruction;


    [Header("Interactibles")]

    public GameObject testTube;//tube for placing sample
    public GameObject tubeCap;//cap of the test tube
    public int tubeInverts;//keeps track of how many times the tube has been inverted

    public GameObject testGloveBox;//glove box in the testing room
    public GameObject cartridge;//testing capsule for GeneXpert
    public GameObject cartridgeLid;//cap of the cartridge
    public GameObject cartridgeSampleArea;//the section of the cartridge that the user places the sample into
    public GameObject cartridgePivot;//empty gameobject that acts as the pivot of the cartridge's lid
    public GameObject pipette;//pipette in testing room
    public GameObject testTrash;//trash can in the testing room
    public GameObject testTrashLid;

    public GameObject cartridgeScan;//the box attached to the GeneXpert that represents the barcode scanning area

    public Transform tableCartridgeTransform;//transform that cartridge snaps to when the player puts down the cartridge
    public GameObject cartridgeCube; //outlined cube that the player aims for when putting down the cartridge

    public GameObject geneXpertHandle;//the handle of the genexpert door
    public GameObject geneXpertCartridge;//the cartridge that is present inside the GeneXpert. Is visible/invisible as is necessary to sell the illusion that you are placing/taking out the cartridge in the machine
    public GameObject geneXpertCartridgeOutline;//the cube attached to the geneXpertCartridge, used for outlines and collision detection;

    public GameObject ghostTube;//the test tube indicator that shows the user how to invert the test tube
    public GameObject tableCap;//fake test tube cap on the table fo rwhen the player "opens" the test tube to take the sample

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
    public Animator geneXpertAnimator;

    [Header("SnapTransforms")]
    public Transform testRoomTransform;//starting position in the sample-testing room

    [Header("DiegeticSounds")]
    public AudioSource audioSource;//the sound source, part of the player
    public AudioClip glovesOn;//sound of putting gloves on
    public AudioClip glovesOff;//sound of taking gloves off
    public AudioClip tubeOpenSound;//sound of opening the test tube
    public AudioClip tubeCloseSound;//sound of closing the test tube
    public AudioClip trashSound;//sound of something going into the trash
    public AudioClip lidOpen;//sound of the cartridge opening
    public AudioClip scannerBeep;//sound of a scanner scanning a barcode

    [Header("GenexpertElements")]
    public GameObject[] screens;//start screen, newTestScreen, scanTubeScreen, confirmTubeScreen, scanCartridge, confirmCartridge, selectAssayScreen, confirmAssayScreen, prepareCartridgeScreen, insertCartridgeScreen, disposeCartridgeScreen
    private string[] headers ={ //list of header text on the GeneX screen
        " ",//0
        "Home",//1
        "Step 1 of 7 - Scan Test Tube",//2
        "Step 2 of 7 - Confirm Specimen",//3
        "Step 3 of 7 - Scan Cartridge",//4
        "Step 4 of 7 - Confirm Test",//5
        "Step 5 of 7 - Prepare Cartridge",//6
        "Step 6 of 7 - Insert Cartridge",//7
        "Step 7 of 7 - Dispose Cartridge",//8
        "Select Test"//9
    };
    public TextMeshProUGUI headerText;
    public GameObject template1; //select assay screen template
    public GameObject template2; //confirm assay screen template

    [Header("Materials")]

    public Material blue;
    public Material glass;
    public Material pipetteColor;

    [Header("BooleansAndMisc")]
    public int gloved; //how many times the hands have been gloved or ungloved in the current state
    private bool fingerPoint;//bool used for pick up tube helper function, determines whether the hand not holding the tube is the default hand or the pointer hand
    public Image panel;//the black panel of the fade out canvas

    //this manager uses a switch statement to navigate between states. When the hands in the scene interact with objects, they send broadcasts up to the manager,
    //which triggers the methods below that update the states
    void Start()
    {
        current = Steps.labGloves; //assign current step to first
        pc.transform.position = testRoomTransform.position;
        pc.transform.rotation = testRoomTransform.rotation;
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

        //pc.GetComponent<CharacterController>().enabled = true;
        tubeInverts = 0;
        fingerPoint = false;

        rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;

        testTube.transform.position = rightHand.transform.position;
        testTube.transform.rotation = rightHand.transform.rotation;
        testTube.transform.parent = rightHand.transform;
        testTube.transform.localPosition += new Vector3(0f, 0.07f, 0f);
        tubeCap.transform.position = testTube.transform.position;
        tubeCap.transform.rotation = testTube.transform.rotation;
        tubeCap.gameObject.transform.parent = testTube.gameObject.transform;
        testTube.GetComponent<Collider>().enabled = true;

        instructionSource.PlayDelayed(1.5f);

        for (int i = 1; i < screens.Length; i++)
        {
            screens[i].SetActive(false);
        }
        headerText.text = headers[0];
        screens[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (current)
        {
            case Steps.labGloves:
                Renderer[] renderers = ghostTube.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)//the phantom test tube in the player's hand is invisible
                    r.enabled = false;

                renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)//the cartridge inside the GeneXpert is invisible
                    r.enabled = false;

                //enable glove box and hands
                testGloveBox.GetComponent<Collider>().enabled = true;
                testGloveBox.GetComponent<Outline>().enabled = true;

                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;

                //set instruction panels
                tableText.text = instructions[16];
                patientText.text = instructions[16];

                testText.text = instructions[16];
                testGloveBox.GetComponent<Outline>().enabled = true;
                testGloveBox.GetComponent<Collider>().enabled = true;

                break;
            case Steps.welcomeScreen:
                //rightHandModel.GetComponent<MeshFilter>().mesh = pointHand;
                testGloveBox.GetComponent<Outline>().enabled = false;
                testGloveBox.GetComponent<Collider>().enabled = false;

                testText.text = instructions[17];

                rightHand.GetComponent<SphereCollider>().enabled = true;
                leftHand.GetComponent<Collider>().enabled = true;
                testTube.GetComponent<CapsuleCollider>().enabled = true;
                break;

            case Steps.startNewTest:
                headerText.text = headers[1];
                break;
            case Steps.scanTube:
                testTube.GetComponent<BoxCollider>().enabled = true;
                testTube.GetComponent<BoxCollider>().isTrigger = false;

                cartridgeScan.GetComponent<Collider>().enabled = true;
                cartridgeScan.GetComponent<Outline>().enabled = true;

                //testTube.GetComponent<BoxCollider>().isTrigger = false;

                screens[2].GetComponentInChildren<TextMeshProUGUI>().text = "Please scan the barcode on the test tube using the scanner on the side of the machine.";
                headerText.text = headers[2];
                testText.text = instructions[18];
                break;
            case Steps.confirmSpecimen:
                testTube.GetComponent<BoxCollider>().isTrigger = true;

                cartridgeScan.GetComponent<Collider>().enabled = false;
                cartridgeScan.GetComponent<Outline>().enabled = false;

                screens[3].GetComponentInChildren<TextMeshProUGUI>().text = "Please verify that the specimen ID is correct: Specimen XXXXX.";
                headerText.text = headers[3];
                testText.text = instructions[19];
                break;
            case Steps.pickUpCartridge:
                testTrash.GetComponent<Collider>().enabled = false;
                testTrash.GetComponent<Outline>().enabled = false;

                cartridge.GetComponent<BoxCollider>().enabled = true;
                rightHand.GetComponent<BoxCollider>().enabled = false;
                rightHand.GetComponent<SphereCollider>().enabled = true;
                Outline[] outlines = cartridge.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = true;

                screens[2].GetComponentInChildren<TextMeshProUGUI>().text = "Please scan the barcode on the cartridge using the scanner on the side of the machine.";
                headerText.text = headers[4];
                testText.text = instructions[20];
                break;
            case Steps.scanCartridge:
                rightHand.GetComponent<Collider>().enabled = false;
                rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;

                cartridgeScan.GetComponent<Collider>().enabled = true;
                cartridgeScan.GetComponent<Outline>().enabled = true;

                cartridge.GetComponent<BoxCollider>().isTrigger = false;

                //Debug.Log("Cartridge: "+cartridge.transform.forward+", Box: "+cartridgeScan.transform.forward);//cartridge should be facing the barcode scanner
                break;
            case Steps.confirmCartridge:
                cartridgeScan.GetComponent<Collider>().enabled = false;
                cartridgeScan.GetComponent<Outline>().enabled = false;

                rightHand.GetComponent<BoxCollider>().enabled = true;

                screens[3].GetComponentInChildren<TextMeshProUGUI>().text = "Please confirm that this is the correct cartridge: Xpert Xpress Flu/RSV.";
                headerText.text = headers[5];
                testText.text = instructions[21];

                outlines = cartridge.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = false;
                break;
            case Steps.selectAssay:
                testText.text = instructions[22];
                break;
            case Steps.putDownCartridge:
                cartridgeCube.SetActive(true);

                headerText.text = headers[6];
                testText.text = instructions[23];
                break;
            case Steps.openCartridge:
                cartridgeCube.SetActive(false);
                cartridgeLid.GetComponent<Collider>().enabled = true;
                cartridgeLid.GetComponent<Outline>().enabled = true;

                testText.text = instructions[23];
                break;
            case Steps.invertTube1:
                cartridgeCube.SetActive(false);

                glass = testTube.GetComponent<MeshRenderer>().material;
                leftHand.GetComponent<Collider>().enabled = false;
                ghostTube.transform.rotation = Quaternion.Euler(0, 0, 180);
                ghostTube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                outlines = ghostTube.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = true;

                renderers = ghostTube.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = true;

                cartridgeLid.GetComponent<Collider>().enabled = false;
                cartridgeLid.GetComponent<Outline>().enabled = false;
                cartridgePivot.transform.rotation = Quaternion.Euler(90, 0, 0);

                testText.text = instructions[24];
                checkRotation(1);
                break;
            case Steps.invertTube2:
                ghostTube.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                ghostTube.transform.rotation = Quaternion.Euler(0, 0, 0);

                testText.text = instructions[24];
                checkRotation(2);
                break;
            case Steps.holdTubeLeft:
                rightHand.GetComponent<Collider>().enabled = false;
                leftHand.GetComponent<Collider>().enabled = true;

                outlines = ghostTube.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = false;

                renderers = ghostTube.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = false;


                testTube.GetComponent<MeshRenderer>().material = blue;

                testText.text = instructions[25];
                break;
            case Steps.openTube:
                rightHand.GetComponent<Collider>().enabled = true;
                leftHand.GetComponent<Collider>().enabled = false;

                tubeCap.GetComponent<Collider>().enabled = true;
                tubeCap.GetComponent<Outline>().enabled = true;
                break;
            case Steps.pickUpPipette:
                rightHand.GetComponent<Collider>().enabled = true;
                pipette.GetComponent<Outline>().enabled = true;
                pipette.GetComponent<CapsuleCollider>().enabled = true;
                pipetteColor = pipette.GetComponent<MeshRenderer>().material;

                tubeCap.GetComponent<MeshRenderer>().enabled = false;
                tableCap.GetComponent<MeshRenderer>().enabled = true;

                testText.text = instructions[26];
                break;
            case Steps.collectSample:
                rightHand.GetComponent<BoxCollider>().enabled = false;
                rightHand.GetComponent<SphereCollider>().enabled = false;

                testTube.GetComponent<BoxCollider>().enabled = true;//opening of the tube
                testTube.GetComponent<Outline>().enabled = true;
                pipette.GetComponent<CapsuleCollider>().enabled = false;//disable handle of pipette
                pipette.GetComponent<BoxCollider>().enabled = true;//tip of the pipette
                break;
            case Steps.depositSample:
                testTube.GetComponent<BoxCollider>().enabled = false;//test tube not interactable anymore
                testTube.GetComponent<Outline>().enabled = false;
                testTube.GetComponent<MeshRenderer>().material = glass;
                pipette.GetComponent<MeshRenderer>().material = blue;

                cartridgeSampleArea.GetComponent<Outline>().enabled = true;//deposit in a certain section of the cartridge
                cartridgeSampleArea.GetComponent<Collider>().enabled = true;

                testText.text = instructions[27];
                break;
            case Steps.disposePipette:
                cartridgeSampleArea.GetComponent<MeshRenderer>().material = blue;
                pipette.GetComponent<MeshRenderer>().material = pipetteColor;

                cartridgeSampleArea.GetComponent<Outline>().enabled = false;
                cartridgeSampleArea.GetComponent<Collider>().enabled = false;

                testTrash.GetComponent<Collider>().enabled = true;
                testTrash.GetComponent<Outline>().enabled = true;

                testTrashLid.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                testText.text = instructions[28];
                break;
            case Steps.closeCartridge:
                testTrash.GetComponent<Collider>().enabled = false;
                testTrash.GetComponent<Outline>().enabled = false;

                testTrashLid.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                rightHand.GetComponent<Collider>().enabled = true;
                rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                cartridgeLid.GetComponent<Collider>().enabled = true;
                cartridgeLid.GetComponent<Outline>().enabled = true;

                testText.text = instructions[29];
                break;
            case Steps.pickUpCap2:
                cartridgeLid.GetComponent<Collider>().enabled = false;
                cartridgeLid.GetComponent<Outline>().enabled = false;
                cartridgePivot.transform.rotation = Quaternion.Euler(0, 180, 0);//close lid

                tableCap.GetComponent<Outline>().enabled = true;
                tableCap.GetComponent<Collider>().enabled = true;

                testText.text = instructions[30];
                break;
            case Steps.closeTube2:
                testTube.GetComponent<BoxCollider>().enabled = true;//tube opening
                rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;
                break;
            case Steps.disposeTube:
                rightHand.GetComponent<BoxCollider>().enabled = false;
                rightHand.GetComponent<SphereCollider>().enabled = false;
                leftHand.GetComponent<Collider>().enabled = false;

                tableCap.GetComponent<MeshRenderer>().enabled = false;
                tableCap.GetComponent<Outline>().enabled = false;
                tableCap.GetComponent<Collider>().enabled = false;

                tubeCap.GetComponent<MeshRenderer>().enabled = true;

                testTube.GetComponent<BoxCollider>().enabled = false;
                testTube.GetComponent<CapsuleCollider>().enabled = true;

                testTrash.GetComponent<Collider>().enabled = true;
                testTrash.GetComponent<Outline>().enabled = true;

                testTrashLid.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

                testText.text = instructions[31];
                break;
            case Steps.openGeneXpert:
                testTrash.GetComponent<Collider>().enabled = false;
                testTrash.GetComponent<Outline>().enabled = false;

                testTrashLid.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                rightHand.GetComponent<BoxCollider>().enabled = true;
                rightHand.GetComponent<SphereCollider>().enabled = false;

                leftHand.GetComponent<Collider>().enabled = true;

                testText.text = instructions[32];
                headerText.text = headers[7];

                geneXpertHandle.GetComponent<Outline>().enabled = true;
                geneXpertHandle.GetComponent<Collider>().enabled = true;
                break;
            case Steps.pickUpCartridge2:
                cartridge.GetComponent<BoxCollider>().enabled = true;
                cartridge.GetComponent<BoxCollider>().isTrigger = true;
                outlines = cartridge.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = true;

                geneXpertHandle.GetComponent<Outline>().enabled = false;
                geneXpertHandle.GetComponent<Collider>().enabled = false;

                testText.text = instructions[33];
                headerText.text = headers[7];
                outlines = cartridge.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = true;
                break;
            case Steps.insertCartridge:
                outlines = cartridge.GetComponentsInChildren<Outline>();
                foreach (Outline o in outlines)
                    o.enabled = false;
                geneXpertCartridgeOutline.GetComponent<Renderer>().enabled = true;
                geneXpertCartridgeOutline.GetComponent<Outline>().enabled = true;

                cartridge.GetComponent<BoxCollider>().isTrigger = false;

                geneXpertCartridgeOutline.GetComponent<Collider>().enabled = true;
                break;
            case Steps.closeGeneXpert:
                geneXpertCartridgeOutline.GetComponent<Outline>().enabled = false;

                geneXpertHandle.GetComponent<Outline>().enabled = true;
                geneXpertHandle.GetComponent<Collider>().enabled = true;

                renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = true;

                renderers = cartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = false;

                testText.text = instructions[34];
                break;
            case Steps.startTest:
                geneXpertHandle.GetComponent<Outline>().enabled = false;
                geneXpertHandle.GetComponent<Collider>().enabled = false;

                screens[6].SetActive(false);
                screens[7].SetActive(true);
                break;
            case Steps.openGeneXpert2:
                geneXpertHandle.GetComponent<Outline>().enabled = true;
                geneXpertHandle.GetComponent<Collider>().enabled = true;

                testText.text = instructions[35];
                break;
            case Steps.pickUpCartridge3:
                geneXpertHandle.GetComponent<Outline>().enabled = false;
                geneXpertHandle.GetComponent<Collider>().enabled = false;


                geneXpertCartridgeOutline.GetComponent<Collider>().enabled = true;
                geneXpertCartridgeOutline.GetComponent<Outline>().enabled = true;


                break;
            case Steps.disposeCartridge:
                testTrash.GetComponent<Collider>().enabled = true;
                testTrash.GetComponent<Outline>().enabled = true;

                rightHand.GetComponent<BoxCollider>().enabled = false;

                geneXpertCartridgeOutline.GetComponent<Outline>().enabled = false;
                geneXpertCartridgeOutline.GetComponent<Collider>().enabled = false;

                renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = false;

                renderers = cartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.enabled = true;

                testText.text = instructions[36];
                break;
            case Steps.closeGeneXpert2:
                testTrash.GetComponent<Outline>().enabled = false;
                testTrash.GetComponent<Collider>().enabled = false;

                geneXpertHandle.GetComponent<Collider>().enabled = true;
                geneXpertHandle.GetComponent<Outline>().enabled = true;

                rightHand.GetComponent<Collider>().enabled = true;

                testText.text = instructions[37];
                break;
            case Steps.openResults:
                screens[7].SetActive(false);
                screens[8].SetActive(true);
                geneXpertHandle.GetComponent<Collider>().enabled = false;
                geneXpertHandle.GetComponent<Outline>().enabled = false;

                rightHandModel.GetComponent<MeshFilter>().mesh = pointHand;

                testText.text = instructions[38];
                break;
            default:
                //Debug.Log("NOTHING");
                break;
        }
    }

    public void buttonPress()
    {
        if (current == Steps.welcomeScreen)
        {
            screens[0].SetActive(false);
            Collider[] colliders = screens[1].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            screens[1].SetActive(true);
            headerText.text = headers[1];
            current = Steps.startNewTest;
        }
        else if (current == Steps.startNewTest)
        {
            screens[1].SetActive(false);
            Collider[] colliders = screens[2].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            screens[2].SetActive(true);
            headerText.text = headers[2];

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[18]);

            current = Steps.scanTube;
        }
        else if (current == Steps.confirmSpecimen)
        {
            screens[3].SetActive(false);
            screens[2].SetActive(true);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[20]);

            current = Steps.pickUpCartridge;
        }
        else if (current == Steps.confirmCartridge)
        {
            screens[3].SetActive(false);
            screens[4].SetActive(true);
            Collider[] colliders = screens[4].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            template1.SetActive(true);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[22]);

            current = Steps.selectAssay;
        }
        else if (current == Steps.selectAssay)
        {
            screens[4].SetActive(false);
            Collider[] colliders = screens[5].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            screens[5].SetActive(true);
            template2.SetActive(true);
            current = Steps.confirmAssay;
        }
        else if (current == Steps.confirmAssay)
        {
            screens[5].SetActive(false);
            screens[6].SetActive(true);
            fingerPoint = false;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[23]);

            current = Steps.putDownCartridge;
        }
        else if (current == Steps.openResults)
        {
            screens[8].SetActive(false);
            screens[9].SetActive(true);
            Collider[] colliders = screens[9].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            fingerPoint = true;
            current = Steps.chooseResult;
        }
        else if (current == Steps.chooseResult)
        {
            screens[9].SetActive(false);
            screens[10].SetActive(true);

        }

    }

    public void Gloves()//called by the hands when they contact a glove box or a trash can
    {
        //Debug.Log("Called Gloves method");
        if (gloved == 0)//neither hands gloved/ungloved
        {
            gloved++;//add counter
            if (current == Steps.labGloves)
            {
                audioSource.PlayOneShot(glovesOn);
            }

        }
        else if (gloved == 1)//one hand gloved
        {
            gloved = 0;
            //next step
            if (current == Steps.labGloves)
            {
                audioSource.PlayOneShot(glovesOn);
                current = Steps.welcomeScreen;

                instructionSource.Stop();
                instructionSource.PlayOneShot(instructionAudio[17]);

                Collider[] colliders = screens[0].GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                    col.enabled = true;

                TMP_Dropdown[] dropdowns = this.GetComponentsInChildren<TMP_Dropdown>();
                foreach (TMP_Dropdown d in dropdowns)
                    d.Show();
                fingerPoint = true;
            }
        }
    }

    public void pickUpTube(string handtag)
    {
        if (current == Steps.holdTubeLeft)
        {
            audioSource.PlayOneShot(tubeOpenSound);
            testTube.transform.position = leftHand.transform.position;
            testTube.transform.rotation = leftHand.transform.rotation;
            testTube.transform.parent = leftHand.transform;
            testTube.transform.localPosition += new Vector3(0f, 0.07f, 0f);

            leftHandModel.GetComponent<MeshFilter>().mesh = closedHand;
            rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[26]);

            current = Steps.pickUpPipette;
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
                if (fingerPoint)
                    rightHandModel.GetComponent<MeshFilter>().mesh = pointHand;
                else
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
                if (fingerPoint)
                    leftHandModel.GetComponent<MeshFilter>().mesh = pointHand;
                else
                    leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
            }
        }
    }

    public void pickUpTubeCap()
    {
        if (current == Steps.openTube)
        {
            audioSource.PlayOneShot(tubeOpenSound);
            current = Steps.pickUpPipette;
        }
        else if (current == Steps.pickUpCap2)
        {
            tableCap.transform.position = rightHand.transform.position;
            tableCap.transform.rotation = rightHand.transform.rotation;
            tableCap.transform.parent = rightHand.transform;
            current = Steps.closeTube2;
        }
    }

    public void closeTube()
    {
        if (current == Steps.closeTube2)
        {
            audioSource.PlayOneShot(tubeCloseSound);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[31]);

            current = Steps.disposeTube;
        }
    }

    public void openDoor()
    {
        if (current == Steps.leaveRoom)
        {
            Debug.Log("open door");
            //doorKnob.GetComponent<Outline>().enabled = false;
            confirmButton.SetActive(false);
            current = Steps.labGloves;
            for (int i = 1; i < screens.Length; i++)
            {
                screens[i].SetActive(false);
            }
            headerText.text = headers[0];
            screens[0].SetActive(true);
            //FadeOut();
        }
    }

    public void putDownCartridge()
    {
        if (current == Steps.putDownCartridge)
        {
            cartridge.transform.parent = null;
            cartridge.transform.SetPositionAndRotation(tableCartridgeTransform.position, tableCartridgeTransform.rotation);
            current = Steps.openCartridge;
        }
    }

    public void cartridgeCap()
    {
        if (current == Steps.openCartridge)
        {
            audioSource.PlayOneShot(lidOpen);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[24]);

            current = Steps.invertTube1;
        }
        else if (current == Steps.closeCartridge)
        {
            //audioSource.PlayOneShot(tubeCloseSound);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[30]);

            current = Steps.pickUpCap2;
        }
    }

    public void checkRotation(int step)
    {
        Debug.Log(rightHand.transform.rotation.eulerAngles.y);
        if (step == 1)
        {
            if (rightHand.transform.rotation.eulerAngles.z < 182 && rightHand.transform.rotation.eulerAngles.z > 178)
            {
                Debug.Log("hit");
                tubeInverts++;
                current = Steps.invertTube2;
            }
        }
        if (step == 2)
        {
            if (rightHand.transform.rotation.eulerAngles.z < 2 && rightHand.transform.rotation.eulerAngles.z > -2)
            {
                Debug.Log("hit");
                if (tubeInverts > 4)//5 inversions
                {

                    instructionSource.Stop();
                    instructionSource.PlayOneShot(instructionAudio[25]);

                    current = Steps.holdTubeLeft;//move on to next step

                    Renderer[] renderers = ghostTube.GetComponentsInChildren<Renderer>();//make ghostTube disappear
                    foreach (Renderer r in renderers)
                        r.enabled = false;
                }
                else
                    current = Steps.invertTube1;//invert again
            }
        }
    }

    public void pickUpPipette()
    {
        if (current == Steps.pickUpPipette)
        {
            pipette.transform.position = rightHand.transform.position;
            pipette.transform.rotation = rightHand.transform.rotation;
            pipette.transform.parent = rightHand.transform;
            rightHandModel.GetComponent<MeshFilter>().mesh = closedHand;
            current = Steps.collectSample;
        }
    }

    public void collectSample()
    {
        if (current == Steps.collectSample)
        {
            pipette.GetComponent<MeshRenderer>().material = blue;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[27]);

            current = Steps.depositSample;
        }
    }

    public void depositSample()
    {
        if (current == Steps.depositSample)
        {
            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[28]);

            current = Steps.disposePipette;
        }
    }

    public void disposePipette()
    {
        if (current == Steps.disposePipette)
        {
            audioSource.PlayOneShot(trashSound);
            rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[29]);

            current = Steps.closeCartridge;
        }
    }

    public void disposeTube()
    {
        if (current == Steps.disposeTube)
        {
            audioSource.PlayOneShot(trashSound);
            leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
            rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;
            Destroy(testTube);
            Destroy(tubeCap);
            Destroy(tableCap);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[32]);

            current = Steps.openGeneXpert;
        }
    }

    public void pickUpCartridge()
    {
        if (current == Steps.pickUpCartridge)
        {
            cartridge.transform.position = rightHand.transform.position;
            cartridge.transform.rotation = rightHand.transform.rotation;
            cartridge.transform.parent = rightHand.transform;
            cartridge.transform.rotation = Quaternion.Euler(0, 90, 0);
            current = Steps.scanCartridge;
        }
        else if (current == Steps.pickUpCartridge2)
        {
            cartridge.transform.position = rightHand.transform.position;
            cartridge.transform.rotation = rightHand.transform.rotation;
            cartridge.transform.parent = rightHand.transform;
            cartridge.transform.rotation = Quaternion.Euler(0, 90, 0);
            current = Steps.insertCartridge;
        }
        else if (current == Steps.pickUpCartridge3)
        {
            Renderer[] renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.enabled = false;

            cartridge.SetActive(true);
            current = Steps.disposeCartridge;
        }
    }
    public void scanCartridge()
    {
        if (current == Steps.scanCartridge)
        {
            audioSource.PlayOneShot(scannerBeep);
            screens[2].SetActive(false);
            Collider[] colliders = screens[3].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            screens[3].SetActive(true);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[21]);

            current = Steps.confirmCartridge;
        }
    }

    public void scanTube()
    {
        if (current == Steps.scanTube)
        {
            audioSource.PlayOneShot(scannerBeep);
            screens[2].SetActive(false);
            Collider[] colliders = screens[3].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            screens[3].SetActive(true);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[19]);

            current = Steps.confirmSpecimen;
        }
    }

    public void geneXpertHandleTouch()
    {
        if (current == Steps.openGeneXpert)
        {
            geneXpertAnimator.SetTrigger("open");
            current = Steps.pickUpCartridge2;
        }
        else if (current == Steps.closeGeneXpert)
        {
            geneXpertHandle.GetComponent<Outline>().enabled = false;
            geneXpertAnimator.SetTrigger("close");
            current = Steps.startTest;

            //PlayCutscene();

            screens[6].SetActive(false);
            screens[7].SetActive(true);

            current = Steps.openGeneXpert2;
        }
        else if (current == Steps.openGeneXpert2)
        {
            geneXpertAnimator.SetTrigger("open");
            current = Steps.pickUpCartridge3;
        }
        else if (current == Steps.closeGeneXpert2)
        {
            geneXpertHandle.GetComponent<Outline>().enabled = false;
            geneXpertAnimator.SetTrigger("close");
            current = Steps.openResults;
            Collider[] colliders = screens[8].GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
                col.enabled = true;
            fingerPoint = true;

        }
    }

    public void insertCartridge()
    {
        if (current == Steps.insertCartridge)
        {
            Renderer[] renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.enabled = true;

            renderers = cartridge.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.enabled = false;

            cartridge.SetActive(false);

            geneXpertHandle.GetComponent<Outline>().enabled = true;
            geneXpertHandle.GetComponent<Collider>().enabled = true;

            current = Steps.closeGeneXpert;
        }
    }

    public void disposeCartridge()
    {
        audioSource.PlayOneShot(trashSound);
        Destroy(cartridge);
        current = Steps.closeGeneXpert2;
    }

    IEnumerator FadeOut()
    {
        Debug.Log("Coroutine Started");
        panel.CrossFadeAlpha(1.0f, 2.5f, true); //fade canvas to black
        yield return new WaitForSeconds(2.5f);//delay
        pc.transform.SetPositionAndRotation(testRoomTransform.position, testRoomTransform.rotation);
        yield return new WaitForSeconds(1.5f);//delay

    }
}
