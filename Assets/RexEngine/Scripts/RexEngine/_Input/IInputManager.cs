/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
    public interface IInputManager
    {
        bool Enabled { get; set; }
        ITouchInputManager TouchInputManager { get; }
        bool GetButton(int playerId, InputAction action);
        bool GetButtonDown(int playerId, InputAction action);
        bool GetButtonUp(int playerId, InputAction action);
        float GetAxis(int playerId, InputAction action);
    }
}
