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
			SemanticPropertyModel propRoh_scientificActivity = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/scientificActivity");
			if(propRoh_scientificActivity != null && propRoh_scientificActivity.PropertyValues.Count > 0)
			{
				this.Roh_scientificActivity = new ScientificActivity(propRoh_scientificActivity.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_personalData = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/personalData");
			if(propRoh_personalData != null && propRoh_personalData.PropertyValues.Count > 0)
			{
				this.Roh_personalData = new PersonalData(propRoh_personalData.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_cvOf = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/cvOf");
			if(propRoh_cvOf != null && propRoh_cvOf.PropertyValues.Count > 0)
			{
				this.Roh_cvOf = new Person(propRoh_cvOf.PropertyValues[0].RelatedEntity,idiomaUsuario);
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
			SemanticPropertyModel propRoh_scientificActivity = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/scientificActivity");
			if(propRoh_scientificActivity != null && propRoh_scientificActivity.PropertyValues.Count > 0)
			{
				this.Roh_scientificActivity = new ScientificActivity(propRoh_scientificActivity.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_personalData = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/personalData");
			if(propRoh_personalData != null && propRoh_personalData.PropertyValues.Count > 0)
			{
				this.Roh_personalData = new PersonalData(propRoh_personalData.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_cvOf = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/cvOf");
			if(propRoh_cvOf != null && propRoh_cvOf.PropertyValues.Count > 0)
			{
				this.Roh_cvOf = new Person(propRoh_cvOf.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/CV"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/CV"; } }
		[LABEL(LanguageEnum.es,"Experiencia científica y tecnológica")]
		[RDFProperty("http://w3id.org/roh/scientificExperience")]
		public  ScientificExperience Roh_scientificExperience { get; set;}

		[RDFProperty("http://w3id.org/roh/scientificActivity")]
		public  ScientificActivity Roh_scientificActivity { get; set;}

		[LABEL(LanguageEnum.es,"Datos personales")]
		[RDFProperty("http://w3id.org/roh/personalData")]
		public  PersonalData Roh_personalData { get; set;}

		[LABEL(LanguageEnum.es,"Currículum de")]
		[RDFProperty("http://w3id.org/roh/cvOf")]
		[Required]
		public  Person Roh_cvOf  { get; set;} 
		public string IdRoh_cvOf  { get; set;} 

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
			Roh_scientificExperience.GetProperties();
			Roh_scientificExperience.GetEntities();
			OntologyEntity entityRoh_scientificExperience = new OntologyEntity("http://w3id.org/roh/ScientificExperience", "http://w3id.org/roh/ScientificExperience", "roh:scientificExperience", Roh_scientificExperience.propList, Roh_scientificExperience.entList);
			entList.Add(entityRoh_scientificExperience);
			Roh_scientificActivity.GetProperties();
			Roh_scientificActivity.GetEntities();
			OntologyEntity entityRoh_scientificActivity = new OntologyEntity("http://w3id.org/roh/ScientificActivity", "http://w3id.org/roh/ScientificActivity", "roh:scientificActivity", Roh_scientificActivity.propList, Roh_scientificActivity.entList);
			entList.Add(entityRoh_scientificActivity);
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
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList,idrecurso,idarticulo);
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
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/CV>", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/CV\"", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}>", list, " . ");
			if(this.Roh_scientificExperience != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/ScientificExperience>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/ScientificExperience\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}", "http://w3id.org/roh/scientificExperience", $"<{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}>", list, " . ");
			if(this.Roh_scientificExperience.Roh_competitiveProjects != null)
			{
			foreach(var item1 in this.Roh_scientificExperience.Roh_competitiveProjects)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedProjects>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedProjects\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://w3id.org/roh/competitiveProjects", $"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}>", list, " . ");
				if(item1.IdVivo_relatedBy != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}",  "http://vivoweb.org/ontology/core#relatedBy", $"<{item1.IdVivo_relatedBy}>", list, " . ");
				}
				if(item1.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item1.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedProjects>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedProjects\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificExperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://w3id.org/roh/nonCompetitiveProjects", $"<{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}>", list, " . ");
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}",  "http://vivoweb.org/ontology/core#relatedBy", $"<{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.IdVivo_relatedBy}>", list, " . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedProjects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/ScientificActivity>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/ScientificActivity\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}", "http://w3id.org/roh/scientificActivity", $"<{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}>", list, " . ");
			if(this.Roh_scientificActivity.Roh_worksSubmittedConferences != null)
			{
			foreach(var item1 in this.Roh_scientificActivity.Roh_worksSubmittedConferences)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedDocuments>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedDocuments\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/worksSubmittedConferences", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}>", list, " . ");
				if(item1.IdRoh_relatedDocument != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{item1.IdRoh_relatedDocument}>", list, " . ");
				}
				if(item1.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item1.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_otherDisseminationActivities != null)
			{
			foreach(var item2 in this.Roh_scientificActivity.Roh_otherDisseminationActivities)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedDocuments>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedDocuments\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/otherDisseminationActivities", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}>", list, " . ");
				if(item2.IdRoh_relatedDocument != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{item2.IdRoh_relatedDocument}>", list, " . ");
				}
				if(item2.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item2.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_scientificPublications != null)
			{
			foreach(var item3 in this.Roh_scientificActivity.Roh_scientificPublications)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedDocuments>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedDocuments\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/scientificPublications", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}>", list, " . ");
				if(item3.IdRoh_relatedDocument != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{item3.IdRoh_relatedDocument}>", list, " . ");
				}
				if(item3.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item3.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item3.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_worksSubmittedSeminars != null)
			{
			foreach(var item4 in this.Roh_scientificActivity.Roh_worksSubmittedSeminars)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/RelatedDocuments>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/RelatedDocuments\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ScientificActivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/worksSubmittedSeminars", $"<{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}>", list, " . ");
				if(item4.IdRoh_relatedDocument != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{item4.IdRoh_relatedDocument}>", list, " . ");
				}
				if(item4.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/RelatedDocuments_{ResourceID}_{item4.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item4.Roh_isPublic.ToString()}\"", list, " . ");
				}
			}
			}
			}
			if(this.Roh_personalData != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/PersonalData>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/PersonalData\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}", "http://w3id.org/roh/personalData", $"<{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}>", list, " . ");
			if(this.Roh_personalData.Roh_hasFax != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#TelephoneType>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#TelephoneType\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/hasFax", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Roh_hasFax.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Roh_hasExtension)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Vcard_hasValue)}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vcard_hasTelephone != null)
			{
			foreach(var item2 in this.Roh_personalData.Vcard_hasTelephone)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#TelephoneType>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#TelephoneType\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "https://www.w3.org/2006/vcard/ns#hasTelephone", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}>", list, " . ");
				if(item2.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(item2.Roh_hasExtension)}\"", list, " . ");
				}
				if(item2.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(item2.Roh_hasInternationalCode)}\"", list, " . ");
				}
				if(item2.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{item2.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(item2.Vcard_hasValue)}\"", list, " . ");
				}
			}
			}
			if(this.Roh_personalData.Roh_hasMobilePhone != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#TelephoneType>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#TelephoneType\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/hasMobilePhone", $"<{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/TelephoneType_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue)}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vivo_researcherId != null)
			{
			foreach(var item4 in this.Roh_personalData.Vivo_researcherId)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://xmlns.com/foaf/0.1/Document>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://xmlns.com/foaf/0.1/Document\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://vivoweb.org/ontology/core#researcherId", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}>", list, " . ");
				if(item4.Foaf_topic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(item4.Foaf_topic)}\"", list, " . ");
				}
				if(item4.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(item4.Dc_title)}\"", list, " . ");
				}
				if(item4.Foaf_primaryTopic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{item4.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(item4.Foaf_primaryTopic)}\"", list, " . ");
				}
			}
			}
			if(this.Roh_personalData.Foaf_openid != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://xmlns.com/foaf/0.1/Document>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://xmlns.com/foaf/0.1/Document\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://xmlns.com/foaf/0.1/openid", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Foaf_openid.Foaf_topic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Foaf_topic)}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_openid.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Dc_title)}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic)}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Roh_birthplace != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#Address>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#Address\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/birthplace", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{this.Roh_personalData.Roh_birthplace.IdVcard_hasCountryName}>", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{this.Roh_personalData.Roh_birthplace.IdVcard_hasRegion}>", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "http://w3id.org/roh/hasProvince", $"<{this.Roh_personalData.Roh_birthplace.IdRoh_hasProvince}>", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_postal_code != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#postal-code", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_postal_code)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_extended_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#extended-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_extended_address)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_street_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#street-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_street_address)}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_locality)}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vcard_address != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#Address>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#Address\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Vcard_address.IdVcard_hasCountryName != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{this.Roh_personalData.Vcard_address.IdVcard_hasCountryName}>", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.IdVcard_hasRegion != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{this.Roh_personalData.Vcard_address.IdVcard_hasRegion}>", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.IdRoh_hasProvince != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "http://w3id.org/roh/hasProvince", $"<{this.Roh_personalData.Vcard_address.IdRoh_hasProvince}>", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_postal_code != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#postal-code", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_postal_code)}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_extended_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#extended-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_extended_address)}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_street_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#street-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_street_address)}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_locality)}\"", list, " . ");
				}
			}
				if(this.Roh_personalData.IdSchema_nationality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://www.schema.org/nationality", $"<{this.Roh_personalData.IdSchema_nationality}>", list, " . ");
				}
				if(this.Roh_personalData.Foaf_homepage != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/homepage", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_homepage)}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_birth_date != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "https://www.w3.org/2006/vcard/ns#birth-date", $"\"{this.Roh_personalData.Vcard_birth_date.Value.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_gender != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/gender", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_gender)}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_img != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/img", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_img)}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_email != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PersonalData_{ResourceID}_{this.Roh_personalData.ArticleID}",  "https://www.w3.org/2006/vcard/ns#email", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_email)}\"", list, " . ");
				}
			}
				if(this.IdRoh_cvOf != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/cvOf", $"<{this.IdRoh_cvOf}>", list, " . ");
				}
				//if(this.IdRoh_gnossUser != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/gnossUser", $"<{this.IdRoh_gnossUser}>", list, " . ");
				//}
				if(this.Foaf_name != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CV_{ResourceID}_{ArticleID}",  "http://xmlns.com/foaf/0.1/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Foaf_name)}\"", list, " . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			List<string> listaSearch = new List<string>();
			AgregarTags(list);
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"\"curriculumvitae\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/type", $"\"http://w3id.org/roh/CV\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechapublicacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hastipodoc", "\"5\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechamodificacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnumeroVisitas", "0", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasprivacidadCom", "\"publico\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://xmlns.com/foaf/0.1/firstName", $"\"{GenerarTextoSinSaltoDeLinea(this.Foaf_name)}\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnombrecompleto", $"\"{GenerarTextoSinSaltoDeLinea(this.Foaf_name)}\"", list, " . ");
			string search = string.Empty;
			if(this.Roh_scientificExperience != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/scientificExperience", $"<{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}>", list, " . ");
			if(this.Roh_scientificExperience.Roh_competitiveProjects != null)
			{
			foreach(var item1 in this.Roh_scientificExperience.Roh_competitiveProjects)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://w3id.org/roh/competitiveProjects", $"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}",  "http://vivoweb.org/ontology/core#relatedBy", $"<{itemRegex}>", list, " . ");
				}
				if(item1.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item1.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificexperience_{ResourceID}_{this.Roh_scientificExperience.ArticleID}", "http://w3id.org/roh/nonCompetitiveProjects", $"<{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}",  "http://vivoweb.org/ontology/core#relatedBy", $"<{itemRegex}>", list, " . ");
				}
				if(this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relatedprojects_{ResourceID}_{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{this.Roh_scientificExperience.Roh_nonCompetitiveProjects.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/scientificActivity", $"<{resourceAPI.GraphsUrl}items/scientificactivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}>", list, " . ");
			if(this.Roh_scientificActivity.Roh_worksSubmittedConferences != null)
			{
			foreach(var item1 in this.Roh_scientificActivity.Roh_worksSubmittedConferences)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificactivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/worksSubmittedConferences", $"<{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item1.ArticleID}>", list, " . ");
				if(item1.IdRoh_relatedDocument != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item1.IdRoh_relatedDocument;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{itemRegex}>", list, " . ");
				}
				if(item1.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item1.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item1.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_otherDisseminationActivities != null)
			{
			foreach(var item2 in this.Roh_scientificActivity.Roh_otherDisseminationActivities)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificactivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/otherDisseminationActivities", $"<{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item2.ArticleID}>", list, " . ");
				if(item2.IdRoh_relatedDocument != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2.IdRoh_relatedDocument;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{itemRegex}>", list, " . ");
				}
				if(item2.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item2.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_scientificPublications != null)
			{
			foreach(var item3 in this.Roh_scientificActivity.Roh_scientificPublications)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificactivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/scientificPublications", $"<{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item3.ArticleID}>", list, " . ");
				if(item3.IdRoh_relatedDocument != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item3.IdRoh_relatedDocument;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item3.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{itemRegex}>", list, " . ");
				}
				if(item3.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item3.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item3.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_scientificActivity.Roh_worksSubmittedSeminars != null)
			{
			foreach(var item4 in this.Roh_scientificActivity.Roh_worksSubmittedSeminars)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/scientificactivity_{ResourceID}_{this.Roh_scientificActivity.ArticleID}", "http://w3id.org/roh/worksSubmittedSeminars", $"<{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item4.ArticleID}>", list, " . ");
				if(item4.IdRoh_relatedDocument != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item4.IdRoh_relatedDocument;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item4.ArticleID}",  "http://w3id.org/roh/relatedDocument", $"<{itemRegex}>", list, " . ");
				}
				if(item4.Roh_isPublic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/relateddocuments_{ResourceID}_{item4.ArticleID}",  "http://w3id.org/roh/isPublic", $"\"{item4.Roh_isPublic.ToString().ToLower()}\"", list, " . ");
				}
			}
			}
			}
			if(this.Roh_personalData != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/personalData", $"<{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}>", list, " . ");
			if(this.Roh_personalData.Roh_hasFax != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/hasFax", $"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Roh_hasFax.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Roh_hasExtension).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Roh_hasInternationalCode).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasFax.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasFax.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasFax.Vcard_hasValue).ToLower()}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vcard_hasTelephone != null)
			{
			foreach(var item2 in this.Roh_personalData.Vcard_hasTelephone)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "https://www.w3.org/2006/vcard/ns#hasTelephone", $"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item2.ArticleID}>", list, " . ");
				if(item2.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(item2.Roh_hasExtension).ToLower()}\"", list, " . ");
				}
				if(item2.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item2.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(item2.Roh_hasInternationalCode).ToLower()}\"", list, " . ");
				}
				if(item2.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{item2.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(item2.Vcard_hasValue).ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_personalData.Roh_hasMobilePhone != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/hasMobilePhone", $"<{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "http://w3id.org/roh/hasExtension", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasExtension).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "http://w3id.org/roh/hasInternationalCode", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Roh_hasInternationalCode).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/telephonetype_{ResourceID}_{this.Roh_personalData.Roh_hasMobilePhone.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasValue", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_hasMobilePhone.Vcard_hasValue).ToLower()}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vivo_researcherId != null)
			{
			foreach(var item4 in this.Roh_personalData.Vivo_researcherId)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://vivoweb.org/ontology/core#researcherId", $"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item4.ArticleID}>", list, " . ");
				if(item4.Foaf_topic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item4.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(item4.Foaf_topic).ToLower()}\"", list, " . ");
				}
				if(item4.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item4.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(item4.Dc_title).ToLower()}\"", list, " . ");
				}
				if(item4.Foaf_primaryTopic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{item4.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(item4.Foaf_primaryTopic).ToLower()}\"", list, " . ");
				}
			}
			}
			if(this.Roh_personalData.Foaf_openid != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://xmlns.com/foaf/0.1/openid", $"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}>", list, " . ");
				if(this.Roh_personalData.Foaf_openid.Foaf_topic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Foaf_topic).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_openid.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Dc_title).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Roh_personalData.Foaf_openid.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_openid.Foaf_primaryTopic).ToLower()}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Roh_birthplace != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "http://w3id.org/roh/birthplace", $"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{itemRegex}>", list, " . ");
				}
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{itemRegex}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "http://w3id.org/roh/hasProvince", $"<{itemRegex}>", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_postal_code != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#postal-code", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_postal_code).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_extended_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#extended-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_extended_address).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_street_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#street-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_street_address).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Roh_birthplace.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Roh_birthplace.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Roh_birthplace.Vcard_locality).ToLower()}\"", list, " . ");
				}
			}
			if(this.Roh_personalData.Vcard_address != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{itemRegex}>", list, " . ");
				}
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{itemRegex}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "http://w3id.org/roh/hasProvince", $"<{itemRegex}>", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_postal_code != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#postal-code", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_postal_code).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_extended_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#extended-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_extended_address).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_street_address != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#street-address", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_street_address).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Roh_personalData.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_address.Vcard_locality).ToLower()}\"", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://www.schema.org/nationality", $"<{itemRegex}>", list, " . ");
				}
				if(this.Roh_personalData.Foaf_homepage != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/homepage", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_homepage).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_birth_date != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "https://www.w3.org/2006/vcard/ns#birth-date", $"{this.Roh_personalData.Vcard_birth_date.Value.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Roh_personalData.Foaf_gender != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/gender", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_gender).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Foaf_img != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "http://xmlns.com/foaf/0.1/img", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Foaf_img).ToLower()}\"", list, " . ");
				}
				if(this.Roh_personalData.Vcard_email != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/personaldata_{ResourceID}_{this.Roh_personalData.ArticleID}",  "https://www.w3.org/2006/vcard/ns#email", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_personalData.Vcard_email).ToLower()}\"", list, " . ");
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
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/cvOf", $"<{itemRegex}>", list, " . ");
				}
				//if(this.IdRoh_gnossUser != null)
				//{
				//	Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
				//	string itemRegex = this.IdRoh_gnossUser;
				//	if (regex.IsMatch(itemRegex))
				//	{
				//		itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
				//	}
				//	else
				//	{
				//		itemRegex = itemRegex.ToLower();
				//	}
				//	AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/gnossUser", $"<{itemRegex}>", list, " . ");
				//}
				if(this.Foaf_name != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://xmlns.com/foaf/0.1/name", $"\"{GenerarTextoSinSaltoDeLinea(this.Foaf_name).ToLower()}\"", list, " . ");
				}
			if (listaSearch != null && listaSearch.Count > 0)
			{
				foreach(string valorSearch in listaSearch)
				{
					search += $"{valorSearch} ";
				}
			}
			if(!string.IsNullOrEmpty(search))
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/search", $"\"{search.ToLower()}\"", list, " . ");
			}
			return list;
		}

		public override KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI)
		{

			//Insert en la tabla Documento
			string titulo = $"{this.Foaf_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
			string descripcion = $"{this.Foaf_name.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
			string tablaDoc = $"'{titulo}', '{descripcion}', '{resourceAPI.GraphsUrl}'";
			KeyValuePair<Guid, string> valor = new KeyValuePair<Guid, string>(ResourceID, tablaDoc);

			return valor;
		}

		protected List<object> ObtenerObjetosDePropiedad(object propiedad)
		{
			List<object> lista = new List<object>();
			if(propiedad is IList)
			{
				foreach (object item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else
			{
				lista.Add(propiedad);
			}
			return lista;
		}
		protected List<string> ObtenerStringDePropiedad(object propiedad)
		{
			List<string> lista = new List<string>();
			if (propiedad is IList)
			{
				foreach (string item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else if (propiedad is IDictionary)
			{
				foreach (object key in ((IDictionary)propiedad).Keys)
				{
					if (((IDictionary)propiedad)[key] is IList)
					{
						List<string> listaValores = (List<string>)((IDictionary)propiedad)[key];
						foreach(string valor in listaValores)
						{
							lista.Add(valor);
						}
					}
					else
					{
					lista.Add((string)((IDictionary)propiedad)[key]);
					}
				}
			}
			else if (propiedad is string)
			{
				lista.Add((string)propiedad);
			}
			return lista;
		}
		public override string GetURI(ResourceApi resourceAPI)
		{
			return $"{resourceAPI.GraphsUrl}items/CurriculumvitaeOntology_{ResourceID}_{ArticleID}";
		}

		private string GenerarTextoSinSaltoDeLinea(string pTexto)
		{
			return pTexto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"");
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Foaf_name;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Foaf_name;
		}

		private void AgregarTripleALista(string pSujeto, string pPredicado, string pObjeto, List<string> pLista, string pDatosExtra)
		{
			if(!string.IsNullOrEmpty(pObjeto) && !pObjeto.Equals("\"\"") && !pObjeto.Equals("<>"))
			{
				pLista.Add($"<{pSujeto}> <{pPredicado}> {pObjeto}{pDatosExtra}");
			} 
		} 

		private void AgregarTags(List<string> pListaTriples)
		{
			foreach(string tag in tagList)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://rdfs.org/sioc/types#Tag", tag.ToLower(), pListaTriples, " . ");
			}
		}


	}
}
