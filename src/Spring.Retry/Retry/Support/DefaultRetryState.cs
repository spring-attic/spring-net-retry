// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRetryState.cs" company="The original author or authors.">
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
using Spring.Retry.Classify;
#endregion

namespace Spring.Retry.Retry.Support
{
    /// <summary>
    /// Default Retry State.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class DefaultRetryState : IRetryState
    {
        private readonly object key;

        private readonly bool forceRefresh;

        private readonly IClassifier<Exception, bool> rollbackClassifier;

        /// <summary>Initializes a new instance of the <see cref="DefaultRetryState"/> class. Initializes a new instance of the <see cref="DefaultRetryState"/> representing the state for a new retry attempt.</summary>
        /// <param name="key">The key for the state to allow this retry attempt to be recognised.</param>
        /// <param name="forceRefresh">True if the attempt is known to be a brand new state (could not have previously failed).</param>
        /// <param name="rollbackClassifier">The rollback classifier to set. The rollback classifier answers true if the exception provided should cause a rollback.</param>
        public DefaultRetryState(object key, bool forceRefresh, IClassifier<Exception, bool> rollbackClassifier)
        {
            this.key = key;
            this.forceRefresh = forceRefresh;
            this.rollbackClassifier = rollbackClassifier;
        }

        /// <summary>Initializes a new instance of the <see cref="DefaultRetryState"/> class, defaulting the force refresh flag to false.</summary>
        /// <param name="key">The key.</param>
        /// <param name="rollbackClassifier">The rollback classifier.</param>
        public DefaultRetryState(object key, IClassifier<Exception, bool> rollbackClassifier) : this(key, false, rollbackClassifier) { }
        
        /// <summary>Initializes a new instance of the <see cref="DefaultRetryState"/> class, defaulting the rollback classifier to null.</summary>
        /// <param name="key">The key.</param>
        /// <param name="forceRefresh">The force refresh.</param>
        public DefaultRetryState(object key, bool forceRefresh) : this(key, forceRefresh, null) { }
        
        /// <summary>Initializes a new instance of the <see cref="DefaultRetryState"/> class, defaulting the force refresh flag to false and the rollback classifier to null.</summary>
        /// <param name="key">The key.</param>
        public DefaultRetryState(object key) : this(key, false, null) { }

        /// <summary>The get key.</summary>
        /// <returns>The System.Object.</returns>
        public object GetKey() { return key; }

        /// <summary>The is force refresh.</summary>
        /// <returns>The System.Boolean.</returns>
        public bool IsForceRefresh() { return forceRefresh; }

        /// <summary>The rollback for.</summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The System.Boolean.</returns>
        public bool RollbackFor(Exception exception)
        {
            if (rollbackClassifier == null)
            {
                return true;
            }

            return rollbackClassifier.Classify(exception);
        }

        public override string ToString() { return string.Format("[{0}: key={1}, forceRefresh={2}]", this.GetType().Name, key, forceRefresh); }
    }
}
