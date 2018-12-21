using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdfDemo
{
    internal static class JsonLdDocuments
    {
        public static readonly JToken John = JToken.Parse(@"{
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
        }");

        public static readonly JToken Jane = JToken.Parse(@"{
            '@id': 'http://example.com/demo/002',
            '@type': [
                'http://xmlns.com/foaf/0.1/Person'
            ],
            'http://xmlns.com/foaf/0.1/givenName': [
                {
                    '@value': 'Jane'
                }
            ],
            'http://xmlns.com/foaf/0.1/familyName': [
                {
                    '@value': 'Doe'
                }
            ],
            'http://schema.org/birthDate': [
                {
                    '@value': '1970-01-02T00:00:00Z',
                    '@type': 'http://www.w3.org/2001/XMLSchema#date'
                }
            ],
            'http://schema.org/gender': [
                {
                    '@value': 'http://schema.org/Female'
                }
            ],
            'http://xmlns.com/foaf/0.1/knows': [
                " + John + @"
            ]
        }");
    }
}
