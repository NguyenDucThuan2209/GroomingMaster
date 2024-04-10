using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

namespace GrommingMaster
{
    public class ReceptionPhase : PhaseManagement
    {
        [Header("Individual Properties")]
        [SerializeField] GameObject Customer;
        [SerializeField] Animator CustomerAnimator;

        [Header("UI Properties")]
        [SerializeField] GameObject TargetCanvas;
        [SerializeField] Image TargetImage;
        [SerializeField] Button NextPhaseButton;

        private void Awake()
        {
            CustomerAnimator = Customer.GetComponent<Animator>();            
        }
        // Start is called before the first frame update
        void Start()
        {
            StartEvent();
        }

        public void OnEndPhaseButtonClicked()
        {
            TargetImage.gameObject.SetActive(false);
            NextPhaseButton.gameObject.SetActive(false);
            EndEvent();
        }

        void StartPhase()
        {
            Camera.main.transform.DOMove(CameraTransform.position, 0.5f);
            Camera.main.transform.DORotate(CameraTransform.eulerAngles, 0.5f);

            DogTransform.DOLocalRotate(new Vector3(0, DogTransform.localEulerAngles.y - 45, 0), 0.5f);

            TargetImage.DOFade(1, 0.5f).OnComplete(() => NextPhaseButton.gameObject.SetActive(true));
        }
        IEnumerator EndPhase()
        {
            TargetCanvas.SetActive(false);

            CustomerAnimator.Play("Walking");
            Customer.transform.DOLocalRotate(new Vector3(0, 270, 0), 0.15f).
            OnComplete(() =>
            {
                Customer.transform.DOLocalMoveX(-5, 2f);
            });
            
            yield return new WaitForSeconds(1.5f);

            DogTransform.DOLocalRotate(new Vector3(0, DogTransform.localEulerAngles.y + 45, 0), 0.5f);

            CurrentPhase.SetActive(false);
            NextPhase.GetComponent<PhaseManagement>().GameStatus = Status.Playing;
            NextPhase.SetActive(true);
        }

        protected override void StartEvent()
        {
            StartPhase();
        }
        protected override void EndEvent()
        {
            StartCoroutine(EndPhase());
        }
    }
}