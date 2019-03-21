// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExceptionClassifier.cs" company="The original author or authors.">
//   Copyright 2002-2012 the original author or authors.
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//   the License. You may obtain a copy of the License at
//   
//   https://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//   an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//   specific language governing permissions and limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
#endregion

namespace Spring.Retry.Classify
{
    /// <summary>
    /// Binary Exception Classifier
    /// </summary>
    public class BinaryExceptionClassifier : SubclassClassifier<Exception, bool>
    {
        /// <summary>Initializes a new instance of the <see cref="BinaryExceptionClassifier"/> class.</summary>
        /// <param name="defaultValue">The default value.</param>
        public BinaryExceptionClassifier(bool defaultValue) : base(defaultValue) { }

        /// <summary>Initializes a new instance of the <see cref="BinaryExceptionClassifier"/> class.</summary>
        /// <param name="exceptionClasses">The exception classes.</param>
        /// <param name="value">The value.</param>
        public BinaryExceptionClassifier(IEnumerable<Type> exceptionClasses, bool value) : this(!value)
        {
            if (exceptionClasses != null)
            {
                var map = new Dictionary<Type, bool>();
                foreach (var type in exceptionClasses)
                {
                    map.Add(type, !this.GetDefault());
                }

                this.SetTypeMap(map);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="BinaryExceptionClassifier"/> class.</summary>
        /// <param name="exceptionClasses">The exception classes.</param>
        public BinaryExceptionClassifier(IEnumerable<Type> exceptionClasses) : this(exceptionClasses, true) { }

        /// <summary>Initializes a new instance of the <see cref="BinaryExceptionClassifier"/> class.</summary>
        /// <param name="typeMap">The type map.</param>
        public BinaryExceptionClassifier(IDictionary<Type, bool> typeMap) : this(typeMap, false) { }

        /// <summary>Initializes a new instance of the <see cref="BinaryExceptionClassifier"/> class.</summary>
        /// <param name="typeMap">The type map.</param>
        /// <param name="defaultValue">The default value.</param>
        public BinaryExceptionClassifier(IDictionary<Type, bool> typeMap, bool defaultValue) : base(typeMap, defaultValue) { }
    }
}
