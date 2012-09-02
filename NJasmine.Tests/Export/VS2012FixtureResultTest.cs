using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class VS2012FixtureResultTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a TRX file with only successes", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='1' passed='1' failed='0' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";
                var results = arrange(() => new VS2012FixtureResult(result));

                then("verification of succeeds() passes", () => results.succeeds());
                then("verification of failed() fails", () => Assert.Throws<Exception>(() => results.failed()));
            });

            given("a TRX file that only has failures", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='1' passed='0' failed='1' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";

                var results = arrange(() => new VS2012FixtureResult(result));

                then("verification of succeeds() fails", () => Assert.Throws<Exception>(() => results.succeeds()));
                then("verification of failed() succeeds", () => results.failed());
            });

            given("a TRX file that hasn't ran any tests", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='0' passed='0' failed='0' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";

                var results = arrange(() => new VS2012FixtureResult(result));

                then("verification of succeeds() fails", () => Assert.Throws<Exception>(() => results.succeeds()));
                then("verification of failed() fails", () => Assert.Throws<Exception>(() => results.failed()));
            });
        }
    }
}
