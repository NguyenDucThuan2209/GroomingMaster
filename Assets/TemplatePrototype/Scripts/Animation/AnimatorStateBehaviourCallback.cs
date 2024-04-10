using System;
using UnityEngine;

public class AnimatorStateBehaviourCallback : StateMachineBehaviour
{
    public event Action<Animator, AnimatorStateInfo, int> onStateEnter = delegate { };
    public event Action<Animator, AnimatorStateInfo, int> onStateExit = delegate { };

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateEnter?.Invoke(animator, stateInfo, layerIndex);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onStateExit?.Invoke(animator, stateInfo, layerIndex);
    }
}
