using UnityEngine;

namespace ChroMapper_ColorPresetManager
{
    [Plugin("ChroMapper-ColorPresetManager")]
    public class Plugin
    {
        [Init]
        private void Init()
        {
            Debug.Log("ChroMapper-ColorPresetManager Plugin has loaded!");
        }
        [Exit]
        private void Exit()
        {
            Debug.Log("ChroMapper-ColorPresetManager Plugin has closed!");
        }
    }
}
