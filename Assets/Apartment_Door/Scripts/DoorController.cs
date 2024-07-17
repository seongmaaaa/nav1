using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public bool keyNeeded = false;
    public bool gotKey;
    public GameObject keyGameObject;            // If player has Key,  assign it here
    public GameObject txtToDisplay;             // Display the information about how to close/open the door

    private bool playerInZone;                  // Check if the player is in the zone
    private bool doorOpened;                    // Check if door is currently opened or not

    private Animation doorAnim;
    private BoxCollider doorCollider;           // To enable the player to go through the door if door is opened else block him

    enum DoorState
    {
        Closed,
        Opened,
        Jammed
    }

    DoorState doorState = DoorState.Closed;     // To check the current state of the door

    /// <summary>
    /// Initial State of every variables
    /// </summary>
    private void Start()
    {
        doorOpened = false;                     // Is the door currently opened
        playerInZone = false;                   // Player not in zone

        // Ensure text display object is active only when player is in zone
        if (txtToDisplay != null)
            txtToDisplay.SetActive(false);

        // Get the Animation component from the parent game object
        doorAnim = transform.parent.gameObject.GetComponent<Animation>();

        // Get the BoxCollider component from the parent game object
        doorCollider = transform.parent.gameObject.GetComponent<BoxCollider>();

        // If Key is needed and the KeyGameObject is not assigned, log an error
        if (keyNeeded && keyGameObject == null)
        {
            Debug.LogError("Assign Key GameObject");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Display the interaction text when player enters the trigger zone
        if (txtToDisplay != null)
            txtToDisplay.SetActive(true);

        playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the interaction text when player exits the trigger zone
        if (txtToDisplay != null)
            txtToDisplay.SetActive(false);

        playerInZone = false;
    }

    private void Update()
    {
        // Check if the player is in the zone
        if (playerInZone)
        {
            // Update the interaction text based on the door state and player having the key
            if (doorState == DoorState.Opened)
            {
                if (txtToDisplay != null)
                    txtToDisplay.GetComponent<Text>().text = "Press 'E' to Close";
                doorCollider.enabled = false; // Disable collider when door is open
            }
            else if (doorState == DoorState.Closed || gotKey)
            {
                if (txtToDisplay != null)
                    txtToDisplay.GetComponent<Text>().text = "Press 'E' to Open";
                doorCollider.enabled = true; // Enable collider when door is closed or jammed
            }
            else if (doorState == DoorState.Jammed)
            {
                if (txtToDisplay != null)
                    txtToDisplay.GetComponent<Text>().text = "Needs Key";
                doorCollider.enabled = true; // Enable collider when door is jammed
            }
        }

        // Check if the player presses 'E' and is in the interaction zone
        if (Input.GetKeyDown(KeyCode.E) && playerInZone)
        {
            doorOpened = !doorOpened; // Toggle the door state

            // Handle the door animations and state transitions
            if (doorState == DoorState.Closed && !doorAnim.isPlaying)
            {
                if (!keyNeeded)
                {
                    doorAnim.Play("Door_Open");
                    doorState = DoorState.Opened;
                }
                else if (keyNeeded && !gotKey)
                {
                    if (doorAnim.GetClip("Door_Jam") != null)
                        doorAnim.Play("Door_Jam");
                    doorState = DoorState.Jammed;
                }
            }

            if (doorState == DoorState.Closed && gotKey && !doorAnim.isPlaying)
            {
                doorAnim.Play("Door_Open");
                doorState = DoorState.Opened;
            }

            if (doorState == DoorState.Opened && !doorAnim.isPlaying)
            {
                doorAnim.Play("Door_Close");
                doorState = DoorState.Closed;
            }

            if (doorState == DoorState.Jammed && !gotKey)
            {
                if (doorAnim.GetClip("Door_Jam") != null)
                    doorAnim.Play("Door_Jam");
                doorState = DoorState.Jammed;
            }
            else if (doorState == DoorState.Jammed && gotKey && !doorAnim.isPlaying)
            {
                doorAnim.Play("Door_Open");
                doorState = DoorState.Opened;
            }


        }
    }
}
