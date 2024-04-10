using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrommingMaster
{
    public class Shower : ToolMovement
    {
        [Header("Individual Properties")]
        [SerializeField] ParticleSystem WaterEffect;

        Ray ray;

        private void Update()
        {
            switch (PhaseManagement.GameStatus)
            {
                case PhaseManagement.Status.Standing:
                case PhaseManagement.Status.Win:
                case PhaseManagement.Status.Lose:
                    WaterEffect.gameObject.SetActive(false);
                    SoundControl.Stop();
                    return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                SoundControl.Play();                
                WaterEffect.gameObject.SetActive(true);
            }
            if (Input.GetMouseButton(0))
            {
                MoveModel();
                //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hitObjects = Physics.SphereCastAll(new Ray(transform.position, transform.right), 0.15f);
                for (int i = 0; i < hitObjects.Length; i++)
                {
                    if (hitObjects[i].transform.CompareTag("Bubble"))
                    {
                        hitObjects[i].transform.tag = "Untagged";
                        hitObjects[i].transform.DOScale(0, 0.5f).OnComplete(() => Destroy(hitObjects[i].transform.gameObject));
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                SoundControl.Stop();
                WaterEffect.gameObject.SetActive(false);
            }
        }
    }
}