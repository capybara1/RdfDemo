using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RdfDemo
{
    internal static class Util
    {
        private static readonly IEnumerable<RDFSharp.Model.RDFModelEnums.RDFFormats> AvailableSerializationFormats = Enum.GetValues(typeof(RDFSharp.Model.RDFModelEnums.RDFFormats))
            .Cast<RDFSharp.Model.RDFModelEnums.RDFFormats>()
            .Where(f => f != RDFSharp.Model.RDFModelEnums.RDFFormats.TriX);

        private static readonly IEnumerable<string> RelevantNodeKeys = new[] { "value", "language" };

        public static void WriteSerializedRepresentations(RDFSharp.Model.RDFGraph graph)
        {
            var serializationsByFormat = AvailableSerializationFormats.ToDictionary(
                    f => (object)f,
                    f => SerializeGraph(graph, f)
                );

            WriteLine($"JSON-LD representation of graph '{graph.Context}'");
            var contentDocument = JsonLD.Core.JsonLdProcessor.FromRDF(
                serializationsByFormat[RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples],
                new JsonLD.Impl.NQuadRDFParser());
            var contextDocument = JToken.Parse("{ '@context': { '@vocab': 'http://xmlns.com/foaf/0.1/' } }");
            contentDocument = JsonLD.Core.JsonLdProcessor.Compact(
                contentDocument,
                contextDocument,
                new JsonLD.Core.JsonLdOptions());
            serializationsByFormat["JSON-LD"] = contentDocument.ToString();

            foreach (var kvp in serializationsByFormat)
            {
                WriteLine($"{kvp.Key} representation of graph '{graph.Context}'");
                WriteLine();
                WriteLine(kvp.Value);
                WriteLine();
            }
        }

        public static string SerializeGraph(RDFSharp.Model.RDFGraph graph, RDFSharp.Model.RDFModelEnums.RDFFormats format)
        {
            using (var buffer = new MemoryStream())
            {
                graph.ToStream(format, buffer);
                var result = Encoding.UTF8.GetString(buffer.ToArray()).Trim();
                return result;
            }
        }

        public static RDFSharp.Model.RDFGraph DeserializeGraph(string data, RDFSharp.Model.RDFModelEnums.RDFFormats format)
        {
            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var result = RDFSharp.Model.RDFGraph.FromStream(format, buffer);
                return result;
            }
        }
        
        public static RDFSharp.Model.RDFGraph DeserializeGraphFromJsonLd(string data)
        {
            var result = new RDFSharp.Model.RDFGraph();
            result.SetContext(new Uri("http://example.com/context/JsonLd"));

            var token = (JsonLD.Core.RDFDataset)JsonLD.Core.JsonLdProcessor.ToRDF(JsonLD.Util.JSONUtils.FromString(data));

            foreach (var quad in token.GetQuads("@default"))
            {
                var subject = ToRDFSharpObject(quad["subject"]);
                var predicate = ToRDFSharpObject(quad["predicate"]);
                var @object = ToRDFSharpObject(quad["object"]);
                var triple = CreateTriple(subject, predicate, @object);
                result.AddTriple(triple);
            }

            return result;
        }

        public static void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        public static void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
        
        public static void WriteDataset(JsonLD.Core.RDFDataset dataset)
        {
            foreach (var quad in dataset.GetQuads("@default"))
            {
                var values = quad.Keys
                    .Select(key => quad[key])
                    .OfType<JsonLD.Core.RDFDataset.Node>()
                    .SelectMany(node => node.Keys.Intersect(RelevantNodeKeys).Select(k => node[k]));
                Util.WriteLine(string.Join(" ", values));
            }
        }

        private static RDFSharp.Query.RDFPatternMember ToRDFSharpObject(object node)
        {
            RDFSharp.Query.RDFPatternMember result;

            switch ((JsonLD.Core.RDFDataset.Node)node)
            {
                case JsonLD.Core.RDFDataset.IRI iri:
                    result = new RDFSharp.Model.RDFResource(iri["value"].ToString());
                    break;

                case JsonLD.Core.RDFDataset.BlankNode blankNode:
                    result = new RDFSharp.Model.RDFResource(blankNode["value"].ToString());
                    break;

                case JsonLD.Core.RDFDataset.Literal literal:
                    if (literal.Keys.Contains("language"))
                    {
                        result = new RDFSharp.Model.RDFPlainLiteral(
                            literal["value"].ToString(),
                            literal["language"].ToString());
                    }
                    else if (literal.Keys.Contains("datatype"))
                    {
                        var dataType = ToRDFDataType(literal["datatype"].ToString());
                        if (dataType == RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_STRING)
                        {
                            result = new RDFSharp.Model.RDFPlainLiteral(literal["value"].ToString());
                        }
                        else
                        {
                            result = new RDFSharp.Model.RDFTypedLiteral(
                                literal["value"].ToString(),
                                dataType);
                        }
                    }
                    else
                    {
                        result = new RDFSharp.Model.RDFPlainLiteral(literal["value"].ToString());
                    }
                    break;

                default:
                    throw new NotSupportedException($"The type {node.GetType().Name} is not supported");
            }

            return result;
        }

        private static RDFSharp.Model.RDFTriple CreateTriple(
            RDFSharp.Query.RDFPatternMember subject,
            RDFSharp.Query.RDFPatternMember predicate,
            RDFSharp.Query.RDFPatternMember @object)
        {
            RDFSharp.Model.RDFTriple result;
            if (@object is RDFSharp.Model.RDFLiteral literal)
            {
                result = new RDFSharp.Model.RDFTriple(
                    (RDFSharp.Model.RDFResource)subject,
                    (RDFSharp.Model.RDFResource)predicate,
                    literal);
            }
            else
            {
                result = new RDFSharp.Model.RDFTriple(
                    (RDFSharp.Model.RDFResource)subject,
                    (RDFSharp.Model.RDFResource)predicate,
                    (RDFSharp.Model.RDFResource)@object);
            }

            return result;
        }

        private static RDFSharp.Model.RDFModelEnums.RDFDatatypes ToRDFDataType(string value)
        {
            RDFSharp.Model.RDFModelEnums.RDFDatatypes result;
            switch (value)
            {
                case "http://www.w3.org/2000/01/rdf-schema#Literal":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.RDFS_LITERAL;
                    break;
                case "http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.RDF_XMLLITERAL;
                    break;
                case "http://www.w3.org/2001/XMLSchema#string":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_STRING;
                    break;
                case "http://www.w3.org/2001/XMLSchema#boolean":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_BOOLEAN;
                    break;
                case "http://www.w3.org/2001/XMLSchema#decimal":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_DECIMAL;
                    break;
                case "http://www.w3.org/2001/XMLSchema#float":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_FLOAT;
                    break;
                case "http://www.w3.org/2001/XMLSchema#double":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_DOUBLE;
                    break;
                case "http://www.w3.org/2001/XMLSchema#positiveInteger":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_POSITIVEINTEGER;
                    break;
                case "http://www.w3.org/2001/XMLSchema#negativeInteger":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NEGATIVEINTEGER;
                    break;
                case "http://www.w3.org/2001/XMLSchema#nonPositiveInteger":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NONPOSITIVEINTEGER;
                    break;
                case "http://www.w3.org/2001/XMLSchema#nonNegativeInteger":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NONNEGATIVEINTEGER;
                    break;
                case "http://www.w3.org/2001/XMLSchema#integer":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_INTEGER;
                    break;
                case "http://www.w3.org/2001/XMLSchema#long":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_LONG;
                    break;
                case "http://www.w3.org/2001/XMLSchema#int":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_INT;
                    break;
                case "http://www.w3.org/2001/XMLSchema#short":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_SHORT;
                    break;
                case "http://www.w3.org/2001/XMLSchema#byte":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_BYTE;
                    break;
                case "http://www.w3.org/2001/XMLSchema#unsignedLong":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_UNSIGNEDLONG;
                    break;
                case "http://www.w3.org/2001/XMLSchema#unsignedInt":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_UNSIGNEDINT;
                    break;
                case "http://www.w3.org/2001/XMLSchema#unsignedShort":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_UNSIGNEDSHORT;
                    break;
                case "http://www.w3.org/2001/XMLSchema#unsignedByte":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_UNSIGNEDBYTE;
                    break;
                case "http://www.w3.org/2001/XMLSchema#duration":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_DURATION;
                    break;
                case "http://www.w3.org/2001/XMLSchema#dateTime":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_DATETIME;
                    break;
                case "http://www.w3.org/2001/XMLSchema#date":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_DATE;
                    break;
                case "http://www.w3.org/2001/XMLSchema#time":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_TIME;
                    break;
                case "http://www.w3.org/2001/XMLSchema#gYear":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_GYEAR;
                    break;
                case "http://www.w3.org/2001/XMLSchema#gMonth":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_GMONTH;
                    break;
                case "http://www.w3.org/2001/XMLSchema#gDay":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_GDAY;
                    break;
                case "http://www.w3.org/2001/XMLSchema#gYearMonth":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_GYEARMONTH;
                    break;
                case "http://www.w3.org/2001/XMLSchema#gMonthDay":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_GMONTHDAY;
                    break;
                case "http://www.w3.org/2001/XMLSchema#hexBinary":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_HEXBINARY;
                    break;
                case "http://www.w3.org/2001/XMLSchema#base64Binary":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_BASE64BINARY;
                    break;
                case "http://www.w3.org/2001/XMLSchema#anyURI":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_ANYURI;
                    break;
                case "http://www.w3.org/2001/XMLSchema#QName":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_QNAME;
                    break;
                case "http://www.w3.org/2001/XMLSchema#NOTATION":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NOTATION;
                    break;
                case "http://www.w3.org/2001/XMLSchema#language":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_LANGUAGE;
                    break;
                case "http://www.w3.org/2001/XMLSchema#normalizedString":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NORMALIZEDSTRING;
                    break;
                case "http://www.w3.org/2001/XMLSchema#token":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_TOKEN;
                    break;
                case "http://www.w3.org/2001/XMLSchema#NMToken":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NMTOKEN;
                    break;
                case "http://www.w3.org/2001/XMLSchema#Name":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NAME;
                    break;
                case "http://www.w3.org/2001/XMLSchema#NCName":
                    result = RDFSharp.Model.RDFModelEnums.RDFDatatypes.XSD_NCNAME;
                    break;
                default:
                    throw new NotSupportedException($"Type {value} is not supported");
            }

            return result;
        }
    }
}
