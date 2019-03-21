// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodInvokerUtils.cs" company="The original author or authors.">
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
using System.Reflection;
using System.Text;
using Spring.Aop.Framework;
using Spring.Retry.Support;
using Spring.Util;
#endregion

namespace Spring.Retry.Classify.Util
{
    /// <summary>
    /// Utility methods for create MethodInvoker instances.
    /// </summary>
    /// <author>Lucas Ward</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class MethodInvokerUtils
    {
        /// <summary>Create a <see cref="IMethodInvoker"/> using the provided method name to search.</summary>
        /// <param name="targetObject">The target object to be invoked.</param>
        /// <param name="methodName">The method name of the method to be invoked.</param>
        /// <param name="paramsRequired">Boolean indicating whether the parameters are required. If false, a no args version of the method will be searched for.</param>
        /// <param name="paramTypes">Parameter types of the method to be searched for.</param>
        /// <returns>The Spring.Retry.Classify.Util.IMethodInvoker if it is found, null if it is not.</returns>
        public static IMethodInvoker GetMethodInvokerByName(object targetObject, string methodName, bool paramsRequired, params Type[] paramTypes)
        {
            AssertUtils.ArgumentNotNull(targetObject, "Object to invoke must not be null");
            var method = targetObject.GetType().GetMethodIfAvailable(methodName, paramTypes);

            // ClassUtils.getMethodIfAvailable(targetObject.getClass(), methodName, paramTypes);
            if (method == null)
            {
                var errorMsg = "no method found with name [" + methodName + "] on type [" + targetObject.GetType().Name + "] compatable with the signature [" + GetParamTypesString(paramTypes) + "].";
                AssertUtils.IsTrue(!paramsRequired, errorMsg);

                // if no method was found for the given parameters, and the
                // parameters aren't required, then try with no params
                method = targetObject.GetType().GetMethodIfAvailable(methodName, new Type[] { });
                AssertUtils.IsTrue(method != null, errorMsg);
            }

            return new SimpleMethodInvoker(targetObject, method);
        }

        /// <summary>Create a String representation of the array of parameter types.</summary>
        /// <param name="paramTypes">The param types.</param>
        /// <returns>The System.String.</returns>
        public static string GetParamTypesString(params Type[] paramTypes)
        {
            var paramTypesList = new StringBuilder("(");
            for (var i = 0; i < paramTypes.Length; i++)
            {
                paramTypesList.Append(paramTypes[i].Name);
                if (i + 1 < paramTypes.Length)
                {
                    paramTypesList.Append(", ");
                }
            }

            return paramTypesList.Append(")").ToString();
        }

        /// <summary>Create a <see cref="IMethodInvoker"/> using the provided interface, and method name from that interface.</summary>
        /// <param name="cls">The interface to search for the method named.</param>
        /// <param name="methodName">The method name of the method to be invoked.</param>
        /// <param name="targetObject">The target object to be invoked.</param>
        /// <param name="paramTypes">The parameter types of the method to search for..</param>
        /// <returns>The Spring.Retry.Classify.Util.IMethodInvoker if the method is found, null if it is not.</returns>
        public static IMethodInvoker GetMethodInvokerForInterface(Type cls, string methodName, object targetObject, params Type[] paramTypes)
        {
            if (cls.IsInstanceOfType(targetObject))
            {
                return GetMethodInvokerByName(targetObject, methodName, true, paramTypes);
            }
            else
            {
                return null;
            }
        }

        /// <summary>Create a MethodInvoker from the delegate based on the attributeType. Ensure that the decorated method has a valid set of parameters.</summary>
        /// <param name="attributeType">The attribute type.</param>
        /// <param name="target">The target.</param>
        /// <param name="expectedParamTypes">The expected param types.</param>
        /// <returns>The Spring.Retry.Classify.Util.IMethodInvoker.</returns>
        public static IMethodInvoker GetMethodInvokerByAttribute(Type attributeType, object target, params Type[] expectedParamTypes)
        {
            AssertUtils.AssertArgumentType(attributeType, "attributeType", typeof(Attribute), "attributeType should be an Attribute");
            var mi = GetMethodInvokerByAttribute(attributeType, target);
            var targetClass = (target is IAdvised) ? ((IAdvised)target).TargetSource.TargetType : target.GetType();
            if (mi != null)
            {
                var methods = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var method in methods)
                {
                    var attribute = method.FindAttribute(attributeType);
                    if (attribute != null)
                    {
                        var paramTypes = method.GetParameters();

                        if (paramTypes.Length > 0)
                        {
                            var errorMsg = "The method [" + method.Name + "] on target class [" + targetClass.Name + "] is incompatable with the signature ["
                                           + GetParamTypesString(expectedParamTypes) + "] expected for the attribute [" + attributeType.Name + "].";

                            AssertUtils.IsTrue(paramTypes.Length == expectedParamTypes.Length, errorMsg);
                            for (var i = 0; i < paramTypes.Length; i++)
                            {
                                AssertUtils.IsTrue(expectedParamTypes[i].IsAssignableFrom(paramTypes[i].ParameterType), errorMsg);
                            }
                        }
                    }
                }
            }

            return mi;
        }

