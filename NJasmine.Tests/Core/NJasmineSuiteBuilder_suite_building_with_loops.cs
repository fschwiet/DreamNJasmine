using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class NJasmineSuiteBuilder_suite_building_with_loops : ExpectationsFixture
    {
        public class has_test_in_loop : ObservableNJasmineFixture
        {
            public override void Tests()
            {
                Observe("1");

                foreach(var i in Enumerable.Range(1,3))
                {
                    Observe("a" + i);

                    it("a" + i, () =>
                    {
                        Observe("ai" + i);
                    });
                }

                Observe("2");

                describe("nested", () =>
                {
                    Observe("3");

                    foreach(var i in Enumerable.Range(1,3))
                    {
                        Observe("a" + i);

                        it("b" + i, () =>
                        {
                            Observe("bi" + i);
                        });
                    }

                    Observe("4");
                });

                Observe("5");
            }
        }

        [Test]
        public void works_with_loops()
        {
            
        }

    }
}
