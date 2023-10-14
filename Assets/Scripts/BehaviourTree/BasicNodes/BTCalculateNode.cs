using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Linq;

public class BTCalculateNode : BTActionNode
{
    BTBlackboard b;
    public string targetVariable;
    float newFloat = 0;
    int newInt = 0;
    ScriptOptions opt;
    Script<float> script;

    [TextArea]
    public string expression;

    

    protected override void OnStart()
    {
        b = tree.blackboard;
        opt = ScriptOptions.Default.WithImports("System.Math");
        opt.AddReferences(typeof(ScriptableObject).Assembly);
        opt.AddReferences("UnityEngine");
        script = CSharpScript.Create<float>("using UnityEngine;" + expression, opt, typeof(BTCalculateNode));
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        /*if (b.FindVariable(targetVariable, out newFloat))
        {
            newFloat = ParseExpression(expression);
            b.SetVariable(targetVariable, newFloat);
        }
        else if(b.FindVariable(targetVariable, out newInt))
        {
            newInt = (int)MathF.Round(ParseExpression(expression));
            b.SetVariable(targetVariable, newInt);
        }*/
        try
        {
            if (b.FindVariable(targetVariable, out newFloat))
                newFloat = script.RunAsync(globals: this).Result.ReturnValue;
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        return State.Succeeded;
    }

    public float ParseExpression(string expression)
    {
        expression = expression.Replace(" ", string.Empty);
        string expr = expression;

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
        for (int i = 0; i < variables.Count; i++)
        {
            if (b.floatVariables.ContainsKey(variables[i]))
            {
                expr = expr.Replace(variables[i], b.floatVariables[variables[i]].ToString());
            }
            else if (b.intVariables.ContainsKey(variables[i]))
            {
                expr = expr.Replace(variables[i], b.intVariables[variables[i]].ToString());
            }
        }


        // 计算表达式的结果
        try
        {
            var runner = CSharpScript.EvaluateAsync<float>(expr, opt, globals:this);
            //var result = Convert.ToSingle(new System.Data.DataTable().Compute(expr, null));
            
            
            return runner.Result;
            //return result;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
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

    public static T EvaluateCode<T>(string code)
    {
        var options = ScriptOptions.Default.WithImports("System");
        var script = CSharpScript.Create<T>(code, options);
        var compilation = script.GetCompilation();
        var diagnostics = compilation.GetDiagnostics();

        if (diagnostics.Any())
        {
            for (int i = 0; i < diagnostics.Length; i++)
            {
                Debug.LogWarning(diagnostics[i]);
            }
            
        }

        var result = script.RunAsync().Result;
        return result.ReturnValue;
    }
    
}