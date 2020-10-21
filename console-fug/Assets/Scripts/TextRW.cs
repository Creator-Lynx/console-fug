using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextRW : MonoBehaviour
{
    [SerializeField] Text field;
    [SerializeField] InputField inField;

    public void ExecuteCommand()
    {
        field.text = field.text + "\n" + inField.text;
        inField.text = "";

    }
}
