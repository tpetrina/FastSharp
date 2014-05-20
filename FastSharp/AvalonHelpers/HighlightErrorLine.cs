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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using System;
using System.Windows.Media;

namespace AvalonHelpers
{
    public class HighlightErrorLine : DocumentColorizingTransformer
    {
        private readonly Func<ReadOnlyArray<CommonDiagnostic>> _errorsFunc;

        public HighlightErrorLine(Func<ReadOnlyArray<CommonDiagnostic>> errorsFunc)
        {
            _errorsFunc = errorsFunc;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            var errors = _errorsFunc();
            if (errors == null)
                return;

            var text = CurrentContext.Document.GetText(line);
            var start = line.Offset;
            var end = line.Offset + text.Length;

            foreach (var error in errors)
            {
                var span = error.Location.SourceSpan;
                if (span.Start >= start && span.Start <= end)
                    ChangeLinePart(span.Start, Math.Min(span.End, end), HighlightError);
            }
        }

        private void HighlightError(VisualLineElement visualLineElement)
        {
            visualLineElement.TextRunProperties.SetBackgroundBrush(Brushes.PaleVioletRed);
        }
    }
}
