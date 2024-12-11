using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonExecute : MonoBehaviour
{
    public string scene;

    public void selected(){
        //Loads scene based on name
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
