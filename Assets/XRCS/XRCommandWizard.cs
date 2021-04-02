#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEditor;


public class XRCommandWizard : ScriptableWizard
{
    /*
     * TO THE MOMO OF THE FUTURE
     * 
     * 1. Look at how to generate instances of generic types at runtime (Bookmarked on chrome)
     * 2. For each string of each feature populate a Dictionary<string, System.Type> appropriately.
     * 3. Use dictionary along with feature string of command to create generic types for TryGetFeatureValue() using the constructor for InputFeatureThingandlasbdka<T0>(string)
     * 4. If this doesn't work scrap everything
     * 
     */


    // List of commands in XRprocessor
    public List<Command> commands;
    
    // The gameObject responsible for processing all XR Commands
    public XRProcessor XRprocessor;

    // The new command that will be added to XRProcessor
    public Command commandToAdd;

    // The index of a command to delete
    public int commandIndexToDelete;

    public InputFeatureUsage<float> use = CommonUsages.batteryLevel;

    [MenuItem("XRInput/XR Command Editor")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<XRCommandWizard>("XR Command Editor", "Add Command", "Delete Command");
    }

    void OnWizardCreate()
    {
        // Sets command name as command's name and pushes it into the list.
        commandToAdd.name = commandToAdd.commandName;
        XRprocessor.allCommands.Add(commandToAdd);
    }

    private void OnWizardOtherButton()
    {
        // Deletes command at index commandIndexToDelete
        if (commandIndexToDelete >= 0 && commandIndexToDelete < commands.Count)
        {
            XRprocessor.allCommands.RemoveAt(commandIndexToDelete);
            OnWizardUpdate();
        }
    }

    void OnWizardUpdate()
    {
        // Creates new commandToAdd when the wizard is created
        if (commandToAdd == null)
        {
            commandToAdd = ScriptableObject.CreateInstance<Command>();
        }

        // Pulls in XRProcessor object if available.
        if(XRprocessor == null)
        {
            XRprocessor = (XRProcessor)FindObjectOfType(typeof(XRProcessor));
        }

        // Updates list of commands and the names of each command
        if(XRprocessor != null)
        {
            commands = new List<Command>();

            foreach (Command c in XRprocessor.allCommands)
            {
                c.name = c.commandName;
                commands.Add(c);
            }
        }
    }

}
#endif