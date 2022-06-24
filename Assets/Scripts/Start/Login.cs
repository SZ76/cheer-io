using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unisave.Facets;
using Unisave.Facades;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{

    public TMP_InputField user;
    public TMP_InputField pass;
    public void CallRegister()
    {
        OnFacet<SaveFacet>
            .Call(
                nameof(SaveFacet.Register),
                user.text, pass.text
            )
            .Done();
    }

    public void CallLogin()
    {
        OnFacet<SaveFacet>
            .Call
                <bool>(nameof(SaveFacet.UserLogin),
                user.text, pass.text)
            .Then((result) =>
            {
                if (result)
                {
                    SceneManager.LoadScene(1);
                }
            })
            .Done();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            OnFacet<SaveFacet>
            .Call<string>(nameof(SaveFacet.CheckLogin))
            .Then((logName) =>
            {
                if (logName != "")
                {
                    JoinGame();
                }
            })
            .Done();
        }
       

    }

    public static void JoinGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LogOut()
    {
        OnFacet<SaveFacet>
            .Call(
                nameof(SaveFacet.Logout)
            )
            .Done();
        SceneManager.LoadScene(1);
    }
}
