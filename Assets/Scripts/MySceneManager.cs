using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public enum Scene
    {
        Home = 0,
        Combat,
    }

    private Canvas canvas;
   
    private Scene CurrentScene { get; set; } = Scene.Home;

    // TODO: singleton 으로 만들기
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        
    }

    public void ChangeScene(int sceneIndex)
    {
        CurrentScene = (Scene)sceneIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    // TODO: Quit 종료처리
}
