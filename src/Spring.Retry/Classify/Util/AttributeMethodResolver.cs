// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeMethodResolver.cs" company="The original author or authors.">
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
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Spring.Aop.Framework;
using Spring.Util;
#endregion

namespace Spring.Retry.Classify.Util
{
    /// <summary>MethodResolver implementation that finds a <em>single</em> Method on the given Type that is decorated with the specified attribute.</summary>
    /// <typeparam name="T">Type T where T is an Attribute</typeparam>
    /// <author>Mark Fisher</author><author>Joe Fitzgerald (.NET)</author>
    public class AttributeMethodResolver<T> : IMethodResolver where T : Attribute
    {
        private readonly Type attributeType;

        /// <summary>Initializes a new instance of the <see cref="AttributeMethodResolver{T}"/> class. Create a MethodResolver for the specified Method-level attribute type</summary>
        public AttributeMethodResolver()
        {
            var attributes = (AttributeUsageAttribute[])typeof(T).GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            AssertUtils.IsTrue(attributes.Count() == 1, "Expected only one AttributeUsage Attribute on the target Attribute Type");
            AssertUtils.IsTrue((attributes[0].ValidOn & AttributeTargets.Method) == AttributeTargets.Method, "Attribute [" + typeof(T).Name + "] is not a Method-level attribute.");
            
            this.attributeType = typeof(T);
        }

        /// <summary>The find method.</summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>The System.Reflection.MethodInfo.</returns>
        public MethodInfo FindMethod(object candidate)
        {
            AssertUtils.ArgumentNotNull(candidate, "candidate object must not be null");
            var targetType = AopUtils.GetTargetType(candidate);
            if (targetType == null)
            {
                targetType = candidate.GetType();
            }

            return this.FindMethod(targetType);
        }

        /// <summary>The find method.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The System.Reflection.MethodInfo.</returns>
        public MethodInfo FindMethod(Type type)
        {
            AssertUtils.ArgumentNotNull(type, "type must not be null");

            var decoratedMethod = new BlockingCollection<MethodInfo>(1);

            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attributes = method.GetCustomAttributes(this.attributeType, true);
                if (attributes.Any())
                {
                    AssertUtils.State(decoratedMethod.Count == 0, "found more than one method on target class [" + type + "] with the attribute type [" + this.attributeType + "]");
                    decoratedMethod.Add(method);
                }
            }

            if (decoratedMethod.Count > 0)
            {
                return decoratedMethod.Take();
            }

            return null;
        }
    }
}
