﻿using System;
using CSScriptLibrary;
using H.Core.Runners;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public class CSharpRunner : Runner
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public CSharpRunner()
        {
            Add(new SyncAction("csharp", RunCode, "code"));
            //Add(new SyncAction("code-from-file", RunCodeFromFile, "path"));
        }

        #endregion

        #region Private methods

        private void RunCode(string code)
        {
            var action = CSScript.Evaluator
                .LoadDelegate<Action<Action<string>, Action<string>, Action<string>, Func<string, object?>>>($@"
using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

void Action(Action<string> Say, Action<string> Print, Action<string> Run, Func<string, object> GetVariable)
{{
{code}
}}
");

            action(Say, Print, Run, GetVariable);
        }

        #endregion
    }
}
