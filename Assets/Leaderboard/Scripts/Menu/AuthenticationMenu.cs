using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class AuthenticationMenu : Panel
{

    [SerializeField] private TMP_InputField usernameInput = null;
    [SerializeField] private TMP_InputField passwordInput = null;
    [SerializeField] private Button signinButton = null;
    [SerializeField] private Button signupButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        signinButton.onClick.AddListener(SignIn);
        signupButton.onClick.AddListener(SignUp);
        
        if (usernameInput != null)
        {
            usernameInput.onValueChanged.AddListener(OnUsernameChanged);
            usernameInput.characterLimit = 10;
        }

        if (passwordInput != null)
        {
            passwordInput.onValueChanged.AddListener(OnPasswordChanged);
            passwordInput.characterLimit = 20;
        }
        
        base.Initialize();
    }

    public override void Open()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        base.Open();
    }

    private void SignIn()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();
        if (string.IsNullOrEmpty(user) == false && string.IsNullOrEmpty(pass) == false)
        {
            MenuManager.Singleton.SignInWithUsernameAndPasswordAsync(user, pass);
        }
    }

    private void SignUp()
    {
        string user = usernameInput.text.Trim();
        string pass = passwordInput.text.Trim();
        if (string.IsNullOrEmpty(user) == false && string.IsNullOrEmpty(pass) == false)
        {
            if (IsUsernameValid(user))
            {
                if (IsPasswordValid(pass))
                {
                    MenuManager.Singleton.SignUpWithUsernameAndPasswordAsync(user, pass);
                }
                else
                {
                    ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
                    panel.Open(ErrorMenu.Action.None, "Password must contain at least 1 letter and 1 number. Can include special characters (!@#$%^). Length: 8-20 characters.", "OK");
                }
            }
            else
            {
                ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
                panel.Open(ErrorMenu.Action.None, "Username can only contain letters, numbers, and underscores (_). Length: 2-10 characters.", "OK");
            }
        }
    }

    private void OnUsernameChanged(string value)
    {
        string filteredValue = FilterUsername(value);
        if (filteredValue != value)
        {
            usernameInput.text = filteredValue;
        }
    }

    private void OnPasswordChanged(string value)
    {
        string filteredValue = FilterPassword(value);
        if (filteredValue != value)
        {
            passwordInput.text = filteredValue;
        }
    }

    private string FilterUsername(string username)
    {
        return Regex.Replace(username, "[^a-zA-Z0-9_]", "");
    }

    private string FilterPassword(string password)
    {
        return Regex.Replace(password, "[^a-zA-Z0-9!@#$%^]", "");
    }

    private bool IsUsernameValid(string username)
    {
        if (username.Length < 2 || username.Length > 10)
        {
            return false;
        }
        return Regex.IsMatch(username, "^[a-zA-Z0-9_]+$");
    }
    
    private bool IsPasswordValid(string password)
    {
        if (password.Length < 8 || password.Length > 20)
        {
            return false;
        }
        
        bool hasLetter = false;
        bool hasDigit = false;

        foreach (char c in password)
        {
            if (char.IsLetter(c))
            {
                hasLetter = true;
            }
            else if (char.IsDigit(c))
            {
                hasDigit = true;
            }
        }
        return hasLetter && hasDigit;
    }
    
}