using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ChangeString 
{
    public static string StringChange(string source, string newValue, string replaceVarName)
    {
        // Replace one substring with another with String.Replace.
        // Only exact matches are supported.
        var replacement = source.Replace(replaceVarName, newValue);

        return (replacement);
    }
}
