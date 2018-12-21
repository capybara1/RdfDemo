using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace RdfDemo
{
    [TestClass]
    public class JsonLdDemos
    {
        [TestMethod]
        public void CompactionWithEmbeddedContext()
        {
            var expandedContentDocument = JsonLdDocuments.SingleObject;
            var contextDocument = JToken.Parse(@"{
                '@context':
                {
                    'xsd': 'http://www.w3.org/2001/XMLSchema#',
                    'Person': 'http://xmlns.com/foaf/0.1/Person',
                    'title': 'http://xmlns.com/foaf/0.1/title',
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
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithEmbeddedContextUsingDefaultLanguage()
        {
            var expandedContentDocument = JsonLdDocuments.SingleObject;
            var contextDocument = JToken.Parse(@"{
                '@context':
                {
                    'xsd': 'http://www.w3.org/2001/XMLSchema#',
                    'Person': 'http://xmlns.com/foaf/0.1/Person',
                    'title': {
                        '@id': 'http://xmlns.com/foaf/0.1/title',
                        '@language': 'en'
                    },
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
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithEmbeddedContextUsingVocab()
        {
            var expandedContentDocument = JsonLdDocuments.SingleObject;
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://xmlns.com/foaf/0.1/',
                    'xsd': 'http://www.w3.org/2001/XMLSchema#',
                    'title': {
                        '@language': 'en'
                    },
                    'born': {
                        '@id': 'http://schema.org/birthDate',
                        '@type': 'xsd:date'
                    },
                    'gender': 'http://schema.org/gender',
                    'homepage': {
                        '@type': '@id'
                    },
                    'knows':
                    {
                        '@type': '@id'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionWithRemoteContext()
        {
            var expandedContentDocument = JsonLdDocuments.SingleObject;
            var contextDocument = JToken.Parse(@"{
                '@context': [
                    {
                        'title': 'http://xmlns.com/foaf/0.1/title',
                    },
                    'https://json-ld.org/contexts/person.jsonld'
                ]
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void CompactionOfGraphWithLocalContextUsingReverseProperty()
        {
            var expandedContentDocument = JsonLdDocuments.Graph;
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://example.com/demo/vocab/',
                    'residentOf':
                    {
                        '@type': '@id'
                    },
                    'homeOf': {
                        '@type': '@id',
                        '@reverse': 'residentOf'
                    },
                    'address':
                    {
                        '@type': '@id'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void FlatteningOfNestedObjects()
        {
            var expandedContentDocument = JsonLdDocuments.NestedObjects;
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
                    },
                    'knows':
                    {
                        '@type': '@id'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Flatten(
                expandedContentDocument,
                contextDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void ImposeStructureOnAGraphWithFraming()
        {
            var graphDocument = JsonLdDocuments.Graph;
            var frameDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://example.com/demo/vocab/',
                    'residentOf':
                    {
                        '@type': '@id'
                    },
                    'address':
                    {
                        '@type': '@id'
                    }
                },
                '@type':'Person',
                'residentOf': {
                    '@type':'Building',
                    'address': {
                        '@type':'Address'
                    }
                }
            }");
            var options = new JsonLD.Core.JsonLdOptions();
            var contentDocument = JsonLD.Core.JsonLdProcessor.Frame(
                graphDocument,
                frameDocument,
                options);

            Util.WriteLine(contentDocument.ToString());
        }

        [TestMethod]
        public void DemonstrateDifferenceBetweenContainers()
        {
            var examples = new[]
            {
                JToken.Parse(@"{
                    '@context': {
                        '@vocab': 'http://example.com/demo/vocab/'
                    },
                    'items': [ 'a', 'b', 'c' ]
                }"),
                // @set does not add semantics but can be used for documentation
                JToken.Parse(@"{
                    '@context': {
                        '@vocab': 'http://example.com/demo/vocab/',
                        'items': {
                            '@container': '@set'
                        }
                    },
                    'items': [ 'a', 'b', 'c' ]
                }"),
                JToken.Parse(@"{
                    '@context': {
                        '@vocab': 'http://example.com/demo/vocab/',
                        'items': {
                            '@container': '@list'
                        }
                    },
                    'items': [ 'a', 'b', 'c' ]
                }"),
                // @index does not add semantics but can be used to preserve structure
                JToken.Parse(@"{
                    '@context': {
                        '@vocab': 'http://example.com/demo/vocab/',
                        'items': {
                            '@container': '@index'
                        }
                    },
                    'items': {
                        'x': 'a',
                        'y': 'b',
                        'z': 'c' 
                    }
                }"),
                JToken.Parse(@"{
                    '@context': {
                        '@vocab': 'http://example.com/demo/vocab/',
                        'items': {
                            '@container': '@language'
                        }
                    },
                    'items': {
                        'en': 'a',
                        'de': 'b',
                        'nl': 'c' 
                    }
                }")
            };

            var relevantKeys = new[] { "value", "language" };
            foreach (var example in examples)
            {
                Util.WriteLine(example.ToString());
                var dataset = (JsonLD.Core.RDFDataset)JsonLD.Core.JsonLdProcessor.ToRDF(example);
                foreach (var quad in dataset.GetQuads("@default"))
                {
                    var values = quad.Keys
                        .Select(key => quad[key])
                        .OfType<JsonLD.Core.RDFDataset.Node>()
                        .SelectMany(node => node.Keys.Intersect(relevantKeys).Select(k => node[k]));
                    Util.WriteLine(string.Join(" ", values));
                }
            }
        }
    }
}
