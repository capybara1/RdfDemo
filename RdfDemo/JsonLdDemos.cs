using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RdfDemo
{
    [TestClass]
    public class JsonLdDemos
    {
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
                    },
                    'knows': {
                        '@id': 'http://xmlns.com/foaf/0.1/knows',
                        '@type': '@id'
                    },
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                JsonLdDocuments.John,
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
                JsonLdDocuments.John,
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
                JsonLdDocuments.John,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void FlatteningOfNestedObjects()
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
            var contentDocument = JsonLD.Core.JsonLdProcessor.Flatten(
                JsonLdDocuments.Jane,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }
    }
}
