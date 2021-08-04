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
using Person = PersonOntology.Person;

namespace CurriculumvitaeOntology
{
	public class CV : GnossOCBase
	{

		public CV() : base() { } 

		public CV(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			SemanticPropertyModel propRoh_scientificExperience = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/scientificExperience");
			if(propRoh_scientificExperience != null && propRoh_scientificExperience.PropertyValues.Count > 0)
			{
				this.Roh_scientificExperience = new ScientificExperience(propRoh_scientificExperience.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_cvOf = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/cvOf");
			if(propRoh_cvOf != null && propRoh_cvOf.PropertyValues.Count > 0)
			{
				this.Roh_cvOf = new Person(propRoh_cvOf.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_personalData = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/personalData");
			if(propRoh_personalData != null && propRoh_personalData.PropertyValues.Count > 0)
			{
				this.Roh_personalData = new PersonalData(propRoh_personalData.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
		}

		public CV(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propRoh_scientificExperience = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/scientificExperience");
			if(propRoh_scientificExperience != null && propRoh_scientificExperience.PropertyValues.Count > 0)
			{
				this.Roh_scientificExperience = new ScientificExperience(propRoh_scientificExperience.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_cvOf = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/cvOf");
			if(propRoh_cvOf != null && propRoh_cvOf.PropertyValues.Count > 0)
			{
				this.Roh_cvOf = new Person(propRoh_cvOf.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_personalData = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/personalData");
			if(propRoh_personalData != null && propRoh_personalData.PropertyValues.Count > 0)
			{
				this.Roh_personalData = new PersonalData(propRoh_personalData.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
		}

		[LABEL(LanguageEnum.es,"Experiencia científica y tecnológica")]
		[RDFProperty("http://w3id.org/roh/scientificExperience")]
		public  ScientificExperience Roh_scientificExperience { get; set;}

		[LABEL(LanguageEnum.es,"Currículum de")]
		[RDFProperty("http://w3id.org/roh/cvOf")]
		[Required]
		public  Person Roh_cvOf  { get; set;} 
		public string IdRoh_cvOf  { get; set;} 

		[LABEL(LanguageEnum.es,"Datos personales")]
		[RDFProperty("http://w3id.org/roh/personalData")]
		public  PersonalData Roh_personalData { get; set;}

		[LABEL(LanguageEnum.es,"Usuario Gnoss")]
		[RDFProperty("http://w3id.org/roh/gnossUser")]
		public  string Roh_gnossUser { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://xmlns.com/foaf/0.1/name")]
		public  string Foaf_name { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("roh:cvOf", this.IdRoh_cvOf));
			propList.Add(new StringOntologyProperty("roh:gnossUser", this.Roh_gnossUser));
			propList.Add(new StringOntologyProperty("foaf:name", this.Foaf_name));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			Roh_scientificExperience.GetProperties();
			Roh_scientificExperience.GetEntities();
			OntologyEntity entityRoh_scientificExperience = new OntologyEntity("http://w3id.org/roh/ScientificExperience", "http://w3id.org/roh/ScientificExperience", "roh:scientificExperience", Roh_scientificExperience.propList, Roh_scientificExperience.entList);
			entList.Add(entityRoh_scientificExperience);
			Roh_personalData.GetProperties();
			Roh_personalData.GetEntities();
			OntologyEntity entityRoh_personalData = new OntologyEntity("http://w3id.org/roh/PersonalData", "http://w3id.org/roh/PersonalData", "roh:personalData", Roh_personalData.propList, Roh_personalData.entList);
			entList.Add(entityRoh_personalData);
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
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://w3id.org/roh/CV", "http://w3id.org/roh/CV", prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://w3id.org/roh/CV", "http://w3id.org/roh/CV", prefList, propList, entList,idrecurso,idarticulo);
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
			list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/CV> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/CV\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> . ");
			if(this.Roh_scientificExperience != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/ScientificExperience> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/ScientificExperience\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://w3id.org/roh/scientificExperience> <{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> . ");
			if(this.Roh_scientificExperience.Roh_competitiveProjects != null)
			{
			foreach(var item1 in this.Roh_scientificExperience.Roh_competitiveProjects)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/RelatedProjects> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/RelatedProjects\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://w3id.org/roh/competitiveProjects> <{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> . ");
				if(item1.IdVivo_relatedBy != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> <http://vivoweb.org/ontology/core#relatedBy> <{item1.IdVivo_relatedBy}> . ");
				}
				if(item1.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> <http://w3id.org/roh/order> {item1.Roh_order.ToString()} . ");
				}
				if(item1.Roh_isPublic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}> <http://w3id.org/roh/isPublic> \"{item1.Roh_isPublic.ToString()}\" . ");
				}
			}
			}
			if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/RelatedProjects> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/RelatedProjects\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://w3id.org/roh/nonCompetitiveProjects> <{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> . ");
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://vivoweb.org/ontology/core#relatedBy> <{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy}> . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://w3id.org/roh/order> {this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_order.ToString()} . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://w3id.org/roh/isPublic> \"{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic.ToString()}\" . ");
				}
			}
			}
			if(this.Roh_personalData != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/PersonalData> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/PersonalData\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://w3id.org/roh/personalData> <{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> . ");
			if(this.Roh_personalData.Vcard_address != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#Address> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#Address\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#address> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> . ");
				if(this.Roh_personalData.Vcard_address.IdVcard_hasRegion != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{this.Roh_personalData.Vcard_address.IdVcard_hasRegion}> . ");
				}
				if(this.Roh_personalData.Vcard_address.IdVcard_hasCountryName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{this.Roh_personalData.Vcard_address.IdVcard_hasCountryName}> . ");
				}
				if(this.Roh_personalData.Vcard_address.IdRoh_hasProvince != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <http://w3id.org/roh/hasProvince> <{this.Roh_personalData.Vcard_address.IdRoh_hasProvince}> . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_postal_code != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#postal-code> \"{this.Roh_personalData.Vcard_address.Vcard_postal_code.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_extended_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#extended-address> \"{this.Roh_personalData.Vcard_address.Vcard_extended_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_street_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#street-address> \"{this.Roh_personalData.Vcard_address.Vcard_street_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Roh_personalData.Vcard_address.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			if(this.Roh_personalData.Roh_birthplace != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#Address> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#Address\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/birthplace> <{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> . ");
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <http://w3id.org/roh/hasProvince> <{this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_postal_code != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#postal-code> \"{this.Roh_personalData.Roh_birthplace.Vcard_postal_code.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_extended_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#extended-address> \"{this.Roh_personalData.Roh_birthplace.Vcard_extended_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_street_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#street-address> \"{this.Roh_personalData.Roh_birthplace.Vcard_street_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Roh_personalData.Roh_birthplace.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			if(this.Roh_personalData.Roh_hasFax != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#TelephoneType> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#TelephoneType\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/hasFax> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> . ");
				if(this.Roh_personalData.Roh_hasFax.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://w3id.org/roh/hasExtension> \"{this.Roh_personalData.Roh_hasFax.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{this.Roh_personalData.Roh_hasFax.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			if(this.Roh_personalData.Vcard_hasTelephone != null)
			{
			foreach(var item4 in this.Roh_personalData.Vcard_hasTelephone)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#TelephoneType> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#TelephoneType\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasTelephone> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> . ");
				if(item4.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> <http://w3id.org/roh/hasExtension> \"{item4.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item4.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{item4.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item4.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item4.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{item4.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			}
			if(this.Roh_personalData.Roh_hasMobilePhone != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://www.w3.org/2006/vcard/ns#TelephoneType> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"https://www.w3.org/2006/vcard/ns#TelephoneType\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/hasMobilePhone> <{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> . ");
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://w3id.org/roh/hasExtension> \"{this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			if(this.Roh_personalData.Vivo_researcherId != null)
			{
			foreach(var item6 in this.Roh_personalData.Vivo_researcherId)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Document> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://xmlns.com/foaf/0.1/Document\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://vivoweb.org/ontology/core#researcherId> <{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> . ");
				if(item6.Foaf_topic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> <http://xmlns.com/foaf/0.1/topic> \"{item6.Foaf_topic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item6.Dc_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> <http://purl.org/dc/elements/1.1/title> \"{item6.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item6.Foaf_primaryTopic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item6.ArticleID}> <http://xmlns.com/foaf/0.1/primaryTopic> \"{item6.Foaf_primaryTopic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			}
			if(this.Roh_personalData.Foaf_openid != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://xmlns.com/foaf/0.1/Document> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://xmlns.com/foaf/0.1/Document\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/openid> <{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> . ");
				if(this.Roh_personalData.Foaf_openid.Foaf_topic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://xmlns.com/foaf/0.1/topic> \"{this.Roh_personalData.Foaf_openid.Foaf_topic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Foaf_openid.Dc_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://purl.org/dc/elements/1.1/title> \"{this.Roh_personalData.Foaf_openid.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://xmlns.com/foaf/0.1/primaryTopic> \"{this.Roh_personalData.Foaf_openid.Foaf_primaryTopic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
				if(this.Roh_personalData.IdSchema_nationality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://www.schema.org/nationality> <{this.Roh_personalData.IdSchema_nationality}> . ");
				}
				if(this.Roh_personalData.Foaf_homepage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/homepage> \"{this.Roh_personalData.Foaf_homepage.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Vcard_birth_date != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#birth-date> \"{this.Roh_personalData.Vcard_birth_date.Value.ToString("yyyyMMddHHmmss")}\" . ");
				}
				if(this.Roh_personalData.Foaf_gender != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/gender> \"{this.Roh_personalData.Foaf_gender.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Foaf_img != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/img> \"{this.Roh_personalData.Foaf_img.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_personalData.Vcard_email != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#email> \"{this.Roh_personalData.Vcard_email.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
				if(this.IdRoh_cvOf != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://w3id.org/roh/cvOf> <{this.IdRoh_cvOf}> . ");
				}
				/*if(this.IdRoh_gnossUser != null)
				{
					//list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://w3id.org/roh/gnossUser> <{this.IdRoh_gnossUser}> . ");
				}*/
				if(this.Foaf_name != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}> <http://xmlns.com/foaf/0.1/name> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"curriculumvitae\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/type> \"http://w3id.org/roh/CV\" . ");
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
			if(this.Roh_scientificExperience != null)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/scientificExperience> <{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> . ");
			if(this.Roh_scientificExperience.Roh_competitiveProjects != null)
			{
			foreach(var item1 in this.Roh_scientificExperience.Roh_competitiveProjects)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://w3id.org/roh/competitiveProjects> <{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}> . ");
				if(item1.IdVivo_relatedBy != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item1.IdVivo_relatedBy;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}> <http://vivoweb.org/ontology/core#relatedBy> <{itemRegex}> . ");
				}
				if(item1.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}> <http://w3id.org/roh/order> {item1.Roh_order.ToString()} . ");
				}
				if(item1.Roh_isPublic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}> <http://w3id.org/roh/isPublic> \"{item1.Roh_isPublic.ToString().ToLower()}\" . ");
				}
			}
			}
			if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}> <http://w3id.org/roh/nonCompetitiveProjects> <{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> . ");
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://vivoweb.org/ontology/core#relatedBy> <{itemRegex}> . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://w3id.org/roh/order> {this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_order.ToString()} . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}> <http://w3id.org/roh/isPublic> \"{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic.ToString().ToLower()}\" . ");
				}
			}
			}
			if(this.Roh_personalData != null)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/personalData> <{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> . ");
			if(this.Roh_personalData.Vcard_address != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#address> <{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> . ");
				if(this.Roh_personalData.Vcard_address.IdVcard_hasRegion != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Vcard_address.IdVcard_hasRegion;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Vcard_address.IdVcard_hasCountryName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Vcard_address.IdVcard_hasCountryName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Vcard_address.IdRoh_hasProvince != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Vcard_address.IdRoh_hasProvince;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <http://w3id.org/roh/hasProvince> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_postal_code != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#postal-code> \"{this.Roh_personalData.Vcard_address.Vcard_postal_code.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_extended_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#extended-address> \"{this.Roh_personalData.Vcard_address.Vcard_extended_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_street_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#street-address> \"{this.Roh_personalData.Vcard_address.Vcard_street_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Roh_personalData.Vcard_address.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			if(this.Roh_personalData.Roh_birthplace != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/birthplace> <{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> . ");
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasRegion> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasCountryName> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <http://w3id.org/roh/hasProvince> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_postal_code != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#postal-code> \"{this.Roh_personalData.Roh_birthplace.Vcard_postal_code.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_extended_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#extended-address> \"{this.Roh_personalData.Roh_birthplace.Vcard_extended_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_street_address != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#street-address> \"{this.Roh_personalData.Roh_birthplace.Vcard_street_address.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_locality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}> <https://www.w3.org/2006/vcard/ns#locality> \"{this.Roh_personalData.Roh_birthplace.Vcard_locality.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			if(this.Roh_personalData.Roh_hasFax != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/hasFax> <{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> . ");
				if(this.Roh_personalData.Roh_hasFax.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://w3id.org/roh/hasExtension> \"{this.Roh_personalData.Roh_hasFax.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{this.Roh_personalData.Roh_hasFax.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			if(this.Roh_personalData.Vcard_hasTelephone != null)
			{
			foreach(var item4 in this.Roh_personalData.Vcard_hasTelephone)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasTelephone> <{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item4.ArticleID}> . ");
				if(item4.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item4.ArticleID}> <http://w3id.org/roh/hasExtension> \"{item4.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item4.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item4.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{item4.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item4.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item4.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{item4.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			}
			if(this.Roh_personalData.Roh_hasMobilePhone != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://w3id.org/roh/hasMobilePhone> <{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> . ");
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://w3id.org/roh/hasExtension> \"{this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <http://w3id.org/roh/hasInternationalCode> \"{this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}> <https://www.w3.org/2006/vcard/ns#hasValue> \"{this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			if(this.Roh_personalData.Vivo_researcherId != null)
			{
			foreach(var item6 in this.Roh_personalData.Vivo_researcherId)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://vivoweb.org/ontology/core#researcherId> <{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item6.ArticleID}> . ");
				if(item6.Foaf_topic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item6.ArticleID}> <http://xmlns.com/foaf/0.1/topic> \"{item6.Foaf_topic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item6.Dc_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item6.ArticleID}> <http://purl.org/dc/elements/1.1/title> \"{item6.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item6.Foaf_primaryTopic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item6.ArticleID}> <http://xmlns.com/foaf/0.1/primaryTopic> \"{item6.Foaf_primaryTopic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			}
			if(this.Roh_personalData.Foaf_openid != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/openid> <{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> . ");
				if(this.Roh_personalData.Foaf_openid.Foaf_topic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://xmlns.com/foaf/0.1/topic> \"{this.Roh_personalData.Foaf_openid.Foaf_topic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Foaf_openid.Dc_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://purl.org/dc/elements/1.1/title> \"{this.Roh_personalData.Foaf_openid.Dc_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}> <http://xmlns.com/foaf/0.1/primaryTopic> \"{this.Roh_personalData.Foaf_openid.Foaf_primaryTopic.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
				if(this.Roh_personalData.IdSchema_nationality != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Roh_personalData.IdSchema_nationality;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://www.schema.org/nationality> <{itemRegex}> . ");
				}
				if(this.Roh_personalData.Foaf_homepage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/homepage> \"{this.Roh_personalData.Foaf_homepage.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Vcard_birth_date != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#birth-date> {this.Roh_personalData.Vcard_birth_date.Value.ToString("yyyyMMddHHmmss")} . ");
				}
				if(this.Roh_personalData.Foaf_gender != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/gender> \"{this.Roh_personalData.Foaf_gender.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Foaf_img != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <http://xmlns.com/foaf/0.1/img> \"{this.Roh_personalData.Foaf_img.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_personalData.Vcard_email != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}> <https://www.w3.org/2006/vcard/ns#email> \"{this.Roh_personalData.Vcard_email.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
				if(this.IdRoh_cvOf != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdRoh_cvOf;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/cvOf> <{itemRegex}> . ");
				}
				/*if(this.IdRoh_gnossUser != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdRoh_gnossUser;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/gnossUser> <{itemRegex}> . ");
				}*/
				if(this.Foaf_name != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/name> \"{this.Foaf_name.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
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
			return $"{resourceAPI.GraphsUrl}items/CurriculumvitaeOntology_{ResourceID}_{ArticleID}";
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
