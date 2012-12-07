// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleMethodInvoker.cs" company="The original author or authors.">
//   Copyright 2002-2012 the original author or authors.
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//   the License. You may obtain a copy of the License at
//   
//   http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//   an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//   specific language governing permissions and limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Reflection;
using Spring.Aop.Framework;
using Spring.Retry.Support;
using Spring.Util;
#endregion

namespace Spring.Retry.Classify.Util
{
    /// <summary>
    /// Simple implementation of the <see cref="IMethodInvoker"/> interface that invokes a method on an object. If the method has no arguments, but arguments are
    /// provided, they are ignored and the method is invoked anyway. If there are more arguments than there are provided, then an exception is thrown.
    /// </summary>
    /// <author>Lucas Ward</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class SimpleMethodInvoker : IMethodInvoker
    {
        private readonly object targetObject;

        private readonly MethodInfo method;

        /// <summary>Initializes a new instance of the <see cref="SimpleMethodInvoker"/> class.</summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="method">The method.</param>
        public SimpleMethodInvoker(object targetObject, MethodInfo method)
        {
            AssertUtils.ArgumentNotNull(targetObject, "Object to invoke must not be null");
            AssertUtils.ArgumentNotNull(method, "Method to invoke must not be null");
            this.method = method;
            this.targetObject = targetObject;
        }

        /// <summary>Initializes a new instance of the <see cref="SimpleMethodInvoker"/> class.</summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="methodName">The method name.</param>
        /// <param name="paramTypes">The param types.</param>
        public SimpleMethodInvoker(object targetObject, string methodName, params Type[] paramTypes)
        {
            AssertUtils.ArgumentNotNull(targetObject, "Object to invoke must not be null");
            this.method = targetObject.GetType().GetMethodIfAvailable(methodName, paramTypes);
            if (this.method == null)
            {
                // try with no params
                this.method = targetObject.GetType().GetMethodIfAvailable(methodName, new Type[] { });
            }

            if (this.method == null)
            {
                throw new ArgumentException("No methods found for name: [" + methodName + "] in class: [" + targetObject.GetType().Name + "] with arguments of type: [" + paramTypes.GetParamsString() + "]");
            }

            this.targetObject = targetObject;
        }

        /// <summary>The invoke method.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The System.Object.</returns>
        public object InvokeMethod(params object[] args)
        {
            var parameterTypes = this.method.GetParameters();
            object[] invokeArgs;
            if (parameterTypes.Length == 0)
            {
                invokeArgs = new object[] { };
            }
            else if (parameterTypes.Length != args.Length)
            {
                throw new InvalidOperationException("Wrong number of arguments, expected no more than: [" + parameterTypes.Length + "]");
            }
            else
            {
                invokeArgs = args;
            }

            // method.setAccessible(true);

            try
            {
                // Extract the target from an Advised as late as possible
                // in case it contains a lazy initialization
                var target = this.ExtractTarget(this.targetObject, this.method);
                return this.method.Invoke(target, invokeArgs);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to invoke method: [" + this.method + "] on object: [" + this.targetObject + "] with arguments: [" + args.GetArgsString() + "]", e);
            }
        }

        private object ExtractTarget(object target, MethodInfo method)
        {
            if (target is IAdvised)
            {
                object source;
                try
                {
                    source = ((IAdvised)target).TargetSource.GetTarget();
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("Could not extract target from proxy", e);
                }

                if (source is IAdvised)
                {
                    source = this.ExtractTarget(source, method);
                }

                if (method.DeclaringType != null && method.DeclaringType.IsInstanceOfType(source))
                {
                    target = source;
                }
            }

            return target;
        }

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The System.Boolean.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SimpleMethodInvoker))
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            var rhs = (SimpleMethodInvoker)obj;
            return rhs.method.Equals(this.method) && rhs.targetObject.Equals(this.targetObject);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The System.Int32.</returns>
        public override int GetHashCode()
        {
            var result = 25;
            result = 31 * result + this.targetObject.GetHashCode();
            result = 31 * result + this.method.GetHashCode();
            return result;
        }
    }
}
