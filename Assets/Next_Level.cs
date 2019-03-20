﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Next_Level : MonoBehaviour
{

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

    private Interactable interactable;

    // Use this for initialization
    void Start()
    {
        interactable = this.GetComponent<Interactable>();
    }

    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

        if (startingGrabType != GrabTypes.None)
        {
            // Call this to continue receiving HandHoverUpdate messages,
            // and prevent the hand from hovering over anything else
            hand.HoverLock(interactable);

            // Attach this object to the hand
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        }
        else if (isGrabEnding)
        {
            List<GameObject> list = GameObject.Find("Cube").GetComponent<Genirator>().elements;
            while (list.Count != 0)
            {
                Destroy(list[0]);
                list.RemoveAt(0);
            }
            transform.position = new Vector3(-1, 1, 5);
            transform.rotation = new Quaternion(0, 0, 0, 1);
            GameObject.Find("Cube").GetComponent<Genirator>().count++;
            GameObject.Find("Cube").GetComponent<Genirator>().Generator();
            // Detach this object from the hand
            hand.DetachObject(gameObject);

            // Call this to undo HoverLock
            hand.HoverUnlock(interactable);

        }
    }
}