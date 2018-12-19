using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RdfDemo
{
    [TestClass]
    public class JsonLdDemos
    {
        private static readonly JToken ExpandedDocument = JToken.Parse(@"{
            '@id': 'http://example.com/subject/001',
            'http://xmlns.com/foaf/0.1/firstName': 'John',
            'http://xmlns.com/foaf/0.1/familyName': 'Doe',
            'http://xmlns.com/foaf/0.1/age': 41
        }");

        [TestMethod]
        public void CompactionWithVocab()
        {
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://xmlns.com/foaf/0.1/'
                }
            }");
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                ExpandedDocument,
                contextDocument,
                new JsonLD.Core.JsonLdOptions());

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithExplicitReplacement()
        {
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    'firstName': 'http://xmlns.com/foaf/0.1/firstName',
                    'familyName': 'http://xmlns.com/foaf/0.1/familyName',
                    'age': 'http://xmlns.com/foaf/0.1/age',
                }
            }");
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                ExpandedDocument,
                contextDocument,
                new JsonLD.Core.JsonLdOptions());

            Util.WriteLine(contentDocument.ToString());
        }
    }
}
