using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GrommingMaster
{
    public class Shaver : ToolMovement
    {
        [Header("Individual Properties")]
        [SerializeField] public ParticleSystem Fur;
        [SerializeField] public bool CanUse;

        int UILayer;
        // Start is called before the first frame update
        void Start()
        {
            UILayer = LayerMask.NameToLayer("UI");
        }

        // Update is called once per frame
        void Update()
        {
            switch (PhaseManagement.GameStatus)
            {
                case PhaseManagement.Status.Standing:
                case PhaseManagement.Status.Win:
                case PhaseManagement.Status.Lose:
                    SoundControl.Stop();
                    return;
            }

            // if tool is in Using-mode then set it CanUse for the Shaving Phase
            if (this.ToolStatus == Status.Using) CanUse = true;
            else CanUse = false;

            if (Input.GetMouseButton(0) && CanUse)
            {
                if (!SoundControl.isPlaying) SoundControl.Play();                
            }
            if (Input.GetMouseButtonUp(0))
            {
                SoundControl.Stop();                
            }            

            // if mouse-pointer is not on over the UI elements then make it moves
            if (!IsPointerOverUIElement(GetEventSystemRaycastResults()))
            {
                RotationZ(-180, 0, 1f);                
                UpdateHoldCircle();
            }
        }

        bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaycastResult)
        {
            // Going through every raycastResult
            for (int i = 0; i < eventSystemRaycastResult.Count; i++)
            {
                if (eventSystemRaycastResult[i].gameObject.layer == UILayer) // if a raycastResult is in the same layer with UI 
                {
                    return true;
                }
            }
            return false;
        }
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            // Get pointer from mouse
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            return raycastResults;
        }
    }
}