using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Helpers;
using GnossBase;
using Es.Riam.Gnoss.Web.MVC.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Gnoss.ApiWrapper.Exceptions;
using System.Diagnostics.CodeAnalysis;
//using Person = PersonOntology.Person;
//using Document = DocumentOntology.Document;
//using Project = ProjectOntology.Project;
//using Patent = PatentOntology.Patent;
using OfferState = OfferstateOntology.OfferState;
using FramingSector = FramingsectorOntology.FramingSector;
using MatureState = MaturestateOntology.MatureState;

namespace OfferOntology
{
	[ExcludeFromCodeCoverage]
	public class Offer : GnossOCBase
	{

		public Offer() : base() { } 

		public virtual string RdfType { get { return "http://www.schema.org/Offer"; } }
		public virtual string RdfsLabel { get { return "http://www.schema.org/Offer"; } }
		[RDFProperty("http://w3id.org/roh/researchers")]
		//public  List<Person> Roh_researchers { get; set;}
		public List<string> IdsRoh_researchers { get; set;}

		[RDFProperty("http://w3id.org/roh/document")]
		//public  List<Document> Roh_document { get; set;}
		public List<string> IdsRoh_document { get; set;}

		[RDFProperty("http://w3id.org/roh/project")]
		//public  List<Project> Roh_project { get; set;}
		public List<string> IdsRoh_project { get; set;}

		[RDFProperty("http://w3id.org/roh/availabilityChangeEvent")]
		[MinLength(1)]
		public  List<AvailabilityChangeEvent> Roh_availabilityChangeEvent { get; set;}

		[RDFProperty("http://w3id.org/roh/patents")]
		//public  List<Patent> Roh_patents { get; set;}
		public List<string> IdsRoh_patents { get; set;}

		[RDFProperty("http://vocab.data.gov/def/drm#origin")]
		public  string Drm_origin { get; set;}

		[RDFProperty("http://w3id.org/roh/lineResearch")]
		public  List<string> Roh_lineResearch { get; set;}

		[RDFProperty("http://vivoweb.org/ontology/core#freetextKeyword")]
		public  List<string> Vivo_freetextKeyword { get; set;}

		[RDFProperty("http://w3id.org/roh/innovation")]
		public  string Roh_innovation { get; set;}

		[RDFProperty("http://w3id.org/roh/collaborationSought")]
		public  string Roh_collaborationSought { get; set;}

		[RDFProperty("http://w3id.org/roh/partnerType")]
		public  string Roh_partnerType { get; set;}

		[RDFProperty("http://purl.org/linked-data/cube#observation")]
		public  string Qb_observation { get; set;}

		[RDFProperty("http://purl.org/ontology/bibo/recipient")]
		public  string Bibo_recipient { get; set;}

		[RDFProperty("http://purl.org/dc/terms/issued")]
		public  DateTime Dct_issued { get; set;}

		[RDFProperty("http://www.schema.org/offeredBy")]
		[Required]
		//public  Person Schema_offeredBy  { get; set;} 
		public string IdSchema_offeredBy  { get; set;} 

		[RDFProperty("http://w3id.org/roh/application")]
		public  string Roh_application { get; set;}

		[RDFProperty("http://www.schema.org/availability")]
		[Required]
		public  OfferState Schema_availability  { get; set;} 
		public string IdSchema_availability  { get; set;} 

		[RDFProperty("http://www.schema.org/description")]
		public  string Schema_description { get; set;}

		[RDFProperty("http://w3id.org/roh/framingSector")]
		[Required]
		public  FramingSector Roh_framingSector  { get; set;} 
		public string IdRoh_framingSector  { get; set;} 

		[RDFProperty("http://purl.org/ontology/bibo/status")]
		[Required]
		public  MatureState Bibo_status  { get; set;} 
		public string IdBibo_status  { get; set;} 

		[RDFProperty("http://www.schema.org/name")]
		public  string Schema_name { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("roh:researchers", this.IdsRoh_researchers));
			propList.Add(new ListStringOntologyProperty("roh:document", this.IdsRoh_document));
			propList.Add(new ListStringOntologyProperty("roh:project", this.IdsRoh_project));
			propList.Add(new ListStringOntologyProperty("roh:patents", this.IdsRoh_patents));
			propList.Add(new StringOntologyProperty("drm:origin", this.Drm_origin));
			propList.Add(new ListStringOntologyProperty("roh:lineResearch", this.Roh_lineResearch));
			propList.Add(new ListStringOntologyProperty("vivo:freetextKeyword", this.Vivo_freetextKeyword));
			propList.Add(new StringOntologyProperty("roh:innovation", this.Roh_innovation));
			propList.Add(new StringOntologyProperty("roh:collaborationSought", this.Roh_collaborationSought));
			propList.Add(new StringOntologyProperty("roh:partnerType", this.Roh_partnerType));
			propList.Add(new StringOntologyProperty("qb:observation", this.Qb_observation));
			propList.Add(new StringOntologyProperty("bibo:recipient", this.Bibo_recipient));
			propList.Add(new DateOntologyProperty("dct:issued", this.Dct_issued));
			propList.Add(new StringOntologyProperty("schema:offeredBy", this.IdSchema_offeredBy));
			propList.Add(new StringOntologyProperty("roh:application", this.Roh_application));
			propList.Add(new StringOntologyProperty("schema:availability", this.IdSchema_availability));
			propList.Add(new StringOntologyProperty("schema:description", this.Schema_description));
			propList.Add(new StringOntologyProperty("roh:framingSector", this.IdRoh_framingSector));
			propList.Add(new StringOntologyProperty("bibo:status", this.IdBibo_status));
			propList.Add(new StringOntologyProperty("schema:name", this.Schema_name));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Roh_availabilityChangeEvent!=null){
				foreach(AvailabilityChangeEvent prop in Roh_availabilityChangeEvent){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityAvailabilityChangeEvent = new OntologyEntity("http://w3id.org/roh/AvailabilityChangeEvent", "http://w3id.org/roh/AvailabilityChangeEvent", "roh:availabilityChangeEvent", prop.propList, prop.entList);
				entList.Add(entityAvailabilityChangeEvent);
				prop.Entity= entityAvailabilityChangeEvent;
				}
			}
		} 
		
		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string tags = "";
			foreach(string tag in tagList)
			{
				tags += $"{tag}, ";
			}
			if (!string.IsNullOrEmpty(tags))
			{
				tags = tags.Substring(0, tags.LastIndexOf(','));
			}
			string titulo = $"{this.Schema_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
			string descripcion = $"{this.Schema_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
			string tablaDoc = $"'{titulo}', '{descripcion}', '{resourceAPI.GraphsUrl}', '{tags}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/OfferOntology_{ResourceID}_{ArticleID}";
		}


		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Schema_name;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Schema_name;
		}




	}
}
