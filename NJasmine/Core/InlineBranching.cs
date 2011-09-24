using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class InlineBranching
    {
        public class ContinuationException : Exception
        {
        }

        public static void RunBranchOption(Action<Action> option)
        {
            try
            {
                option(() =>
                {
                    throw new InlineBranching.ContinuationException();    
                });
            }
            catch (InlineBranching.ContinuationException)
            {
            }
        }

        public static void HandleInlineBranches(TestPosition position, Action<Action>[] options, Action<Action<Action>, TestPosition> optionHandler)
        {
            var eitherBranch = position.GetFirstChildPosition();

            for (var i = 0; i < options.Length; i++)
            {
                optionHandler(options[i], eitherBranch);

                eitherBranch = eitherBranch.GetNextSiblingPosition();
            }
        }
    }
}
