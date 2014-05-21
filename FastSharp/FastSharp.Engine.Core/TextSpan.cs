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

namespace FastSharp.Engine.Core
{
    public class TextSpan
    {
        public int Start { get; private set; }
        public int Length { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return Length == 0;
            }
        }

        public int End
        {
            get
            {
                return Start + Length;
            }
        }

        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}
