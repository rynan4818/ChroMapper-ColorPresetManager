using UnityEngine;
using UnityEngine.SceneManagement;
using ChroMapper_ColorPresetManager.Component;

namespace ChroMapper_ColorPresetManager
{
    [Plugin("Color Preset Manager")]
    public class Plugin
    {
        public static ColorPresetController colorPresetController;
        [Init]
        private void Init()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            Debug.Log("ChroMapper-ColorPresetManager Plugin has loaded!");
        }
        [Exit]
        private void Exit()
        {
            Debug.Log("ChroMapper-ColorPresetManager Plugin has closed!");
        }
        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex != 3) // Mapper scene
                return;
            if (colorPresetController != null && colorPresetController.isActiveAndEnabled)
                return;
            colorPresetController = new GameObject("ColorPresetManager").AddComponent<ColorPresetController>();
        }
    }
}
