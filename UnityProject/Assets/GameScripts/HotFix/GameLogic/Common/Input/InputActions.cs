//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/GameScripts/HotFix/GameLogic/Common/Input/InputActions.inputactions
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
                    ""name"": ""LookDelta"",
                    ""type"": ""Value"",
                    ""id"": ""9ac4461b-a0bd-43bc-9258-03bf2a17ca60"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LookConst"",
                    ""type"": ""Value"",
                    ""id"": ""847bdfb6-f46b-498d-b93b-4647302db5ac"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""b58e7016-1fbe-440f-851d-fe5dc0b9bdf5"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
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
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""fe797924-662a-430c-adec-0ad8e35f90c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""025599b3-e820-4333-99db-6432435a6567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""NextWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""3db36a8b-73c0-4ad9-986f-d26dc9b0438d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponSlot1"",
                    ""type"": ""Button"",
                    ""id"": ""5b3f84e6-a899-4b8f-8fed-f72d7abd5eed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponSlot2"",
                    ""type"": ""Button"",
                    ""id"": ""d9d197aa-3f21-44ab-a572-5c7e16fdf2cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponSlot3"",
                    ""type"": ""Button"",
                    ""id"": ""31093662-dffe-4730-aecd-a070b168b76c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WeaponSlot4"",
                    ""type"": ""Button"",
                    ""id"": ""8210360a-e49e-4740-9483-49d2d452bdf6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OptionMenu"",
                    ""type"": ""Button"",
                    ""id"": ""bbd4e72a-e091-4faa-a032-c934e78acc5d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EnableSync"",
                    ""type"": ""Button"",
                    ""id"": ""44026697-a2d7-4bfe-a209-712467b7798d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AutoMove"",
                    ""type"": ""Button"",
                    ""id"": ""742975a8-a402-430b-9836-8109b172cc8e"",
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
                    ""id"": ""f10408b2-1e7d-4dbe-b86f-fc66f618ec70"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.05,y=0.05)"",
                    ""groups"": """",
                    ""action"": ""LookDelta"",
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
                    ""id"": ""ddf3d29a-c6cc-4cd7-af23-85f44d1ec93a"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0.1),Invert"",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3789536a-16ff-4409-b7e5-8ac630a304b9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df640d72-e032-4852-808d-495f215c9c9d"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa9e117b-f5d4-49e4-b431-f90b8df669fa"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39dce1d2-a182-4885-a5ac-fa83a5b86f7b"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83ae0f87-a0ba-46ae-bda3-8d91c2139b3c"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.2,max=1),ScaleVector2(x=70,y=70)"",
                    ""groups"": """",
                    ""action"": ""LookConst"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ca0cfd93-3ee6-44ac-970d-ea0516d0d47a"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWeapon"",
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
                    ""action"": ""WeaponSlot1"",
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
                    ""action"": ""WeaponSlot2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""488d7705-b1d9-4841-bee5-393eea2786ca"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WeaponSlot3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a08bbfa5-d8d3-46af-a915-313faf10b4fd"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WeaponSlot4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c2c6e152-4567-4b6a-8513-a4d4f74fca57"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OptionMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5f4953c-3f31-45f0-a266-9fa002b622f6"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EnableSync"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e57c717-04e3-490c-9d2e-0ac1996a44e5"",
                    ""path"": ""<Keyboard>/f8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AutoMove"",
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
        m_GameplayMap_LookDelta = m_GameplayMap.FindAction("LookDelta", throwIfNotFound: true);
        m_GameplayMap_LookConst = m_GameplayMap.FindAction("LookConst", throwIfNotFound: true);
        m_GameplayMap_Scroll = m_GameplayMap.FindAction("Scroll", throwIfNotFound: true);
        m_GameplayMap_Jump = m_GameplayMap.FindAction("Jump", throwIfNotFound: true);
        m_GameplayMap_Shoot = m_GameplayMap.FindAction("Shoot", throwIfNotFound: true);
        m_GameplayMap_Aim = m_GameplayMap.FindAction("Aim", throwIfNotFound: true);
        m_GameplayMap_NextWeapon = m_GameplayMap.FindAction("NextWeapon", throwIfNotFound: true);
        m_GameplayMap_WeaponSlot1 = m_GameplayMap.FindAction("WeaponSlot1", throwIfNotFound: true);
        m_GameplayMap_WeaponSlot2 = m_GameplayMap.FindAction("WeaponSlot2", throwIfNotFound: true);
        m_GameplayMap_WeaponSlot3 = m_GameplayMap.FindAction("WeaponSlot3", throwIfNotFound: true);
        m_GameplayMap_WeaponSlot4 = m_GameplayMap.FindAction("WeaponSlot4", throwIfNotFound: true);
        m_GameplayMap_OptionMenu = m_GameplayMap.FindAction("OptionMenu", throwIfNotFound: true);
        m_GameplayMap_EnableSync = m_GameplayMap.FindAction("EnableSync", throwIfNotFound: true);
        m_GameplayMap_AutoMove = m_GameplayMap.FindAction("AutoMove", throwIfNotFound: true);
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
    private readonly InputAction m_GameplayMap_LookDelta;
    private readonly InputAction m_GameplayMap_LookConst;
    private readonly InputAction m_GameplayMap_Scroll;
    private readonly InputAction m_GameplayMap_Jump;
    private readonly InputAction m_GameplayMap_Shoot;
    private readonly InputAction m_GameplayMap_Aim;
    private readonly InputAction m_GameplayMap_NextWeapon;
    private readonly InputAction m_GameplayMap_WeaponSlot1;
    private readonly InputAction m_GameplayMap_WeaponSlot2;
    private readonly InputAction m_GameplayMap_WeaponSlot3;
    private readonly InputAction m_GameplayMap_WeaponSlot4;
    private readonly InputAction m_GameplayMap_OptionMenu;
    private readonly InputAction m_GameplayMap_EnableSync;
    private readonly InputAction m_GameplayMap_AutoMove;
    public struct GameplayMapActions
    {
        private @InputActions m_Wrapper;
        public GameplayMapActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_GameplayMap_Move;
        public InputAction @LookDelta => m_Wrapper.m_GameplayMap_LookDelta;
        public InputAction @LookConst => m_Wrapper.m_GameplayMap_LookConst;
        public InputAction @Scroll => m_Wrapper.m_GameplayMap_Scroll;
        public InputAction @Jump => m_Wrapper.m_GameplayMap_Jump;
        public InputAction @Shoot => m_Wrapper.m_GameplayMap_Shoot;
        public InputAction @Aim => m_Wrapper.m_GameplayMap_Aim;
        public InputAction @NextWeapon => m_Wrapper.m_GameplayMap_NextWeapon;
        public InputAction @WeaponSlot1 => m_Wrapper.m_GameplayMap_WeaponSlot1;
        public InputAction @WeaponSlot2 => m_Wrapper.m_GameplayMap_WeaponSlot2;
        public InputAction @WeaponSlot3 => m_Wrapper.m_GameplayMap_WeaponSlot3;
        public InputAction @WeaponSlot4 => m_Wrapper.m_GameplayMap_WeaponSlot4;
        public InputAction @OptionMenu => m_Wrapper.m_GameplayMap_OptionMenu;
        public InputAction @EnableSync => m_Wrapper.m_GameplayMap_EnableSync;
        public InputAction @AutoMove => m_Wrapper.m_GameplayMap_AutoMove;
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
            @LookDelta.started += instance.OnLookDelta;
            @LookDelta.performed += instance.OnLookDelta;
            @LookDelta.canceled += instance.OnLookDelta;
            @LookConst.started += instance.OnLookConst;
            @LookConst.performed += instance.OnLookConst;
            @LookConst.canceled += instance.OnLookConst;
            @Scroll.started += instance.OnScroll;
            @Scroll.performed += instance.OnScroll;
            @Scroll.canceled += instance.OnScroll;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Shoot.started += instance.OnShoot;
            @Shoot.performed += instance.OnShoot;
            @Shoot.canceled += instance.OnShoot;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @NextWeapon.started += instance.OnNextWeapon;
            @NextWeapon.performed += instance.OnNextWeapon;
            @NextWeapon.canceled += instance.OnNextWeapon;
            @WeaponSlot1.started += instance.OnWeaponSlot1;
            @WeaponSlot1.performed += instance.OnWeaponSlot1;
            @WeaponSlot1.canceled += instance.OnWeaponSlot1;
            @WeaponSlot2.started += instance.OnWeaponSlot2;
            @WeaponSlot2.performed += instance.OnWeaponSlot2;
            @WeaponSlot2.canceled += instance.OnWeaponSlot2;
            @WeaponSlot3.started += instance.OnWeaponSlot3;
            @WeaponSlot3.performed += instance.OnWeaponSlot3;
            @WeaponSlot3.canceled += instance.OnWeaponSlot3;
            @WeaponSlot4.started += instance.OnWeaponSlot4;
            @WeaponSlot4.performed += instance.OnWeaponSlot4;
            @WeaponSlot4.canceled += instance.OnWeaponSlot4;
            @OptionMenu.started += instance.OnOptionMenu;
            @OptionMenu.performed += instance.OnOptionMenu;
            @OptionMenu.canceled += instance.OnOptionMenu;
            @EnableSync.started += instance.OnEnableSync;
            @EnableSync.performed += instance.OnEnableSync;
            @EnableSync.canceled += instance.OnEnableSync;
            @AutoMove.started += instance.OnAutoMove;
            @AutoMove.performed += instance.OnAutoMove;
            @AutoMove.canceled += instance.OnAutoMove;
        }

        private void UnregisterCallbacks(IGameplayMapActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @LookDelta.started -= instance.OnLookDelta;
            @LookDelta.performed -= instance.OnLookDelta;
            @LookDelta.canceled -= instance.OnLookDelta;
            @LookConst.started -= instance.OnLookConst;
            @LookConst.performed -= instance.OnLookConst;
            @LookConst.canceled -= instance.OnLookConst;
            @Scroll.started -= instance.OnScroll;
            @Scroll.performed -= instance.OnScroll;
            @Scroll.canceled -= instance.OnScroll;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Shoot.started -= instance.OnShoot;
            @Shoot.performed -= instance.OnShoot;
            @Shoot.canceled -= instance.OnShoot;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @NextWeapon.started -= instance.OnNextWeapon;
            @NextWeapon.performed -= instance.OnNextWeapon;
            @NextWeapon.canceled -= instance.OnNextWeapon;
            @WeaponSlot1.started -= instance.OnWeaponSlot1;
            @WeaponSlot1.performed -= instance.OnWeaponSlot1;
            @WeaponSlot1.canceled -= instance.OnWeaponSlot1;
            @WeaponSlot2.started -= instance.OnWeaponSlot2;
            @WeaponSlot2.performed -= instance.OnWeaponSlot2;
            @WeaponSlot2.canceled -= instance.OnWeaponSlot2;
            @WeaponSlot3.started -= instance.OnWeaponSlot3;
            @WeaponSlot3.performed -= instance.OnWeaponSlot3;
            @WeaponSlot3.canceled -= instance.OnWeaponSlot3;
            @WeaponSlot4.started -= instance.OnWeaponSlot4;
            @WeaponSlot4.performed -= instance.OnWeaponSlot4;
            @WeaponSlot4.canceled -= instance.OnWeaponSlot4;
            @OptionMenu.started -= instance.OnOptionMenu;
            @OptionMenu.performed -= instance.OnOptionMenu;
            @OptionMenu.canceled -= instance.OnOptionMenu;
            @EnableSync.started -= instance.OnEnableSync;
            @EnableSync.performed -= instance.OnEnableSync;
            @EnableSync.canceled -= instance.OnEnableSync;
            @AutoMove.started -= instance.OnAutoMove;
            @AutoMove.performed -= instance.OnAutoMove;
            @AutoMove.canceled -= instance.OnAutoMove;
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
        void OnLookDelta(InputAction.CallbackContext context);
        void OnLookConst(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnWeaponSlot1(InputAction.CallbackContext context);
        void OnWeaponSlot2(InputAction.CallbackContext context);
        void OnWeaponSlot3(InputAction.CallbackContext context);
        void OnWeaponSlot4(InputAction.CallbackContext context);
        void OnOptionMenu(InputAction.CallbackContext context);
        void OnEnableSync(InputAction.CallbackContext context);
        void OnAutoMove(InputAction.CallbackContext context);
    }
}
