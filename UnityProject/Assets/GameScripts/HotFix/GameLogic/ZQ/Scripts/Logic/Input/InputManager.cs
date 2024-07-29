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
        [System.Flags]
        public enum Button : uint
        {
            None = 0,
            Jump = 1 << 0,
            Skill1 = 1 << 1,
            Skill2 = 1 << 2,
        }

        public static readonly PlayerCommand Empty = new PlayerCommand();
        public static readonly int k_buttonCount = sizeof(Button) * 8;
        public int ButtonField;
        public LVector2 inputUV;
        public int EntityId;
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

        public readonly bool IsSet(Button button)
        {
            return (ButtonField & (int)button) > 0;
        }

        public void Or(Button button, bool val)
        {
            if (val)
            {
                ButtonField |= (int)button;
            }
        }

        public void Set(Button button, bool val)
        {
            if (val)
            {
                ButtonField |= (int)button;
            }
            else
            {
                ButtonField &= ~(int)button;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerCommand other && Equals(other);
        }

        public bool Equals(PlayerCommand other)
        {
            return this.inputUV == other.inputUV && 
                this.ButtonField == other.ButtonField &&
                this.EntityId == other.EntityId && 
                this.Tick == other.Tick;
        }

        public static bool operator ==(PlayerCommand a, PlayerCommand b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PlayerCommand a, PlayerCommand b)
        {
            return !(a == b);
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