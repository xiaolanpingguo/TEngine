//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/GameScripts/HotFix/GameLogic/ZQ/Scripts/Logic/Input/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""GameplayMap"",
            ""id"": ""46bf95ba-b7a8-4da1-a4d5-ad49677d3765"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""1b319d9c-b705-46f9-b440-deabe5108883"",
                    ""expectedControlType"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""73c5ed6d-6e83-4693-857f-7a639b926770"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Skill1"",
                    ""type"": ""Button"",
                    ""id"": ""5b3f84e6-a899-4b8f-8fed-f72d7abd5eed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill2"",
                    ""type"": ""Button"",
                    ""id"": ""d9d197aa-3f21-44ab-a572-5c7e16fdf2cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Arrows"",
                    ""id"": ""57a3c129-4935-42e8-a713-862f74ef7291"",
                    ""path"": ""2DVector(normalize=false)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""df9b328d-3e19-4d55-8b87-b363ce2180f5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e12587eb-6fef-49da-8ea2-5639a419e541"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b4c41209-0011-46e0-91fe-210de67b2e01"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""787aa252-db2b-485b-91df-c886ca6889fe"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f706f9e6-91c2-47f7-a528-f703615fe4f0"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.1,max=1)"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14ff7398-fb3b-4aad-a46c-5776489455e5"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c29791b5-ad84-48b8-a28b-7f9fc41ff7f9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb223a86-339e-41ba-8833-058f8b7477df"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d83f1b5b-501c-4ce0-a4c4-d24e2d461f34"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GameplayMap
        m_GameplayMap = asset.FindActionMap("GameplayMap", throwIfNotFound: true);
        m_GameplayMap_Move = m_GameplayMap.FindAction("Move", throwIfNotFound: true);
        m_GameplayMap_Jump = m_GameplayMap.FindAction("Jump", throwIfNotFound: true);
        m_GameplayMap_Skill1 = m_GameplayMap.FindAction("Skill1", throwIfNotFound: true);
        m_GameplayMap_Skill2 = m_GameplayMap.FindAction("Skill2", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // GameplayMap
    private readonly InputActionMap m_GameplayMap;
    private List<IGameplayMapActions> m_GameplayMapActionsCallbackInterfaces = new List<IGameplayMapActions>();
    private readonly InputAction m_GameplayMap_Move;
    private readonly InputAction m_GameplayMap_Jump;
    private readonly InputAction m_GameplayMap_Skill1;
    private readonly InputAction m_GameplayMap_Skill2;
    public struct GameplayMapActions
    {
        private @InputActions m_Wrapper;
        public GameplayMapActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_GameplayMap_Move;
        public InputAction @Jump => m_Wrapper.m_GameplayMap_Jump;
        public InputAction @Skill1 => m_Wrapper.m_GameplayMap_Skill1;
        public InputAction @Skill2 => m_Wrapper.m_GameplayMap_Skill2;
        public InputActionMap Get() { return m_Wrapper.m_GameplayMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayMapActions set) { return set.Get(); }
        public void AddCallbacks(IGameplayMapActions instance)
        {
            if (instance == null || m_Wrapper.m_GameplayMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameplayMapActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Skill1.started += instance.OnSkill1;
            @Skill1.performed += instance.OnSkill1;
            @Skill1.canceled += instance.OnSkill1;
            @Skill2.started += instance.OnSkill2;
            @Skill2.performed += instance.OnSkill2;
            @Skill2.canceled += instance.OnSkill2;
        }

        private void UnregisterCallbacks(IGameplayMapActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Skill1.started -= instance.OnSkill1;
            @Skill1.performed -= instance.OnSkill1;
            @Skill1.canceled -= instance.OnSkill1;
            @Skill2.started -= instance.OnSkill2;
            @Skill2.performed -= instance.OnSkill2;
            @Skill2.canceled -= instance.OnSkill2;
        }

        public void RemoveCallbacks(IGameplayMapActions instance)
        {
            if (m_Wrapper.m_GameplayMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameplayMapActions instance)
        {
            foreach (var item in m_Wrapper.m_GameplayMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameplayMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameplayMapActions @GameplayMap => new GameplayMapActions(this);
    public interface IGameplayMapActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSkill1(InputAction.CallbackContext context);
        void OnSkill2(InputAction.CallbackContext context);
    }
}
