using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GrommingMaster
{
    public class ResultPhase : PhaseManagement
    {
        [Header("Individual Properties")]
        [SerializeField] LevelManagement Level;
        [SerializeField] GameObject ResultCanvas;
        [SerializeField] Sprite QualifiedSprite;
        [SerializeField] Sprite UnqualifiedSprite;

        [Header("Customer Properties")]
        [SerializeField] Transform Customer;
        [SerializeField] SkinnedMeshRenderer CustomerReaction;
        [SerializeField] Image CustomerEmoji;
        [SerializeField] Animator CustomerAnimator;

        public CustomLevelController m_CustomLevelController;
        // Start is called before the first frame update
        void Start()
        {
            m_CustomLevelController = FindObjectOfType<CustomLevelController>();
            StartCoroutine(IE_StartPhase());
        }

        IEnumerator IE_StartPhase()
        {
            Camera.main.transform.DOMove(CameraTransform.position, 1f);
            Camera.main.transform.DORotate(CameraTransform.eulerAngles, 1f);
            
            CustomerAnimator.Play("Walking");
            Customer.DOLocalMoveX(0f, 1.5f);

            DogTransform.DORotate(new Vector3(0, 135, 0), 1f);

            yield return new WaitForSeconds(1.5f);

            Customer.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);
            ResultCanvas.SetActive(true);
            switch (Level.GetLevelResult())
            {
                case LevelManagement.Result.Qualified:
                    CustomerReaction.SetBlendShapeWeight(0, 100);
                    CustomerEmoji.sprite = QualifiedSprite;
                    CustomerAnimator.Play("Happy");
                    m_CustomLevelController.LevelState = CustomLevelController.State.Win;
                    break;

                case LevelManagement.Result.Unqualified:
                    CustomerReaction.SetBlendShapeWeight(1, 100);
                    CustomerEmoji.sprite = UnqualifiedSprite;
                    CustomerAnimator.Play("Angry");
                    m_CustomLevelController.LevelState = CustomLevelController.State.Lose;
                    break;
            }

            yield return new WaitForSeconds(0.5f);
            m_CustomLevelController.EndLevel();
        }
    }
}