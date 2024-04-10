using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrommingMaster
{
    public class PhaseManagement : MonoBehaviour
    {
        [Header("Common Level Properties")]
        public Status GameStatus;
        public enum Status { Standing, Playing, Win, Lose }

        [SerializeField] protected GameObject CurrentPhase;
        [SerializeField] protected GameObject NextPhase;
        [SerializeField] protected Transform CameraTransform;
        [SerializeField] protected Transform DogTransform;
        [SerializeField] protected GameObject SparkleParticles;

        protected virtual void StartEvent() { }
        protected virtual void EndEvent() { }
    }
}