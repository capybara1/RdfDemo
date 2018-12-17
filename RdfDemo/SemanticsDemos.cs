﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Semantics;
using System;
using System.IO;
using System.Text;

namespace RdfDemo
{
    [TestClass]
    public class SemanticsDemos
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            RDFSemanticsEvents.OnSemanticsInfo += LogEvent;
            RDFSemanticsEvents.OnSemanticsWarning += LogEvent;
        }
        
        private void LogEvent(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            TestContext.WriteLine(message);
        }

        [TestMethod]
        public void DemonstrateInference()
        {
            var graph = LoadOntologyGraph();

            var ontology = RDFOntology.FromRDFGraph(graph);

            var reasoner = RDFOntologyReasoner.CreateNew("Default")
                .WithRule(RDFBASEReasonerRuleset.InverseOfEntailment);

            reasoner.ApplyToOntology(ref ontology);

            var x = new RDFSharp.Query.RDFVariable("x");
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                x,
                new RDFSharp.Model.RDFResource("http://example.com/demo#parentOf"),
                new RDFSharp.Model.RDFResource("http://example.com/demo#subject001")));
            var query = new RDFSharp.Query.RDFSelectQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToOntology(ontology);

            Assert.AreEqual(1, result.SelectResultsCount);

            string subject;
            foreach (System.Data.DataRow item in result.SelectResults.Rows)
            {
                subject = item[0].ToString();
                TestContext.WriteLine($"Subject: {subject}");
            }
        }

        private RDFSharp.Model.RDFGraph LoadOntologyGraph()
        {
            var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"
<http://example.com/demo> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Ontology> .
<http://example.com/demo#Person> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Class> .
<http://example.com/demo#Person> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Class> .
<http://example.com/demo#parentOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#parentOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#childOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#ObjectProperty> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#domain> <http://example.com/demo#Person> .
<http://example.com/demo#childOf> <http://www.w3.org/2000/01/rdf-schema#range> <http://example.com/demo#Person> .
<http://example.com/demo#childOf> <http://www.w3.org/2002/07/owl#inverseOf> <http://example.com/demo#parentOf> .
<http://example.com/demo#subject001> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://example.com/demo#Person> .
<http://example.com/demo#subject002> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://example.com/demo#Person> .
<http://example.com/demo#subject001> <http://example.com/demo#childOf> <http://example.com/demo#subject002> .
".Trim()));

            var result = RDFSharp.Model.RDFGraph.FromStream(
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples,
                buffer);

            result.SetContext(new Uri("http://example.com/demo"));

            return result;
        }
    }
}
