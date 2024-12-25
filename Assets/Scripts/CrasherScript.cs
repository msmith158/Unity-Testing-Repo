using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CrasherScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField][Tooltip("Use this if you want a limit of how many loop iteration are executed. For an infinite loop execution, set to 0.")] private int iterationLimit;
    
    [Header("Memory Eater")] 
    [SerializeField] private bool useMemoryEater;
    [SerializeField] private string mem_StringContent;
    
    [Header("CPU Eater")] 
    [SerializeField] private bool useCpuEater;
    [SerializeField][Tooltip("How many CPU-expensive calculations should be done per iteration?")] private int cpu_CalcAmount;

    [Header("Unity Crash")] 
    [SerializeField] private bool useUnityCrash;
    [SerializeField] private ForcedCrashCategory crashType;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (useCpuEater) CrashTheFuckingCPU();
            if (useMemoryEater) CrashTheFuckingMemory();
            if (useUnityCrash) CrashTheFuckingEngine();
            if (!useMemoryEater && !useCpuEater) Debug.Log("No crash method selected!");
        }
    }

    private void CrashTheFuckingMemory()
    {
        bool shitBool = true;
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
                    iteration++;
                    Debug.Log(iteration);
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
    }

    private void CrashTheFuckingCPU()
    {
        bool shitBool = true;
        switch (iterationLimit)
        {
            case 0:
                // Start multiple threads to fully utilize all CPU cores
                for (int i = 0; i < System.Environment.ProcessorCount; i++)
                {
                    Thread thread = new Thread(() =>
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
                    thread.IsBackground = true; // Ensures threads terminate when the application closes
                    thread.Start();
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

    private void CrashTheFuckingEngine()
    {
        switch (crashType)
        {
            case ForcedCrashCategory.Abort:
                Utils.ForceCrash(ForcedCrashCategory.Abort);
                break;
            case ForcedCrashCategory.FatalError:
                Utils.ForceCrash(ForcedCrashCategory.FatalError);
                break;
            case ForcedCrashCategory.AccessViolation:
                Utils.ForceCrash(ForcedCrashCategory.AccessViolation);
                break;
            case ForcedCrashCategory.MonoAbort:
                Utils.ForceCrash(ForcedCrashCategory.MonoAbort);
                break;
            case ForcedCrashCategory.PureVirtualFunction:
                Utils.ForceCrash(ForcedCrashCategory.PureVirtualFunction);
                break;
        }
    }
}