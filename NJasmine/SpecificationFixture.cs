namespace NJasmine
{
    public abstract class SpecificationFixture
    {
        protected SkeleFixture _skeleFixture;

        public SpecificationFixture()
        {
            _skeleFixture = new SkeleFixture(this.Specify);
        }

        protected SpecificationFixture(SkeleFixture fixture)
        {
            _skeleFixture = fixture;
        }

        public abstract void Specify();

        //  Making this static so people don't run across it generally
        public static SkeleFixture GetUnderlyingSkelefixture(SpecificationFixture fixture)
        {
            return fixture._skeleFixture;
        }
    }
}