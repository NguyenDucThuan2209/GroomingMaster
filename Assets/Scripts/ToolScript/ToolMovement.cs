using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace GrommingMaster
{
    public class ToolMovement : MonoBehaviour
    {
        [Header("Tool Properties")]
        [SerializeField] protected PhaseManagement PhaseManagement;
        [SerializeField] protected bool IsHoldToUse;
        [SerializeField] protected Status ToolStatus;
        [SerializeField] protected float Offset;
        [SerializeField] protected AudioSource SoundControl;
        public enum Status { Holding, Using }

        [Header("UI Properties")]
        [SerializeField] protected Canvas HoldCircle;
        [SerializeField] protected Image HoldDuration;

        float horizontalAmount, verticalAmount;

        protected void UpdateHoldCircle()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HoldCircle.gameObject.SetActive(true);
            }
            if (Input.GetMouseButton(0))
            {
                if (HoldDuration.fillAmount < 1)
                {
                    HoldDuration.fillAmount += Time.deltaTime * 2;
                }
                else
                {
                    HoldCircle.gameObject.SetActive(false);
                    ToolStatus = Status.Using;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                HoldCircle.gameObject.SetActive(false);
                HoldDuration.fillAmount = 0;
                ToolStatus = Status.Holding;                
            }
            HoldCircle.transform.LookAt(Camera.main.transform.position);
        }

        protected void RotationX(float min, float max, float offset)
        {
            if (Input.GetMouseButton(0))
            {
                // Rotation
                horizontalAmount += Input.GetAxis("Mouse X") * Time.deltaTime * 1500;
                horizontalAmount = Mathf.Clamp(horizontalAmount, min, max); 

                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(horizontalAmount, rotation.y, rotation.z);

                // Position
                if (IsHoldToUse) MoveModel(offset);
                else MoveModel();
            }
            if (Input.GetMouseButtonUp(0))
            {
                horizontalAmount = 0;
                transform.DOLocalRotate(Vector3.zero, 1f);                
            }
        }
        protected void RotationY(float min, float max, float offset)
        {
            if (Input.GetMouseButton(0))
            {
                // Rotation
                horizontalAmount += Input.GetAxis("Mouse X") * Time.deltaTime * 1500;
                horizontalAmount = Mathf.Clamp(horizontalAmount, min, max);

                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(rotation.x, horizontalAmount, rotation.z);

                // Position
                if (IsHoldToUse) MoveModel(offset);
                else MoveModel();
            }
            if (Input.GetMouseButtonUp(0))
            {
                horizontalAmount = 0;
                transform.DOLocalRotate(Vector3.zero, 1f);
            }
        }
        protected void RotationZ(float min, float max, float offset)
        {
            if (Input.GetMouseButton(0))
            {
                // Rotation
                verticalAmount += Input.GetAxis("Mouse Y") * Time.deltaTime * 1500;
                verticalAmount = Mathf.Clamp(verticalAmount, min, max); // 0 180

                Vector3 rotation = transform.localEulerAngles;
                transform.localEulerAngles = new Vector3(rotation.x, rotation.y, verticalAmount);

                // Position
                if (IsHoldToUse) MoveModel(offset);
                else MoveModel();
            }
            if (Input.GetMouseButtonUp(0))
            {
                verticalAmount = 0;
                transform.DOLocalRotate(new Vector3(0, transform.localEulerAngles.y, 0), 1f);
            }
        }        

        protected void MoveModel(float offset)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.blue);
            this.transform.position = ray.GetPoint(offset);
        }
        protected void MoveModel()
        {
            float xMove = this.transform.position.x + Input.GetAxis("Mouse X") * 0.15f;
            float yMove = this.transform.position.y + Input.GetAxis("Mouse Y") * 0.15f;
            float zMove = this.transform.position.z;

            this.transform.position = new Vector3(xMove, yMove, zMove);
        }
    }
}