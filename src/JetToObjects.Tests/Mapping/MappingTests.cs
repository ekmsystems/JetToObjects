using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ekm.oledb.data.Tests.Mapping.Data;
using NUnit.Framework;

namespace ekm.oledb.data.Tests.Mapping
{
    [TestFixture]
    public class MappingTests
    {
        [Test]
        public void Map_WithFakeMappingAndFakeDestination_DynamicFieldsAreMappedToNewInstanceOfFakeDestination()
        {
            dynamic source = new ExpandoObject();

            source.id = 42;
            source.some_column = "Test";
            source.This_is_a_Test = true;
            source.ExcludedIsAdmin = true;
            source.Implicit = "Implicit";

            var result = Mapper.Map<FakeDestination, FakeMapping>(source);

            Assert.That(result.ID, Is.EqualTo(42));
            Assert.That(result.SomeProperty, Is.EqualTo("Test"));
            Assert.That(result.ThisIsATest, Is.True);
            Assert.That(result.ExcludedIsAdmin, Is.False);
            Assert.That(result.Implicit, Is.EqualTo("Implicit"));
        }

        [Test]
        public void Map_WithEnumerable_MapsAllValues()
        {
            dynamic item1 = new ExpandoObject();
            item1.id = 1;
            item1.some_column = "item1";
            item1.This_is_a_Test = true;

            dynamic item2= new ExpandoObject();
            item2.id = 2;
            item2.some_column = "item2";
            item2.This_is_a_Test = true;

            var source = new List<dynamic> { item1, item2 };

            var result = Mapper.MapMany<FakeDestination, FakeMapping>(source);

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.ElementAt(0).ID, Is.EqualTo(1));
            Assert.That(result.ElementAt(1).ID, Is.EqualTo(2));
        }
    }
}
