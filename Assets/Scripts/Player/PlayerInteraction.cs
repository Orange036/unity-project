using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class PlayerInteraction : MonoBehaviour
{

    [SerializeField] private Inventory inventory;

    [SerializeField, Range(0, 3)] private int currentPosition;

    public GameObject player;
    public Transform holdPos;

    private int LayerNumber; //layer index

    private float throwForce = 500f; //force at which the object is thrown at
    private float pickUpRange = 100f; //how far the player can pickup the object from
    private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private float interactRange = 10f;

    private GameObject heldObj; //object which we pick up

    private Rigidbody heldObjRb; //rigidbody of object we pick up

    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private bool isRotating = false;

    private InputAction scrollAction;

    public event EventHandler CurrentPositionChanged;

    private Controls actions;

    void Start()
    {
        scrollAction = actions.Player.ScrolHotbar;

        Cursor.lockState = CursorLockMode.Locked;

        actions.Player.Throw.performed += ThrowAction;
        actions.Player.PickUp.performed += PickUpAction;
        actions.Player.Rotate.performed += RotateObjectAction;
        actions.Player.Rotate.canceled += ReleaseRotateObjectAction;
        actions.Player.Interaction.performed += OnInteraction;
        actions.Player.TakeFromInventory.performed += TakeFromInventoryPerformed;
        actions.Player.Log.performed += OnLog;
        actions.Player.ScrolHotbar.performed += OnScrolHotbar;
        LayerNumber = LayerMask.NameToLayer("holdLayer");

    }

    void Update()
    {
        if (isRotating && heldObj != null)
        {
            InputAction lookAction = actions.Player.Look;
            float XaxisRotation = 0f;
            float YaxisRotation = 0f;
            Vector2 vector2 = lookAction.ReadValue<Vector2>();
            XaxisRotation += vector2.y * rotationSensitivity;
            YaxisRotation += vector2.x * rotationSensitivity;
            heldObj.transform.Rotate(new Vector3(XaxisRotation, YaxisRotation, 0f), Space.Self);
        }


        //Gizmo tracking
        Ray r = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitinfo, interactRange))
        {
            if (hitinfo.collider.TryGetComponent(out IInteractable interactable))
            {
                Transform interactableTransformn = interactable.GetTransform();
                Debug.DrawLine(transform.position, interactableTransformn.position, Color.green);
            }
        }
    }

    private void ThrowAction(InputAction.CallbackContext context)
    {
        if (heldObj != null) //if player is holding object
        {
            if (canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                StopClipping();
                ThrowObject();
            }

        }
    }

    private void PickUpAction(InputAction.CallbackContext context) 
    {
        if (heldObj == null) //if currently not holding anything
        {
            //perform raycast to check if player is looking at object within pickuprange
            Ray r = new Ray(transform.position, transform.forward);
            bool rayCastHit = Physics.Raycast(r, out RaycastHit hit, pickUpRange);
            if (rayCastHit)
            {
                //make sure pickup tag is attached
                if (hit.transform.gameObject.tag == "canPickUp")
                {
                    //pass in object hit into the PickUpObject function
                    heldObj = hit.transform.gameObject; //assign heldObj to the object that was hit by the raycast (no longer == null)
                    heldObjRb = hit.transform.gameObject.GetComponent<Rigidbody>(); //assign Rigidbody
                    heldObjRb.isKinematic = true;
                    heldObj.transform.position = holdPos.transform.position;
                    heldObjRb.transform.parent = holdPos.transform; //parent object to holdposition
                    heldObj.layer = LayerNumber; //change the object layer to the holdLayer
                    heldObj.GetComponent<Collider>().enabled = false;
                                                 //make sure object doesnt collide with player, it can cause weird bugs
                }
            }
        }
        else
        {
            if (canDrop == true)
            {
                heldObj.GetComponent<Collider>().enabled = true;
                StopClipping(); //prevents object from clipping through walls
                DropObject();
            }
        }
    }

    void DropObject()
    {
        //re-enable collision with player
        heldObj.layer = 0; //object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //unparent object
        heldObj = null; //undefine game object
    }

    void RotateObjectAction(InputAction.CallbackContext context)
    {
        if(heldObj != null)
        {
            PlayerMovement playerActions = player.GetComponent<PlayerMovement>();
            playerActions.actions.Disable();

            Debug.Log("rotate");
            canDrop = false; //make sure throwing can't occur during rotating
            isRotating = true;
            //disable player being able to look around
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;
        }
    }

    private void ReleaseRotateObjectAction(InputAction.CallbackContext context)
    {   
        PlayerMovement playerActions = player.GetComponent<PlayerMovement>();
        playerActions.actions.Enable();
        canDrop = true;
        isRotating = false;
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        heldObj.GetComponent<Collider>().enabled = true;
        StopClipping();
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    private void OnInteraction(InputAction.CallbackContext context)
    {
        Ray r = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(r, out RaycastHit hitinfo, interactRange))
        {
            if (hitinfo.collider.TryGetComponent(out ItemData item))
            {
                
                bool insertionStatus = inventory.AddInInventory(currentPosition, item);
                if (insertionStatus)
                {
                    item.pefabOfItself.SetActive(false);
                }
            }
        }
    }

    private void TakeFromInventoryPerformed(InputAction.CallbackContext callback)
    {
        GameObject prefabOfItself = inventory.RemoveFromInventory(currentPosition);
        if (prefabOfItself != null)
        {
            Ray r = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            prefabOfItself.transform.position = holdPos.position;
            prefabOfItself.SetActive(true);
        }
    }

    private void OnLog(InputAction.CallbackContext context)
    {
        inventory.ClearInventory();
    }

    private void OnScrolHotbar(InputAction.CallbackContext context)
    {
        if(scrollAction.ReadValue<float>()  > 0)
        {
            if(currentPosition < 3)
            {
                currentPosition++;
            }
            else
            {
                currentPosition = 0;
            }
        }
        else
        {
            if(currentPosition < 1)
            {
                currentPosition = 3;
            }
            else
            {
                currentPosition--;
            }
        }

        CurrentPositionChanged?.Invoke(currentPosition, EventArgs.Empty);
    }

    private void OnEnable()
    {
        actions = new Controls();
        actions.Enable();
    }
}