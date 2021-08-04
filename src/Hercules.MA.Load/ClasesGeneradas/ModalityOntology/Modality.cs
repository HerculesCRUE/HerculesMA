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

namespace ModalityOntology
{
	public class Modality : GnossOCBase
	{

		public Modality() : base() { } 

		public Modality(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Dc_title = new Dictionary<LanguageEnum,string>();
			this.Dc_title.Add(idiomaUsuario , GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/title")));
			
			this.Dc_identifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/identifier"));
		}

		[LABEL(LanguageEnum.es,"Modalidad")]
		[RDFProperty("http://purl.org/dc/elements/1.1/title")]
		public  Dictionary<LanguageEnum,string> Dc_title { get; set;}

		[LABEL(LanguageEnum.es,"Identificador de modalidad")]
		[RDFProperty("http://purl.org/dc/elements/1.1/identifier")]
		public  string Dc_identifier { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			foreach (LanguageEnum LanguageEnum in Enum.GetValues(typeof(LanguageEnum)))
			{
				propList.Add(new StringOntologyProperty("dc:title", this.Dc_title[LanguageEnum], LanguageEnum.ToString()));
			}
			propList.Add(new StringOntologyProperty("dc:identifier", this.Dc_identifier));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 
		public virtual SecondaryResource ToGnossApiResource(ResourceApi resourceAPI,string identificador)
		{
			SecondaryResource resource = new SecondaryResource();
			List<SecondaryEntity> listSecondaryEntity = null;
			GetProperties();
			SecondaryOntology ontology = new SecondaryOntology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://w3id.org/roh/Modality", "http://w3id.org/roh/Modality", prefList, propList,identificador,listSecondaryEntity, null);
			resource.SecondaryOntology = ontology;
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<{resourceAPI.GraphsUrl}items/Modality_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/Modality> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/Modality_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/Modality\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Modality_{ResourceID}_{ArticleID}> . ");
				if(this.Dc_title != null)
				{
					foreach (LanguageEnum LanguageEnum in Enum.GetValues(typeof(LanguageEnum)))
					{
						//list.Add($"<{resourceAPI.GraphsUrl}items/Modality_{ResourceID}_{ArticleID}> <http://purl.org/dc/elements/1.1/title> \"{this.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" @[LanguageEnum]" .);
					}
				}
				if(this.Dc_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Modality_{ResourceID}_{ArticleID}> <http://purl.org/dc/elements/1.1/identifier> \"{this.Dc_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
				if(this.Dc_title != null)
				{
					foreach (LanguageEnum LanguageEnum in Enum.GetValues(typeof(LanguageEnum)))
					{
						//list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://purl.org/dc/elements/1.1/title> \"{this.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" @[LanguageEnum]" .);
					}
				}
				if(this.Dc_identifier != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://purl.org/dc/elements/1.1/identifier> \"{this.Dc_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
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
			return $"{resourceAPI.GraphsUrl}items/ModalityOntology_{ResourceID}_{ArticleID}";
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
		}



	}
}
