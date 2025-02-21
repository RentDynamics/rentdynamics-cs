using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RentDynamics.RdClient.Tests
{
    [TestClass]
    public class JsonSortHelperTests
    {
        private void ShouldBeSorted(JToken jToken)
        {
            switch (jToken)
            {
                case JArray jArray:
                {
                    foreach (JToken token in jArray)
                    {
                        ShouldBeSorted(token);
                    }

                    break;
                }
                case JObject jObject:
                    jObject.Properties().Select(p => p.Name).Should().BeInAscendingOrder();
                    foreach (JToken token in jObject.Properties().Select(p => p.Value))
                    {
                        ShouldBeSorted(token);
                    }

                    break;
            }
        }

        [TestMethod]
        public void ObjectProperties_ShouldBeSorted()
        {
            var unsortedDictionary = new Dictionary<string, object>
            {
                { "c", 3 },
                { "a", 1 },
                { "b", 2 }
            };

            var jObject = JObject.FromObject(unsortedDictionary);
            JsonSortHelper.Sort(jObject);

            ShouldBeSorted(jObject);
        }

        [TestMethod]
        public void ObjectProperties_ShouldBeSorted_ByAscii()
        {
            var unsortedDictionary = new Dictionary<string, object>
            {
                { "Id", 2 },
                { "ID", 1 }
            };

            var jObject = JObject.FromObject(unsortedDictionary);
            JsonSortHelper.Sort(jObject);

            ShouldBeSorted(jObject);
        }

        [TestMethod]
        public void ObjectProperties_ShouldBeSorted_WhenObjectIsInsideArray()
        {
            var unsortedDictionary = new Dictionary<string, object>
            {
                { "c", 3 },
                { "a", 1 },
                { "b", 2 }
            };

            var jArray = JArray.FromObject(new[] { unsortedDictionary });
            JsonSortHelper.Sort(jArray);

            ShouldBeSorted(jArray);
        }

        [TestMethod]
        public void HierarchyOfObjects_ShouldBeSorted()
        {
            var unsortedGrandChild = new Dictionary<string, object>
            {
                { "y", 2 },
                { "x", 1 }
            };
   
            var unsortedChild = new Dictionary<string, object>
            {
                { "f", 3 },
                { "e", new[] { unsortedGrandChild } },
                { "d", 1 }
            };
            
            var unsortedParent = new Dictionary<string, object>
            {
                { "c", 3 },
                { "a", 1 },
                { "b", unsortedChild }
            };

            var jObject = JObject.FromObject(unsortedParent);
            JsonSortHelper.Sort(jObject);
            
            ShouldBeSorted(jObject);
        }
    }
}