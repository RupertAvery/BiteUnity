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
        vm.InitVm();
        vm.RegisterSystemModuleCallables();
        vm.SynchronizationContext = SynchronizationContext.Current;
        vm.RegisterExternalGlobalObjects(new Dictionary<string, object>()
            {
                { "GameObjectFromUnity", m_GameObjectToMove },
                { "TransformToUse", m_TransformToMove },
                { "Vector3", m_Vector3 }
            });

        IEnumerable< string > files = Directory.EnumerateFiles(
            "Assets\\Bite\\TestMoveGameObject",
            "*.bite",
            SearchOption.AllDirectories );

        BiteCompiler compiler = new BiteCompiler();

        program = compiler.Compile(files.Select(File.ReadAllText));

        Task.Run(() =>
        {
            vm.Interpret(program);
        });       
    }

    private void Update()
    {

    }
}
