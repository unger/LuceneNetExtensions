namespace LuceneNetExtensions.Tests.Mapping
{
    using LuceneNetExtensions.Mapping;
    using LuceneNetExtensions.Tests.Model;

    public class ProfileRuleMap : IndexClassMap<ProfileRule>
    {
        public ProfileRuleMap()
        {
            this.Index("ProfileRules");

            this.Map(s => s.Species);
            this.Map(s => s.Provinces);
            this.Map(s => s.Municipalities);
        }
    }
}
