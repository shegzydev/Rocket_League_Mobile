//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/RLInput.inputactions
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

public partial class @RLInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @RLInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RLInput"",
    ""maps"": [
        {
            ""name"": ""Input"",
            ""id"": ""2aa59784-ffae-42e0-b7b7-9a047931721c"",
            ""actions"": [
                {
                    ""name"": ""Drive"",
                    ""type"": ""Value"",
                    ""id"": ""0bf49e22-030e-402e-a875-b86db007f8be"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Steer"",
                    ""type"": ""Value"",
                    ""id"": ""eafed68c-ae58-42e6-adc7-39ac560bed10"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HandBrake"",
                    ""type"": ""Value"",
                    ""id"": ""3ebc6eb8-e30b-4172-9230-1dc0e2b58b82"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Tilt"",
                    ""type"": ""Value"",
                    ""id"": ""93bd302a-9d10-4be6-8c36-3d1a246764bc"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""6c355686-76e1-4a0a-bcb9-e36cc6f0619d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""7461c00e-6a9b-47e0-a46d-075a3f821c1d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""85ab5611-91c4-46c5-8e40-13a2c21173a6"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""d52d7e06-bca9-46c9-84ef-2860219648cc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""27e71860-de7c-427b-aa96-9de2be1dc30b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""3df4bb19-ae1d-4c82-9539-57b9579921d6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drive"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""8c52d55e-374f-430b-91f6-832b2f584daa"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""84a7ee39-e02b-4870-9ef1-0534849567ad"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a2c274c2-3ef1-4528-9846-415c11577234"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""cd17f8ef-d326-4d59-ae04-2c8cbeed4dba"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandBrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f24dac0-99ed-421f-ba86-d43fac4cab18"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandBrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc6bc200-67e0-4de2-a196-df0de40f293d"",
                    ""path"": ""<Accelerometer>/acceleration"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Input
        m_Input = asset.FindActionMap("Input", throwIfNotFound: true);
        m_Input_Drive = m_Input.FindAction("Drive", throwIfNotFound: true);
        m_Input_Steer = m_Input.FindAction("Steer", throwIfNotFound: true);
        m_Input_HandBrake = m_Input.FindAction("HandBrake", throwIfNotFound: true);
        m_Input_Tilt = m_Input.FindAction("Tilt", throwIfNotFound: true);
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

    // Input
    private readonly InputActionMap m_Input;
    private List<IInputActions> m_InputActionsCallbackInterfaces = new List<IInputActions>();
    private readonly InputAction m_Input_Drive;
    private readonly InputAction m_Input_Steer;
    private readonly InputAction m_Input_HandBrake;
    private readonly InputAction m_Input_Tilt;
    public struct InputActions
    {
        private @RLInput m_Wrapper;
        public InputActions(@RLInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Drive => m_Wrapper.m_Input_Drive;
        public InputAction @Steer => m_Wrapper.m_Input_Steer;
        public InputAction @HandBrake => m_Wrapper.m_Input_HandBrake;
        public InputAction @Tilt => m_Wrapper.m_Input_Tilt;
        public InputActionMap Get() { return m_Wrapper.m_Input; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(InputActions set) { return set.Get(); }
        public void AddCallbacks(IInputActions instance)
        {
            if (instance == null || m_Wrapper.m_InputActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_InputActionsCallbackInterfaces.Add(instance);
            @Drive.started += instance.OnDrive;
            @Drive.performed += instance.OnDrive;
            @Drive.canceled += instance.OnDrive;
            @Steer.started += instance.OnSteer;
            @Steer.performed += instance.OnSteer;
            @Steer.canceled += instance.OnSteer;
            @HandBrake.started += instance.OnHandBrake;
            @HandBrake.performed += instance.OnHandBrake;
            @HandBrake.canceled += instance.OnHandBrake;
            @Tilt.started += instance.OnTilt;
            @Tilt.performed += instance.OnTilt;
            @Tilt.canceled += instance.OnTilt;
        }

        private void UnregisterCallbacks(IInputActions instance)
        {
            @Drive.started -= instance.OnDrive;
            @Drive.performed -= instance.OnDrive;
            @Drive.canceled -= instance.OnDrive;
            @Steer.started -= instance.OnSteer;
            @Steer.performed -= instance.OnSteer;
            @Steer.canceled -= instance.OnSteer;
            @HandBrake.started -= instance.OnHandBrake;
            @HandBrake.performed -= instance.OnHandBrake;
            @HandBrake.canceled -= instance.OnHandBrake;
            @Tilt.started -= instance.OnTilt;
            @Tilt.performed -= instance.OnTilt;
            @Tilt.canceled -= instance.OnTilt;
        }

        public void RemoveCallbacks(IInputActions instance)
        {
            if (m_Wrapper.m_InputActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IInputActions instance)
        {
            foreach (var item in m_Wrapper.m_InputActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_InputActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public InputActions @Input => new InputActions(this);
    public interface IInputActions
    {
        void OnDrive(InputAction.CallbackContext context);
        void OnSteer(InputAction.CallbackContext context);
        void OnHandBrake(InputAction.CallbackContext context);
        void OnTilt(InputAction.CallbackContext context);
    }
}