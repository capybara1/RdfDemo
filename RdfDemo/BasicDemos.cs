using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RdfDemo
{
    [TestClass]
    public class BasicDemos
    {
        [TestInitialize]
        public void TestInitialize()
        {
            RDFSharp.Query.RDFQueryEvents.OnSELECTQueryEvaluation += Util.WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnASKQueryEvaluation += Util.WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnDESCRIBEQueryEvaluation += Util.WriteLine;
            RDFSharp.Query.RDFQueryEvents.OnCONSTRUCTQueryEvaluation += Util.WriteLine;
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForSimpleGraph()
        {
            var graph = ConstructGraph();
            
            var contextDocument = JToken.Parse(@"{
                '@context': {
                    '@vocab': 'http://xmlns.com/foaf/0.1/'
                }
            }");
            Util.WriteSerializedRepresentations(graph, contextDocument);
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

            foreach (System.Data.DataRow row in result.SelectResults.Rows)
            foreach (System.Data.DataColumn column in result.SelectResults.Columns)
            {
                var cellValue = row[column];
                Util.WriteLine($"{column.ColumnName}: {cellValue}");
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
            query.AddProjectionVariable(x);
            query.AddProjectionVariable(y);
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToStore(store);

            Assert.AreEqual(2, result.SelectResultsCount);

            foreach (System.Data.DataRow row in result.SelectResults.Rows)
            foreach (System.Data.DataColumn column in result.SelectResults.Columns)
            {
                var cellValue = row[column];
                Util.WriteLine($"{column.ColumnName}: {cellValue}");
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
            var result = Util.DeserializeGraphFromJsonLd(@"{
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
            }");

            return result;
        }
    }
}
