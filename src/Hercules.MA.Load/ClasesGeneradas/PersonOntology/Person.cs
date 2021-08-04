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

namespace PersonOntology
{
	public class Person : GnossOCBase
	{

		public Person() : base() { } 

		public Person(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			this.Foaf_familyName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/familyName"));
			this.Roh_ORCID = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/ORCID"));
			SemanticPropertyModel propFoaf_nick = pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/nick");
			this.Foaf_nick = new List<string>();
			if (propFoaf_nick != null && propFoaf_nick.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propFoaf_nick.PropertyValues)
				{
					this.Foaf_nick.Add(propValue.Value);
				}
			}
			this.Foaf_firstName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/firstName"));
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
			this.Foaf_lastName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/lastName"));
		}

		public Person(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Foaf_familyName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/familyName"));
			this.Roh_ORCID = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/ORCID"));
			SemanticPropertyModel propFoaf_nick = pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/nick");
			this.Foaf_nick = new List<string>();
			if (propFoaf_nick != null && propFoaf_nick.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propFoaf_nick.PropertyValues)
				{
					this.Foaf_nick.Add(propValue.Value);
				}
			}
			this.Foaf_firstName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/firstName"));
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
			this.Foaf_lastName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/lastName"));
		}

		[LABEL(LanguageEnum.es,"Segundo apellido")]
		[RDFProperty("http://xmlns.com/foaf/0.1/familyName")]
		public  string Foaf_familyName { get; set;}

		[LABEL(LanguageEnum.es,"ORCID")]
		[RDFProperty("http://w3id.org/roh/ORCID")]
		public  string Roh_ORCID { get; set;}

		[LABEL(LanguageEnum.es,"Firma")]
		[RDFProperty("http://xmlns.com/foaf/0.1/nick")]
		public  List<string> Foaf_nick { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://xmlns.com/foaf/0.1/firstName")]
		public  string Foaf_firstName { get; set;}

		[LABEL(LanguageEnum.es,"Nombre completo")]
		[RDFProperty("http://xmlns.com/foaf/0.1/name")]
		public  string Foaf_name { get; set;}

		[LABEL(LanguageEnum.es,"Primer apellido")]
		[RDFProperty("http://xmlns.com/foaf/0.1/lastName")]
		public  string Foaf_lastName { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("foaf:familyName", this.Foaf_familyName));
			propList.Add(new StringOntologyProperty("roh:ORCID", this.Roh_ORCID));
			propList.Add(new ListStringOntologyProperty("foaf:nick", this.Foaf_nick));
			propList.Add(new StringOntologyProperty("foaf:firstName", this.Foaf_firstName));
			propList.Add(new StringOntologyProperty("foaf:name", this.Foaf_name));
			propList.Add(new StringOntologyProperty("foaf:lastName", this.Foaf_lastName));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 
		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias)
		{
			return ToGnossApiResource(resourceAPI, listaDeCategorias, Guid.Empty, Guid.Empty);
		}

		public virtual ComplexOntologyResource ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias, Guid idrecurso, Guid idarticulo)
		{
			ComplexOntologyResource resource = new ComplexOntologyResource();
			Ontology ontology=null;
			GetProperties();
			if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))
			{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://xmlns.com/foaf/0.1/Person", "http://xmlns.com/foaf/0.1/Person", prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://xmlns.com/foaf/0.1/Person", "http://xmlns.com/foaf/0.1/Person", prefList, propList, entList,idrecurso,idarticulo);
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
			list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Person> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://xmlns.com/foaf/0.1/Person\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> . ");
				if(this.Foaf_familyName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/familyName> \"{this.Foaf_familyName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_ORCID != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://w3id.org/roh/ORCID> \"{this.Roh_ORCID.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Foaf_nick != null)
				{
					foreach(var item2 in this.Foaf_nick)
					{
						list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/nick> \"{item2.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
					}
				}
				if(this.Foaf_firstName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/firstName> \"{this.Foaf_firstName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Foaf_name != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/name> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Foaf_lastName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Person_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/lastName> \"{this.Foaf_lastName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"person\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/type> \"http://xmlns.com/foaf/0.1/Person\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasfechapublicacion> {DateTime.Now.ToString("yyyyMMddHHmmss")} . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hastipodoc> \"5\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasfechamodificacion> {DateTime.Now.ToString("yyyyMMddHHmmss")} . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasnumeroVisitas>  0 . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasprivacidadCom> \"publico\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/firstName> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/hasnombrecompleto> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
			string search = string.Empty;
			search = $"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}";
			if(!string.IsNullOrEmpty(this.Foaf_name))
			{
				search += $"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}";
			}
			if(!string.IsNullOrEmpty(search))
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/search> \"{search.ToLower()}\" . ");
			}
				if(this.Foaf_familyName != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/familyName> \"{this.Foaf_familyName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_ORCID != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/ORCID> \"{this.Roh_ORCID.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Foaf_nick != null)
				{
					foreach(var item2 in this.Foaf_nick)
					{
						list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/nick> \"{item2.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
					}
				}
				if(this.Foaf_firstName != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/firstName> \"{this.Foaf_firstName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Foaf_name != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/name> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Foaf_lastName != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/lastName> \"{this.Foaf_lastName.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string tablaDoc = $"'{this.Foaf_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}', '{this.Foaf_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}', '{resourceAPI.GraphsUrl}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/PersonOntology_{ResourceID}_{ArticleID}";
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Foaf_name;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Foaf_name;
		}


	}
}
