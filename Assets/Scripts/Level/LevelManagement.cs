using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrommingMaster
{
    public class LevelManagement : MonoBehaviour
    {        
        [SerializeField] PhaseManagement[] TargetResultPhases;

        public enum Result { Qualified, Unqualified}

        public Result GetLevelResult()
        {
            for (int i = 0; i < TargetResultPhases.Length; i++)
            {
                if (TargetResultPhases[i].GameStatus != PhaseManagement.Status.Win) return Result.Unqualified;
            }
            
            return Result.Qualified;
        }
    }
}