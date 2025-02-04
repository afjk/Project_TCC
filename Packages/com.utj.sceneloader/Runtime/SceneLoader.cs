using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.SceneManagement
{
    /// <summary>
    /// Component for scene management.
    /// This class manages loading, unloading, and information about the loading status of scenes.
    /// Scene management is done using Addressable.
    /// </summary>
    [DisallowMultipleComponent]
    [RenamedFrom("Tcc.SceneManagement.SceneLoader")]
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// Scene to be loaded.
        /// </summary>
        [SerializeField] private AssetReferenceScene _scene;

        /// <summary>
        /// Priority when loading the scene.
        /// </summary>
        [SerializeField] private int _priority = 100;

        /// <summary>
        /// Set as an active scene when it's loaded.
        /// </summary>
        [SerializeField] private bool _isActive = false;

        /// <summary>
        /// Callback when loading is complete.
        /// </summary>
        public UnityEvent<Scene> OnLoaded;

        /// <summary>
        /// Callback when unloading is complete.
        /// </summary>
        public UnityEvent OnUnloaded;

        /// <summary>
        /// Scene name used for identifying managed scenes at runtime.
        /// </summary>
        [HideInInspector] [SerializeField] 
        private string _sceneName;

#if UNITY_EDITOR

        public State state = State.NotLoad;

        public SceneAsset SceneAsset => _scene.editorAsset;

        private void OnValidate()
        {
            if (_scene == null)
                return;

            _sceneName = _scene.editorAsset != null ? _scene.editorAsset.name : string.Empty;
            name = _sceneName;
        }

        /// <summary>
        /// Behavior when opening scenes in the editor.
        /// LoadAndEdit immediately opens the scene if selected.
        /// NotLoad requires pressing the Open/Close button to load/unload the scene.
        /// </summary>
        public enum State
        {
            LoadAndEdit,
            NotLoad
        }
#endif

        /// <summary>
        /// Scene loading handle.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> Handle =>
            SceneLoaderManager.GetHandle(_scene, out var handle) ? handle : default;

        /// <summary>
        /// Returns true if the scene is loaded.
        /// </summary>
        public bool IsLoaded { get; private set; } = false;

        /// <summary>
        /// Returns true if the scene is currently being loaded.
        /// </summary>
        public bool InProgress { get; private set; } = false;

        /// <summary>
        /// Get the progress of scene loading.
        /// Returns 0 if the scene is not currently loading.
        /// </summary>
        public float Progress
        {
            get
            {
                // Returns 0 if the scene is not currently loading
                if (InProgress == false ||
                    SceneLoaderManager.GetHandle(_scene, out var handle) == false)
                    return 0;

                return handle.PercentComplete;
            }
        }

        /// <summary>
        /// Current scene.
        /// </summary>
        public Scene Scene =>  SceneManager.GetSceneByName(_sceneName) ;


        /// <summary>
        /// Get the owner added by SceneLoader.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The GameObject owner.</returns>
        public static GameObject Owner(Scene scene) => SceneLoaderManager.GetOwner(scene);

        /// <summary>
        /// Get the owner added by SceneLoader.
        /// </summary>
        /// <param name="loadedGameObject">GameObject loaded by SceneLoader.</param>
        /// <returns>The GameObject owner.</returns>
        public static GameObject Owner(GameObject loadedGameObject) => Owner(loadedGameObject.scene);

        private void OnEnable()
        {
#if UNITY_EDITOR
            // If scene loading is already completed, immediately invoke OnComplete
            if (Scene.IsValid() || Scene.isLoaded)
            {
                if (_isActive)
                    Invoke(nameof(OnCompleteLoad), 0.1f);
                else
                    OnCompleteLoad();
            }
            else
            {
                // Load the scene
                LoadScene();
            }
#else
            LoadScene();
#endif
        }

        private void OnDisable()
        {
            UnloadScene();
        }

        /// <summary>
        /// Load the scene.
        /// </summary>
        private void LoadScene()
        {
            if (SceneLoaderManager.Load(_scene, _sceneName, gameObject, _priority, _isActive, OnCompleteLoad))
            {
                InProgress = true;
            }
            else
            {
                Debug.LogError($"{_sceneName} load failed");
            }
        }
        
        /// <summary>
        /// Unload the scene.
        /// </summary>
        private void UnloadScene()
        {
            SceneLoaderManager.Unload(_scene, _sceneName, () =>
            {
                OnUnloaded?.Invoke();
                IsLoaded = false;
            });
        }

        private void OnCompleteLoad()
        {
            InProgress = false;
            IsLoaded = true;
            OnLoaded?.Invoke(Scene);
        }

    }
}
