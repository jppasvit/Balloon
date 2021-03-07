using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCharacterAnimatorToggle : MonoBehaviour
{
    [SerializeField]
    private string[] animationStates;

    private int current = 0;

    [SerializeField]
    private Animator characterAnimator = null;
    
    public void SetAnimationTrigger(string triggerName)
    {
       GetComponent<ARDebugManager>().LogError($"SetAnimationTrigger {triggerName}");

        foreach(var animationState in animationStates)
        {
            if(animationState == triggerName)
            {
                characterAnimator.SetBool(animationState, true);
            }
            else
                characterAnimator.SetBool(animationState, false);
        }
    }

    public void Cycle()
    {
        if(characterAnimator == null || animationStates.Length == 0)
        {
           GetComponent<ARDebugManager>().LogError("Character animator is not set");
            return;   
        }

        current++;

        //change trigger animation
        characterAnimator.SetTrigger(animationStates[0]);

        //check for max
        if(current >= animationStates.Length) current= 0;
    }
}
