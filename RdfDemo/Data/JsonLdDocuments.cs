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
        public static readonly JToken SingleObject = JToken.Parse(@"{
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
            ],
            'http://xmlns.com/foaf/0.1/knows': [
                {
                    '@id': 'http://example.com/demo/002'
                }
            ]
        }");

        public static readonly JToken NestedObjects = JToken.Parse(@"{
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
            'http://xmlns.com/foaf/0.1/homepage': [
                {
                    '@id': 'http://www.janedoe.com'
                }
            ],
            'http://xmlns.com/foaf/0.1/knows': [
                " + SingleObject + @"
            ]
        }");

        public static readonly JToken Graph = JToken.Parse(@"[
            {
                '@id': 'http://example.com/demo/001',
                '@type': [
                    'http://example.com/demo/vocab/Person'
                ],
                'http://example.com/demo/vocab/name': [
                    {
                        '@value': 'March Hare'
                    }
                ],
                'http://example.com/demo/vocab/residentOf': [
                    {
                        '@id': 'http://example.com/demo/002'
                    }
                ]
            },
            {
                '@id': 'http://example.com/demo/002',
                '@type': [
                    'http://example.com/demo/vocab/Building'
                ],
                'http://example.com/demo/vocab/address': [
                    {
                        '@id': 'http://example.com/demo/003'
                    }
                ],
                'http://example.com/demo/vocab/name': [
                    {
                        '@value': 'Garden of the March Hare'
                    }
                ]
            },
            {
                '@id': 'http://example.com/demo/003',
                '@type': [
                    'http://example.com/demo/vocab/Address'
                ],
                'http://example.com/demo/vocab/country': [
                    {
                        '@value': 'Wonderland'
                    }
                ]
            }
        ]");
    }
}
