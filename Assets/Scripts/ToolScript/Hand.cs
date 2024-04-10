using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrommingMaster
{
    public class Hand : ToolMovement
    {
        [Header("Individual Properties")]
        [SerializeField] GameObject Shampoo;
        Ray ray;

        // Start is called before the first frame update
        void Start()
        {
            if (IsHoldToUse) ToolStatus = Status.Holding;
            else ToolStatus = Status.Using;
            this.IsHoldToUse = true;
        }

        // Update is called once per frame
        void Update()
        {
            switch (PhaseManagement.GameStatus)
            {
                case PhaseManagement.Status.Standing:
                case PhaseManagement.Status.Win:
                case PhaseManagement.Status.Lose:
                    return;
            }

            // Movement
            RotationZ(-180, 0, 3f);
            // Operation
            if (Input.GetMouseButton(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hitObjects = Physics.SphereCastAll(transform.position, 0.1f, ray.direction);
                for (int i = 0; i < hitObjects.Length; i++)                
                {
                    if (hitObjects[i].transform.CompareTag("BubblePosition"))
                    {
                        Instantiate(Shampoo, hitObjects[i].transform.position, Quaternion.identity, hitObjects[i].transform.parent);
                        Destroy(hitObjects[i].transform.gameObject);
                    }
                }
            }
        }
    }
}