using Lockstep.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


namespace Lockstep.Game
{
    public struct PlayerCommands
    {
        public LVector2 inputUV;
        public byte skillId;
        public bool isJump;
        public byte ActorId;
        public int Tick;
        public bool IsMiss;

        public void Reset()
        {
            inputUV = LVector2.zero;
            skillId = 0;
            isJump = false;
            ActorId = 0;
            Tick = -1;
            IsMiss = false;
        }
    }

    public class InputManager
    {
        public static PlayerCommands CurrentInput = new PlayerCommands();

        private static InputActions.GameplayMapActions _inputActionsMap;

        public static void Init()
        {
            InputActions actions = new InputActions();
            actions.Enable();
            _inputActionsMap = actions.GameplayMap;
            _inputActionsMap.Enable();
        }

        public static void Update()
        {
            CurrentInput.Reset();

            Vector2 move = Vector2.ClampMagnitude(_inputActionsMap.Move.ReadValue<Vector2>(), 1f);
            CurrentInput.inputUV = move.ToLVector2();

            if (_inputActionsMap.Jump.IsPressed())
            {
                CurrentInput.isJump = true;
            }

            if (_inputActionsMap.Skill1.IsPressed())
            {
                CurrentInput.skillId++;
            }
            if (_inputActionsMap.Skill2.IsPressed())
            {
                CurrentInput.skillId++;
            }
        }
    }
}