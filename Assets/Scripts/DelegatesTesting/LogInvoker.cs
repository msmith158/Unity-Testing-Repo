using UnityEngine;

namespace DelegatesTesting 
{
    // This script is for learning how to use the multiple different ways to reference and invoke methods
    // as references/variables, known as delegates. This script makes use of delegate fields.
    //
    // Linked tutorials:
    // - "What are Delegates? (C# Basics, Lambda, Action, Func)" by CodeMonkey: https://youtu.be/3ZfwqWl-YI0
    // - "Func,Action,Predicate Delegates in C#" by Tactic Devs: https://youtu.be/LlZpno4_ylw
    
    public class LogInvoker : MonoBehaviour
    {
        // =================[#]  CONFIGURATION  [#]=================

        public DelegateTypeEnum DelegateType = DelegateTypeEnum.DelegateField;
        
        // =================[#]  INTERNAL VARIABLES/REFERENCES  [#]=================
        
        public enum DelegateTypeEnum
        {
            DelegateField
        };

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}