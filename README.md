# RDF Demo

Demo Code with Examples for educational purpose

## Linked Data

- [Linked Data](https://www.w3.org/DesignIssues/LinkedData.html)
  - Author: Tim Berners-Lee
  - Date: 18.06.2009

## RDF, RDFS and OWL

### Concepts

#### Blank nodes

Blank nodes are nodes in a graph that cannot be addressed by an URL.
Thus irrelevant for plain statements, blank nodes can be used to build advanced structure e.g. linked lists.

#### RDF Containers

Container types such as `rdf:Bag`, `rdf:Seq` and `rdf:Alt` are formally indifferent,
because a gaph does not impose any order to the edges of a node.
However, the type indicates to the human reader that there is a special meaning to the order of the items.

The class `rdfs:ContainerMembershipProperty` has as instances the properties `rdf:_1`, `rdf:_2` ... .
Those properties are intended to assign members to a container.

Containers are open for extension.

#### RDF Collections

A collection is a sub-graph that resembles a linked list.
The type `rdf:List` represents a node in the linked list with the properties `rdf:first` and `rdf:rest`.
The resource `rdf:nil` represents the terminal node in the linked list.

Collections are closed.

#### Reification

Reification allows modeling a statement explicitly, using a blank node of type `rdf:Statement`.
The properties `rdf:subject`, `rdf:predicate` and `rdf:object` may be used with a `rdf:Statement`.

In contrast to a simple triple, which is the implicit form of a statement,
the explicit form e.g. is open for providing additional properties about the statement itself.

### Reference

- [RDF 1.1 Primer](https://www.w3.org/TR/rdf11-primer/)
- [RDF 1.1 Concepts](https://www.w3.org/TR/rdf11-concepts/)
- [RDF 1.1 Semantics](https://www.w3.org/TR/rdf11-mt/)
- [RDF Schema 1.1](https://www.w3.org/TR/rdf-schema/)
- [OWL 2 Web Ontology Language Primer (Second Edition)](https://www.w3.org/TR/2012/REC-owl2-primer-20121211/)
- [OWL 2 Web Ontology Language Document Overview (Second Edition)](https://www.w3.org/TR/2012/REC-owl2-overview-20121211/)
- [SPARQL 1.1 Overview](http://www.w3.org/TR/sparql11-overview/)

### Tutorials

- [infowebml.ws on RDF and OWL](http://infowebml.ws/rdf-owl/)
- [etutorials.org on RDF](http://etutorials.org/Misc/Practical+resource+description+framework+rdf/)
- [w3schools.com on RDF](https://www.w3schools.com/xml/xml_rdf.asp)
- [Introducing Linked Data And The Semantic Web](http://www.linkeddatatools.com/semantic-web-basics)

### Utilities

- [EasyRDF Converter](http://www.easyrdf.org/converter)

### Libraries

- [Guide for the libraray RDFSharp](http://dadev.cloudapp.net/Datos%20Abiertos/PDF/ReferenceGuide.pdf)

### RDF Encoding Syntaxes

#### RDF/XML

##### Reference

- [RDF 1.1 XML Syntax](https://www.w3.org/TR/rdf-syntax-grammar/)

#### N-Triples/N-Quads

##### Reference

- [RDF 1.1 N-Triples](https://www.w3.org/TR/n-triples/)
- [RDF 1.1 N-Quads](https://www.w3.org/TR/n-quads/)

#### Turtle

##### Reference

- [RDF 1.1 Turtle](https://www.w3.org/TR/turtle/)

##### Examples

- [BBC Things](https://www.bbc.co.uk/things/)

#### JSON-LD

##### Reference

- [JSON-LD 1.0](https://www.w3.org/TR/json-ld/)
- [JSON-LD 1.1 Processing Algorithms and API](https://www.w3.org/TR/json-ld11-api/)

##### Slides

- [JSON-LD by Gregg Kellogg](https://de.slideshare.net/gkellogg1/json-for-linked-data)

##### Discussion

- [Comment on the intentional isolation of JSON-LD from other Semantic Web Technologies](http://manu.sporny.org/2014/json-ld-origins-2/)

##### Utilities

- [JSON-LD Playground](https://json-ld.org/playground/)

#### TriG

##### Reference

- [RDF 1.1 TriG](https://www.w3.org/TR/trig/)

#### RDFa

##### Reference

- [RDFa 1.1 Primer](https://www.w3.org/TR/rdfa-primer/)

#### Microdata

##### Reference

- [HTML Microdata](https://www.w3.org/TR/microdata/)

#### Microformats

##### Reference

- [microformats2](http://microformats.org/wiki/microformats2)

### OWL Encoding Syntaxes

- [OWL Syntaxes](http://ontogenesis.knowledgeblog.org/88)

#### Functional

##### Reference

- [OWL 2 Web Ontology Language - Structural Specification and Functional-Style Syntax (Second Edition)](https://www.w3.org/TR/owl2-syntax/)

#### OWL/XML

##### Reference

- [OWL 2 Web Ontology Language - XML Serialization (Second Edition)](https://www.w3.org/TR/2012/REC-owl2-xml-serialization-20121211/)

#### Manchester

##### Reference

- [OWL 2 Web Ontology Language - Manchester Syntax (Second Edition)](https://www.w3.org/TR/owl2-manchester-syntax/)
