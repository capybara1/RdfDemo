using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RdfDemo
{
    [TestClass]
    public class BasicDemos
    {
        public TestContext TestContext { get; set; }

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
                TestContext.WriteLine($"Subject: {subject}");
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
                TestContext.WriteLine($"Context: {context}, Subject: {subject}");
            }
        }

        private static RDFSharp.Model.RDFGraph ConstructGraph()
        {
            var graph = new RDFSharp.Model.RDFGraph();
            graph.SetContext(new Uri("http://example.com/context/Local"));

            var exampleNamespace = new RDFSharp.Model.RDFNamespace("ex", "http://example.com/");
            RDFSharp.Model.RDFNamespaceRegister.AddNamespace(exampleNamespace);

            graph.AddTriple(new RDFSharp.Model.RDFTriple(
                new RDFSharp.Model.RDFResource("ex:subject/001"),
                RDFSharp.Model.RDFVocabulary.FOAF.FIRSTNAME, // Equivalent to RDFResource("foaf:firstName") which is equivalent to RDFResource("http://xmlns.com/foaf/0.1/firstName")
                new RDFSharp.Model.RDFPlainLiteral("John")));
            graph.AddTriple(new RDFSharp.Model.RDFTriple(
                new RDFSharp.Model.RDFResource("ex:subject/001"),
                RDFSharp.Model.RDFVocabulary.FOAF.FAMILY_NAME,
                new RDFSharp.Model.RDFPlainLiteral("Doe")));
            return graph;
        }

        private static RDFSharp.Model.RDFGraph LoadGraphFromJsonLd()
        {
            var graph = new RDFSharp.Model.RDFGraph();
            graph.SetContext(new Uri("http://example.com/context/JsonLd"));

            var test = (JsonLD.Core.RDFDataset)JsonLD.Core.JsonLdProcessor.ToRDF(JsonLD.Util.JSONUtils.FromString(@"{
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

            foreach (var quad in test.GetQuads("@default"))
            {
                var subject = (JsonLD.Core.RDFDataset.IRI)quad["subject"];
                var predicate = (JsonLD.Core.RDFDataset.IRI)quad["predicate"];
                var literal = (JsonLD.Core.RDFDataset.Literal)quad["object"];
                graph.AddTriple(new RDFSharp.Model.RDFTriple(
                    new RDFSharp.Model.RDFResource(subject["value"].ToString()),
                    new RDFSharp.Model.RDFResource(predicate["value"].ToString()),
                    new RDFSharp.Model.RDFPlainLiteral(literal["value"].ToString())));
            }

            return graph;
        }
    }
}
