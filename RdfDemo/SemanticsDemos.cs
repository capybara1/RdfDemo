using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Semantics;
using System;
using System.IO;
using System.Text;

namespace RdfDemo
{
    [TestClass]
    public class SemanticsDemos
    {
        [TestInitialize]
        public void TestInitialize()
        {
            RDFSemanticsEvents.OnSemanticsInfo += Util.WriteLine;
            RDFSemanticsEvents.OnSemanticsWarning += Util.WriteLine;
        }

        [TestMethod]
        public void DemonstrateInference()
        {
            var graph = LoadOntologyGraph();

            var ontology = RDFOntology.FromRDFGraph(graph);

            var report = ontology.Validate();
            foreach (var error in report.SelectErrors())
            {
                Util.WriteLine(error.EvidenceMessage);
            }

            var reasoner = RDFOntologyReasoner.CreateNew("Default")
                .WithRule(RDFBASEReasonerRuleset.SubPropertyTransitivity)
                .WithRule(RDFBASEReasonerRuleset.TransitivePropertyEntailment)
                .WithRule(RDFBASEReasonerRuleset.SymmetricPropertyEntailment)
                .WithRule(RDFBASEReasonerRuleset.InverseOfEntailment);

            reasoner.ApplyToOntology(ref ontology);

            var x = new RDFSharp.Query.RDFVariable("x");
            var y = new RDFSharp.Query.RDFVariable("y");
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                x,
                new RDFSharp.Model.RDFResource("http://example.com/demo#name"),
                new RDFSharp.Model.RDFPlainLiteral("Alice")));
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                y,
                new RDFSharp.Model.RDFResource("http://example.com/demo#parentOf"),
                x));
            var query = new RDFSharp.Query.RDFSelectQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToOntology(ontology);

            Assert.AreEqual(1, result.SelectResultsCount);
            
            foreach (System.Data.DataRow row in result.SelectResults.Rows)
            foreach (System.Data.DataColumn column in result.SelectResults.Columns)
            {
                var cellValue = row[column];
                Util.WriteLine($"{column.ColumnName}: {cellValue}");
            }
        }

        private RDFSharp.Model.RDFGraph LoadOntologyGraph()
        {
            var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"
<http://example.com/demo> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Ontology> .
<http://example.com/demo#Person> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Class> .
<http://example.com/demo#Person> <http://www.w3.org/2000/01/rdf-schema#label> ""person"" .
<http://example.com/demo#name> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#DatatypeProperty> .
<http://example.com/demo#name> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#name> <http://www.w3.org/2000/01/rdf-schema#range> <http://www.w3.org/2001/XMLSchema#string> .
<http://example.com/demo#name> <http://www.w3.org/2000/01/rdf-schema#label> ""has name"" .
<http://example.com/demo#relativeOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#TransitiveProperty> .
<http://example.com/demo#relativeOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#SymmetricProperty> .
<http://example.com/demo#relativeOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#relativeOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#relativeOf> <http://www.w3.org/2000/01/rdf-schema#label> ""is relative of"" .
<http://example.com/demo#ancestorOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#TransitiveProperty> .
<http://example.com/demo#ancestorOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#ancestorOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#ancestorOf> <http://www.w3.org/2000/01/rdf-schema#subPropertyOf> <http://example.com/demo#relativeOf> .
<http://example.com/demo#ancestorOf> <http://www.w3.org/2000/01/rdf-schema#label> ""is ancestor of"" .
<http://example.com/demo#predecessorOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#TransitiveProperty> .
<http://example.com/demo#predecessorOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#predecessorOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#predecessorOf> <http://www.w3.org/2000/01/rdf-schema#subPropertyOf> <http://example.com/demo#relativeOf> .
<http://example.com/demo#predecessorOf> <http://www.w3.org/2000/01/rdf-schema#label> ""is predecessor of"" .
<http://example.com/demo#parentOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#subPropertyOf> <http://example.com/demo#ancestorOf> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#label> ""is ancestor of"" .
<http://example.com/demo#childOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#subPropertyOf> <http://example.com/demo#predecessorOf> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#label> ""is child of"" .
_:genid1 <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Restriction> .
_:genid1 <http://www.w3.org/2002/07/owl#onProperty> <http://example.com/demo#childOf> .
_:genid1 <http://www.w3.org/2002/07/owl#maxCardinality> ""2""^^<http://www.w3.org/2001/XMLSchema#nonNegativeInteger> . 
<http://example.com/demo#childOf> <http://www.w3.org/2002/07/owl#inverseOf> <http://example.com/demo#parentOf> .
<http://example.com/demo#subject001> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://example.com/demo#Person> .
<http://example.com/demo#subject001> <http://example.com/demo#name> ""Alice"" .
<http://example.com/demo#subject002> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://example.com/demo#Person> .
<http://example.com/demo#subject002> <http://example.com/demo#name> ""Bob"" .
<http://example.com/demo#subject003> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://example.com/demo#Person> .
<http://example.com/demo#subject003> <http://example.com/demo#name> ""Claire"" .
<http://example.com/demo#subject001> <http://example.com/demo#childOf> <http://example.com/demo#subject002> .
<http://example.com/demo#subject002> <http://example.com/demo#childOf> <http://example.com/demo#subject003> .
".Trim()));

            var result = RDFSharp.Model.RDFGraph.FromStream(
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples,
                buffer);

            result.SetContext(new Uri("http://example.com/demo"));

            return result;
        }
    }
}
