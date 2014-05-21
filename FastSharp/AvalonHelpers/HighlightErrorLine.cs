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

using System.Collections.ObjectModel;
using System.Linq;
using FastSharp.Engine.Core;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Windows.Media;

namespace AvalonHelpers
{
    public class HighlightErrorLine : DocumentColorizingTransformer
    {
        private ObservableCollection<CompilerError> _errors;

        public HighlightErrorLine(ObservableCollection<CompilerError> errors)
        {
            _errors = errors;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (!_errors.Any())
                return;

            var text = CurrentContext.Document.GetText(line);
            var start = line.Offset;
            var end = line.Offset + text.Length;

            foreach (var error in _errors)
            {
                var span = error.SourceSpan;
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
