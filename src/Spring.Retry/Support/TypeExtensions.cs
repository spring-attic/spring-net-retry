// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="The original author or authors.">
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
using System.Linq;
using System.Reflection;
using Spring.Util;
#endregion

namespace Spring.Retry.Support
{
    /// <summary>
    /// Type Extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>The get method if available.</summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="paramTypes">The param types.</param>
        /// <returns>The System.Reflection.MethodInfo.</returns>
        public static MethodInfo GetMethodIfAvailable(this Type type, string methodName, Type[] paramTypes)
        {
            AssertUtils.ArgumentNotNull(type, "Type must not be null");
            AssertUtils.ArgumentNotNull(methodName, "Method name must not be null");
            try
            {
                return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>The get param string.</summary>
        /// <param name="paramArray">The param array.</param>
        /// <returns>The System.String.</returns>
        public static string GetParamsString(this Type[] paramArray)
        {
            if (paramArray == null)
            {
                return string.Empty;
            }

            try
            {
                var result = paramArray.Aggregate(string.Empty, (current, item) => current == string.Empty ? item.Name : current + ", " + item.Name);
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetArgsString(this object[] args)
        {
            if (args == null)
            {
                return string.Empty;
            }

            try
            {
                var result = args.Aggregate(string.Empty, (current, item) => current == string.Empty ? item.GetType().Name : current + ", " + item.GetType().Name);
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static Attribute FindAttribute(this MethodInfo type, Type attributeType)
        {
            try
            {
                var attributes = type.FindAttributes(attributeType);
                if (attributes != null && attributes.Count() == 1)
                {
                    return attributes.First();
                }
                else if (attributes != null && attributes.Count() > 1)
                {
                    throw new InvalidOperationException("More than one instance of the specified attribute is defined for this method.");
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static Attribute[] FindAttributes(this MethodInfo type, Type attributeType)
        {
            try
            {
                var attributes = type.GetCustomAttributes(attributeType, true);
                if (attributes is Attribute[] && attributes.Any())
                {
                    return (Attribute[])attributes;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
