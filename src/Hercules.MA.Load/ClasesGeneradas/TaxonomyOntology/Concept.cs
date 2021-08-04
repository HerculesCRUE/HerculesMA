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
	public class Concept : GnossOCBase
	{

		public Concept() : base() { } 

		public Concept(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Skos_narrower = new List<Concept>();
			SemanticPropertyModel propSkos_narrower = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#narrower");
			if(propSkos_narrower != null && propSkos_narrower.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos_narrower.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos_narrower = new Concept(propValue.RelatedEntity,idiomaUsuario);
						this.Skos_narrower.Add(skos_narrower);
					}
				}
			}
			this.Skos_broader = new List<Concept>();
			SemanticPropertyModel propSkos_broader = pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#broader");
			if(propSkos_broader != null && propSkos_broader.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propSkos_broader.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept skos_broader = new Concept(propValue.RelatedEntity,idiomaUsuario);
						this.Skos_broader.Add(skos_broader);
					}
				}
			}
			this.Skos_prefLabel = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#prefLabel"));
			this.Skos_symbol = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/2004/02/skos/core#symbol"));
			this.Dc_identifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/identifier"));
			this.Dc_source = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/source"));
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Específico")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#narrower")]
		public  List<Concept> Skos_narrower { get; set;}

		[LABEL(LanguageEnum.es,"Genérico")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#broader")]
		public  List<Concept> Skos_broader { get; set;}

		[LABEL(LanguageEnum.es,"Etiqueta preferente")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#prefLabel")]
		public  string Skos_prefLabel { get; set;}

		[LABEL(LanguageEnum.es,"Símbolo")]
		[RDFProperty("http://www.w3.org/2004/02/skos/core#symbol")]
		public  string Skos_symbol { get; set;}

		[LABEL(LanguageEnum.es,"Identificador")]
		[RDFProperty("http://purl.org/dc/elements/1.1/identifier")]
		public  string Dc_identifier { get; set;}

		[LABEL(LanguageEnum.es,"Fuente")]
		[RDFProperty("http://purl.org/dc/elements/1.1/source")]
		public  string Dc_source { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("skos:prefLabel", this.Skos_prefLabel));
			propList.Add(new StringOntologyProperty("skos:symbol", this.Skos_symbol));
			propList.Add(new StringOntologyProperty("dc:identifier", this.Dc_identifier));
			propList.Add(new StringOntologyProperty("dc:source", this.Dc_source));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			if(Skos_narrower!=null){
				foreach(Concept prop in Skos_narrower){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2004/02/skos/core#Concept", "http://www.w3.org/2004/02/skos/core#Concept", "skos:narrower", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity= entityConcept;
				}
			}
			if(Skos_broader!=null){
				foreach(Concept prop in Skos_broader){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityConcept = new OntologyEntity("http://www.w3.org/2004/02/skos/core#Concept", "http://www.w3.org/2004/02/skos/core#Concept", "skos:broader", prop.propList, prop.entList);
				entList.Add(entityConcept);
				prop.Entity= entityConcept;
				}
			}
		} 



		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>();

			return valor;
		}





	}
}
