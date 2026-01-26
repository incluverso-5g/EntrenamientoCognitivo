using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave.Essence;
using Wave.Native;
using Wave.Essence.Raycast;


public class MenuChangeScene : MonoBehaviour
{
    
    public ControllerRaycastPointer raycastPointer;
    public RandomizeShelfPositions2 randomizeScript; //MARTINA
    public RandomizeShelfPositions3 randomizeScript3;
    public GameObject shelfObject; // MARTINA
    public GameObject shelfObject3;
    public ReponerLogic1 reponerLogic1;
    



    private bool isButtonAPressed = false; //MARTINA
    private bool isButtonBPressed = false; // MARTINA
    private void Start()
    {
        raycastPointer = GetComponent<ControllerRaycastPointer>();

        randomizeScript = shelfObject.GetComponent<RandomizeShelfPositions2>();
        randomizeScript3 = shelfObject3.GetComponent<RandomizeShelfPositions3>();


    }
    

    void Update()
    {  if (SceneManager.GetActiveScene().name == "MenuPrincipal")
        {
            raycastPointer.ShowRay = true;
        }
        else
        {
            raycastPointer.ShowRay = false;
        }

        //_______________________________________ MARTINA



        /* if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
         {

             if (!isButtonAPressed)
             {
                 Debug.Log("!!!! Tasto A premuto, ripristinando posizioni...");
                 RandomizeShelfPositions2.instance.RestoreOriginalPositions();
                 isButtonAPressed = true;
             }
         }
                 else
                 {

                     isButtonAPressed = false;
                 }*/

         if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
         {
            
            if (!isButtonAPressed)
            {
                Debug.Log("!!!! Tasto A premuto ");
                ReponerLogic1.instance.TotResetButton();
                isButtonAPressed = true;
            }
         }
        else
        {
                    
            isButtonAPressed = false;
        }





        if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_B))
        {
            
            if (!isButtonBPressed)
            {
                Debug.Log("!!!! Tasto B premuto, ripristinando posizioni...");
                RandomizeShelfPositions3.instance3.RestoreOriginalPositions3();
                isButtonBPressed = true;
            }
        }
        else
        {

            isButtonBPressed = false;
        }




        /*

             if (WXRDevice.ButtonHold(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_A))
             {   
                 //SceneManager.LoadScene("MenuPrincipal");

             }
        */
    }
}
