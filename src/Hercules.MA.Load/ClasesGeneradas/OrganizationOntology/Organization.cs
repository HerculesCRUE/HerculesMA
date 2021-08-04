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
using OrganizationType = OrganizationTypeOntology.OrganizationType;

namespace OrganizationOntology
{
	public class Organization : GnossOCBase
	{

		public Organization() : base() { } 

		public Organization(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			SemanticPropertyModel propVcard_hasAddress = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasAddress");
			if(propVcard_hasAddress != null && propVcard_hasAddress.PropertyValues.Count > 0)
			{
				this.Vcard_hasAddress = new Address(propVcard_hasAddress.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propDc_type = pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/type");
			if(propDc_type != null && propDc_type.PropertyValues.Count > 0)
			{
				this.Dc_type = new OrganizationType(propDc_type.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vivo_identifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#identifier"));
			this.Roh_otherTypeMoreInfo = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/otherTypeMoreInfo"));
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
		}

		public Organization(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propVcard_hasAddress = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasAddress");
			if(propVcard_hasAddress != null && propVcard_hasAddress.PropertyValues.Count > 0)
			{
				this.Vcard_hasAddress = new Address(propVcard_hasAddress.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propDc_type = pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/type");
			if(propDc_type != null && propDc_type.PropertyValues.Count > 0)
			{
				this.Dc_type = new OrganizationType(propDc_type.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vivo_identifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#identifier"));
			this.Roh_otherTypeMoreInfo = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/otherTypeMoreInfo"));
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
		}

		[LABEL(LanguageEnum.es,"Dirección")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#hasAddress")]
		public  Address Vcard_hasAddress { get; set;}

		[LABEL(LanguageEnum.es,"Tipo de entidad")]
		[RDFProperty("http://purl.org/dc/elements/1.1/type")]
		public  OrganizationType Dc_type  { get; set;} 
		public string IdDc_type  { get; set;} 

		[LABEL(LanguageEnum.es,"Identificador")]
		[RDFProperty("http://vivoweb.org/ontology/core#identifier")]
		public  string Vivo_identifier { get; set;}

		[LABEL(LanguageEnum.es,"Tipo de entidad, otros")]
		[RDFProperty("http://w3id.org/roh/otherTypeMoreInfo")]
		public  string Roh_otherTypeMoreInfo { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://w3id.org/roh/title")]
		public  string Roh_title { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("dc:type", this.IdDc_type));
			propList.Add(new StringOntologyProperty("vivo:identifier", this.Vivo_identifier));
			propList.Add(new StringOntologyProperty("roh:otherTypeMoreInfo", this.Roh_otherTypeMoreInfo));
			propList.Add(new StringOntologyProperty("roh:title", this.Roh_title));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			if(Vcard_hasAddress!=null){
				Vcard_hasAddress.GetProperties();
				Vcard_hasAddress.GetEntities();
				OntologyEntity entityVcard_hasAddress = new OntologyEntity("https://www.w3.org/2006/vcard/ns#Address", "https://www.w3.org/2006/vcard/ns#Address", "vcard:hasAddress", Vcard_hasAddress.propList, Vcard_hasAddress.entList);
				entList.Add(entityVcard_hasAddress);
			}
		} 
		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias)
		{
			return ToGnossApiResource(resourceAPI, listaDeCategorias, Guid.Empty, Guid.Empty);
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias, Guid idrecurso, Guid idarticulo)
		{
			ComplexOntologyResource resource = new ComplexOntologyResource();
			Ontology ontology=null;
			GetEntities();
			GetProperties();
			if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))
			{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://xmlns.com/foaf/0.1/Organization", "http://xmlns.com/foaf/0.1/Organization", prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://xmlns.com/foaf/0.1/Organization", "http://xmlns.com/foaf/0.1/Organization", prefList, propList, entList,idrecurso,idarticulo);
			}
			resource.Id = GNOSSID;
			resource.Ontology = ontology;
			resource.TextCategories=listaDeCategorias;
			AddResourceTitle(resource);
			AddResourceDescription(resource);
			AddImages(resource);
			AddFiles(resource);
			return resource;
		}

		public override List<string> ToOntologyGnossTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Organization> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://xmlns.com/foaf/0.1/Organization\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> . ");
			if(this.Vcard_hasAddress != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#Address> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#Address\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <https://www.w3.org/2006/vcard/ns#hasAddress> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> . ");
				if(this.Vcard_hasAddress.IdVcard_hasRegion != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{this.Vcard_hasAddress.IdVcard_hasRegion}> . ");
				}
				if(this.Vcard_hasAddress.IdVcard_hasCountryName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{this.Vcard_hasAddress.IdVcard_hasCountryName}> . ");
				}
				if(this.Vcard_hasAddress.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Vcard_hasAddress.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
				if(this.IdDc_type != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://purl.org/dc/elements/1.1/type> <{this.IdDc_type}> . ");
				}
				if(this.Vivo_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://vivoweb.org/ontology/core#identifier> \"{this.Vivo_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_otherTypeMoreInfo != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://w3id.org/roh/otherTypeMoreInfo> \"{this.Roh_otherTypeMoreInfo.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Organization_{ResourceID}_{ArticleID}> <http://w3id.org/roh/title> \"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"Organization\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/type> \"http://xmlns.com/foaf/0.1/Organization\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasfechapublicacion> {DateTime.Now.ToString("yyyyMMddHHmmss")} . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hastipodoc> \"5\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasfechamodificacion> {DateTime.Now.ToString("yyyyMMddHHmmss")} . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasnumeroVisitas>  0 . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasprivacidadCom> \"publico\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/firstName> \"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasnombrecompleto> \"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
			string search = string.Empty;
			search = $"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}";
			if(!string.IsNullOrEmpty(this.Roh_title))
			{
				search += $"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}";
			}
			if(!string.IsNullOrEmpty(search))
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/search> \"{search.ToLower()}\" . ");
			}
			if(this.Vcard_hasAddress != null)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <https://www.w3.org/2006/vcard/ns#hasAddress> <{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> . ");
				if(this.Vcard_hasAddress.IdVcard_hasRegion != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Vcard_hasAddress.IdVcard_hasRegion;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{itemRegex}> . ");
				}
				if(this.Vcard_hasAddress.IdVcard_hasCountryName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Vcard_hasAddress.IdVcard_hasCountryName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{itemRegex}> . ");
				}
				if(this.Vcard_hasAddress.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_hasAddress.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Vcard_hasAddress.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
				if(this.IdDc_type != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdDc_type;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://purl.org/dc/elements/1.1/type> <{itemRegex}> . ");
				}
				if(this.Vivo_identifier != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://vivoweb.org/ontology/core#identifier> \"{this.Vivo_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_otherTypeMoreInfo != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/otherTypeMoreInfo> \"{this.Roh_otherTypeMoreInfo.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_title != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/title> \"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string tablaDoc = $"'{this.Roh_title.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}', '{this.Roh_title.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}', '{resourceAPI.GraphsUrl}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/OrganizationOntology_{ResourceID}_{ArticleID}";
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Roh_title;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Roh_title;
		}


	}
}
