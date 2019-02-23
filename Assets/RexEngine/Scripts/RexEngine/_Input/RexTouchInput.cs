/* Copyright Sky Tyrannosaur */

using UnityEngine;
using System.Collections;

namespace RexEngine
{
    public class RexTouchInput:RexInput
    {
        public void ToggleTouchInterface(bool willShow)
        {
            if(InputManager.Instance.TouchInputManager != null) 
			{
				InputManager.Instance.TouchInputManager.ToggleTouchInterface(willShow);
			}
        }
    }
}