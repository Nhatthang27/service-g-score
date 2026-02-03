using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GScore.Application.Constants;

public static class Languages
{
    public const string CSharp = "csharp";
    public const string Java = "java";
    public const string Python = "python";
    public const string Cpp = "cpp";
    public const string JavaScript = "javascript"; 

    private static readonly HashSet<string> _supported = new()
    {
        CSharp, Java, Python, Cpp, JavaScript
    };

    public static bool IsSupported(string lang) => _supported.Contains(lang.ToLower());
}