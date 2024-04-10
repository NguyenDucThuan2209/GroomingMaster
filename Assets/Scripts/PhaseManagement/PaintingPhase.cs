using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using PaintIn3D;
using System;

namespace GrommingMaster
{
    public class PaintingPhase : PhaseManagement
    {
        [Header("Individual Properties")]
        [SerializeField] P3dPaintSphere PaintManagement;
        [SerializeField] Transform DogFur;
        [SerializeField] Transform DogPaint;

        [SerializeField] Transform SprayGun;
        [SerializeField] float PaintRadius;
        [SerializeField] Image TargetImage;
        [SerializeField] Transform Stencil;                

        [SerializeField] ParticleSystem CloudEffect;
        [SerializeField] Transform hitPoints;

        [Header("UI Properties")]
        [SerializeField] RectTransform ColorPanel;
        [SerializeField] RectTransform TargetPanel;        

        bool isUseRightColor;
        bool isFullyPainted;
        Image currentButton;
        
        // Turning Properties        
        Side turnDirection;
        public enum Side { None, Left, Right }
        
        // Start is called before the first frame update
        void Start()
        {
            StartPhase();
        }

        // Update is called once per frame
        void Update()
        {
            switch (GameStatus)
            {
                case Status.Win:
                case Status.Lose:
                case Status.Standing:
                    return;
            }

            // Turn on or off the spray gun        
            if (SprayGun.GetComponent<Sprayer>().CanUse)
            {
                PaintManagement.Radius = PaintRadius;                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.transform.CompareTag("Bubble"))
                    {
                        Destroy(hit.transform.gameObject);                        
                    }
                }                
            }
            else
            {               
                PaintManagement.Radius = 0f;
                if (hitPoints.childCount - 1 <= 0)
                {
                    isFullyPainted = true;
                }
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                // rotate the camera & shaver
                switch (turnDirection)
                {
                    case Side.Left:                        
                        Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, 50 * Time.deltaTime);
                        SprayGun.RotateAround(DogTransform.position, SprayGun.up, 50 * Time.deltaTime);                        
                        break;
                    case Side.Right:
                        Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, -50 * Time.deltaTime);
                        SprayGun.RotateAround(DogTransform.position, SprayGun.up, -50 * Time.deltaTime);                        
                        break;
                }
                
            }
            if (Input.GetMouseButtonUp(0))
            {
                SprayGun.GetComponent<Sprayer>().CurrentRotationY = SprayGun.transform.localEulerAngles.y;
                turnDirection = Side.None;
            }
        }             
        public void SetColor(Image image)
        {
            if (currentButton != null) currentButton.transform.GetChild(0).gameObject.SetActive(false);
            currentButton = image;
            currentButton.transform.GetChild(0).gameObject.SetActive(true); 

            if (image.color == TargetImage.color) isUseRightColor = true;

            PaintManagement.Color = image.color;
            SprayGun.GetComponent<Sprayer>().ColorPocket.material.color = image.color;
            
            // Set material for particles system
            var main = SprayGun.GetComponent<Sprayer>().SprayEffect.GetComponent<ParticleSystem>().main;
            main.startColor = image.color; 

            main = SprayGun.GetComponent<Sprayer>().SprayEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;            
            main.startColor = new ParticleSystem.MinMaxGradient(image.color);
            main = SprayGun.GetComponent<Sprayer>().SprayEffect.transform.GetChild(1).GetComponent<ParticleSystem>().main;
            main.startColor = new ParticleSystem.MinMaxGradient(image.color);
        }
        public void TurnCamera(bool type)
        {
            // true is right, false is left
            if (type)
            {
                turnDirection = Side.Right;
            }
            else
            {
                turnDirection = Side.Left;
            }
        }

        void StartPhase()
        {
            StartCoroutine(IE_StartPhase());
        }
        public void EndPhase()
        {
            StartCoroutine(IE_EndPhase());
        }

        IEnumerator IE_StartPhase()
        {
            yield return new WaitWhile(() => this.GameStatus == Status.Standing);

            if (DogFur != null)
            {
                DogFur.gameObject.AddComponent<P3dPaintable>();
                DogFur.gameObject.AddComponent<P3dPaintableTexture>();
                DogFur.gameObject.AddComponent<P3dMaterialCloner>();
            }

            DogPaint.gameObject.SetActive(true);
            DogPaint.gameObject.AddComponent<P3dPaintable>();
            DogPaint.gameObject.AddComponent<P3dPaintableTexture>();
            DogPaint.gameObject.AddComponent<P3dMaterialCloner>();

            Camera.main.transform.DOMove(CameraTransform.position, 1f);
            Camera.main.transform.DORotate(CameraTransform.eulerAngles, 1f);

            // Moving UI
            ColorPanel.DOAnchorPosY(150, 1.5f);
            TargetPanel.DOAnchorPosY(-250, 1.5f);            

            if (Stencil != null)
            {
                Stencil.gameObject.SetActive(true);
                Stencil.DOLocalRotate(new Vector3(0, 0, 0), 1f);
            }
            hitPoints.gameObject.SetActive(true);

            SprayGun.gameObject.SetActive(true);
            SprayGun.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
            SprayGun.transform.DOLocalMove(new Vector3(0f, 2.5f, -1f), 1.5f);
        }
        IEnumerator IE_EndPhase()
        {
            if (isUseRightColor && isFullyPainted)
            {
                this.GameStatus = Status.Win;
            }
            else
            {
                this.GameStatus = Status.Lose;
            }
            
            CloudEffect.Play();
            SprayGun.gameObject.SetActive(false);
            if (Stencil != null) Stencil.DOLocalRotate(new Vector3(0, 90, 0), 1).OnComplete(() => Stencil.gameObject.SetActive(false));

            yield return new WaitWhile(() => CloudEffect.isPlaying);

            SparkleParticles.SetActive(true);
            yield return new WaitForSeconds(2);

            SparkleParticles.SetActive(false);
            this.CurrentPhase.SetActive(false);
            this.NextPhase.SetActive(true);
        }
    }
}