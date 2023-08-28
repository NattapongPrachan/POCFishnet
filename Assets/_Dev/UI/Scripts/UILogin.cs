using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    [SerializeField]TestLobby testLobby;
    [SerializeField]TextMeshProUGUI playerNameTxt;
    [SerializeField]TMP_InputField playerNameInputField;
    [SerializeField]Button acceptBtn;
    // Start is called before the first frame update
    public void Accept()
    {
        if(!string.IsNullOrEmpty(playerNameInputField.text))
        {
            playerNameTxt.text = playerNameInputField.text;
            playerNameInputField.text = string.Empty;
            testLobby.UpdatePlayerName(playerNameTxt.text);
        }
    }
}
