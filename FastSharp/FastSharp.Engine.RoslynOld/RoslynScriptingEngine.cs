// Copyright 2014 Toni Petrina
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using FastSharp.Engine.Core;
using Roslyn.Compilers;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System.Linq;
using System.Threading.Tasks;
using TextSpan = FastSharp.Engine.Core.TextSpan;

namespace FastSharp.Engine.RoslynOld
{
    public class RoslynScriptingEngine : IScriptingEngine
    {
        private readonly ScriptEngine _engine;
        private Session _session;

        public RoslynScriptingEngine()
        {
            _engine = new ScriptEngine();
            _session = _engine.CreateSession();
        }

        public void Reset()
        {
            _session = _engine.CreateSession();
        }

        public void Terminate()
        {
        }

        public async Task<ScriptResult> ExecuteAsync(string code)
        {
            try
            {
                var o = await Task.Run(() => _session.Execute(code));

                return new ScriptResult
                {
                    ReturnValue = o
                };
            }
            catch (CompilationErrorException ex)
            {
                return FromCompilationException(ex);
            }
        }

        public ScriptResult Compile(string code)
        {
            try
            {
                var session = _engine.CreateSession();
                session.CompileSubmission<object>(code);
                return new ScriptResult();
            }
            catch (CompilationErrorException ex)
            {
                return FromCompilationException(ex);
            }
        }

        private static ScriptResult FromCompilationException(CompilationErrorException ex)
        {
            return new ScriptResult
            {
                CompilerErrors = ex.Diagnostics.Select(
                    e =>
                        new CompilerError
                        {
                            Text = e.ToString(),
                            SourceSpan = new TextSpan(e.Location.SourceSpan.Start, e.Location.SourceSpan.Length)
                        }).ToArray()
            };
        }
    }
}
