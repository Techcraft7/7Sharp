using System;
using System.Collections.Generic;
using System.Linq;
using Techcraft7_DLL_Pack.Text;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using _7Sharp;
using ExpressionEvaluation;

namespace _7Sharp.Intrerpreter
{
    using static Console;
    using static ConsoleColor;
    using static ColorConsoleMethods;
    public class Interpreter
    {
        internal Dictionary<string, dynamic> variables = new Dictionary<string, dynamic>();
        internal Dictionary<string, _7sFunction> functions = new Dictionary<string, _7sFunction>();

        private bool ContainsOnly(string s, string tester)
        {
            foreach (char c in s)
            {
                if (!tester.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private string[] GetArgs(string s)
        {
            s = s.Replace("\\\"", string.Empty); //removed escaped strings
            int last = 0;
            bool inString = false;
            List<string> args = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\"')
                {
                    inString = !inString;
                }
                if (!inString && s[i] != ' ')
                {
                    if (s[i] == ',' || i == s.Length - 1)
                    {
                        string v = s.Substring(last, i - last + (i == s.Length - 1 ? 1 : 0));
                        if (v[v.Length - 1] == ',')
                        {
                            v.Substring(0, v.Length - 2);
                        }
                        WriteLine("Arg: " + v);
                        args.Add(v);
                        last = i + 2;
                    }
                }
            }
            return args.ToArray();
        }

        private string StripComments(string s)
        {
            string[] split = s.Split('\n');
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].LastIndexOf("/") > -1)
                {
                    split[i] = split[i].Split('/')[0];
                }
            }
            return string.Join("\n", split);
        }

        public string GetArgString(string s)
        {
            int start = s.IndexOf("(");
            int e = s.LastIndexOf(")");
            return s.Substring(start, e - start + 1);
        }

        //replace escaped quotes and check if its divisible by 2
        public bool ValidateStrings(string code)
        {
            WriteLine(code.Replace("\\\"", string.Empty).Split('\"').Length);
            return (code.Replace("\\\"", string.Empty).Split('\"').Length - 1) % 2 == 0;
        }

        public bool ValidateBrackets(string code, out List<Tuple<int, int>> brackets)
        {
            WriteLine("Validate brackets");
            brackets = new List<Tuple<int, int>>();
            bool isString = false;
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>();
            for (int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '\"':
                        if (i == 0) //first char is a " it HAS to be a string
                        {
                            isString = true;
                            break;
                        }
                        if (code[i - 1] == '\\') break; //escaped "
                        isString = !isString;
                        break;
                    case '{':
                        if (isString) break;
                        stack.Push(new Tuple<int, int>(i, -1));
                        break;
                    case '}':
                        if (isString) break;
                        brackets.Add(new Tuple<int, int>(stack.Pop().Item1, i));
                        break;
                }
            }
            WriteLine("Done!");
            return stack.Count == 0;
        }

        public void Run(string code)
        {
            code = StripComments(code);
            string[] split = code.Split('\n');
            List<Tuple<int, int>> brackets = new List<Tuple<int, int>>();
            if (!ValidateBrackets(code, out brackets))
            {
                WriteLineColor("Invalid brackets!", Red);
                return;
            }
            //remove whitespace
            WriteLine("Removing whitespace!");
            for (int i = 0; i < split.Length; i++)
            {
                int start = 0;
                int end = split[i].Length - 1;
                while (string.IsNullOrWhiteSpace(split[i][start].ToString()))
                {
                    start++;
                }
                while (string.IsNullOrWhiteSpace(split[i][end].ToString()))
                {
                    end--;
                }
                split[i] = split[i].Substring(start, end - start + 1);
            }
            WriteLine("Done!");
            WriteLine("Respliting!");
            code = string.Join("\n", split);
            split = code.Split('\n');
            WriteLine("Done!");
            WriteLine("Revaluate brackets");
            _ = ValidateBrackets(code, out brackets);
            WriteLine("Done!");
            foreach (var i in brackets)
            {
                WriteLine(i);
            }
            int codeIndex = 0;
            for (int i = 0; i < split.Length; i++)
            {
                string line = split[i];
                WriteLine($"Evaluating line: {line}");
                if (line.StartsWith("if"))
                {
                    HandleIf(ref code, ref split, ref brackets, ref line, ref i, ref codeIndex);
                }
                else if (line.StartsWith("loop"))
                {
                    HandleLoop(ref code, ref split, ref brackets, ref line, ref i, ref codeIndex);
                }
                codeIndex += line.Length + 1;
            }
        }

        public bool HandleIf(ref string code, ref string[] split, ref List<Tuple<int, int>> brackets, ref string line, ref int i, ref int codeIndex)
        {
            if (!line.EndsWith("{"))
            {
                WriteLineColor($"Error on line {i + 1}: if statement has no brackets (are they on the same line?)!", Red);
                return false;
            }
            line = line.Replace("\\\"", string.Empty);
            string args = GetArgString(line);
            if (args.Length == 0)
            {
                WriteLineColor($"Error on line {i + 1}: if statement has no arguments!", Red);
                return false;
            }
            ExpressionEval ee = new ExpressionEval(args);
            WriteLine($"Expression to evaluate: {ee.Expression}");
            foreach (string k in variables.Keys)
            {
                WriteLine($"Var {k} is {variables[k]}");
                ee.SetVariable(k, variables[k]);
            }
            try
            {
                if (!ee.EvaluateBool())
                {
                    WriteLine("FALSE");
                    int end = codeIndex + line.Length - 1;
                    WriteLine($"Index of end: {end} char: {code[end]}");
                    int x = end;
                    x = brackets.Find(v => v.Item1 == x).Item2;
                    WriteLine($"Skipping to {code[x]} at {x}");
                    int total = 0;
                    for (int l = 0; l < split.Length; l++)
                    {
                        for (int k = 0; k < split[l].Length; k++)
                        {
                            total++;
                            if (total == x)
                            {
                                i = l - 1;
                                l = split.Length;
                                break;
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                WriteLineColor($"Error on line {i + 1} of type {e.GetType()}: {e.Message}", Red);
                return false;
            }
            return true;
        }

        public bool HandleLoop(ref string code, ref string[] split, ref List<Tuple<int, int>> brackets, ref string line, ref int i, ref int codeIndex)
        {

            return true;
        }
    }
}
