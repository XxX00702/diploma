using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private GameObject _settingsCanvas;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OperateMenus()
    {
        _mainCanvas.SetActive(!_mainCanvas.activeSelf);
        _settingsCanvas.SetActive(!_mainCanvas.activeSelf);
    }
}
