using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateMachine : MonoBehaviour
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
        labGloves,//putting on gloves
        holdTubeLeft, welcomeScreen, startNewTest, scanTube, confirmSpecimen,//starting the assay and confirming the specimen
        pickUpCartridge, scanCartridge, confirmCartridge, selectAssay, confirmAssay,//confirming the specimen and selecting the correct assay
        putDownCartridge,
        invertTube1, invertTube2, openCartridge, openTube, pickUpPipette, collectSample, depositSample,//preparing the sample for placement in the cartridge
        disposePipette, closeCartridge, pickUpCap2, closeTube2, disposeTube,//preparing the cartridge
        pickUpCartridge2, openGeneXpert, insertCartridge, closeGeneXpert,//starting the test
        startTest, openGeneXpert2, pickUpCartridge3, disposeCartridge, closeGeneXpert2, openResults, chooseResult
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
        start,
        openPack, pickUpSwab, insertSwabLeft, applyPressureLeft, rotateSwabLeft, swapSwab,//taking the sample
        insertSwabRight, applyPressureRight, rotateSwabRight, //second nostril
        pickupTube, breakSwab, disposeSwab, pickUpTubeCap, closeTube, //depositing the sample
        disposeGloves, door, //move to lab
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
    public int tubeInverts;//keeps track of how many times the tube has been inverted

    public GameObject trashCan;//trash can in sample room
    public GameObject doorKnob;//door knob of sample room

    public GameObject testGloveBox;//glove box in the testing room
    public GameObject cartridge;//testing capsule for GeneXpert
    public GameObject cartridgeLid;//cap of the cartridge
    public GameObject cartridgeSampleArea;//the section of the cartridge that the user places the sample into
    public GameObject cartridgePivot;//empty gameobject that acts as the pivot of the cartridge's lid
    public GameObject pipette;//pipette in testing room
    public GameObject testTrash;//trash can in the testing room

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
    public Animator characterAnimator;
    public Animator geneXpertAnimator;
    public Animator bigPacketAnimator;
    public Animator swabPacketAnimator;

    [Header("SnapTransforms")]
    public Transform leftSwabTransform;//transform that the test swab snaps to when swabbing the left nostril
    public Transform leftPressureTransform;//transform that the finger snaps to when swabbing the left nostril
    public Transform rightSwabTransform;//transform that the test swab snaps to when swabbing the right nostril
    public Transform rightPressureTransform;//transform that the finger snaps to when swabbing the right nostril

    public Transform sampleRoomTransform;//starting position in the sample-taking room
    public Transform testRoomTransform;//starting position in the sample-testing room
 
    [Header("DiegeticSounds")]
    public AudioSource audioSource;//the sound source, part of the player
    public AudioClip glovesOn;//sound of putting gloves on
    public AudioClip glovesOff;//sound of taking gloves off
    public AudioClip vacuumPackSound;//sound of opening the vacuum pack
    public AudioClip tubeOpenSound;//sound of opening the test tube
    public AudioClip swabSnap;//sound of swab snapping in half
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
        current = Steps.start; //assign current step to first
        currentInstruction = instructionText.start;
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

        //pc.GetComponent<CharacterController>().enabled = true;
        tubeInverts = 0;
        fingerPoint = false;

        instructionSource.PlayDelayed(1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (current)
        {
            case Steps.start: //player must put on gloves
                //doorknob disabled
                //doorKnob.GetComponent<Collider>().enabled = false;
                //doorKnob.GetComponent<Outline>().enabled = false;

                //some componenents not visible
                testTube.GetComponent<MeshRenderer>().enabled = true;
                //tubeCap.GetComponent<MeshRenderer>().enabled = false;
                tubeSample.GetComponent<MeshRenderer>().enabled = false;
                //swab.GetComponent<MeshRenderer>().enabled = false;
                rightPressure.GetComponent<MeshRenderer>().enabled = false;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                Renderer[] renderers = ghostTube.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)//the phantom test tube in the player's hand is invisible
                    r.enabled = false;

                renderers = geneXpertCartridge.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)//the cartridge inside the GeneXpert is invisible
                    r.enabled = false;

                //enable glove box and hands
                gloveBox.GetComponent<Collider>().enabled = true;
                gloveBox.GetComponent<Outline>().enabled = true;

                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;

                //set instruction panels
                tableText.text = instructions[0];
                patientText.text = instructions[0];

                break;
            case Steps.openPack://User has put on gloves, player must open the vacuum pack to move to next step
                gloveBox.GetComponent<Collider>().enabled = false;
                gloveBox.GetComponent<Outline>().enabled = false;

                vacuumPack.GetComponent<Collider>().enabled = true;
                vacuumPack.GetComponentInChildren<Outline>().enabled = true;

                tableText.text = instructions[1];
                patientText.text = instructions[1];

                break;
            case Steps.openSwab://need to unwrap the swab
                vacuumPack.GetComponent<Collider>().enabled = false;
                //vacuumPack.GetComponent<Outline>().enabled = false;

                swabPack.GetComponent<Collider>().enabled = true;
                swabPack.GetComponentInChildren<Outline>().enabled = true;

                tableText.text = instructions[2];
                patientText.text = instructions[2];
                break;
            case Steps.pickUpSwab://pack now opened, player must pick up the swab with right hand

                swabPack.GetComponent<Collider>().enabled = false;
                swabPack.GetComponentInChildren<Outline>().enabled = false;

                testTube.GetComponent<MeshRenderer>().enabled = true;
                //tubeCap.GetComponent<MeshRenderer>().enabled = true;

                //swab.GetComponent<CapsuleCollider>().enabled = true;
                //swab.GetComponent<MeshRenderer>().enabled = true;
                //swab.GetComponent<Outline>().enabled = true;

                swabBottom.GetComponent<CapsuleCollider>().enabled = true;
                swabBottom.GetComponent<Outline>().enabled = true;

                tableText.text = instructions[3];
                patientText.text = instructions[3];

                leftHand.GetComponent<Collider>().enabled = false;
                break;
            case Steps.insertSwabLeft://player has picked up swab, must insert swab into patient's left nostril
                //swab.GetComponent<Outline>().enabled = false;
                //swab.GetComponent<CapsuleCollider>().enabled = false;
                //swab.GetComponent<BoxCollider>().enabled = true;

                swabBottom.GetComponent<CapsuleCollider>().enabled = false;
                swabBottom.GetComponent<Outline>().enabled = false;

                swabTop.GetComponent<CapsuleCollider>().enabled = true;
                swabTop.GetComponentInChildren<Outline>().enabled = true;

                leftNostril.GetComponent<Outline>().enabled = true;
                leftNostril.GetComponent<Collider>().enabled = true;

                tableText.text = instructions[4];
                patientText.text = instructions[4];

                rightHand.GetComponent<Collider>().enabled = false;
                rightHandModel.GetComponent<MeshFilter>().mesh = swabHand;
                break;
            case Steps.applyPressureLeft://player has inserted swab into nostril left, swab is now frozen(?), player must apply pressure on the nostril with their other hand
                //swab.GetComponent<CapsuleCollider>().enabled = false;
                //swab.GetComponent<BoxCollider>().enabled = false;

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

                tableText.text = instructions[5];
                patientText.text = instructions[5];
                break;
            case Steps.rotateSwabLeft://player has applied pressure, must rotate swab in nostril for 3 seconds
                rightHand.GetComponent<Outline>().enabled = true;

                leftHand.transform.position = leftPressureTransform.position;
                leftHand.transform.rotation = leftPressureTransform.rotation;

                leftPressure.GetComponent<Outline>().enabled = false;

                rightHand.transform.position = leftSwabTransform.position;
                rightHand.transform.eulerAngles = new Vector3(leftSwabTransform.rotation.x, rightHand.transform.eulerAngles.y, leftSwabTransform.rotation.z);

                //pc.GetComponent<PlayerController>().SendHaptics(false, 0.75f, .1f);

                leftPressure.GetComponent<Outline>().enabled = false;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                break;
            case Steps.swapSwab://player has taken the sample from the first nostril, must swap hands to swab the second nostril
                patientText.text = instructions[6];
                leftHand.GetComponent<Outline>().enabled = true;
                leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                rightHand.GetComponent<Outline>().enabled = false;
                rightHand.GetComponent<Collider>().enabled = false;

                //swab.GetComponent<CapsuleCollider>().enabled = true;

                swabBottom.GetComponent<CapsuleCollider>().enabled = true;
                leftPressure.GetComponent<MeshRenderer>().enabled = false;

                break;
            case Steps.insertSwabRight://player has swapped hands, must insert swab into the second nostril
                //swab.GetComponent<Outline>().enabled = false;
                //swab.GetComponent<CapsuleCollider>().enabled = false;
                //swab.GetComponent<BoxCollider>().enabled = true;

                swabBottom.GetComponent<CapsuleCollider>().enabled = false;
                swabBottom.GetComponent<Outline>().enabled = false;

                swabTop.GetComponent<CapsuleCollider>().enabled = true;
                swabTop.GetComponentInChildren<Outline>().enabled = true;

                leftHand.GetComponent<Outline>().enabled = false;
                leftHandModel.GetComponent<MeshFilter>().mesh = swabHand;

                rightHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                rightNostril.GetComponent<Outline>().enabled = true;
                rightNostril.GetComponent<Collider>().enabled = true;

                tableText.text = instructions[7];
                patientText.text = instructions[7];
                break;
            case Steps.applyPressureRight://player has inserted swab, must apply pressure outside the nostril
                //swab.GetComponent<BoxCollider>().enabled = false;

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

                tableText.text = instructions[8];
                patientText.text = instructions[8];
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
                patientText.text = instructions[10];
                tableText.text = instructions[10];

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

                patientText.text = instructions[11];
                tableText.text = instructions[11];
                break;
            case Steps.disposeSwab://player has broken the swab in the tube, must dispose of the swab
                patientText.text = instructions[12];
                tableText.text = instructions[12];
                testTube.GetComponent<CapsuleCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = false;
                tubeSample.GetComponent<MeshRenderer>().enabled = true;//broken swab tip appears in the tube

                //swab.GetComponent<BoxCollider>().enabled = false;
                //swab.GetComponent<CapsuleCollider>().enabled = true;

                swabTop.GetComponentInChildren<MeshRenderer>().enabled = false;
                swabBottom.GetComponent<CapsuleCollider>().enabled = true;

                trashCan.GetComponent<Collider>().enabled = true;
                trashCan.GetComponent<Outline>().enabled = true;
                break;
            case Steps.pickUpTubeCap://player has disposed of the swab, must pick up the cap
                trashCan.GetComponent<Collider>().enabled = false;
                trashCan.GetComponent<Outline>().enabled = false;

                testTube.GetComponent<CapsuleCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = true;

                tubeCap.GetComponent<Collider>().enabled = true;
                tubeCap.GetComponent<Outline>().enabled = true;

                leftHand.GetComponent<Collider>().enabled = true;
                leftHandModel.GetComponent<MeshFilter>().mesh = defaultHand;

                tableText.text = instructions[13];
                patientText.text = instructions[13];
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

                leftHand.GetComponent<Collider>().enabled = true;
                rightHand.GetComponent<Collider>().enabled = true;

                testTube.GetComponent<CapsuleCollider>().enabled = true;
                testTube.GetComponent<BoxCollider>().enabled = false;
                testTube.GetComponent<Outline>().enabled = false;
                tubeCap.GetComponent<Collider>().enabled = false;
                tubeCap.GetComponent<Outline>().enabled = false;

                tableText.text = instructions[14];
                patientText.text = instructions[14];
                break;
            case Steps.door:
                trashCan.GetComponent<Collider>().enabled = false;
                trashCan.GetComponent<Outline>().enabled = false;
                //doorKnob.GetComponent<Collider>().enabled = true;
                //doorKnob.GetComponent<Outline>().enabled = true;
                confirmButton.SetActive(true);

                patientText.text = instructions[15];
                tableText.text = instructions[15];
                break;


            case Steps.labGloves:
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

                cartridgeScan.GetComponent<Collider>().enabled = true;
                cartridgeScan.GetComponent<Outline>().enabled = true;

                testTube.GetComponent<BoxCollider>().isTrigger = false;

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
                ghostTube.transform.localScale = new Vector3(0.15f,0.15f,0.15f);
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

                testText.text = instructions[28];
                break;
            case Steps.closeCartridge:
                testTrash.GetComponent<Collider>().enabled = false;
                testTrash.GetComponent<Outline>().enabled = false;

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

                testText.text = instructions[31];
                break;
            case Steps.openGeneXpert:
                testTrash.GetComponent<Collider>().enabled = false;
                testTrash.GetComponent<Outline>().enabled = false;

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
            if (current == Steps.start)
            {
                audioSource.PlayOneShot(glovesOn);
            }
            else if (current == Steps.disposeGloves)
            {
                audioSource.PlayOneShot(glovesOff);
            }
            else if (current == Steps.labGloves)
            {
                audioSource.PlayOneShot(glovesOn);
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
            else if (current == Steps.labGloves)
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

        public void pickUpSwab()
    {
        if (current == Steps.pickUpSwab)
        {
            GameObject swabParent = swabBottom.transform.parent.gameObject;
            swabParent.transform.parent = rightHand.transform;
            swabParent.transform.position = rightHand.transform.position;
            swabParent.transform.localPosition += new Vector3(0.0f, 0.07f, 0f);
            swabParent.transform.rotation = rightHand.transform.rotation * Quaternion.Euler(15, 0, 180);
            //swab.transform.parent = rightHand.transform;
            //swab.transform.position = rightHand.transform.position;
            //swab.transform.localPosition += new Vector3(0.0f, 0.7f, 0f);
            //swab.transform.rotation = rightHand.transform.rotation * Quaternion.Euler(15, 0, 0);

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

            //swab.transform.parent = leftHand.transform;
            //swab.transform.position = leftHand.transform.position;
            //swab.transform.localPosition += new Vector3(0.0f, 0.7f, 0f);
            //swab.transform.rotation = leftHand.transform.rotation * Quaternion.Euler(15, 0, 0);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[7]);

            current = Steps.insertSwabRight;
        }
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
        else if (current == Steps.holdTubeLeft)
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

    public void breakSwab()
    {
        pc.GetComponent<AudioSource>().PlayOneShot(swabSnap);
        //swab.GetComponent<MeshFilter>().mesh = brokenSwab;
        //swab.GetComponent<CapsuleCollider>().height = swab.GetComponent<CapsuleCollider>().height * 0.5f;

        swabTop.SetActive(false);
        //swabTop.GetComponentInChildren<Collider>().enabled = false;

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
        else if (current == Steps.openTube)
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
        else if (current == Steps.closeTube2)
        {
            audioSource.PlayOneShot(tubeCloseSound);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[31]);

            current = Steps.disposeTube;
        }
    }

    public void openDoor()
    {
        if (current == Steps.door)
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
        if(current == Steps.openGeneXpert)
        {
            geneXpertAnimator.SetTrigger("open");
            current = Steps.pickUpCartridge2;
        }
        else if (current == Steps.closeGeneXpert)
        {
            geneXpertHandle.GetComponent<Outline>().enabled = false;
            geneXpertAnimator.SetTrigger("close");
            current = Steps.startTest;

            PlayCutscene();

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
        if(current == Steps.insertCartridge)
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

    IEnumerator Rotate()
    {
        Debug.Log("Coroutine Started");
        yield return new WaitForSeconds(0.3f);
        if (current == Steps.rotateSwabLeft)
        {
            patientText.text = "Rotate swab against the inside of the nostril for 3 seconds.";
            yield return new WaitForSeconds(3f);
            patientText.text = "Rotate swab against the inside of the nostril for 2 seconds.";
            yield return new WaitForSeconds(1f);
            patientText.text = "Rotate swab against the inside of the nostril for 1 seconds.";
            yield return new WaitForSeconds(1f);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[6]);

            current = Steps.swapSwab;
        }
        else if (current == Steps.rotateSwabRight)
        {
            patientText.text = "Repeat: Rotate swab against the inside of the nostril for 3 seconds.";
            yield return new WaitForSeconds(3f);
            patientText.text = "Repeat: Rotate swab against the inside of the nostril for 2 seconds.";
            yield return new WaitForSeconds(1f);
            patientText.text = "Repeat: Rotate swab against the inside of the nostril for 1 seconds.";
            yield return new WaitForSeconds(1f);

            instructionSource.Stop();
            instructionSource.PlayOneShot(instructionAudio[10]);

            current = Steps.pickupTube;
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
        pc.transform.SetPositionAndRotation(testRoomTransform.position, testRoomTransform.rotation);
        yield return new WaitForSeconds(1.5f);//delay

        //panel.CrossFadeAlpha(0.0f, 2.5f, true);//fade in view
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
    }
}
