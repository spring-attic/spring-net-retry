// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProxyMethodInvocation.cs" company="The original author or authors.">
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
using AopAlliance.Intercept;
#endregion

namespace Spring.Aop
{
    /// <summary>
    /// Extension of the AOP Alliance {@link org.aopalliance.intercept.MethodInvocation}
    /// interface, allowing access to the proxy that the method invocation was made through.
    /// Useful to be able to substitute return values with the proxy,
    /// if necessary, for example if the invocation target returned itself.
    /// </summary>
    /// <author>Juergen Hoeller</author>
    /// <author>Adrian Colyer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public interface IProxyMethodInvocation : IMethodInvocation
    {
        /// <summary>Return the proxy that this method invocation was made through.</summary>
        /// <returns>The original proxy object.</returns>
        object GetProxy();

        /// <summary>
        /// Create a clone of this object. If cloning is done before <code>proceed()</code>
        /// is invoked on this object, <code>proceed()</code> can be invoked once per clone
        /// to invoke the joinpoint (and the rest of the advice chain) more than once.
        /// </summary>
        /// <returns>An invocable clone of this invocation. Proceed() can be called once per clone.</returns>
        IMethodInvocation InvocableClone();

        /// <summary>
        /// Create a clone of this object. If cloning is done before <code>proceed()</code>
        /// is invoked on this object, <code>proceed()</code> can be invoked once per clone
        /// to invoke the joinpoint (and the rest of the advice chain) more than once.
        /// </summary>
        /// <param name="arguments">The arguments that the cloned invocation is supposed to use, overriding the original arguments.</param>
        /// <returns>An invocable clone of this invocation.</returns>
        IMethodInvocation InvocableClone(object[] arguments);

        /// <summary>Set the arguments to be used on subsequent invocations in the any advice in this chain.</summary>
        /// <param name="arguments">The arguments array.</param>
        void SetArguments(object[] arguments);

        /// <summary>
        /// Add the specified user attribute with the given value to this invocation.
        /// Such attributes are not used within the AOP framework itself. They are
        /// just kept as part of the invocation object, for use in special interceptors.
        /// </summary>
        /// <param name="key">The name of the attribute.</param>
        /// <param name="value">The value of the attribute, or null to reset it.</param>
        void SetUserAttribute(string key, object value);

        /// <summary>Return the value of the specified user attribute.</summary>
        /// <param name="key">The name of the attribute.</param>
        /// <returns>The value of the attribute, or null if not set.</returns>
        object GetUserAttribute(string key);
    }
}
