using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCalculateNode: BTActionNode
{
    BTBlackboard b;
    public string targetVariable;
    float newFloat = 0;
    int newInt = 0;

    [TextArea]
    public string expression;

    protected override void OnStart()
    {
        b = tree.blackboard;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (b.FindVariable(targetVariable, out newFloat))
        {
            newFloat = ParseExpression(expression);
            b.SetVariable(targetVariable, newFloat);
        }
        else if(b.FindVariable(targetVariable, out newInt))
        {
            newInt = (int)MathF.Round(ParseExpression(expression));
            b.SetVariable(targetVariable, newInt);
        }
        return State.Succeeded;
    }

    public float ParseExpression(string expression)
    {
        string expr = string.Empty;
        expression = expression.Replace(" ", string.Empty);

        // 检查表达式中的变量名是否在blackboard中存在
        var variables = GetVariables(expression);

        foreach (var variable in variables)
        {
            if (!b.floatVariables.ContainsKey(variable) && !b.intVariables.ContainsKey(variable))
            {
                Debug.LogError($"Variable '{variable}' not found in the dictionary.");
            }
        }

        // 替换变量名为对应的值
        foreach (var variable in variables)
        {
            if(b.floatVariables.ContainsKey(variable))
                expr = expression.Replace(variable, b.floatVariables[variable].ToString());
            else if(b.intVariables.ContainsKey(variable))
                expr = expression.Replace(variable, b.intVariables[variable].ToString());
        }

        // 计算表达式的结果
        try
        {
            var result = Convert.ToSingle(new System.Data.DataTable().Compute(expr, null));
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to evaluate the expression.", ex);
        }
    }

    private List<string> GetVariables(string expression)
    {
        var variables = new List<string>();
        var expressionChars = expression.ToCharArray();
        var variable = string.Empty;
        for (int i = 0; i < expressionChars.Length; i++)
        {
            var c = expressionChars[i];
            if (char.IsLetter(c))
            {
                variable += c;
                if (i == expressionChars.Length - 1)
                {
                    variables.Add(variable);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(variable))
                {
                    variables.Add(variable);
                    variable = string.Empty;
                }
            }
        }
        return variables;
    }
}