using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BitBox.AI
{
    public class Agent : MonoBehaviour
    {
        [BoxGroup("Settings"), SerializeField] private bool _isRegistered;
        [BoxGroup("Settings"), ShowIf(nameof(_isRegistered)), SerializeField] private EAgent _agentType;

#if UNITY_EDITOR
        [BoxGroup("Settings")] public bool ShowGizmos;
        [FoldoutGroup("Information"), ShowInInspector, ReadOnly] private string CurrentModule => _currentModule?.GetType().Name ?? string.Empty;
#endif
        
        private Module _currentModule;
        private int _currentModuleIndex;
        
        //[ValidateInput(nameof(CheckDependencies), "There are missing dependencies.")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(ListModules))]
#endif
        [SerializeReference] private List<Module> _modules = new();


        
        bool updating;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!ShowGizmos) return;
            if (_currentModule is null) return;
            
            _currentModule.DrawGizmos();
        }
#endif
        
        private void Awake()
        {
            if (_isRegistered) Registry.Add(_agentType, gameObject);
        }

        private void Start()
        {
            foreach (var t in _modules)
            {
                //modules[i] = Instantiate(modules[i]);
                t.Initialize(this);
            }
        }

        void Update()
        {
            var (nextModule, nextModuleIndex) = GetNextModule();
            
            // If the next module couldn't be evaluated, return
            if (nextModuleIndex == -1) return;

            // If the current module is null, set it to the next module and execute it
            if (_currentModule is null)
            {
                _currentModule = nextModule;
                _currentModuleIndex = nextModuleIndex;
                Execute();
            }

            // If the current module has more priority than the next module
            else if (_currentModuleIndex < nextModuleIndex)
            {
                if (_currentModule.IsExecuting)
                {
                    updating = _currentModule.RunsInUpdate || _currentModule.RunsInFixedUpdate;
                }
                else
                {
                    _currentModule = nextModule;
                    _currentModuleIndex = nextModuleIndex;
                    Execute();
                }
            }

            else if (nextModuleIndex != _currentModuleIndex)
            {
                _currentModule.Stop();
                _currentModule = nextModule;
                _currentModuleIndex = nextModuleIndex;
                Execute();
            }

            else if (!_currentModule.IsExecuting)
            {
                _currentModule = nextModule;
                _currentModuleIndex = nextModuleIndex;
                Execute();
            }
        }

        private void LateUpdate()
        {
            if (updating) _currentModule.Update();
        }

        void FixedUpdate()
        {
            if (updating) _currentModule.FixedUpdate();
        }

        private (Module, int) GetNextModule()
        {
            for (var i = 0; i < _modules.Count; ++i)
            {
                _modules[i].UpdateExecutableState();
                if (_modules[i].CanExecute)
                {
                    return (_modules[i], i);
                }
            }
            return (null, -1);
        }

        void Execute()
        {
            _currentModule.Execute();
            updating = _currentModule.RunsInUpdate || _currentModule.RunsInFixedUpdate;
            //onExecute?.Invoke(currentModule.type);
        }

#if UNITY_EDITOR
        private bool CheckDependencies()
        {
            var missingDependencies = GetMissingDependencies(_modules);
            return missingDependencies.Count is 0;
        }
        
        private List<Type> GetMissingDependencies(List<Module> modules)
        {
            var neededComponents = modules.SelectMany(module => module.GetRequiredComponents()).Distinct();
            return neededComponents.Where(component => !GetComponentInChildren(component)).ToList();
        }
        
        [HideIf(nameof(CheckDependencies))]
        [Button("Find Missing Dependencies", ButtonSizes.Gigantic, ButtonStyle.Box), GUIColor(1f, 1f, 0f)]
        private void LogMissingReferences()
        {
            var missingDependencies = GetMissingDependencies(_modules);
            foreach (var dependency in missingDependencies)
            {
                Debug.LogWarning($"[AI] Missing component: {dependency.Name}");
            }
        }
        
        public static IEnumerable ListModules() 
        {
            var scriptGuids = AssetDatabase.FindAssets("t:MonoScript");

            foreach (var guid in scriptGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                var scriptClass = monoScript?.GetClass();

                if (scriptClass == null) continue;

                if (typeof(Module).IsAssignableFrom(scriptClass) && !scriptClass.IsAbstract)
                {
                    object instance = null;
                    try
                    {
                        instance = Activator.CreateInstance(scriptClass);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Erro ao instanciar {scriptClass.Name}: {e.Message}");
                    }

                    if (instance != null)
                    {
                        var shortPath = path.Substring(path.LastIndexOf('/'));
                        yield return new ValueDropdownItem(shortPath, instance);
                    }
                }
            }
        }
#endif
    }
}
