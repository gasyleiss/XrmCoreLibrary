﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Microsoft.Xrm.Sdk;

namespace Microsoft.Pfe.Xrm.Samples
{
    /// <summary>
    /// Base class for each of the scenarios to establish the steps to be performed
    /// </summary>    
    public abstract class ScenarioBase
    {
        public ScenarioBase(string name, int requestCount)
        {
            this.Data = new Requests(requestCount);
            this.ScenarioName = name;
        }
        public Requests Data { get; protected set; }
        public string ScenarioName { get; protected set; }

        protected abstract Action Step1CreateAccounts { get; }
        protected abstract Action Step2CreateRelatedData { get; }
        protected abstract Action Step3UpdateAccountsAndOpportunities { get; }
        protected abstract Action Step4ExecuteWinAndDeactivateRequests { get; }
        protected abstract Action Step5RetrieveMultipleEntities { get; }
        protected abstract Action Step6DeleteData { get; }

        public void Execute()
        {
            ScenarioExecutor.Execute(this.ScenarioName, () =>
            {                
                ScenarioExecutor.Execute("Step 1: create account data", this.Step1CreateAccounts);
                ScenarioExecutor.Execute("Step 2: create related data", this.Step2CreateRelatedData);
                ScenarioExecutor.Execute("Step 3: update account and opportunities", this.Step3UpdateAccountsAndOpportunities);
                ScenarioExecutor.Execute("Step 4: execute win and deactivate requests", this.Step4ExecuteWinAndDeactivateRequests);
                ScenarioExecutor.Execute("Step 5: retrieve multiple entities", this.Step5RetrieveMultipleEntities);
                ScenarioExecutor.Execute("Step 6: delete all data", this.Step6DeleteData);
            });
        }

        protected static void HandleAggregateExceptions(AggregateException ae)
        {
            ae.InnerExceptions.ToList().ForEach(ex =>
            {
                var fault = ex as FaultException<OrganizationServiceFault>;

                if (fault != null)
                    HandleFaultException(fault);
                else
                    HandleException(ex.Message);
            });
        }

        protected static void HandleFaultException(FaultException<OrganizationServiceFault> fault)
        {
            HandleException(fault.Detail.Message);
        }

        protected static void HandleException(string message)
        {
            Console.WriteLine("ERROR: {0}", message);
        }
    }

    public static class ScenarioExecutor
    {        
        public static void Execute(string scenarioName, Action scenario)
        {
            var watch = Stopwatch.StartNew();

            //Console.WriteLine("Starting {0} at {1}", scenarioName, DateTime.Now.ToString("u"));

            scenario();

            watch.Stop();

            //Console.WriteLine("Completed {0} at {1}", scenarioName, DateTime.Now.ToString("u"));
            Console.WriteLine("{0} took {1} to execute", scenarioName, watch.Elapsed.ToString());
        }
    }
}
