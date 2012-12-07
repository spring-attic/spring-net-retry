// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierAdapter.cs" company="The original author or authors.">
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
using Spring.Retry.Classify.Attributes;
using Spring.Retry.Classify.Util;
using Spring.Util;
#endregion

namespace Spring.Retry.Classify
{
    /// <summary>Wrapper for an object to adapt it to the {@link Classifier} interface.</summary>
    /// <typeparam name="C">Type C.</typeparam>
    /// <typeparam name="T">Type T.</typeparam>
    /// <author>Dave Syer</author><author>Joe Fitzgerald (.NET)</author>
    public class ClassifierAdapter<C, T> : IClassifier<C, T>
    {
        private IMethodInvoker invoker;

        private IClassifier<C, T> classifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierAdapter{C,T}"/> class.
        /// Default constructor for use with setter injection.
        /// </summary>
        public ClassifierAdapter() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierAdapter{C,T}"/> class.
        /// Create a new <see cref="IClassifier{C,T}"/> from the delegate provided. Use the
        /// constructor as an alternative to the 
        /// <see cref="SetDelegate(Spring.Retry.Classify.IClassifier{C,T})"/> method.
        /// </summary>
        /// <param name="classifierDelegate">The classifier delegate.</param>
        public ClassifierAdapter(object classifierDelegate) { SetDelegate(classifierDelegate); }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierAdapter{C,T}"/> class.
        /// Create a new <see cref="IClassifier{C,T}"/> from the delegate provided. Use the
        /// constructor as an alternative to the 
        /// <see cref="SetDelegate(Spring.Retry.Classify.IClassifier{C,T})"/> method.
        /// </summary>
        /// <param name="classifierDelegate">The classifier delegate.</param>
        public ClassifierAdapter(IClassifier<C, T> classifierDelegate) { this.classifier = classifierDelegate; }

        /// <summary>The set delegate.</summary>
        /// <param name="classifierDelegate">The classifier delegate.</param>
        public void SetDelegate(IClassifier<C, T> classifierDelegate)
        {
            this.classifier = classifierDelegate;
            this.invoker = null;
        }

        /// <summary>
        /// Search for the <see cref="ClassifierAttribute"/> attribute on a method in 
        /// the supplied delegate and use that to create a
        /// <see cref="IClassifier{C,T}"/> from the parameter type to the return type. 
        /// If the attribute is not found a unique non-void method with a
        /// single parameter will be used, if it exists. The signature of the method
        /// cannot be checked here, so might be a runtime exception when the method
        /// is invoked if the signature doesn't match the classifier types.
        /// </summary>
        /// <param name="classifierDelegate">The classifier delegate.</param>
        public void SetDelegate(object classifierDelegate)
        {
            this.classifier = null;
            this.invoker = MethodInvokerUtils.GetMethodInvokerByAttribute(typeof(ClassifierAttribute), classifierDelegate);
            if (this.invoker == null)
            {
                this.invoker = MethodInvokerUtils.GetMethodInvokerForSingleArgument(classifierDelegate);
            }

            AssertUtils.State(this.invoker != null, "No single argument public method with or without [Classifier] was found in delegate of type " + classifierDelegate.GetType().Name);
        }

        /// <summary>The classifier.</summary>
        /// <param name="classifiable">The classifiable.</param>
        /// <returns>The T.</returns>
        public T Classify(C classifiable)
        {
            if (this.classifier != null)
            {
                return this.classifier.Classify(classifiable);
            }

            return (T)this.invoker.InvokeMethod(classifiable);
        }
    }
}
