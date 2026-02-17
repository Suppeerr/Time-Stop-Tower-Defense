using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindUI : MonoBehaviour
{
    public TMP_Text bindingText;
    public TMP_Text rebindPrompt;

    [SerializeField] private InputActionReference actionReference;
    private InputAction action;
    private int bindingIndex = 0;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    void Start()
    {
        string mapName = actionReference.action.actionMap.name;
        string actionName = actionReference.action.name;
        action = InputManager.Instance.Input.asset
            .FindActionMap(mapName)
            .FindAction(actionName);

        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        action.Disable();

        rebindPrompt?.gameObject.SetActive(true);
        bindingText?.gameObject.SetActive(false);

        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(op =>
            {
                op.Dispose();
                rebindOperation = null;

                action.Enable();

                UpdateBindingDisplay();
                rebindPrompt?.gameObject.SetActive(false);
                bindingText?.gameObject.SetActive(true);
            })
            .Start();
    }

    public void ResetToDefault()
    {
        action.RemoveBindingOverride(bindingIndex);
        UpdateBindingDisplay();
    }

    private void UpdateBindingDisplay()
    {
        bindingText.text = action.GetBindingDisplayString(bindingIndex);
    }
}