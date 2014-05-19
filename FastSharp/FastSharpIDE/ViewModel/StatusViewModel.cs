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

namespace FastSharpIDE.ViewModel
{
    public enum StatusType
    {
        Info
    }

    public class StatusViewModel : ViewModelBase
    {
        #region Bindable properties
        public string Text
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public StatusType Type
        {
            get { return Get<StatusType>(); }
            set { Set(value); }
        }
        #endregion

        public StatusViewModel()
        {
            SetReady();
        }

        private void SetReady()
        {
            Text = "Ready";
            Type = StatusType.Info;
        }
    }
}
