using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class MenuControl : MonoBehaviour
{
    int selection = 0;
    public UnityEngine.UI.Button start, control, credit;
    private PlayerInput playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        select();
    }

    // Update is called once per frame
    void Update(){
        //checks for exit application
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }

    void select(){
        if(selection == 0){
            EventSystem.current.SetSelectedGameObject(start.gameObject);
        } else if(selection == 1){
            EventSystem.current.SetSelectedGameObject(control.gameObject);
        } else if(selection == 2){
            EventSystem.current.SetSelectedGameObject(credit.gameObject);
        }
    }

    //menu navigation split up by direction
    public void HandleRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (selection < 2){
                ++selection;
            } else{
                selection = 0;
                
            }
            select();
        }
    }

    public void HandleLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (selection > 0){
                --selection;
            } else{
                selection = 2;
                
            }
            select();
        }
    }

    public void HandleSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            selectButton();
        }
    }
    void selectButton(){
        if(selection == 0){
            start.GetComponent<ButtonExecute>().selected();
        } else if(selection == 1){
            control.GetComponent<ButtonExecute>().selected();
        } else if(selection == 2){
            credit.GetComponent<ButtonExecute>().selected();
        }
    }

      
}
