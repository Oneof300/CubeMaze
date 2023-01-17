using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;

    Transform _cube; Transform cube => _cube ? _cube : (_cube = GameObject.FindWithTag("CubeMaze").transform);
    GameObject _ball; GameObject ball => _ball ? _ball : (_ball = GameObject.FindWithTag("Player"));

    public void Next()
    {
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            StartCoroutine(TransitionToScene(nextSceneIndex));
        else
            Reload();
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {
        StartCoroutine(TransitionIn());
    }

    IEnumerator TransitionIn()
    {
        ball.SetActive(false);
        var startPosition = cube.position;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            cube.position = Vector3.Lerp(-targetPosition, startPosition, t);
            yield return null;
        }
        cube.position = startPosition;
        ball.SetActive(true);
    }

    IEnumerator TransitionToScene(int buildIndex)
    {
        ball.SetActive(false);
        var startPosition = cube.position;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            cube.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        SceneManager.LoadScene(buildIndex);
    }
}
