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

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FastSharpIDE.ViewModel
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        protected new bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
        {
            return Set(propertyName, ref field, newValue);
        }

        protected T Get<T>([CallerMemberName] string propertyName = "")
        {
            object value;
            if (_properties.TryGetValue(propertyName, out value))
                return (T)value;
            return default(T);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = "")
        {
            object value;
            if (_properties.TryGetValue(propertyName, out value))
            {
                if (EqualityComparer<T>.Default.Equals((T)value, newValue))
                    return false;
            }

            _properties[propertyName] = newValue;
            return true;
        }
    }
}
