using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RdfDemo
{
    [TestClass]
    public class JsonLdDemos
    {
        private static readonly JToken ExpandedDocument = JToken.Parse(@"
        [
            {
                '@id': 'http://example.com/demo/001',
                '@type': [
                    'http://xmlns.com/foaf/0.1/Person'
                ],
                'http://xmlns.com/foaf/0.1/givenName': [
                    {
                        '@value': 'John'
                    }
                ],
                'http://xmlns.com/foaf/0.1/familyName': [
                    {
                        '@value': 'Doe'
                    }
                ],
                'http://schema.org/birthDate': [
                    {
                        '@value': '1970-01-01T00:00:00Z',
                        '@type': 'http://www.w3.org/2001/XMLSchema#date'
                    }
                ],
                'http://schema.org/gender': [
                    {
                        '@value': 'http://schema.org/Male'
                    }
                ],
                'http://xmlns.com/foaf/0.1/homepage': [
                    {
                        '@id': 'http://www.johndoe.com'
                    }
                ]
            }
        ]".Trim());

        [TestMethod]
        public void CompactionWithEmbeddedContext()
        {
            var contextDocument = JToken.Parse(@"{
                '@context':
                {
                    'Person': 'http://xmlns.com/foaf/0.1/Person',
                    'xsd': 'http://www.w3.org/2001/XMLSchema#',
                    'givenName': 'http://xmlns.com/foaf/0.1/givenName',
                    'familyName': 'http://xmlns.com/foaf/0.1/familyName',
                    'born': {
                        '@id': 'http://schema.org/birthDate',
                        '@type': 'xsd:date'
                    },
                    'gender': 'http://schema.org/gender',
                    'homepage': {
                        '@id': 'http://xmlns.com/foaf/0.1/homepage',
                        '@type': '@id'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                ExpandedDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithEmbeddedContextUsingVocab()
        {
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://xmlns.com/foaf/0.1/',
                    'xsd': 'http://www.w3.org/2001/XMLSchema#',
                    'born': {
                        '@id': 'http://schema.org/birthDate',
                        '@type': 'xsd:date'
                    },
                    'gender': 'http://schema.org/gender',
                    'homepage': {
                        '@type': '@id'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                ExpandedDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithRemoteContext()
        {
            var contextDocument = JToken.Parse(@"{
                '@context': [ 'https://json-ld.org/contexts/person.jsonld' ]
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                ExpandedDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }
    }
}
