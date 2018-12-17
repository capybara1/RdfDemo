using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RdfDemo
{
    [TestClass]
    public class BasicDemos
    {
        [TestInitialize]
        public void TestInitialize()
        {
            RDFSharp.Model.RDFModelEvents.OnTripleAdded += WriteLine;
            RDFSharp.Model.RDFModelEvents.OnTripleRemoved += WriteLine;
            RDFSharp.Model.RDFModelEvents.OnGraphCleared += WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnSELECTQueryEvaluation += WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnASKQueryEvaluation += WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnDESCRIBEQueryEvaluation += WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnCONSTRUCTQueryEvaluation += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnStoreInitialized += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnStoreAdded += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnStoreOptimized += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnStoreRemoved += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnStoreCleared += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnQuadrupleAdded += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnQuadrupleRemoved += WriteLine;
            RDFSharp.Store.RDFStoreEvents.OnFederationCleared += WriteLine;
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForSimpleGraph()
        {
            var graph = ConstructGraph();

            WriteSerializedRepresentations(graph);
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForContainer()
        {
            var graph = DeserializeGraph(
                @"
<http://example.org/courses/6.001> <http://example.org/students/vocab#students> _:genid1 .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Bag> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_1> <http://example.org/students/Amy> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_2> <http://example.org/students/Mohamed> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_3> <http://example.org/students/Johann> .
".Trim(),
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);

            WriteSerializedRepresentations(graph);
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForCollection()
        {
            var graph = DeserializeGraph(
                @"
<http://recshop.fake/cd/Beatles> <http://recshop.fake/cd#artist> _:genid1 .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://recshop.fake/cd/Beatles/George> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> _:genid2 .
_:genid2 <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://recshop.fake/cd/Beatles/John> .
_:genid2 <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> _:genid3 .
_:genid3 <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://recshop.fake/cd/Beatles/Paul> .
_:genid3 <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> _:genid4 .
_:genid4 <http://www.w3.org/1999/02/22-rdf-syntax-ns#first> <http://recshop.fake/cd/Beatles/Ringo> .
_:genid4 <http://www.w3.org/1999/02/22-rdf-syntax-ns#rest> <http://www.w3.org/1999/02/22-rdf-syntax-ns#nil> .
".Trim(),
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);

            WriteSerializedRepresentations(graph);
        }

        [TestMethod]
        public void UseSimpleGraphToPerformAskQuery()
        {
            var graph = ConstructGraph();

            var x = new RDFSharp.Query.RDFVariable("x");
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                x,
                RDFSharp.Model.RDFVocabulary.FOAF.FAMILY_NAME,
                new RDFSharp.Model.RDFPlainLiteral("Doe")));
            var query = new RDFSharp.Query.RDFAskQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToGraph(graph);

            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public void UseSimpleGraphToPerformSelectQuery()
        {
            var graph = ConstructGraph();

            var x = new RDFSharp.Query.RDFVariable("x");
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                x,
                RDFSharp.Model.RDFVocabulary.FOAF.FAMILY_NAME,
                new RDFSharp.Model.RDFPlainLiteral("Doe")));
            var query = new RDFSharp.Query.RDFSelectQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToGraph(graph);

            Assert.AreEqual(1, result.SelectResultsCount);

            string subject;
            foreach (System.Data.DataRow item in result.SelectResults.Rows)
            {
                subject = item[0].ToString();
                WriteLine($"Subject: {subject}");
            }
        }

        [TestMethod]
        public void UseMultipleGraphToPerformSelectQuery()
        {
            var firstGraph = ConstructGraph();
            var secondGraph = LoadGraphFromJsonLd();

            var store = new RDFSharp.Store.RDFMemoryStore();
            store.MergeGraph(firstGraph);
            store.MergeGraph(secondGraph);
            
            var x = new RDFSharp.Query.RDFVariable("x");
            var y = new RDFSharp.Query.RDFVariable("y");
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                x,
                y,
                RDFSharp.Model.RDFVocabulary.FOAF.FAMILY_NAME,
                new RDFSharp.Model.RDFPlainLiteral("Doe")));
            var query = new RDFSharp.Query.RDFSelectQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToStore(store);

            Assert.AreEqual(2, result.SelectResultsCount);

            string context;
            string subject;
            foreach (System.Data.DataRow item in result.SelectResults.Rows)
            {
                context = item[0].ToString();
                subject = item[1].ToString();
                WriteLine($"Context: {context}, Subject: {subject}");
            }
        }

        private static RDFSharp.Model.RDFGraph ConstructGraph()
        {
            var result = new RDFSharp.Model.RDFGraph();
            result.SetContext(new Uri("http://example.com/context/Local"));

            var exampleNamespace = new RDFSharp.Model.RDFNamespace("ex", "http://example.com/");
            RDFSharp.Model.RDFNamespaceRegister.AddNamespace(exampleNamespace);

            result.AddTriple(new RDFSharp.Model.RDFTriple(
                new RDFSharp.Model.RDFResource("ex:subject/001"),
                RDFSharp.Model.RDFVocabulary.FOAF.FIRSTNAME, // Equivalent to RDFResource("foaf:firstName") which is equivalent to RDFResource("http://xmlns.com/foaf/0.1/firstName")
                new RDFSharp.Model.RDFPlainLiteral("John")));
            result.AddTriple(new RDFSharp.Model.RDFTriple(
                new RDFSharp.Model.RDFResource("ex:subject/001"),
                RDFSharp.Model.RDFVocabulary.FOAF.FAMILY_NAME,
                new RDFSharp.Model.RDFPlainLiteral("Doe")));
            return result;
        }

        private static RDFSharp.Model.RDFGraph LoadGraphFromJsonLd()
        {
            var result = new RDFSharp.Model.RDFGraph();
            result.SetContext(new Uri("http://example.com/context/JsonLd"));

            var token = (JsonLD.Core.RDFDataset)JsonLD.Core.JsonLdProcessor.ToRDF(JsonLD.Util.JSONUtils.FromString(@"{
                '@context': 
                {
                    given_name: 'http://xmlns.com/foaf/0.1/firstName',
                    family_name: 'http://xmlns.com/foaf/0.1/familyName',
                    age: 'http://xmlns.com/foaf/0.1/age'
                },
                '@id': 'http://example.com/subject/002',
                given_name: 'Jane',
                family_name: 'Doe',
                age: 41
            }"));

            foreach (var quad in token.GetQuads("@default"))
            {
                var subject = (JsonLD.Core.RDFDataset.IRI)quad["subject"];
                var predicate = (JsonLD.Core.RDFDataset.IRI)quad["predicate"];
                var literal = (JsonLD.Core.RDFDataset.Literal)quad["object"];
                result.AddTriple(new RDFSharp.Model.RDFTriple(
                    new RDFSharp.Model.RDFResource(subject["value"].ToString()),
                    new RDFSharp.Model.RDFResource(predicate["value"].ToString()),
                    new RDFSharp.Model.RDFPlainLiteral(literal["value"].ToString())));
            }

            return result;
        }

        private void WriteSerializedRepresentations(RDFSharp.Model.RDFGraph graph)
        {
            foreach (RDFSharp.Model.RDFModelEnums.RDFFormats format in Enum.GetValues(typeof(RDFSharp.Model.RDFModelEnums.RDFFormats)))
            {
                WriteLine($"{format} representation of graph '{graph.Context}'");
                var serializationOutput = SerializeGraph(graph, format);
                WriteLine(serializationOutput);

                if (format == RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples)
                {
                    WriteLine($"JSON-LD representation of graph '{graph.Context}'");
                    var jsonDocument = JsonLD.Core.JsonLdProcessor.FromRDF(serializationOutput, new JsonLD.Impl.NQuadRDFParser());
                    var context = JToken.Parse("{ '@context': { '@vocab': 'http://xmlns.com/foaf/0.1/' } }");
                    jsonDocument = JsonLD.Core.JsonLdProcessor.Compact(jsonDocument, context, new JsonLD.Core.JsonLdOptions());
                    WriteLine(jsonDocument.ToString());
                    WriteLine();
                }
            }
        }

        private string SerializeGraph(RDFSharp.Model.RDFGraph graph, RDFSharp.Model.RDFModelEnums.RDFFormats format)
        {
            using (var buffer = new MemoryStream())
            {
                graph.ToStream(format, buffer);
                var result = Encoding.UTF8.GetString(buffer.ToArray());
                return result;
            }
        }

        private RDFSharp.Model.RDFGraph DeserializeGraph(string data, RDFSharp.Model.RDFModelEnums.RDFFormats format)
        {
            using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var result = RDFSharp.Model.RDFGraph.FromStream(format, buffer);
                return result;
            }
        }

        private void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        private void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
