// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcelessTransactionManager.cs" company="The original author or authors.">
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
using System.Collections.Generic;
using Spring.Transaction;
using Spring.Transaction.Support;
#endregion

namespace Spring.Retry.Tests.Retry
{
    /// <summary>
    /// Resourceless Transaction Manager
    /// </summary>
    public class ResourcelessTransactionManager : AbstractPlatformTransactionManager
    {
        /// <summary>The do begin.</summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="definition">The definition.</param>
        protected override void DoBegin(object transaction, ITransactionDefinition definition) { ((ResourcelessTransaction)transaction).Begin(); }

        /// <summary>The do commit.</summary>
        /// <param name="status">The status.</param>
        protected override void DoCommit(DefaultTransactionStatus status) { this.log.Debug("Committing resourceless transaction on [" + status.Transaction + "]"); }

        /// <summary>The do get transaction.</summary>
        /// <returns>The System.Object.</returns>
        protected override object DoGetTransaction()
        {
            var transaction = new ResourcelessTransaction();
            Stack<object> resources;
            if (!TransactionSynchronizationManager.HasResource(this))
            {
                resources = new Stack<object>();
                TransactionSynchronizationManager.BindResource(this, resources);
            }
            else
            {
                var stack = (Stack<object>)TransactionSynchronizationManager.GetResource(this);
                resources = stack;
            }

            resources.Push(transaction);
            return transaction;
        }

        /// <summary>The do rollback.</summary>
        /// <param name="status">The status.</param>
        protected override void DoRollback(DefaultTransactionStatus status) { this.log.Debug("Rolling back resourceless transaction on [" + status.Transaction + "]"); }

        /// <summary>The is existing transaction.</summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>The System.Boolean.</returns>
        protected override bool IsExistingTransaction(object transaction)
        {
            if (TransactionSynchronizationManager.HasResource(this))
            {
                var stack = (Stack<object>)TransactionSynchronizationManager.GetResource(this);
                return stack.Count > 1;
            }

            return ((ResourcelessTransaction)transaction).Active;
        }

        /// <summary>The do set rollback only.</summary>
        /// <param name="status">The status.</param>
        protected override void DoSetRollbackOnly(DefaultTransactionStatus status) { }

        /// <summary>The do cleanup after completion.</summary>
        /// <param name="transaction">The transaction.</param>
        protected override void DoCleanupAfterCompletion(object transaction)
        {
            var list = (Stack<object>)TransactionSynchronizationManager.GetResource(this);
            var resources = list;
            resources.Clear();
            TransactionSynchronizationManager.UnbindResource(this);
            ((ResourcelessTransaction)transaction).Clear();
        }
    }

    internal class ResourcelessTransaction
    {
        private bool active;

        /// <summary>Gets a value indicating whether active.</summary>
        public bool Active { get { return this.active; } }

        /// <summary>The begin.</summary>
        public void Begin() { this.active = true; }

        /// <summary>The clear.</summary>
        public void Clear() { this.active = false; }
    }
}
