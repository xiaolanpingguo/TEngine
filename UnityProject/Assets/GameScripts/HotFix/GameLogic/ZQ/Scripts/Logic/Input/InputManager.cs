using Lockstep.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


namespace Lockstep.Game
{
    public struct PlayerCommand
    {
        public static readonly PlayerCommand Empty = new PlayerCommand();

        [System.Flags]
        public enum Button : uint
        {
            None = 0,
            Jump = 1 << 0,
            Skill1 = 1 << 1,
            Skill2 = 1 << 2,
        }

        public readonly bool IsSet(Button button)
        {
            return (ButtonField & (uint)button) > 0;
        }

        public void Or(Button button, bool val)
        {
            if (val)
            {
                ButtonField |= (uint)button;
            }
        }

        public void Set(Button button, bool val)
        {
            if (val)
            {
                ButtonField |= (uint)button;
            }
            else
            {
                ButtonField &= ~(uint)button;
            }
        }

        public const int k_buttonCount = sizeof(Button) * 8;

        public uint ButtonField;
        public LVector2 inputUV;
        public byte EntityId;
        public int Tick;
        public bool IsMiss;

        public void Reset()
        {
            inputUV = LVector2.zero;
            EntityId = 0;
            Tick = -1;
            IsMiss = false;
            ButtonField = 0;
        }
    }

    public class InputManager
    {
        public static PlayerCommand CurrentInput = new PlayerCommand();

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
                CurrentInput.Set(PlayerCommand.Button.Jump, true);
            }

            if (_inputActionsMap.Skill1.IsPressed())
            {
                CurrentInput.Set(PlayerCommand.Button.Skill1, true);
            }
            if (_inputActionsMap.Skill2.IsPressed())
            {
                CurrentInput.Set(PlayerCommand.Button.Skill2, true);
            }
        }
    }
}