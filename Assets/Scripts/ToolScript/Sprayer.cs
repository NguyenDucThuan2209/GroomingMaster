using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

namespace GrommingMaster
{
    public class Sprayer : ToolMovement
    {
        [Header("Individual Properties")]
        public MeshRenderer ColorPocket;
        public bool CanUse;
        public Transform SprayEffect;

        [Header("Turning Properties")]
        public float CurrentRotationY;

        int UILayer;

        private void Start()
        {
            UILayer = LayerMask.NameToLayer("UI");
        }
        private void Update()
        {
            switch (PhaseManagement.GameStatus)
            {
                case PhaseManagement.Status.Standing:
                case PhaseManagement.Status.Win:
                case PhaseManagement.Status.Lose:
                    return;
            }

            if (ToolStatus == Status.Using)
            {
                if (!SoundControl.isPlaying) SoundControl.Play();
                CanUse = true;
                SprayEffect.gameObject.SetActive(true);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    Debug.DrawRay(this.transform.position, ray.direction, Color.black);
                    SprayEffect.LookAt(hit.point);
                }         
                else
                {
                    
                }
            }
            else
            {
                CanUse = false;
                SoundControl.Stop();
                SprayEffect.gameObject.SetActive(false);
            }

            if (IsPointerOverUIElement())
            {
                //this.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
                //this.transform.localPosition = new Vector3(-0.2f, 2.5f, -3.5f);
            }
            else
            {
                //RotationY(CurrentRotationY - 90, CurrentRotationY + 90, Offset);
                MoveModel(Offset);
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