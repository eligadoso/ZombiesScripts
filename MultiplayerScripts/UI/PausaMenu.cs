using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausaMenu : MonoBehaviour
{
    public GameObject Menu;
    private static bool MenuStatus;
    private void Start()
    {
        MenuStatus = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarMenu();
        }
    }
    public void AlternarMenu()
    {
        MenuStatus = !MenuStatus;
        Menu.SetActive(MenuStatus);

        Cursor.lockState = MenuStatus ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = MenuStatus;
    }
}
