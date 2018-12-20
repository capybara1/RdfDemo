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

            Util.WriteSerializedRepresentations(graph);
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForContainer()
        {
            var graph = Util.DeserializeGraph(
                @"
<http://example.org/courses/6.001> <http://example.org/students/vocab#students> _:genid1 .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/1999/02/22-rdf-syntax-ns#Bag> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_1> <http://example.org/students/Amy> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_2> <http://example.org/students/Mohamed> .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#_3> <http://example.org/students/Johann> .
".Trim(),
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);

            Util.WriteSerializedRepresentations(graph);
        }

        [TestMethod]
        public void DemonstrateSerializationFormatsForCollection()
        {
            var graph = Util.DeserializeGraph(
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

            Util.WriteSerializedRepresentations(graph);
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
