using System.Collections;
using UnityEngine;

public class Fruit
{
    public Fruit()
    {
        Debug.Log("A new fruit has been created! I am a constructor, hai :3");
    }
    
    // These following methods are virtual and thus can be overriden in child classes.
    /// <summary>
    /// Chops the fruit.
    /// </summary>
    public virtual void Chop()
    {
        Debug.Log("The fruit has been chopped.");
    }

    /// <summary>
    /// Says hello to the fruit.
    /// </summary>
    public virtual void SayHello()
    {
        Debug.Log("You say hello to the fruit. It says hello back to you.");
    }
}
