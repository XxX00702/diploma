using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private string _sceneToLoad;
    private PlayerInputs _playerInput;

    private bool isPaused = false;

    private void Start()
    {
        _playerInput = Player._playerInputsAction;
        if (_playerInput != null)
        {
            _playerInput.Player.Pause.performed += OnPausePerformed;
        }
    }

    private void OnEnable()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        deathMenuUI.SetActive(false);
        if (_playerInput != null)
        {
            _playerInput.Player.Pause.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (_playerInput != null)
        {
            _playerInput.Player.Pause.performed -= OnPausePerformed;
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (deathMenuUI.activeSelf) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            Player.SetPaused(true);
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            Player.SetPaused(false);
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneToLoad);
    }
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        Player.SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void ShowDeathMenu()
    {
        Time.timeScale = 0f;
        Player.SetPaused(true);
        deathMenuUI.SetActive(true);
    }
}
