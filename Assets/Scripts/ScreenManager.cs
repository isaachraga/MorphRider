using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] public Camera player1, player2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //player1.rect(0,0.5,1,0.5);
        player1.rect = new Rect(0f, 0.5f,1f,0.5f);
        //player2.rect(0,0,1,0.5);
        player2.rect = new Rect(0f, 0f,1f,0.5f);
    }
}
