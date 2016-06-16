using JetToObjects.Mapping;

namespace JetToObjects.Tests.Mapping.Data
{
    public class FakeMapping : ObjectMapping
    {
        public FakeMapping()
        {
            Map("id", "ID");
            Map("some_column", "SomeProperty");
            Map("This_is_a_Test", "ThisIsATest");
            Exclude("ExcludedIsAdmin");
        }
    }
}