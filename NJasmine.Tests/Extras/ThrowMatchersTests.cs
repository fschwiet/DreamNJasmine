using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NJasmine;
using NJasmine.Extras;
using NUnit.Core;
using Should.Fluent;
using Should.Fluent.Model;
using NUnit.Framework;

namespace NJasmineTests.Misc
{
    
    public class CustomMatchers : NJasmineFixture
    {
        public class SpecializedInvalidCastOperation : InvalidOperationException
        {
            
        }

        public override void Tests()
        {


Action actionThrowingException = () => { throw new SpecializedInvalidCastOperation(); };
Action actionThrowingWrongType = () => { throw new InvalidCastException(); };
Action actionNotThrowing = () => {  };
            
it("can allows function with correct exception", delegate
{
    expect(actionThrowingException).to.Throw<InvalidOperationException>();
    expect(actionNotThrowing).not.to.Throw<InvalidOperationException>();
});

it("will check if any exception is thrown", delegate
{
    try
    {
        expect(actionNotThrowing).to.Throw<InvalidOperationException>();
    }
    catch (Exception e)
    {
        expect(e.Message).to.Equal("Expected exception of type InvalidOperationException, no exception was thrown.");
        return;
    }

    Assert.Fail("Exception expected");
});

it("will check if exception is of expected type", delegate
{
    try
    {
        expect(actionThrowingWrongType).to.Throw<InvalidOperationException>();
    }
    catch (Exception e)
    {
        expect(e.Message).to.Equal("Expected exception of type InvalidOperationException, saw exception of type InvalidCastException.");
        return;
    }

    Assert.Fail("Exception expected");
});

it("will check if exception is of unexpected type", delegate
{
    try
    {
        expect(actionThrowingException).not.to.Throw<InvalidOperationException>();
    }
    catch (Exception e)
    {
        expect(e.Message).to.Equal("Expected not to see exception of type InvalidOperationException, saw exception of type SpecializedInvalidCastOperation.");
        return;
    }

    Assert.Fail("Exception expected");
});


        }
    }
}

