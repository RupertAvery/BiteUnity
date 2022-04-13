using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bite.Compiler;
using Bite.Modules.Callables;
using Bite.Runtime;
using Bite.Runtime.CodeGen;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

public class BiteMoveGameObject : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObjectToMove;
    
    [SerializeField]
    private Transform m_TransformToMove;

    public Vector3 m_Vector3;
    private BiteProgram program;
    private BiteVm vm = new BiteVm();

    void Start()
    {
        Debug.Log("Started BiteMoveGameObject!");
        vm.InitVm();
        vm.RegisterSystemModuleCallables();
        vm.SynchronizationContext = SynchronizationContext.Current;
        vm.RegisterExternalGlobalObjects(new Dictionary<string, object>()
            {
                { "GameObjectFromUnity", m_GameObjectToMove },
                { "TransformToUse", m_TransformToMove },
                { "Vector3", m_Vector3 }
            });

        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

        IEnumerable< string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestMoveGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        program = compiler.Compile(files.Select(File.ReadAllText));


        Task.Run(() =>
        {
            Debug.Log("Running program");
            vm.Interpret(program);
        }).ContinueWith(t=>
        {
            Debug.Log("Stopped!");
            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception.InnerException.Message);
            }
        });       
    }

    private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        if(obj == PlayModeStateChange.ExitingPlayMode)
        {
            vm.Stop();
        }
    }

    private void Update()
    {

    }
}
