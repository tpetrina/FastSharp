using System.Linq;
using System.Threading.Tasks;
using FastSharp.Engine.Core;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Recommendations;

namespace FastSharp.Engine.Roslyn
{
    public class RoslynScriptingEngine : IScriptingEngine
    {
        private object state = null;

        public ScriptResult Compile(string code)
        {
            // CompilationErrorException
            try
            {
                var tree = SyntaxFactory.ParseSyntaxTree(code,
                    CSharpParseOptions.Default
                    .WithKind(SourceCodeKind.Script)
                    .WithLanguageVersion(LanguageVersion.CSharp6));

                var compilation = CSharpCompilation
                    .Create("test.exe")
                    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                    .AddSyntaxTrees(tree);

                var semanticModel = compilation.GetSemanticModel(tree, true);
                //var workspace = new AdhocWorkspace();
                //var symbols = Recommender.GetRecommendedSymbolsAtPosition(
                //    semanticModel,
                //    1,
                //    workspace);

                var diagnostics = compilation.GetDiagnostics();

                return new ScriptResult()
                {
                    CompilerErrors = diagnostics
                    .Select(i => new CompilerError
                    {
                        SourceSpan = new TextSpan(i.Location.SourceSpan.Start, i.Location.SourceSpan.Length),
                        Text = i.GetMessage()
                    })
                    .ToArray()
                };
            }
            catch (CompilationErrorException ex)
            {
                return new ScriptResult
                {
                    CompilerErrors = new CompilerError[] {
                        new CompilerError ()
                        {
                            Text = "Error"
                        }
                    }
                };
            }
        }

        public Task<ScriptResult> ExecuteAsync(string code)
        {
            return Task.Run(() =>
            {
                var result = CSharpScript.Run(code, state);
                state = result;
                return new ScriptResult
                {
                    ReturnValue = result.ReturnValue
                };
            });
        }

        public void Reset()
        {
            state = null;
        }

        public void Terminate()
        {
        }
    }
}
