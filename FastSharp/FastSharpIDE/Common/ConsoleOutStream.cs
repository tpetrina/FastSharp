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

using System.IO;
using System.Text;

namespace FastSharpIDE.Common
{
    public class ConsoleOutStream : TextWriter
    {
        public StringBuilder Output { get; set; }

        public ConsoleOutStream()
        {
            Output = new StringBuilder();
        }

        public override void Write(char value)
        {
            Output.Append(value);
        }

        public override void Write(string value)
        {
            Output.Append(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}
