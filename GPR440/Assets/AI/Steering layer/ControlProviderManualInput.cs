using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public sealed class ControlProviderManualInput : ISteeringProvider
{
    [SerializeField] private string controlBindingName = "Movement";
    [SerializeField] [HideInInspector] private InputAction boundControl;

    private void Start() => _RefreshBoundControls();

    private void _RefreshBoundControls()
    {
        PlayerInput inputComponent = GetComponent<PlayerInput>();
        Debug.Assert(inputComponent.actions != null);

        InputActionMap controlMap = inputComponent.actions.actionMaps[0];
        Debug.Assert(controlMap != null);

        boundControl = controlMap.FindAction(controlBindingName);
        Debug.Assert(boundControl != null);
    }

    public override ControlData GetControlCommand()
    {
        return new ControlData {
            steering = -boundControl.ReadValue<Vector2>().x,
            targetSpeed = boundControl.ReadValue<Vector2>().y
        };
    }
}