        /// <summary>Create <see cref="IMethodInvoker"/> for the method with the provided attribute on the provided object.
        /// Attributes that cannot be applied to methods (i.e. that aren't decorated with an element type of METHOD)
        /// will cause an exception to be thrown.</summary>
        /// <param name="attributeType">The attribute type to be searched for.</param>
        /// <param name="target">The target to be invoked.</param>
        /// <returns>The Spring.Retry.Classify.Util.IMethodInvoker for the provided attribute, null if none is found.</returns>
        public static IMethodInvoker GetMethodInvokerByAttribute(Type attributeType, object target)
        {
            AssertUtils.IsTrue(typeof(Attribute).IsAssignableFrom(attributeType), "attributeType should be an Attribute");

            AssertUtils.ArgumentNotNull(target, "Target must not be null");
            AssertUtils.ArgumentNotNull(attributeType, "AttributeType must not be null");
            var attributes = (AttributeUsageAttribute[])attributeType.GetCustomAttributes(typeof(AttributeUsageAttribute), true);
            AssertUtils.IsTrue(attributes.Length == 1, "Expected only one AttributeUsage Attribute on the target Attribute Type");
            AssertUtils.IsTrue((attributes[0].ValidOn & AttributeTargets.Method) == AttributeTargets.Method, "Attribute [" + attributeType.Name + "] is not a Method-level attribute.");

            var targetClass = (target is IAdvised) ? ((IAdvised)target).TargetSource.TargetType : target.GetType();
            if (targetClass == null)
            {
                // Proxy with no target cannot have attributes
                return null;
            }

            var decoratedMethod = new BlockingCollection<MethodInfo>(1);
            var methods = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                var attribute = method.FindAttribute(attributeType);
                if (attribute != null)
                {
                    AssertUtils.IsTrue(decoratedMethod.Count == 0, "found more than one method on target class [" + targetClass.Name + "] with the attribute type [" + attributeType.Name + "].");
                    decoratedMethod.Add(method);
                }
            }

            if (decoratedMethod.Count < 1)
            {
                return null;
            }
            else
            {
                return new SimpleMethodInvoker(target, decoratedMethod.Take());
            }
        }

        /// <summary>Create a <see cref="IMethodInvoker"/> for the delegate from a single public method.</summary>
        /// <param name="target">The target.</param>
        /// <returns>The Spring.Retry.Classify.Util.IMethodInvoker.</returns>
        public static IMethodInvoker GetMethodInvokerForSingleArgument(object target)
        {
            var methodHolder = new BlockingCollection<MethodInfo>();
            var methods = target.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.GetParameters().Length != 1)
                {
                    break;
                }

                if (method.ReturnType == typeof(void) || method.Name == "Equals")
                {
                    break;
                }

                AssertUtils.State(methodHolder.Count == 0, "More than one non-void public method detected with single argument.");
                methodHolder.Add(method);
            }

            if (methodHolder.Count > 0)
            {
                var targetMethod = methodHolder.Take();
                return new SimpleMethodInvoker(target, targetMethod);
            }

            return null;
        }
    }
}
