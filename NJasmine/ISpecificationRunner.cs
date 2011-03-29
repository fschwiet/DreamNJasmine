using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public interface ISpecificationRunner
    {
        //            using (fixture.UseVisitor(new VisitorPositionAdapter(runner)))
        //              return _buildContext._fixtureInstanceForDiscovery.UseVisitor(new VisitorPositionAdapter(position, this));

        TestPosition CurrentPosition { get; set; }
        ISpecPositionVisitor Visitor { get; set; }
        void Run();
    }
}