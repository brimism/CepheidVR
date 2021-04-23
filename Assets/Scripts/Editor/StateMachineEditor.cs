using UnityEditor;

[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor
{
    // The various categories the editor will display the variables in
    public enum DisplayCategory
    {
        All, State, NonDiegeticUI, Interactibles, PCElements, Animation, DiegeticSounds, GenexpertElements, Materials, BooleansAndMisc
    }
    // The enum field that will determine what variables to display in the Inspector
    public DisplayCategory categoryToDisplay;

    // The function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        // Display the enum popup in the inspector
        categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        // Create a space to separate this enum popup from the other variables 
        EditorGUILayout.Space();

        //Switch statement to handle what happens for each category
        switch (categoryToDisplay)
        {
            case DisplayCategory.All:
                DisplayAll();
                break;
            case DisplayCategory.State:
                DisplayStateInfo();
                break;

            case DisplayCategory.NonDiegeticUI:
                DisplayNonDiegeticUIInfo();
                break;
            case DisplayCategory.Interactibles:
                    DisplayInteractiblesInfo();
                    break;
            case DisplayCategory.PCElements:
                    DisplayPCElements();
                    break;
            case DisplayCategory.Animation:
                    DisplayAnimationInfo();
                    break;
            case DisplayCategory.DiegeticSounds:
                    DisplayDiegeticSoundsInfo();
                    break;

            case DisplayCategory.GenexpertElements:
                    DisplayGenexpertInfo();
                    break;

            case DisplayCategory.Materials:
                    DisplayMaterialsInfo();
                    break;

            case DisplayCategory.BooleansAndMisc:
                    DisplayBooleansAndMiscInfo();
                break;     
        }

        //Save all changes made on the inspector
        serializedObject.ApplyModifiedProperties();
    }

    void DisplayAll()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("current"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableInstructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patientInstructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patientText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testText"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("instructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("instructionSource"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("instructionAudio"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentInstruction"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("confirmButton"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gloveBox"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("vacuumPack"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabPack"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("openVacuumPack"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabTop"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabBottom"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftNostril"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightNostril"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPressure"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPressure"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("testTube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeCap"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeSample"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeInverts"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("trashCan"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doorKnob"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("testGloveBox"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridge"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeLid"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeSampleArea"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgePivot"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pipette"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testTrash"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeScan"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCartridgeTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeCube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertHandle"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertCartridge"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertCartridgeOutline"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ghostTube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCap"));

        //PC
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHandModel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHandModel"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("closedHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pointHand"));

        //animators
        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bigPacketAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabPacketAnimator"));

        //snapTransforms
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSwabTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPressureTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSwabTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPressureTransform"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("sampleRoomTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testRoomTransform"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCartridgeTransform"));

        //diegeticsounds
        EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glovesOn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glovesOff"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vacuumPackSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeOpenSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabSnap"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeCloseSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("trashSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lidOpen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scannerBeep"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("screens"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("headers"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headerText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("template1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("template2"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("blue"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glass"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pipetteColor"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gloved"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("fingerPoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panel"));
    }

    void DisplayStateInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("current"));
    }
    void DisplayNonDiegeticUIInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableInstructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patientInstructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("patientText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testText"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("instructions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("instructionSource"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("instructionAudio"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentInstruction"));
    }
    void DisplayInteractiblesInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gloveBox"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("vacuumPack"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabPack"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("openVacuumPack"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabTop"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabBottom"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftNostril"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightNostril"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPressure"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPressure"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("testTube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeCap"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeSample"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeInverts"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("trashCan"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doorKnob"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("testGloveBox"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridge"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeLid"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeSampleArea"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgePivot"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pipette"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testTrash"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeScan"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCartridgeTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cartridgeCube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertCartridge"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertCartridgeOutline"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertHandle"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ghostTube"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCap"));
    }
    void DisplayPCElements()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pc"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHandModel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHandModel"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("closedHand"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pointHand"));
    }
    void DisplayAnimationInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("geneXpertAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bigPacketAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabPacketAnimator"));
    }
    void DisplaySnapTransformsInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSwabTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPressureTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSwabTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPressureTransform"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("sampleRoomTransform"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("testRoomTransform"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("tableCartridgeTransform"));
    }
    void DisplayDiegeticSoundsInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glovesOn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glovesOff"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vacuumPackSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeOpenSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("swabSnap"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tubeCloseSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("trashSound"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lidOpen"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scannerBeep"));
    }
    void DisplayGenexpertInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("screens"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("headers"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headerText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("template1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("template2"));
    }
    void DisplayMaterialsInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("blue"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("glass"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pipetteColor"));
    }
    void DisplayBooleansAndMiscInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gloved"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("fingerPoint"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("panel"));
    }
}

