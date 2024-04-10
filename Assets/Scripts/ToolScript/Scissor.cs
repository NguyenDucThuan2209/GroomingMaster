using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GrommingMaster
{
    public class Scissor : ToolMovement
    {
        [Header("Individual Properties")]
        public ParticleSystem Fur;
        public bool CanUse;
        public Animator ScissorAnimator;

        int UILayer;

        // Start is called before the first frame update
        void Start()
        {
            UILayer = LayerMask.NameToLayer("UI");
        }

        // Update is called once per frame
        void Update()
        {
            if (ToolStatus == Status.Using)
            {
                CanUse = true;
                ScissorAnimator.speed = 2.5f;
            }
            else
            {
                CanUse = false;
                ScissorAnimator.speed = 0;
            }
            
            if (Input.GetMouseButton(0) && CanUse && !SoundControl.isPlaying)
            {
                SoundControl.Play();                
            }
            if (Input.GetMouseButtonUp(0))
            {
                SoundControl.Stop();                
            }

            if (IsPointerOverUIElement())
            {
                this.transform.DOLocalRotate(new Vector3(0, transform.localEulerAngles.y, 0), 1f);
                //this.transform.localPosition = new Vector3(-0.2f, 2.5f, -3.5f);
            }
            else
            {
                RotationZ(-180, 0, Offset);
                //RotationToDirection(5, 500);
                UpdateHoldCircle();
            }
        }

        //Returns 'true' if we touched or hovering on Unity UI element.
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return true;
            }
            return false;
        }


        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}