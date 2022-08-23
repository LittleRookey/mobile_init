using UnityEngine;
using UnityEngine.SceneManagement;

namespace AllIn1SpriteShader
{
    public class All1DemoChangeScene : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                NextScene();
            }
        }

        public void NextScene()
        {
            if (SceneManager.sceneCountInBuildSettings <= SceneManager.GetActiveScene().buildIndex + 1)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}