using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CrasherScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField][Tooltip("Use this if you want a limit of how many loop iteration are executed. For an infinite loop execution, set to 0.")] private int iterationLimit;
    [SerializeField] private TextMeshProUGUI screenText;
    [SerializeField] private string activationString;
    [SerializeField] private string failureString;
    
    [Header("Memory Eater")] 
    [SerializeField] private bool useMemoryEater;
    [SerializeField] private MemoryCrasherModes memoryCrashModes;
    [SerializeField] private string mem_StringContent;
    [SerializeField] private int mem_ByteIncrement = 1024;
    private enum MemoryCrasherModes { InfinitelyExtendingString, MemoryOverflow }
    
    [Header("CPU Eater")] 
    [SerializeField] private bool useCpuEater;
    [SerializeField] private bool cpu_UseMulticore;
    [SerializeField][Tooltip("How many CPU-expensive calculations should be done per iteration?")] private int cpu_CalcAmount;

    [Header("GPU Eater")] 
    [SerializeField] private bool useGpuEater;
    [SerializeField][Tooltip("How many meshes should be drawn in a single iteration?")] private int gpu_MeshesToDraw;
    [SerializeField][Tooltip("How big should the spawn sphere for these meshes be?")] private float gpu_UnitSphereExponent = 100;
    [SerializeField] private Mesh gpu_MeshToUse;
    [SerializeField] private Material gpu_MaterialToUse;

    [Header("Unity Crash")] 
    [SerializeField] private bool useUnityCrash;
    [SerializeField] private ForcedCrashCategory crashType;

    [Header("Stack Overflow")] 
    [SerializeField] private bool useStackOverflow;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (useCpuEater) StartCoroutine(BeginCrashing(activationString, 1));
            if (useMemoryEater) StartCoroutine(BeginCrashing(activationString, 2));
            if (useGpuEater) StartCoroutine(BeginCrashing(activationString, 3));
            if (useUnityCrash) StartCoroutine(BeginCrashing(activationString, 4));
            if (useStackOverflow) StartCoroutine(BeginCrashing(activationString, 5));
            if (!useMemoryEater && !useCpuEater && !useGpuEater && !useUnityCrash && !useStackOverflow) StartCoroutine(BeginCrashing(failureString, 0));
        }
    }

    private IEnumerator BeginCrashing(string textString, int mode)
    {
        screenText.text = textString;
        yield return null;
        switch (mode)
        {
            case 0:
                Debug.Log("No crash method selected!");
                break;
            case 1:
                CrashTheFuckingCPU();
                break;
            case 2:
                CrashTheFuckingMemory();
                break;
            case 3:
                StartCoroutine(CrashTheFuckingGPU());
                break;            
            case 4:
                CrashTheFuckingEngine();
                break;
            case 5:
                OverflowTheFuckingEngine();
                break;
        }
    }

    private void CrashTheFuckingMemory()
    {
        bool shitBool = true;
        switch (memoryCrashModes)
        {
            case MemoryCrasherModes.InfinitelyExtendingString:
                int iteration = 0;
                List<string> stringList = new List<string>();
                string newString = mem_StringContent;
                switch (iterationLimit)
                {
                    case 0:
                        while (shitBool)
                        {
                            newString += mem_StringContent;
                            stringList.Add(newString);
                        }
                        break;
                    case > 0:
                        while (iteration < iterationLimit)
                        {
                            newString += mem_StringContent;
                            stringList.Add(newString);
                            iteration++;
                            Debug.Log(iteration);
                        }
                        break;
                }
                break;
            case MemoryCrasherModes.MemoryOverflow:
                List<byte[]> memoryHog = new List<byte[]>();
                switch (iterationLimit)
                {
                    case 0:
                        while (shitBool)
                        {
                            // Allocate large chunks of memory
                            memoryHog.Add(new byte[mem_ByteIncrement * mem_ByteIncrement]); // Allocate 1 MB
                        }
                        break;
                    case > 0:
                        for (int i = 0; i < iterationLimit; i++)
                        {
                            memoryHog.Add(new byte[mem_ByteIncrement * mem_ByteIncrement]);
                        }
                        break;
                }
                break;
        }
    }

    // Solution made with assistance from ChatGPT: https://chatgpt.com/share/676bdff1-82ac-8002-84cc-181b795877d7
    private void CrashTheFuckingCPU()
    {
        bool shitBool = true;
        switch (iterationLimit)
        {
            case 0:
                switch (cpu_UseMulticore)
                {
                    case true:
                        // Start multiple threads to fully utilize all CPU cores
                        for (int i = 0; i < System.Environment.ProcessorCount; i++)
                        {
                            Thread a_thread = new Thread(() =>
                            {
                                // Perform intensive computations
                                while (shitBool)
                                {
                                    double x = 0;
                                    for (int j = 0; j < cpu_CalcAmount; j++)
                                    {
                                        x += Mathf.Sin(j) * Mathf.Cos(j); // Arbitrary CPU-heavy computation
                                        Debug.Log(j);
                                    }
                                }
                            });
                            a_thread.IsBackground = true; // Ensures threads terminate when the application closes
                            a_thread.Start();
                        }
                        break;
                    case false:
                        Thread b_thread = new Thread(() =>
                        {
                            // Perform intensive computations
                            while (shitBool)
                            {
                                double x = 0;
                                for (int j = 0; j < cpu_CalcAmount; j++)
                                {
                                    x += Mathf.Sin(j) * Mathf.Cos(j); // Arbitrary CPU-heavy computation
                                    Debug.Log(j);
                                }
                            }
                        });
                        b_thread.IsBackground = true; // Ensures thread terminates when the application closes
                        b_thread.Start();
                        break;
                }
                break;
            case > 0:
                // Start multiple threads to fully utilize all CPU cores
                for (int i = 0; i < System.Environment.ProcessorCount; i++)
                {
                    Thread thread = new Thread(() =>
                    {
                        // Perform intensive computations
                        for (int j = 0; j < iterationLimit; j++)
                        {
                            double x = 0;
                            for (int k = 0; k < cpu_CalcAmount; k++)
                            {
                                x += Mathf.Sin(k) * Mathf.Cos(k); // Arbitrary CPU-heavy computation
                                Debug.Log(j);
                            }
                        }
                    });
                    thread.IsBackground = true; // Ensures threads terminate when the application closes
                    thread.Start();
                }
                break;
        }
    }

    // Solution made with assistance from ChatGPT: https://chatgpt.com/share/676bdff1-82ac-8002-84cc-181b795877d7
    private IEnumerator CrashTheFuckingGPU()
    {
        bool shitBool = true;

        switch (iterationLimit)
        {
            case 0:
                while (shitBool)
                {
                    for (int i = 0; i < gpu_MeshesToDraw; i++)
                    {
                        Graphics.DrawMesh(gpu_MeshToUse, Random.insideUnitSphere * gpu_UnitSphereExponent, Quaternion.identity, gpu_MaterialToUse, 0);
                        Graphics.DrawMeshNow(gpu_MeshToUse, Matrix4x4.TRS(Random.insideUnitSphere * gpu_UnitSphereExponent, Quaternion.identity, Vector3.one));
                    }
                    yield return null;
                    Debug.Log("Spongebob1");
                }
                break;
            case > 0:
                for (int i = 0; i < iterationLimit; i++)
                {
                    for (int j = 0; j < gpu_MeshesToDraw; j++)
                    {
                        Graphics.DrawMesh(gpu_MeshToUse, Random.insideUnitSphere * gpu_UnitSphereExponent, Quaternion.identity, gpu_MaterialToUse, 0);
                        Graphics.DrawMeshNow(gpu_MeshToUse, Matrix4x4.TRS(Random.insideUnitSphere * gpu_UnitSphereExponent, Quaternion.identity, Vector3.one));
                    }
                    yield return null;
                    Debug.Log("Spongebob2");
                }
                break;
        }
    }

    private void CrashTheFuckingEngine()
    {
        switch (crashType)
        {
            case ForcedCrashCategory.Abort:
                Utils.ForceCrash(ForcedCrashCategory.Abort); // Cause a crash by calling the abort() function.
                break;
            case ForcedCrashCategory.FatalError:
                Utils.ForceCrash(ForcedCrashCategory.FatalError); // Cause a crash using Unity's native fatal error implementation.
                break;
            case ForcedCrashCategory.AccessViolation:
                Utils.ForceCrash(ForcedCrashCategory.AccessViolation); // Cause a crash by performing an invalid memory access.
                break;
            case ForcedCrashCategory.MonoAbort:
                Utils.ForceCrash(ForcedCrashCategory.MonoAbort); // Cause a crash by calling the abort() function within the Mono dynamic library.
                break;
            case ForcedCrashCategory.PureVirtualFunction:
                Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction); // Cause a crash by calling a pure virtual function to raise an exception.
                break;
        }
    }

    private unsafe void OverflowTheFuckingEngine()
    {
        // Use unmanaged pointers to avoid managed runtime checks
        int* ptr = stackalloc int[1];
        *ptr = 0;
        OverflowTheFuckingEngine();
    }
}