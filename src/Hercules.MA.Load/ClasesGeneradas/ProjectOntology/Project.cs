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
using Organization = OrganizationOntology.Organization;
using GeographicRegion = GeographicRegionOntology.GeographicRegion;
using Modality = ModalityOntology.Modality;

namespace ProjectOntology
{
	public class Project : GnossOCBase
	{

		public Project() : base() { } 

		public Project(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			SemanticPropertyModel propRoh_isSupportedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/isSupportedBy");
			if(propRoh_isSupportedBy != null && propRoh_isSupportedBy.PropertyValues.Count > 0)
			{
				this.Roh_isSupportedBy = new Funding(propRoh_isSupportedBy.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_participates = new List<Organization>();
			SemanticPropertyModel propRoh_participates = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/participates");
			if(propRoh_participates != null && propRoh_participates.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_participates.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Organization roh_participates = new Organization(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_participates.Add(roh_participates);
					}
				}
			}
			SemanticPropertyModel propRoh_conductedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/conductedBy");
			if(propRoh_conductedBy != null && propRoh_conductedBy.PropertyValues.Count > 0)
			{
				this.Roh_conductedBy = new Organization(propRoh_conductedBy.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propVivo_geographicFocus = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#geographicFocus");
			if(propVivo_geographicFocus != null && propVivo_geographicFocus.PropertyValues.Count > 0)
			{
				this.Vivo_geographicFocus = new GeographicRegion(propVivo_geographicFocus.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_grantedBy = new List<Organization>();
			SemanticPropertyModel propRoh_grantedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/grantedBy");
			if(propRoh_grantedBy != null && propRoh_grantedBy.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_grantedBy.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Organization roh_grantedBy = new Organization(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_grantedBy.Add(roh_grantedBy);
					}
				}
			}
			this.Roh_hasResultsProjectClassification = new List<ProjectClassification>();
			SemanticPropertyModel propRoh_hasResultsProjectClassification = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasResultsProjectClassification");
			if(propRoh_hasResultsProjectClassification != null && propRoh_hasResultsProjectClassification.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasResultsProjectClassification.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ProjectClassification roh_hasResultsProjectClassification = new ProjectClassification(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasResultsProjectClassification.Add(roh_hasResultsProjectClassification);
					}
				}
			}
			this.Roh_hasProjectClassification = new List<ProjectClassification>();
			SemanticPropertyModel propRoh_hasProjectClassification = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasProjectClassification");
			if(propRoh_hasProjectClassification != null && propRoh_hasProjectClassification.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasProjectClassification.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ProjectClassification roh_hasProjectClassification = new ProjectClassification(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasProjectClassification.Add(roh_hasProjectClassification);
					}
				}
			}
			this.Vivo_relates = new List<BFO_0000023>();
			SemanticPropertyModel propVivo_relates = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#relates");
			if(propVivo_relates != null && propVivo_relates.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVivo_relates.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						BFO_0000023 vivo_relates = new BFO_0000023(propValue.RelatedEntity,idiomaUsuario);
						this.Vivo_relates.Add(vivo_relates);
					}
				}
			}
			SemanticPropertyModel propRoh_modality = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/modality");
			if(propRoh_modality != null && propRoh_modality.PropertyValues.Count > 0)
			{
				this.Roh_modality = new Modality(propRoh_modality.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_relevantResults = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/relevantResults"));
			this.Roh_peopleYearNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/peopleYearNumber"));
			this.Roh_durationMonths = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationMonths"));
			this.Roh_geographicFocusOther = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/geographicFocusOther"));
			this.Roh_researchersNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/researchersNumber"));
			this.Roh_durationDays = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationDays"));
			this.Vivo_start= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#start"));
			this.Roh_durationYears = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationYears"));
			this.Vivo_end= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#end"));
			this.Roh_monetaryAmount = GetNumberFloatPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/monetaryAmount"));
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
		}

		public Project(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propRoh_isSupportedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/isSupportedBy");
			if(propRoh_isSupportedBy != null && propRoh_isSupportedBy.PropertyValues.Count > 0)
			{
				this.Roh_isSupportedBy = new Funding(propRoh_isSupportedBy.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_participates = new List<Organization>();
			SemanticPropertyModel propRoh_participates = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/participates");
			if(propRoh_participates != null && propRoh_participates.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_participates.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Organization roh_participates = new Organization(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_participates.Add(roh_participates);
					}
				}
			}
			SemanticPropertyModel propRoh_conductedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/conductedBy");
			if(propRoh_conductedBy != null && propRoh_conductedBy.PropertyValues.Count > 0)
			{
				this.Roh_conductedBy = new Organization(propRoh_conductedBy.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propVivo_geographicFocus = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#geographicFocus");
			if(propVivo_geographicFocus != null && propVivo_geographicFocus.PropertyValues.Count > 0)
			{
				this.Vivo_geographicFocus = new GeographicRegion(propVivo_geographicFocus.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_grantedBy = new List<Organization>();
			SemanticPropertyModel propRoh_grantedBy = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/grantedBy");
			if(propRoh_grantedBy != null && propRoh_grantedBy.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_grantedBy.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Organization roh_grantedBy = new Organization(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_grantedBy.Add(roh_grantedBy);
					}
				}
			}
			this.Roh_hasResultsProjectClassification = new List<ProjectClassification>();
			SemanticPropertyModel propRoh_hasResultsProjectClassification = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasResultsProjectClassification");
			if(propRoh_hasResultsProjectClassification != null && propRoh_hasResultsProjectClassification.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasResultsProjectClassification.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ProjectClassification roh_hasResultsProjectClassification = new ProjectClassification(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasResultsProjectClassification.Add(roh_hasResultsProjectClassification);
					}
				}
			}
			this.Roh_hasProjectClassification = new List<ProjectClassification>();
			SemanticPropertyModel propRoh_hasProjectClassification = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasProjectClassification");
			if(propRoh_hasProjectClassification != null && propRoh_hasProjectClassification.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasProjectClassification.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ProjectClassification roh_hasProjectClassification = new ProjectClassification(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasProjectClassification.Add(roh_hasProjectClassification);
					}
				}
			}
			this.Vivo_relates = new List<BFO_0000023>();
			SemanticPropertyModel propVivo_relates = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#relates");
			if(propVivo_relates != null && propVivo_relates.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVivo_relates.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						BFO_0000023 vivo_relates = new BFO_0000023(propValue.RelatedEntity,idiomaUsuario);
						this.Vivo_relates.Add(vivo_relates);
					}
				}
			}
			SemanticPropertyModel propRoh_modality = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/modality");
			if(propRoh_modality != null && propRoh_modality.PropertyValues.Count > 0)
			{
				this.Roh_modality = new Modality(propRoh_modality.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_relevantResults = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/relevantResults"));
			this.Roh_peopleYearNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/peopleYearNumber"));
			this.Roh_durationMonths = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationMonths"));
			this.Roh_geographicFocusOther = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/geographicFocusOther"));
			this.Roh_researchersNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/researchersNumber"));
			this.Roh_durationDays = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationDays"));
			this.Vivo_start= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#start"));
			this.Roh_durationYears = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/durationYears"));
			this.Vivo_end= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#end"));
			this.Roh_monetaryAmount = GetNumberFloatPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/monetaryAmount"));
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
		}

		[LABEL(LanguageEnum.es,"Financiación")]
		[RDFProperty("http://w3id.org/roh/isSupportedBy")]
		public  Funding Roh_isSupportedBy { get; set;}

		[LABEL(LanguageEnum.es,"Entidad/es participante/s")]
		[RDFProperty("http://w3id.org/roh/participates")]
		public  List<Organization> Roh_participates { get; set;}
		public List<string> IdsRoh_participates { get; set;}

		[LABEL(LanguageEnum.es,"Entidad de realización")]
		[RDFProperty("http://w3id.org/roh/conductedBy")]
		public  Organization Roh_conductedBy  { get; set;} 
		public string IdRoh_conductedBy  { get; set;} 

		[LABEL(LanguageEnum.es,"Ámbito geográfico")]
		[RDFProperty("http://vivoweb.org/ontology/core#geographicFocus")]
		public  GeographicRegion Vivo_geographicFocus  { get; set;} 
		public string IdVivo_geographicFocus  { get; set;} 

		[LABEL(LanguageEnum.es,"Entidad/es financiadora/s")]
		[RDFProperty("http://w3id.org/roh/grantedBy")]
		public  List<Organization> Roh_grantedBy { get; set;}
		public List<string> IdsRoh_grantedBy { get; set;}

		[LABEL(LanguageEnum.es,"Resultados de la clasificación del proyecto")]
		[RDFProperty("http://w3id.org/roh/hasResultsProjectClassification")]
		public  List<ProjectClassification> Roh_hasResultsProjectClassification { get; set;}

		[LABEL(LanguageEnum.es,"Clasificación del proyecto")]
		[RDFProperty("http://w3id.org/roh/hasProjectClassification")]
		public  List<ProjectClassification> Roh_hasProjectClassification { get; set;}

		[LABEL(LanguageEnum.es,"Relacionado con")]
		[RDFProperty("http://vivoweb.org/ontology/core#relates")]
		public  List<BFO_0000023> Vivo_relates { get; set;}

		[LABEL(LanguageEnum.es,"Modalidad de proyecto")]
		[RDFProperty("http://w3id.org/roh/modality")]
		public  Modality Roh_modality  { get; set;} 
		public string IdRoh_modality  { get; set;} 

		[LABEL(LanguageEnum.es,"Resultados relevantes")]
		[RDFProperty("http://w3id.org/roh/relevantResults")]
		public  string Roh_relevantResults { get; set;}

		[LABEL(LanguageEnum.es,"Nº de personas/año")]
		[RDFProperty("http://w3id.org/roh/peopleYearNumber")]
		public  int? Roh_peopleYearNumber { get; set;}

		[LABEL(LanguageEnum.es,"Duración (meses)")]
		[RDFProperty("http://w3id.org/roh/durationMonths")]
		public  string Roh_durationMonths { get; set;}

		[LABEL(LanguageEnum.es,"Ámbito geográfico, otros")]
		[RDFProperty("http://w3id.org/roh/geographicFocusOther")]
		public  string Roh_geographicFocusOther { get; set;}

		[LABEL(LanguageEnum.es,"Nº de investigadores/as")]
		[RDFProperty("http://w3id.org/roh/researchersNumber")]
		public  int? Roh_researchersNumber { get; set;}

		[LABEL(LanguageEnum.es,"Duración (días)")]
		[RDFProperty("http://w3id.org/roh/durationDays")]
		public  string Roh_durationDays { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de inicio")]
		[RDFProperty("http://vivoweb.org/ontology/core#start")]
		public  DateTime? Vivo_start { get; set;}

		[LABEL(LanguageEnum.es,"Duración (años)")]
		[RDFProperty("http://w3id.org/roh/durationYears")]
		public  string Roh_durationYears { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de finalización")]
		[RDFProperty("http://vivoweb.org/ontology/core#end")]
		public  DateTime? Vivo_end { get; set;}

		[LABEL(LanguageEnum.es,"Cuantía total")]
		[RDFProperty("http://w3id.org/roh/monetaryAmount")]
		public  float? Roh_monetaryAmount { get; set;}

		[LABEL(LanguageEnum.es,"Nombre del proyecto")]
		[RDFProperty("http://w3id.org/roh/title")]
		public  string Roh_title { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("roh:participates", this.IdsRoh_participates));
			propList.Add(new StringOntologyProperty("roh:conductedBy", this.IdRoh_conductedBy));
			propList.Add(new StringOntologyProperty("vivo:geographicFocus", this.IdVivo_geographicFocus));
			propList.Add(new ListStringOntologyProperty("roh:grantedBy", this.IdsRoh_grantedBy));
			propList.Add(new StringOntologyProperty("roh:modality", this.IdRoh_modality));
			propList.Add(new StringOntologyProperty("roh:relevantResults", this.Roh_relevantResults));
			propList.Add(new StringOntologyProperty("roh:peopleYearNumber", this.Roh_peopleYearNumber.ToString()));
			propList.Add(new StringOntologyProperty("roh:durationMonths", this.Roh_durationMonths));
			propList.Add(new StringOntologyProperty("roh:geographicFocusOther", this.Roh_geographicFocusOther));
			propList.Add(new StringOntologyProperty("roh:researchersNumber", this.Roh_researchersNumber.ToString()));
			propList.Add(new StringOntologyProperty("roh:durationDays", this.Roh_durationDays));
			if (this.Vivo_start.HasValue){
				propList.Add(new DateOntologyProperty("vivo:start", this.Vivo_start.Value));
				}
			propList.Add(new StringOntologyProperty("roh:durationYears", this.Roh_durationYears));
			if (this.Vivo_end.HasValue){
				propList.Add(new DateOntologyProperty("vivo:end", this.Vivo_end.Value));
				}
			propList.Add(new StringOntologyProperty("roh:monetaryAmount", this.Roh_monetaryAmount.ToString()));
			propList.Add(new StringOntologyProperty("roh:title", this.Roh_title));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			if(Roh_isSupportedBy!=null){
				Roh_isSupportedBy.GetProperties();
				Roh_isSupportedBy.GetEntities();
				OntologyEntity entityRoh_isSupportedBy = new OntologyEntity("http://w3id.org/roh/Funding", "http://w3id.org/roh/Funding", "roh:isSupportedBy", Roh_isSupportedBy.propList, Roh_isSupportedBy.entList);
				entList.Add(entityRoh_isSupportedBy);
			}
			if(Roh_hasResultsProjectClassification!=null){
				foreach(ProjectClassification prop in Roh_hasResultsProjectClassification){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityProjectClassification = new OntologyEntity("http://w3id.org/roh/ProjectClassification", "http://w3id.org/roh/ProjectClassification", "roh:hasResultsProjectClassification", prop.propList, prop.entList);
				entList.Add(entityProjectClassification);
				prop.Entity= entityProjectClassification;
				}
			}
			if(Roh_hasProjectClassification!=null){
				foreach(ProjectClassification prop in Roh_hasProjectClassification){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityProjectClassification = new OntologyEntity("http://w3id.org/roh/ProjectClassification", "http://w3id.org/roh/ProjectClassification", "roh:hasProjectClassification", prop.propList, prop.entList);
				entList.Add(entityProjectClassification);
				prop.Entity= entityProjectClassification;
				}
			}
			if(Vivo_relates!=null){
				foreach(BFO_0000023 prop in Vivo_relates){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityBFO_0000023 = new OntologyEntity("http://purl.obolibrary.org/obo/BFO_0000023", "http://purl.obolibrary.org/obo/BFO_0000023", "vivo:relates", prop.propList, prop.entList);
				entList.Add(entityBFO_0000023);
				prop.Entity= entityBFO_0000023;
				}
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
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://vivoweb.org/ontology/core#Project", "http://vivoweb.org/ontology/core#Project", prefList, propList, entList);
			}
			else{
				ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, "http://vivoweb.org/ontology/core#Project", "http://vivoweb.org/ontology/core#Project", prefList, propList, entList,idrecurso,idarticulo);
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
			list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://vivoweb.org/ontology/core#Project> . ");
			list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://vivoweb.org/ontology/core#Project\" . ");
			list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> . ");
			if(this.Roh_isSupportedBy != null)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/Funding> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/Funding\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/isSupportedBy> <{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> . ");
				if(this.Roh_isSupportedBy.Roh_mixedPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/mixedPercentage> {this.Roh_isSupportedBy.Roh_mixedPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Roh_creditPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/creditPercentage> {this.Roh_isSupportedBy.Roh_creditPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Vivo_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://vivoweb.org/ontology/core#identifier> \"{this.Roh_isSupportedBy.Vivo_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_isSupportedBy.Roh_grantsPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/grantsPercentage> {this.Roh_isSupportedBy.Roh_grantsPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Vivo_description != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://vivoweb.org/ontology/core#description> \"{this.Roh_isSupportedBy.Vivo_description.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_isSupportedBy.Roh_monetaryAmount != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/monetaryAmount> {this.Roh_isSupportedBy.Roh_monetaryAmount.Value.ToString(new CultureInfo("en-US"))} . ");
				}
			}
			if(this.Roh_hasResultsProjectClassification != null)
			{
			foreach(var item0 in this.Roh_hasResultsProjectClassification)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/ProjectClassification> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/ProjectClassification\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/hasResultsProjectClassification> <{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdsRoh_projectClassificationNode != null)
				{
					foreach(var item2 in item0.IdsRoh_projectClassificationNode)
					{
						list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/projectClassificationNode> <{item2}> . ");
					}
				}
			}
			}
			if(this.Roh_hasProjectClassification != null)
			{
			foreach(var item0 in this.Roh_hasProjectClassification)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://w3id.org/roh/ProjectClassification> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://w3id.org/roh/ProjectClassification\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/hasProjectClassification> <{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdsRoh_projectClassificationNode != null)
				{
					foreach(var item2 in item0.IdsRoh_projectClassificationNode)
					{
						list.Add($"<{resourceAPI.GraphsUrl}items/ProjectClassification_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/projectClassificationNode> <{item2}> . ");
					}
				}
			}
			}
			if(this.Vivo_relates != null)
			{
			foreach(var item0 in this.Vivo_relates)
			{
				list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <http://purl.obolibrary.org/obo/BFO_0000023> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://www.w3.org/2000/01/rdf-schema#label> \"http://purl.obolibrary.org/obo/BFO_0000023\" . ");
				list.Add($"<{resourceAPI.GraphsUrl}{ResourceID}> <http://gnoss/hasEntidad> <{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> . ");
				list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://vivoweb.org/ontology/core#relates> <{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdRoh_participationType != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/participationType> <{item0.IdRoh_participationType}> . ");
				}
				if(item0.IdRoh_contributionGrade != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/contributionGrade> <{item0.IdRoh_contributionGrade}> . ");
				}
				if(item0.Roh_dedicationRegime != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/dedicationRegime> \"{item0.Roh_dedicationRegime.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Roh_participationTypeOther != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/participationTypeOther> \"{item0.Roh_participationTypeOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Roh_applicantContribution != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/applicantContribution> \"{item0.Roh_applicantContribution.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.Roh_contributionGradeOther != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/contributionGradeOther> \"{item0.Roh_contributionGradeOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(item0.IdRoh_roleOf != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/roleOf> <{item0.IdRoh_roleOf}> . ");
				}
				if(item0.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/order> \"{item0.Roh_order.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			}
			}
				if(this.IdsRoh_participates != null)
				{
					foreach(var item2 in this.IdsRoh_participates)
					{
						list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/participates> <{item2}> . ");
					}
				}
				if(this.IdRoh_conductedBy != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/conductedBy> <{this.IdRoh_conductedBy}> . ");
				}
				if(this.IdVivo_geographicFocus != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://vivoweb.org/ontology/core#geographicFocus> <{this.IdVivo_geographicFocus}> . ");
				}
				if(this.IdsRoh_grantedBy != null)
				{
					foreach(var item2 in this.IdsRoh_grantedBy)
					{
						list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/grantedBy> <{item2}> . ");
					}
				}
				if(this.IdRoh_modality != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/modality> <{this.IdRoh_modality}> . ");
				}
				if(this.Roh_relevantResults != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/relevantResults> \"{this.Roh_relevantResults.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_peopleYearNumber != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/peopleYearNumber> {this.Roh_peopleYearNumber.Value.ToString()} . ");
				}
				if(this.Roh_durationMonths != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/durationMonths> \"{this.Roh_durationMonths.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_geographicFocusOther != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/geographicFocusOther> \"{this.Roh_geographicFocusOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Roh_researchersNumber != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/researchersNumber> {this.Roh_researchersNumber.Value.ToString()} . ");
				}
				if(this.Roh_durationDays != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/durationDays> \"{this.Roh_durationDays.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Vivo_start != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://vivoweb.org/ontology/core#start> \"{this.Vivo_start.Value.ToString("yyyyMMddHHmmss")}\" . ");
				}
				if(this.Roh_durationYears != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/durationYears> \"{this.Roh_durationYears.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
				if(this.Vivo_end != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://vivoweb.org/ontology/core#end> \"{this.Vivo_end.Value.ToString("yyyyMMddHHmmss")}\" . ");
				}
				if(this.Roh_monetaryAmount != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/monetaryAmount> {this.Roh_monetaryAmount.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_title != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}> <http://w3id.org/roh/title> \"{this.Roh_title.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"")}\" . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"Project\" . ");
			list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://gnoss/type> \"http://vivoweb.org/ontology/core#Project\" . ");
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
			if(this.Roh_isSupportedBy != null)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/isSupportedBy> <{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> . ");
				if(this.Roh_isSupportedBy.Roh_mixedPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/mixedPercentage> {this.Roh_isSupportedBy.Roh_mixedPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Roh_creditPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/creditPercentage> {this.Roh_isSupportedBy.Roh_creditPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Vivo_identifier != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://vivoweb.org/ontology/core#identifier> \"{this.Roh_isSupportedBy.Vivo_identifier.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_isSupportedBy.Roh_grantsPercentage != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/grantsPercentage> {this.Roh_isSupportedBy.Roh_grantsPercentage.Value.ToString(new CultureInfo("en-US"))} . ");
				}
				if(this.Roh_isSupportedBy.Vivo_description != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://vivoweb.org/ontology/core#description> \"{this.Roh_isSupportedBy.Vivo_description.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_isSupportedBy.Roh_monetaryAmount != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/funding_{ResourceID}_{this.Roh_isSupportedBy.ArticleID}> <http://w3id.org/roh/monetaryAmount> {this.Roh_isSupportedBy.Roh_monetaryAmount.Value.ToString(new CultureInfo("en-US"))} . ");
				}
			}
			if(this.Roh_hasResultsProjectClassification != null)
			{
			foreach(var item0 in this.Roh_hasResultsProjectClassification)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/hasResultsProjectClassification> <{resourceAPI.GraphsUrl}items/projectclassification_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdsRoh_projectClassificationNode != null)
				{
					foreach(var item2 in item0.IdsRoh_projectClassificationNode)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						list.Add($"<{resourceAPI.GraphsUrl}items/projectclassification_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/projectClassificationNode> <{itemRegex}> . ");
					}
				}
			}
			}
			if(this.Roh_hasProjectClassification != null)
			{
			foreach(var item0 in this.Roh_hasProjectClassification)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/hasProjectClassification> <{resourceAPI.GraphsUrl}items/projectclassification_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdsRoh_projectClassificationNode != null)
				{
					foreach(var item2 in item0.IdsRoh_projectClassificationNode)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						list.Add($"<{resourceAPI.GraphsUrl}items/projectclassification_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/projectClassificationNode> <{itemRegex}> . ");
					}
				}
			}
			}
			if(this.Vivo_relates != null)
			{
			foreach(var item0 in this.Vivo_relates)
			{
				list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://vivoweb.org/ontology/core#relates> <{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> . ");
				if(item0.IdRoh_participationType != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_participationType;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/participationType> <{itemRegex}> . ");
				}
				if(item0.IdRoh_contributionGrade != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_contributionGrade;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/contributionGrade> <{itemRegex}> . ");
				}
				if(item0.Roh_dedicationRegime != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/dedicationRegime> \"{item0.Roh_dedicationRegime.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Roh_participationTypeOther != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/participationTypeOther> \"{item0.Roh_participationTypeOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Roh_applicantContribution != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/applicantContribution> \"{item0.Roh_applicantContribution.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.Roh_contributionGradeOther != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/contributionGradeOther> \"{item0.Roh_contributionGradeOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(item0.IdRoh_roleOf != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_roleOf;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/roleOf> <{itemRegex}> . ");
				}
				if(item0.Roh_order != null)
				{
					list.Add($"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}> <http://w3id.org/roh/order> \"{item0.Roh_order.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
			}
			}
				if(this.IdsRoh_participates != null)
				{
					foreach(var item2 in this.IdsRoh_participates)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/participates> <{itemRegex}> . ");
					}
				}
				if(this.IdRoh_conductedBy != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdRoh_conductedBy;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/conductedBy> <{itemRegex}> . ");
				}
				if(this.IdVivo_geographicFocus != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdVivo_geographicFocus;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://vivoweb.org/ontology/core#geographicFocus> <{itemRegex}> . ");
				}
				if(this.IdsRoh_grantedBy != null)
				{
					foreach(var item2 in this.IdsRoh_grantedBy)
					{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item2;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
						list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/grantedBy> <{itemRegex}> . ");
					}
				}
				if(this.IdRoh_modality != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdRoh_modality;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/modality> <{itemRegex}> . ");
				}
				if(this.Roh_relevantResults != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/relevantResults> \"{this.Roh_relevantResults.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_peopleYearNumber != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/peopleYearNumber> {this.Roh_peopleYearNumber.Value.ToString()} . ");
				}
				if(this.Roh_durationMonths != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/durationMonths> \"{this.Roh_durationMonths.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_geographicFocusOther != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/geographicFocusOther> \"{this.Roh_geographicFocusOther.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Roh_researchersNumber != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/researchersNumber> {this.Roh_researchersNumber.Value.ToString()} . ");
				}
				if(this.Roh_durationDays != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/durationDays> \"{this.Roh_durationDays.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Vivo_start != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://vivoweb.org/ontology/core#start> {this.Vivo_start.Value.ToString("yyyyMMddHHmmss")} . ");
				}
				if(this.Roh_durationYears != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/durationYears> \"{this.Roh_durationYears.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"").ToLower()}\" . ");
				}
				if(this.Vivo_end != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://vivoweb.org/ontology/core#end> {this.Vivo_end.Value.ToString("yyyyMMddHHmmss")} . ");
				}
				if(this.Roh_monetaryAmount != null)
				{
					list.Add($"<http://gnoss/{ResourceID.ToString().ToUpper()}> <http://w3id.org/roh/monetaryAmount> {this.Roh_monetaryAmount.Value.ToString(new CultureInfo("en-US"))} . ");
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
			return $"{resourceAPI.GraphsUrl}items/ProjectOntology_{ResourceID}_{ArticleID}";
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
