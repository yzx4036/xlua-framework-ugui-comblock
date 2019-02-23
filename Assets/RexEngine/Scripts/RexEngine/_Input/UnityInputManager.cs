/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RexEngine
{
    public class UnityInputManager:InputManager
    {
		public bool usePrefix = false;

        protected string playerAxisPrefix = "";
        protected int maxNumberOfPlayers = 2;

        [Header("Unity Axis Mappings")]
        [SerializeField]
        protected string jumpAxis = "Jump";
        [SerializeField]
        protected string attackAxis = "Fire1";
        [SerializeField]
        protected string subAttackAxis = "Fire2";
		[SerializeField]
		protected string subAttack_2Axis = "Fire3";
		[SerializeField]
		protected string subAttack_3Axis = "Fire4";
        [SerializeField]
        protected string dashAxis = "Dash";
        [SerializeField]
        protected string runAxis = "Run";
        [SerializeField]
        protected string horizontalAxis = "Horizontal";
        [SerializeField]
        protected string verticalAxis = "Vertical";
        [SerializeField]
        protected string pauseAxis = "Submit";
		[SerializeField]
		protected string misc_1Axis = "Misc1";
		[SerializeField]
		protected string misc_2Axis = "Misc2";

        public Dictionary<int, string>[] actions;

        protected override void Awake()
        {
            base.Awake();
            
            //Do not override existing input sources so 3rd party managers can override this
            if(InputManager.Instance != null) 
			{
                Enabled = false; // disable this input manager to save resources
				return;
			}

            SetInstance(this); //Set this as the singleton instance

			//Use "PlayerX" as a prefix for each input, but only if there are 2 players or more 
			//(i.e. this will be "Fire1" if there's only one player, but "Player0Fire1" and "Player1Fire1" if there are more)
			if(GameManager.Instance.settings.numberOfPlayers > 1 || usePrefix)
			{
				playerAxisPrefix = "Player";
			}

			maxNumberOfPlayers = GameManager.Instance.settings.numberOfPlayers;

            //Set up Actions dictionary for each player
            actions = new Dictionary<int, string>[maxNumberOfPlayers];
            for(int i = 0; i < maxNumberOfPlayers; i++)
            {
                Dictionary<int, string> playerActions = new Dictionary<int, string>();
                actions[i] = playerActions;
                string prefix = !string.IsNullOrEmpty(playerAxisPrefix) ? playerAxisPrefix + i : string.Empty;
                AddAction(InputAction.Jump, prefix + jumpAxis, playerActions);
                AddAction(InputAction.Attack, prefix + attackAxis, playerActions);
                AddAction(InputAction.SubAttack, prefix + subAttackAxis, playerActions);
				AddAction(InputAction.SubAttack_2, prefix + subAttack_2Axis, playerActions);
				AddAction(InputAction.SubAttack_3, prefix + subAttack_3Axis, playerActions);
                AddAction(InputAction.Dash, prefix + dashAxis, playerActions);
                AddAction(InputAction.Run, prefix + runAxis, playerActions);
                AddAction(InputAction.MoveHorizontal, prefix + horizontalAxis, playerActions);
                AddAction(InputAction.MoveVertical, prefix + verticalAxis, playerActions);
                AddAction(InputAction.Pause, prefix + pauseAxis, playerActions);
				AddAction(InputAction.Misc_1, prefix + misc_1Axis, playerActions);
				AddAction(InputAction.Misc_2, prefix + misc_2Axis, playerActions);

            }
        }

        public override bool GetButton(int playerId, InputAction action)
        {
            bool value = Input.GetButton(actions[playerId][(int)action]);
            if(UseTouchInput) 
			{
				value |= TouchInputManager.GetButton(playerId, action);
			}

            return value;
        }

        public override bool GetButtonDown(int playerId, InputAction action)
        {
            bool value = Input.GetButtonDown(actions[playerId][(int)action]);
            if(UseTouchInput) 
			{
				value |= TouchInputManager.GetButtonDown(playerId, action);
			}

            return value;
        }

        public override bool GetButtonUp(int playerId, InputAction action)
        {
            bool value = Input.GetButtonUp(actions[playerId][(int)action]);
            if(UseTouchInput) 
			{
				value |= TouchInputManager.GetButtonUp(playerId, action);
			}

            return value;
        }

        public override float GetAxis(int playerId, InputAction action)
        {
            float value = Input.GetAxisRaw(actions[playerId][(int)action]);
            if(UseTouchInput)
            {
                float touchValue = TouchInputManager.GetAxis(playerId, action);
                if(Mathf.Abs(touchValue) > Mathf.Abs(value)) value = touchValue;
            }

            return value;
        }

        private static void AddAction(InputAction action, string actionName, Dictionary<int, string> actions)
        {
            if(string.IsNullOrEmpty(actionName)) 
			{
				return;
			}

            actions.Add((int)action, actionName);
        }
    }
}