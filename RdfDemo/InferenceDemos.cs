using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Semantics;
using System.IO;
using System.Text;

namespace RdfDemo
{
    [TestClass]
    public class InferenceDemos
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void DemonstrateTransitivity()
        {
            var graph = LoadOntologyGraph();

            var ontology = RDFOntology.FromRDFGraph(graph);
            
            var patternGroup = new RDFSharp.Query.RDFPatternGroup("PG1");
            patternGroup.AddPattern(new RDFSharp.Query.RDFPattern(
                new RDFSharp.Model.RDFResource("http://example.com/subject/001"),
                new RDFSharp.Model.RDFResource("http://example.com/property/isChefOf"),
                new RDFSharp.Model.RDFResource("http://example.com/subject/003")));
            var query = new RDFSharp.Query.RDFAskQuery();
            query.AddPatternGroup(patternGroup);
            var result = query.ApplyToOntology(ontology);

            Assert.IsTrue(result.AskResult);
        }

        private RDFSharp.Model.RDFGraph LoadOntologyGraph()
        {
            var buffer = new MemoryStream(Encoding.UTF8.GetBytes(@"
<http://example.com/property/isChefOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#Property> .
<http://example.com/property/isChefOf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2002/07/owl#TransitiveProperty> .
<http://example.com/subject/001> <http://example.com/property/isChefOf> <http://example.com/subject/002> .
<http://example.com/subject/002> <http://example.com/property/isChefOf> <http://example.com/subject/003> .
".Trim()));

            var result = RDFSharp.Model.RDFGraph.FromStream(
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples,
                buffer);

            return result;
        }
    }
}
