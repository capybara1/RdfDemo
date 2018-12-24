using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RdfDemo
{
    [TestClass]
    public class RdfXmlDemos
    {
        [TestMethod]
        public void SerializationOfDescriptionElement()
        {
            var graph = Util.DeserializeGraph(@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:rdfs='http://www.w3.org/2000/01/rdf-schema#'
    xmlns:voc='http://example.com/demo/vocab#'>
 
    <Description about='http://example.com/demo/vocab#MyObject'>
        <voc:prop1>123</voc:prop1>
        <voc:prop2 datatype='http://www.w3.org/2001/XMLSchema#int'>123</voc:prop2>
        <voc:prop3 xml:lang='en'>LocalizedLiteralValue</voc:prop3>
        <voc:prop4 resource='http://example.com/demo/vocab#Other' />
        <voc:prop5 nodeID='blankNode1' />
    </Description>

    <Description nodeID='blankNode1'>
        <voc:prop>LiteralValue</voc:prop>
    </Description>

</RDF>",
            RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml);

            Util.WriteSerializedRepresentation(
                graph,
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
        }

        [TestMethod]
        public void SerializationOfDescriptionElementWithXmlBase()
        {
            var graph = Util.DeserializeGraph(@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:rdfs='http://www.w3.org/2000/01/rdf-schema#'
    xmlns:voc='http://example.com/demo/vocab#'
    xml:base='http://example.com/demo/vocab#'>
 
    <Description about='MyObject'>
        <voc:prop resource='Other' />
    </Description>

</RDF>",
            RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml);

            Util.WriteSerializedRepresentation(
                graph,
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
        }

        [TestMethod]
        public void SerializationOfContainers()
        {
            var containerElementNames = new[] { "Bag", "Seq", "Alt" };
            var items = containerElementNames.Select(cnt => 
                new
                {
                    ElementName = cnt,
                    Graph = Util.DeserializeGraph($@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:voc='http://example.com/demo/vocab#'
    xml:base='http://example.com/demo/vocab#'>
 
    <Description about='MyObject'>
        <voc:prop>
            <{cnt}>
              <li>Element1</li>
              <li>Element2</li>
            </{cnt}>
        </voc:prop>
    </Description>

</RDF>",
                    RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml)
                });

            foreach (var item in items)
            {
                Util.WriteLine($"Element {item.ElementName}:");
                Util.WriteSerializedRepresentation(
                    item.Graph,
                    RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
                Util.WriteLine();
            }
        }

        [TestMethod]
        public void SerializationOfCollections()
        {
            var graph = Util.DeserializeGraph(@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:voc='http://example.com/demo/vocab#'
    xml:base='http://example.com/demo/vocab#'>
 
    <Description about='MyObject'>
        <voc:prop parseType='Collection'>
            <Description about='Element1' />
            <Description about='Element2' />
            <Description about='Element3' />
        </voc:prop>
    </Description>

</RDF>",
            RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml);

            Util.WriteSerializedRepresentation(
                graph,
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
        }

        [TestMethod]
        public void SerializationOfXmlLiterals()
        {
            var graph = Util.DeserializeGraph(@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:voc='http://example.com/demo/vocab#'
    xml:base='http://example.com/demo/vocab#'>
 
    <Description about='MyObject'>
        <voc:prop parseType='Literal'>
            <example xmlns='http://example.com/demo/xml' info='Example XML' />
        </voc:prop>
    </Description>

</RDF>",
            RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml);

            Util.WriteSerializedRepresentation(
                graph,
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
        }
        
        [TestMethod]
        public void SerializationOfSpecificRdfSchemaElements()
        {
            var graph = Util.DeserializeGraph(@"<?xml version='1.0'?>
<RDF
    xmlns='http://www.w3.org/1999/02/22-rdf-syntax-ns#'
    xmlns:rdfs='http://www.w3.org/2000/01/rdf-schema#'
    xmlns:voc='http://example.com/demo/vocab#'
    xml:base='http://example.com/demo/vocab#'>
 
    <Class about='MyObject'>
        <rdfs:subClassOf resource='Other' />
    </Class>

    <Property about='prop'>
        <rdfs:domain resource='MyObject' />
        <rdfs:range resource='http://www.w3.org/2001/XMLSchema#int' />
    </Property>

</RDF>",
            RDFSharp.Model.RDFModelEnums.RDFFormats.RdfXml);

            Util.WriteSerializedRepresentation(
                graph,
                RDFSharp.Model.RDFModelEnums.RDFFormats.NTriples);
        }
    }
}
