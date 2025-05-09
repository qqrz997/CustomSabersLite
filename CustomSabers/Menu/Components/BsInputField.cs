using HMUI;
using UnityEngine;
using UnityEngine.Events;

namespace CustomSabersLite.Menu.Components;

public class BsInputField : MonoBehaviour
{
    private InputFieldView inputFieldView = null!;

    public void Init(InputFieldView inputFieldView) => this.inputFieldView = inputFieldView;

    public string Text
    {
        get => inputFieldView.text;
        set => inputFieldView.text = value;
    }

    public void AddInputChangedListener(UnityAction<InputFieldView> a) => inputFieldView?.onValueChanged.AddListener(a);
}