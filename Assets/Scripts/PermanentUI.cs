using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PermanentUI : MonoBehaviour
{
    public int cherries = 0;
    public int health; 
    public TextMeshProUGUI cherryText;1114
    public Text HealthAmount;

    public static PermanentUI perm;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (!perm)
        {
            perm = this;

        }
        else
            Destroy(gameObject);
    }

    public void Reset()
    {
        cherries = 0;
        cherryText.text = cherries.ToString();
    }
}

