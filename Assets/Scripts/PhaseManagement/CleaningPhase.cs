using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace GrommingMaster
{
    public class CleaningPhase : PhaseManagement
    {
        [Header("Individual Properties")]
        [SerializeField] Step CurrentStep;
        [SerializeField] GameObject Hand;
        [SerializeField] Transform LeftHand;
        [SerializeField] Transform RightHand;
        [SerializeField] Transform Bottle;
        [SerializeField] Transform Shampoo;
        [SerializeField] GameObject Shower;        
        [SerializeField] GameObject ShampooHolder;

        [Header("UI Properties")]
        [SerializeField] Slider ProgressBar;

        private enum Step { Apply, Wash, Done }

        // Start is called before the first frame update
        private void Awake()
        {
            GameStatus = Status.Standing;
        }
        void Start()
        {
            StartPhase();
        }

        // Update is called once per frame
        void Update()
        {
            switch (GameStatus)
            {
                case Status.Standing:
                    return;
                case Status.Win:
                    EndEvent();
                    break;
            }
            UpdateStep();
        }
        void UpdateStep()
        {
            switch (CurrentStep)
            {
                case Step.Apply:
                    if (UpdateProgressBar("BubblePosition"))
                    {
                        CurrentStep = Step.Wash;
                        NextState();
                    }
                    break;
                case Step.Wash:
                    if (UpdateProgressBar("Bubble"))
                    {                                                
                        ShampooHolder.SetActive(false);
                        CurrentStep = Step.Done;
                        NextState();
                    }
                    break;
            }
        }
        bool UpdateProgressBar(string Steptype)
        {
            int count = 0;
            for (int i = 0; i < ShampooHolder.transform.childCount; i++)
            {
                if (ShampooHolder.transform.GetChild(i).CompareTag(Steptype)) count++;
            }
            ProgressBar.value = (ShampooHolder.transform.childCount - count);
            if (ProgressBar.value + 10 >= ProgressBar.maxValue && Input.GetMouseButtonUp(0))
            {
                ProgressBar.GetComponent<RectTransform>().DOAnchorPosY(25, 1f).OnComplete(() => ProgressBar.value = 0);
                return true;
            }
            return false;
        }

        void StartPhase()
        {
            CurrentStep = Step.Apply;
            Camera.main.transform.DORotate(CameraTransform.eulerAngles, 1);
            Camera.main.transform.DOMove(CameraTransform.position, 1.5f);
            NextState();
        }
        void NextState()
        {
            GameStatus = Status.Standing;
            switch (CurrentStep)
            {
                case Step.Apply:
                    Hand.SetActive(true);
                    Hand.transform.DOLocalMoveX(0, 1.5f).OnComplete(() =>
                    {
                        StartCoroutine(PrepareForWashing());
                    });

                    ProgressBar.maxValue = ShampooHolder.transform.childCount;                    
                    ProgressBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 1.5f);
                    break;
                case Step.Wash:
                    DogTransform.GetChild(0).gameObject.SetActive(false);
                    DogTransform.GetChild(1).gameObject.SetActive(false);                   

                    Shower.SetActive(true);
                    Shower.transform.DOLocalMoveX(0, 1.5f).OnComplete(() => GameStatus = Status.Playing);

                    Camera.main.transform.DOLocalMoveZ(-4f, 1f);
                    Hand.transform.DOMoveX(3, 1.5f).OnComplete(() => Hand.SetActive(false));

                    ProgressBar.maxValue = ShampooHolder.transform.childCount;
                    ProgressBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 1.5f);                    
                    break;
                case Step.Done:
                    StartCoroutine(EndPhase());
                    Shower.transform.DOMoveX(3, 1.5f).OnComplete(() => Shower.SetActive(false));
                    break;
            }
        }
        IEnumerator PrepareForWashing()
        {
            // Open Hand
            RightHand.localPosition = new Vector3(0, 0, 0);
            LeftHand.DOLocalRotate(new Vector3(0, 150, 0), 1);
            RightHand.DOLocalRotate(new Vector3(0, -150, 0), 1);
            Hand.transform.DOLocalRotate(new Vector3(30, 0, 0), 1);
            yield return new WaitForSeconds(1);

            // Move bottle
            Bottle.DOMove(new Vector3(0.2f, 4f, -0.5f), 0.5f);
            Bottle.DOLocalRotate(new Vector3(0, 0, 135), 0.75f);
            yield return new WaitForSeconds(0.75f);

            // Shampoo came out and drop onto hands
            Bottle.GetChild(0).gameObject.SetActive(true);
            Shampoo.DOScale(new Vector3(0.15f, 0.05f, 0.15f), 0.5f);
            yield return new WaitForSeconds(0.5f);

            // Shampoo disappear and bottle move away
            Shampoo.gameObject.SetActive(false);            
            Bottle.GetChild(0).gameObject.SetActive(false);
            Bottle.DOMoveX(3.5f, 0.5f);

            // Mixing Shampoo on hands
            Hand.transform.GetChild(0).gameObject.SetActive(true);
            LeftHand.DOLocalRotate(new Vector3(0, 90, 0), 0.15f);
            RightHand.DOLocalRotate(new Vector3(0, -90, 0), 0.15f);

            LeftHand.DOLocalMoveZ(0.001f, 0.25f);
            RightHand.DOLocalMoveZ(-0.001f, 0.25f);
            yield return new WaitForSeconds(0.25f);

            LeftHand.DOLocalMoveZ(-0.001f, 0.25f);
            RightHand.DOLocalMoveZ(0.001f, 0.25f);
            yield return new WaitForSeconds(0.25f);

            LeftHand.DOLocalMoveZ(0, 0.25f);
            RightHand.DOLocalMoveZ(0, 0.25f);
            yield return new WaitForSeconds(0.25f);

            // Hand comeback to normal position
            LeftHand.DOLocalRotate(Vector3.zero, 0.1f);
            RightHand.DOLocalRotate(Vector3.zero, 0.1f);
            RightHand.DOLocalMoveX(0.0005f, 0.25f);

            this.GetComponent<PaintIn3D.P3dPaintSphere>().Radius = 0.2f;
            GameStatus = Status.Playing;
            yield return null;
        }
        IEnumerator EndPhase()
        {
            SparkleParticles.SetActive(true);
            yield return new WaitForSeconds(2);
            CurrentPhase.SetActive(false);
            NextPhase.GetComponent<PhaseManagement>().GameStatus = Status.Playing;
            SparkleParticles.SetActive(false);
        }
    }
}