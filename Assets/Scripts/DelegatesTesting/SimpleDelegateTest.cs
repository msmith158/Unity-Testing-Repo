using UnityEngine;

namespace DelegatesTesting
{
    // This script is for learning how to use the multiple different ways to reference and invoke methods
    // as references/variables, known as delegates, in a simple implementation. This script currently
    // uses delegate fields, which are how you can store a method (or a reference to one) as a field.
    // =====================================================================================================
    // Linked tutorials:
    // - "What are Delegates? (C# Basics, Lambda, Action, Func)" by CodeMonkey: https://youtu.be/3ZfwqWl-YI0
    // - "Func,Action,Predicate Delegates in C#" by Tactic Devs: https://youtu.be/LlZpno4_ylw
    
    public class SimpleDelegateTest : MonoBehaviour
    {
        
        private const string MessageText = "How much money ya got?";
        
        // Delegate fields are defined like how methods are defined in interfaces (without implementation) but also have
        // the added "delegate" keyword. This way, they act as a sort of container reference for methods. Unlike how
        // other fields work, however, defining a delegate void here is like defining an enum—it's like a type, which
        // requires a reference in the form of an instance field (see below).
        private delegate void MyDelegate();
        private delegate bool MyBoolDelegate();
        
        // These are instance fields of the delegate types defined above and allows you to use a delegate type as a
        // reference type. Think like how you have to define an enum and then a reference to that enum.
        private MyDelegate myDelegate;
        private MyBoolDelegate myBoolDelegate;

        private void Awake()
        {
            // Delegate assignment can be done either with a method signature or an anonymous method (e.g. lambda
            // expression). This then stores the reference to that method inside the instanced delegate field
            // (myDelegate) and can be called at any time.
            myDelegate = MyDelegateThingy;
            
            // Remember that you can also just make a local variable out of the delegate reference if you need something
            // only within the lifespan of a function.
            MyDelegate myLocalDelegate = () => Debug.Log("Starting the delegate test!");
            myLocalDelegate();
        }

        private void Start()
        {
            // Once you have the delegate method initialised, you can just call it exactly like how you would by just
            // calling a regular function.
            myDelegate();
        }
        
        public void MyDelegateThingy() => Debug.Log(logText);
    }
}