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

namespace TaxonomyOntology
{
	public class Collection : GnossOCBase
	{

		public Collection() : base() { } 

		public Collection(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Skos_member = new List<Concept>();
			SemanticPropertyModel propSkos_member = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#member");
			if(propSkos_member != null && propSkos_member.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos_member.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos_member = new Concept(propValue.RelatedEntity,idiomaUsuario);
						this.Skos_member.Add(skos_member);
					}
				}
			}
			this.Skos_scopeNote = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#scopeNote"));
			this.Dc_source = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/source"));
		}

		[LABEL(LanguageEnum.es,"Miembro")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#member")]
		public  List<Concept> Skos_member { get; set;}

		[LABEL(LanguageEnum.es,"Nota alcance")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#scopeNote")]
		public  string Skos_scopeNote { get; set;}

		[LABEL(LanguageEnum.es,"Fuente")]
		[RDFProperty("http://purl.org/dc/elements/1.1/source")]
		public  string Dc_source { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("skos:scopeNote", this.Skos_scopeNote));
			propList.Add(new StringOntologyProperty("dc:source", this.Dc_source));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			if(Skos_member!=null){
				foreach(Concept prop in Skos_member){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2004/02/skos/core#Concept", "http://www.w3.org/2004/02/skos/core#Concept", "skos:member", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity= entityConcept;
				}
			}
		} 
		public virtual SecondaryResource ToGnossApiResource(ResourceApi resourceAPI,string identificador)
		{
			SecondaryResource resource = new SecondaryResource();
			List<SecondaryEntity> listSecondaryEntity = null;
			GetEntities();
			GetProperties();
			SecondaryOntology ontology = new SecondaryOntology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://www.w3.org/2004/02/skos/core#Collection", "http://www.w3.org/2004/02/skos/core#Collection", prefList, propList,identificador,listSecondaryEntity, entList);
			resource.SecondaryOntology = ontology;
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2004/02/skos/core#Collection> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://www.w3.org/2004/02/skos/core#Collection\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> . ");
			if(this.Skos_member != null)
			{
			foreach(var item0 in this.Skos_member)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://www.w3.org/2004/02/skos/core#Concept> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://www.w3.org/2004/02/skos/core#Concept\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> <http://www.w3.org/2004/02/skos/core#member> <{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.Skos_prefLabel != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2004/02/skos/core#prefLabel> \"{item0.Skos_prefLabel.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Skos_symbol != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2004/02/skos/core#symbol> \"{item0.Skos_symbol.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Dc_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://purl.org/dc/elements/1.1/identifier> \"{item0.Dc_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Dc_source != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Concept_{ResourceID}_{item0.ArticleID}> <http://purl.org/dc/elements/1.1/source> \"{item0.Dc_source.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			}
				if(this.Skos_scopeNote != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> <http://www.w3.org/2004/02/skos/core#scopeNote> \"{this.Skos_scopeNote.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Dc_source != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Collection_{ResourceID}_{ArticleID}> <http://purl.org/dc/elements/1.1/source> \"{this.Dc_source.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			if(this.Skos_member != null)
			{
			foreach(var item0 in this.Skos_member)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/2004/02/skos/core#member> <{resourceAPI.GraphsUrl}items/concept_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.Skos_prefLabel != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2004/02/skos/core#prefLabel> \"{item0.Skos_prefLabel.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Skos_symbol != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/concept_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2004/02/skos/core#symbol> \"{item0.Skos_symbol.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Dc_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/concept_{ResourceID}_{item0.ArticleID}> <http://purl.org/dc/elements/1.1/identifier> \"{item0.Dc_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Dc_source != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/concept_{ResourceID}_{item0.ArticleID}> <http://purl.org/dc/elements/1.1/source> \"{item0.Dc_source.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			}
				if(this.Skos_scopeNote != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/2004/02/skos/core#scopeNote> \"{this.Skos_scopeNote.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Dc_source != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://purl.org/dc/elements/1.1/source> \"{this.Dc_source.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>();

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/TaxonomyOntology_{ResourceID}_{ArticleID}";
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
		}



	}
}
