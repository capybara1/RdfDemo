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
            .Cast<RDFSharp.Model.RDFModelEnums.RDFFormats>();

        public static void WriteSerializedRepresentations(RDFSharp.Model.RDFGraph graph)
        {
            var serializationsByFormat = AvailableSerializationFormats.ToDictionary(
                    f => (object)f,
                    f => SerializeGraph(graph, f)
                );

            WriteLine($"JSON-LD representation of graph '{graph.Context}'");
            var jsonDocument = JsonLD.Core.JsonLdProcessor.FromRDF(
                serializationsByFormat[RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples],
                new JsonLD.Impl.NQuadRDFParser());
            var context = JToken.Parse("{ '@context': { '@vocab': 'http://xmlns.com/foaf/0.1/' } }");
            jsonDocument = JsonLD.Core.JsonLdProcessor.Compact(
                jsonDocument,
                context,
                new JsonLD.Core.JsonLdOptions());
            serializationsByFormat["JSON-LD"] = jsonDocument.ToString();

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

        public static void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        public static void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
