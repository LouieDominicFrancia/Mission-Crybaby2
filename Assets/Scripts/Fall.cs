using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fall : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PermanentUI.perm.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            PermanentUI.perm.health -= 1;
            PermanentUI.perm.HealthAmount.text = PermanentUI.perm.health.ToString();

            if (PermanentUI.perm.health <= 0)
            {
                SceneManager.LoadScene("TheEnd");
            }
        }
    }


}
