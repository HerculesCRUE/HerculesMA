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
using SupportType = SupporttypeOntology.SupportType;
using Language = LanguageOntology.Language;
using PublicationType = PublicationtypeOntology.PublicationType;
using Project = ProjectOntology.Project;

namespace DocumentOntology
{
	public class Document : GnossOCBase
	{

		public Document() : base() { } 

		public Document(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;
			this.Roh_dataAuthor = new List<DataAuthor>();
			SemanticPropertyModel propRoh_dataAuthor = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/dataAuthor");
			if(propRoh_dataAuthor != null && propRoh_dataAuthor.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_dataAuthor.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						DataAuthor roh_dataAuthor = new DataAuthor(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_dataAuthor.Add(roh_dataAuthor);
					}
				}
			}
			SemanticPropertyModel propRoh_supportType = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/supportType");
			if(propRoh_supportType != null && propRoh_supportType.PropertyValues.Count > 0)
			{
				this.Roh_supportType = new SupportType(propRoh_supportType.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propBibo_identifier = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/identifier");
			if(propBibo_identifier != null && propBibo_identifier.PropertyValues.Count > 0)
			{
				this.Bibo_identifier = new Document(propBibo_identifier.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vcard_hasLanguage = new List<Language>();
			SemanticPropertyModel propVcard_hasLanguage = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasLanguage");
			if(propVcard_hasLanguage != null && propVcard_hasLanguage.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVcard_hasLanguage.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Language vcard_hasLanguage = new Language(propValue.RelatedEntity,idiomaUsuario);
						this.Vcard_hasLanguage.Add(vcard_hasLanguage);
					}
				}
			}
			this.Bibo_authorList = new List<BFO_0000023>();
			SemanticPropertyModel propBibo_authorList = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/authorList");
			if(propBibo_authorList != null && propBibo_authorList.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propBibo_authorList.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						BFO_0000023 bibo_authorList = new BFO_0000023(propValue.RelatedEntity,idiomaUsuario);
						this.Bibo_authorList.Add(bibo_authorList);
					}
				}
			}
			this.Roh_impactIndex = new List<ImpactIndex>();
			SemanticPropertyModel propRoh_impactIndex = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/impactIndex");
			if(propRoh_impactIndex != null && propRoh_impactIndex.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_impactIndex.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ImpactIndex roh_impactIndex = new ImpactIndex(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_impactIndex.Add(roh_impactIndex);
					}
				}
			}
			SemanticPropertyModel propVcard_address = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#address");
			if(propVcard_address != null && propVcard_address.PropertyValues.Count > 0)
			{
				this.Vcard_address = new Address(propVcard_address.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_hasMetric = new List<PublicationMetric>();
			SemanticPropertyModel propRoh_hasMetric = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasMetric");
			if(propRoh_hasMetric != null && propRoh_hasMetric.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasMetric.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						PublicationMetric roh_hasMetric = new PublicationMetric(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasMetric.Add(roh_hasMetric);
					}
				}
			}
			SemanticPropertyModel propBibo_presentedAt = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/presentedAt");
			if(propBibo_presentedAt != null && propBibo_presentedAt.PropertyValues.Count > 0)
			{
				this.Bibo_presentedAt = new Event(propBibo_presentedAt.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_hasKnowledgeArea = new List<CategoryPath>();
			SemanticPropertyModel propRoh_hasKnowledgeArea = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasKnowledgeArea");
			if(propRoh_hasKnowledgeArea != null && propRoh_hasKnowledgeArea.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasKnowledgeArea.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						CategoryPath roh_hasKnowledgeArea = new CategoryPath(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasKnowledgeArea.Add(roh_hasKnowledgeArea);
					}
				}
			}
			this.Roh_legalDeposit = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/legalDeposit"));
			this.Roh_publicationTitle = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/publicationTitle"));
			this.Roh_authorsNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/authorsNumber"));
			SemanticPropertyModel propVivo_freeTextKeyword = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#freeTextKeyword");
			this.Vivo_freeTextKeyword = new List<string>();
			if (propVivo_freeTextKeyword != null && propVivo_freeTextKeyword.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVivo_freeTextKeyword.PropertyValues)
				{
					this.Vivo_freeTextKeyword.Add(propValue.Value);
				}
			}
			this.Bibo_isbn = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/isbn"));
			this.Bibo_issue = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/issue"));
			this.Bibo_volume = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/volume"));
			this.Bibo_editor = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/editor"));
			this.Dct_issued= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/terms/issued"));
			this.Bibo_issn = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/issn"));
			this.Bibo_pageStart = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/pageStart"));
			this.Roh_congressProceedingsPublication= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/congressProceedingsPublication"));
			this.Roh_crisIdentifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/crisIdentifier"));
			this.Vcard_url = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#url"));
			this.Bibo_pageEnd = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/pageEnd"));
			this.Roh_collection = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/collection"));
			this.Roh_isRelevantPublication= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/isRelevantPublication"));
			this.Vivo_hasPublicationVenue = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#hasPublicationVenue"));
			SemanticPropertyModel propDc_type = pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/type");
			if(propDc_type != null && propDc_type.PropertyValues.Count > 0)
			{
				this.Dc_type = new PublicationType(propDc_type.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
			SemanticPropertyModel propRoh_project = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/project");
			if (propRoh_project != null && propRoh_project.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_project.PropertyValues)
				{
					if (propValue.RelatedEntity != null)
					{
						Project roh_project = new Project(propValue.RelatedEntity, idiomaUsuario);
						this.Roh_project.Add(roh_project);
					}
				}
			}
		}

		public Document(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Roh_dataAuthor = new List<DataAuthor>();
			SemanticPropertyModel propRoh_dataAuthor = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/dataAuthor");
			if(propRoh_dataAuthor != null && propRoh_dataAuthor.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_dataAuthor.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						DataAuthor roh_dataAuthor = new DataAuthor(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_dataAuthor.Add(roh_dataAuthor);
					}
				}
			}
			SemanticPropertyModel propRoh_supportType = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/supportType");
			if(propRoh_supportType != null && propRoh_supportType.PropertyValues.Count > 0)
			{
				this.Roh_supportType = new SupportType(propRoh_supportType.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propBibo_identifier = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/identifier");
			if(propBibo_identifier != null && propBibo_identifier.PropertyValues.Count > 0)
			{
				this.Bibo_identifier = new Document(propBibo_identifier.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vcard_hasLanguage = new List<Language>();
			SemanticPropertyModel propVcard_hasLanguage = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasLanguage");
			if(propVcard_hasLanguage != null && propVcard_hasLanguage.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVcard_hasLanguage.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Language vcard_hasLanguage = new Language(propValue.RelatedEntity,idiomaUsuario);
						this.Vcard_hasLanguage.Add(vcard_hasLanguage);
					}
				}
			}
			this.Bibo_authorList = new List<BFO_0000023>();
			SemanticPropertyModel propBibo_authorList = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/authorList");
			if(propBibo_authorList != null && propBibo_authorList.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propBibo_authorList.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						BFO_0000023 bibo_authorList = new BFO_0000023(propValue.RelatedEntity,idiomaUsuario);
						this.Bibo_authorList.Add(bibo_authorList);
					}
				}
			}
			this.Roh_impactIndex = new List<ImpactIndex>();
			SemanticPropertyModel propRoh_impactIndex = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/impactIndex");
			if(propRoh_impactIndex != null && propRoh_impactIndex.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_impactIndex.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						ImpactIndex roh_impactIndex = new ImpactIndex(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_impactIndex.Add(roh_impactIndex);
					}
				}
			}
			SemanticPropertyModel propVcard_address = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#address");
			if(propVcard_address != null && propVcard_address.PropertyValues.Count > 0)
			{
				this.Vcard_address = new Address(propVcard_address.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_hasMetric = new List<PublicationMetric>();
			SemanticPropertyModel propRoh_hasMetric = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasMetric");
			if(propRoh_hasMetric != null && propRoh_hasMetric.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasMetric.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						PublicationMetric roh_hasMetric = new PublicationMetric(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasMetric.Add(roh_hasMetric);
					}
				}
			}
			SemanticPropertyModel propBibo_presentedAt = pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/presentedAt");
			if(propBibo_presentedAt != null && propBibo_presentedAt.PropertyValues.Count > 0)
			{
				this.Bibo_presentedAt = new Event(propBibo_presentedAt.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_hasKnowledgeArea = new List<CategoryPath>();
			SemanticPropertyModel propRoh_hasKnowledgeArea = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasKnowledgeArea");
			if(propRoh_hasKnowledgeArea != null && propRoh_hasKnowledgeArea.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_hasKnowledgeArea.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						CategoryPath roh_hasKnowledgeArea = new CategoryPath(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_hasKnowledgeArea.Add(roh_hasKnowledgeArea);
					}
				}
			}
			this.Roh_legalDeposit = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/legalDeposit"));
			this.Roh_publicationTitle = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/publicationTitle"));
			this.Roh_authorsNumber = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/authorsNumber"));
			SemanticPropertyModel propVivo_freeTextKeyword = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#freeTextKeyword");
			this.Vivo_freeTextKeyword = new List<string>();
			if (propVivo_freeTextKeyword != null && propVivo_freeTextKeyword.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVivo_freeTextKeyword.PropertyValues)
				{
					this.Vivo_freeTextKeyword.Add(propValue.Value);
				}
			}
			this.Bibo_isbn = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/isbn"));
			this.Bibo_issue = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/issue"));
			this.Bibo_volume = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/volume"));
			this.Bibo_editor = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/editor"));
			this.Dct_issued= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/terms/issued"));
			this.Bibo_issn = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/issn"));
			this.Bibo_pageStart = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/pageStart"));
			this.Roh_congressProceedingsPublication= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/congressProceedingsPublication"));
			this.Roh_crisIdentifier = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/crisIdentifier"));
			this.Vcard_url = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#url"));
			this.Bibo_pageEnd = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/ontology/bibo/pageEnd"));
			this.Roh_collection = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/collection"));
			this.Roh_isRelevantPublication= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/isRelevantPublication"));
			this.Vivo_hasPublicationVenue = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#hasPublicationVenue"));
			SemanticPropertyModel propDc_type = pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/type");
			if(propDc_type != null && propDc_type.PropertyValues.Count > 0)
			{
				this.Dc_type = new PublicationType(propDc_type.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/title"));
			this.Roh_project = new List<Project>();
			SemanticPropertyModel propRoh_project = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/project");
			if (propRoh_project != null && propRoh_project.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_project.PropertyValues)
				{
					if (propValue.RelatedEntity != null)
					{
						Project roh_project = new Project(propValue.RelatedEntity, idiomaUsuario);
						this.Roh_project.Add(roh_project);
					}
				}
			}
		}

		public virtual string RdfType { get { return "http://purl.org/ontology/bibo/Document"; } }
		public virtual string RdfsLabel { get { return "http://purl.org/ontology/bibo/Document"; } }
		[RDFProperty("http://w3id.org/roh/dataAuthor")]
		public  List<DataAuthor> Roh_dataAuthor { get; set;}

		[LABEL(LanguageEnum.es,"Tipo de soporte")]
		[RDFProperty("http://w3id.org/roh/supportType")]
		public  SupportType Roh_supportType  { get; set;} 
		public string IdRoh_supportType  { get; set;} 

		[LABEL(LanguageEnum.es,"Identificador de la publicación")]
		[RDFProperty("http://purl.org/ontology/bibo/identifier")]
		public  Document Bibo_identifier { get; set;}

		[LABEL(LanguageEnum.es,"Traducciones")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#hasLanguage")]
		public  List<Language> Vcard_hasLanguage { get; set;}
		public List<string> IdsVcard_hasLanguage { get; set;}

		[LABEL(LanguageEnum.es,"Lista de autores")]
		[RDFProperty("http://purl.org/ontology/bibo/authorList")]
		public  List<BFO_0000023> Bibo_authorList { get; set;}

		[LABEL(LanguageEnum.es,"Índice de impacto")]
		[RDFProperty("http://w3id.org/roh/impactIndex")]
		public  List<ImpactIndex> Roh_impactIndex { get; set;}

		[LABEL(LanguageEnum.es,"Lugar de publicación")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#address")]
		public  Address Vcard_address { get; set;}

		[LABEL(LanguageEnum.es,"Métrica")]
		[RDFProperty("http://w3id.org/roh/hasMetric")]
		public  List<PublicationMetric> Roh_hasMetric { get; set;}

		[LABEL(LanguageEnum.es,"Presentado en")]
		[RDFProperty("http://purl.org/ontology/bibo/presentedAt")]
		public  Event Bibo_presentedAt { get; set;}

		[LABEL(LanguageEnum.es,"")]
		[RDFProperty("http://w3id.org/roh/hasKnowledgeArea")]
		public  List<CategoryPath> Roh_hasKnowledgeArea { get; set;}

		[LABEL(LanguageEnum.es,"Depósito legal")]
		[RDFProperty("http://w3id.org/roh/legalDeposit")]
		public  string Roh_legalDeposit { get; set;}

		[LABEL(LanguageEnum.es,"Título de la publicación")]
		[RDFProperty("http://w3id.org/roh/publicationTitle")]
		public  string Roh_publicationTitle { get; set;}

		[LABEL(LanguageEnum.es,"Número de autores")]
		[RDFProperty("http://w3id.org/roh/authorsNumber")]
		public  int? Roh_authorsNumber { get; set;}

		[LABEL(LanguageEnum.es,"Etiquetas")]
		[RDFProperty("http://vivoweb.org/ontology/core#freeTextKeyword")]
		public  List<string> Vivo_freeTextKeyword { get; set;}

		[LABEL(LanguageEnum.es,"ISBN")]
		[RDFProperty("http://purl.org/ontology/bibo/isbn")]
		public  string Bibo_isbn { get; set;}

		[LABEL(LanguageEnum.es,"Número")]
		[RDFProperty("http://purl.org/ontology/bibo/issue")]
		public  string Bibo_issue { get; set;}

		[LABEL(LanguageEnum.es,"Volumen")]
		[RDFProperty("http://purl.org/ontology/bibo/volume")]
		public  string Bibo_volume { get; set;}

		[LABEL(LanguageEnum.es,"Editorial")]
		[RDFProperty("http://purl.org/ontology/bibo/editor")]
		public  string Bibo_editor { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de publicación")]
		[RDFProperty("http://purl.org/dc/terms/issued")]
		public  DateTime? Dct_issued { get; set;}

		[LABEL(LanguageEnum.es,"ISSN")]
		[RDFProperty("http://purl.org/ontology/bibo/issn")]
		public  string Bibo_issn { get; set;}

		[LABEL(LanguageEnum.es,"Página inicial")]
		[RDFProperty("http://purl.org/ontology/bibo/pageStart")]
		public  int? Bibo_pageStart { get; set;}

		[LABEL(LanguageEnum.es,"Publicación en acta congreso")]
		[RDFProperty("http://w3id.org/roh/congressProceedingsPublication")]
		public  bool Roh_congressProceedingsPublication { get; set;}

		[RDFProperty("http://w3id.org/roh/crisIdentifier")]
		public  string Roh_crisIdentifier { get; set;}

		[LABEL(LanguageEnum.es,"URL")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#url")]
		public  string Vcard_url { get; set;}

		[LABEL(LanguageEnum.es,"Página final")]
		[RDFProperty("http://purl.org/ontology/bibo/pageEnd")]
		public  int? Bibo_pageEnd { get; set;}

		[LABEL(LanguageEnum.es,"Colección")]
		[RDFProperty("http://w3id.org/roh/collection")]
		public  string Roh_collection { get; set;}

		[RDFProperty("http://w3id.org/roh/isRelevantPublication")]
		public  bool Roh_isRelevantPublication { get; set;}

		[LABEL(LanguageEnum.es,"Publicado en")]
		[RDFProperty("http://vivoweb.org/ontology/core#hasPublicationVenue")]
		public  string Vivo_hasPublicationVenue { get; set;}

		[LABEL(LanguageEnum.es,"Tipo de producción")]
		[RDFProperty("http://purl.org/dc/elements/1.1/type")]
		[Required]
		public  PublicationType Dc_type  { get; set;} 
		public string IdDc_type  { get; set;} 

		[LABEL(LanguageEnum.es,"Título de la publicación")]
		[RDFProperty("http://w3id.org/roh/title")]
		public  string Roh_title { get; set;}

		[LABEL(LanguageEnum.es, "Proyectos participantes")]
		[RDFProperty("http://w3id.org/roh/project")]
		public List<Project> Roh_project { get; set; }
		public List<string> IdsRoh_project { get; set; }

		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("roh:project", this.IdsRoh_project));
			propList.Add(new StringOntologyProperty("roh:supportType", this.IdRoh_supportType));
			propList.Add(new ListStringOntologyProperty("vcard:hasLanguage", this.IdsVcard_hasLanguage));
			propList.Add(new StringOntologyProperty("roh:legalDeposit", this.Roh_legalDeposit));
			propList.Add(new StringOntologyProperty("roh:publicationTitle", this.Roh_publicationTitle));
			propList.Add(new StringOntologyProperty("roh:authorsNumber", this.Roh_authorsNumber.ToString()));
			propList.Add(new ListStringOntologyProperty("vivo:freeTextKeyword", this.Vivo_freeTextKeyword));
			propList.Add(new StringOntologyProperty("bibo:isbn", this.Bibo_isbn));
			propList.Add(new StringOntologyProperty("bibo:issue", this.Bibo_issue));
			propList.Add(new StringOntologyProperty("bibo:volume", this.Bibo_volume));
			propList.Add(new StringOntologyProperty("bibo:editor", this.Bibo_editor));
			if (this.Dct_issued.HasValue){
				propList.Add(new DateOntologyProperty("dct:issued", this.Dct_issued.Value));
				}
			propList.Add(new StringOntologyProperty("bibo:issn", this.Bibo_issn));
			propList.Add(new StringOntologyProperty("bibo:pageStart", this.Bibo_pageStart.ToString()));
			propList.Add(new BoolOntologyProperty("roh:congressProceedingsPublication", this.Roh_congressProceedingsPublication));
			propList.Add(new StringOntologyProperty("roh:crisIdentifier", this.Roh_crisIdentifier));
			propList.Add(new StringOntologyProperty("vcard:url", this.Vcard_url));
			propList.Add(new StringOntologyProperty("bibo:pageEnd", this.Bibo_pageEnd.ToString()));
			propList.Add(new StringOntologyProperty("roh:collection", this.Roh_collection));
			propList.Add(new BoolOntologyProperty("roh:isRelevantPublication", this.Roh_isRelevantPublication));
			propList.Add(new StringOntologyProperty("vivo:hasPublicationVenue", this.Vivo_hasPublicationVenue));
			propList.Add(new StringOntologyProperty("dc:type", this.IdDc_type));
			propList.Add(new StringOntologyProperty("roh:title", this.Roh_title));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Roh_dataAuthor!=null){
				foreach(DataAuthor prop in Roh_dataAuthor){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityDataAuthor = new OntologyEntity("http://w3id.org/roh/DataAuthor", "http://w3id.org/roh/DataAuthor", "roh:dataAuthor", prop.propList, prop.entList);
				entList.Add(entityDataAuthor);
				prop.Entity= entityDataAuthor;
				}
			}
			if(Bibo_identifier!=null){
				Bibo_identifier.GetProperties();
				Bibo_identifier.GetEntities();
				OntologyEntity entityBibo_identifier = new OntologyEntity("http://xmlns.com/foaf/0.1/Document", "http://xmlns.com/foaf/0.1/Document", "bibo:identifier", Bibo_identifier.propList, Bibo_identifier.entList);
				entList.Add(entityBibo_identifier);
			}
			if(Bibo_authorList!=null){
				foreach(BFO_0000023 prop in Bibo_authorList){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityBFO_0000023 = new OntologyEntity("http://purl.obolibrary.org/obo/BFO_0000023", "http://purl.obolibrary.org/obo/BFO_0000023", "bibo:authorList", prop.propList, prop.entList);
				entList.Add(entityBFO_0000023);
				prop.Entity= entityBFO_0000023;
				}
			}
			if(Roh_impactIndex!=null){
				foreach(ImpactIndex prop in Roh_impactIndex){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityImpactIndex = new OntologyEntity("http://w3id.org/roh/ImpactIndex", "http://w3id.org/roh/ImpactIndex", "roh:impactIndex", prop.propList, prop.entList);
				entList.Add(entityImpactIndex);
				prop.Entity= entityImpactIndex;
				}
			}
			if(Vcard_address!=null){
				Vcard_address.GetProperties();
				Vcard_address.GetEntities();
				OntologyEntity entityVcard_address = new OntologyEntity("https://www.w3.org/2006/vcard/ns#Address", "https://www.w3.org/2006/vcard/ns#Address", "vcard:address", Vcard_address.propList, Vcard_address.entList);
				entList.Add(entityVcard_address);
			}
			if(Roh_hasMetric!=null){
				foreach(PublicationMetric prop in Roh_hasMetric){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityPublicationMetric = new OntologyEntity("http://w3id.org/roh/PublicationMetric", "http://w3id.org/roh/PublicationMetric", "roh:hasMetric", prop.propList, prop.entList);
				entList.Add(entityPublicationMetric);
				prop.Entity= entityPublicationMetric;
				}
			}
			if(Bibo_presentedAt!=null){
				Bibo_presentedAt.GetProperties();
				Bibo_presentedAt.GetEntities();
				OntologyEntity entityBibo_presentedAt = new OntologyEntity("http://purl.org/ontology/bibo/Event", "http://purl.org/ontology/bibo/Event", "bibo:presentedAt", Bibo_presentedAt.propList, Bibo_presentedAt.entList);
				entList.Add(entityBibo_presentedAt);
			}
			if(Roh_hasKnowledgeArea!=null){
				foreach(CategoryPath prop in Roh_hasKnowledgeArea){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityCategoryPath = new OntologyEntity("http://w3id.org/roh/CategoryPath", "http://w3id.org/roh/CategoryPath", "roh:hasKnowledgeArea", prop.propList, prop.entList);
				entList.Add(entityCategoryPath);
				prop.Entity= entityCategoryPath;
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
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://purl.org/ontology/bibo/Document>", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://purl.org/ontology/bibo/Document\"", list, " . ");
			AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}>", list, " . ");
			if(this.Roh_dataAuthor != null)
			{
			foreach(var item0 in this.Roh_dataAuthor)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/DataAuthor>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/DataAuthor\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://w3id.org/roh/dataAuthor", $"<{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdRoh_motivatedBy != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/motivatedBy", $"<{item0.IdRoh_motivatedBy}>", list, " . ");
				}
				if(item0.IdRoh_contributionGrade != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/contributionGrade", $"<{item0.IdRoh_contributionGrade}>", list, " . ");
				}
				if(item0.Roh_relevantResults != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/relevantResults", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_relevantResults)}\"", list, " . ");
				}
				if(item0.Roh_correspondingAuthor != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/correspondingAuthor", $"\"{item0.Roh_correspondingAuthor.ToString()}\"", list, " . ");
				}
				if(item0.Roh_relevantPublication != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/relevantPublication", $"\"{item0.Roh_relevantPublication.ToString()}\"", list, " . ");
				}
				if(item0.Roh_motivatedByOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/motivatedByOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_motivatedByOther)}\"", list, " . ");
				}
				if(item0.IdRdf_member != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/DataAuthor_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#member", $"<{item0.IdRdf_member}>", list, " . ");
				}
			}
			}
			if (this.IdsRoh_project != null)
			{
				foreach (var item2 in this.IdsRoh_project)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Project_{ResourceID}_{ArticleID}", "http://w3id.org/roh/project", $"<{item2}>", list, " . ");
				}
			}
			if (this.Bibo_identifier != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://xmlns.com/foaf/0.1/Document>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://xmlns.com/foaf/0.1/Document\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://purl.org/ontology/bibo/identifier", $"<{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}>", list, " . ");
				//if(this.Bibo_identifier.Foaf_topic != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Foaf_topic)}\"", list, " . ");
				//}
				//if(this.Bibo_identifier.Dc_title != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Dc_title)}\"", list, " . ");
				//}
				//if(this.Bibo_identifier.Foaf_primaryTopic != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Foaf_primaryTopic)}\"", list, " . ");
				//}
			}
			if(this.Bibo_authorList != null)
			{
			foreach(var item0 in this.Bibo_authorList)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://purl.obolibrary.org/obo/BFO_0000023>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://purl.obolibrary.org/obo/BFO_0000023\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://purl.org/ontology/bibo/authorList", $"<{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Foaf_nick != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}",  "http://xmlns.com/foaf/0.1/nick", $"\"{GenerarTextoSinSaltoDeLinea(item0.Foaf_nick)}\"", list, " . ");
				}
				if(item0.IdRdf_member != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#member", $"<{item0.IdRdf_member}>", list, " . ");
				}
				if(item0.Rdf_comment != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/BFO_0000023_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#comment", $"{item0.Rdf_comment.ToString()}", list, " . ");
				}
			}
			}
			if(this.Roh_impactIndex != null)
			{
			foreach(var item0 in this.Roh_impactIndex)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/ImpactIndex>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/ImpactIndex\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://w3id.org/roh/impactIndex", $"<{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdRoh_impactCategory != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactCategory", $"<{item0.IdRoh_impactCategory}>", list, " . ");
				}
				if(item0.Roh_impactIndexInYear != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactIndexInYear", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_impactIndexInYear)}\"", list, " . ");
				}
				if(item0.Roh_impactSourceOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactSourceOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_impactSourceOther)}\"", list, " . ");
				}
				if(item0.Roh_journalNumberInCat != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/journalNumberInCat", $"{item0.Roh_journalNumberInCat.Value.ToString()}", list, " . ");
				}
				if(item0.Roh_publicationPosition != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/publicationPosition", $"{item0.Roh_publicationPosition.Value.ToString()}", list, " . ");
				}
				if(item0.Roh_journalTop25 != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/journalTop25", $"\"{item0.Roh_journalTop25.ToString()}\"", list, " . ");
				}
				if(item0.IdRoh_impactSource != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/ImpactIndex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactSource", $"<{item0.IdRoh_impactSource}>", list, " . ");
				}
			}
			}
			if(this.Vcard_address != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#Address>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#Address\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}>", list, " . ");
				if(this.Vcard_address.IdVcard_hasCountryName != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{this.Vcard_address.IdVcard_hasCountryName}>", list, " . ");
				}
				if(this.Vcard_address.IdVcard_hasRegion != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{this.Vcard_address.IdVcard_hasRegion}>", list, " . ");
				}
				if(this.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Vcard_address.Vcard_locality)}\"", list, " . ");
				}
			}
			if(this.Roh_hasMetric != null)
			{
			foreach(var item0 in this.Roh_hasMetric)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/PublicationMetric>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/PublicationMetric\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://w3id.org/roh/hasMetric", $"<{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Roh_metricNameOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/metricNameOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_metricNameOther)}\"", list, " . ");
				}
				if(item0.IdRoh_metricName != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/metricName", $"<{item0.IdRoh_metricName}>", list, " . ");
				}
				if(item0.Roh_citationCount != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/PublicationMetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/citationCount", $"{item0.Roh_citationCount.ToString()}", list, " . ");
				}
			}
			}
			if(this.Bibo_presentedAt != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://purl.org/ontology/bibo/Event>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://purl.org/ontology/bibo/Event\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://purl.org/ontology/bibo/presentedAt", $"<{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}>", list, " . ");
			if(this.Bibo_presentedAt.Vcard_address != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<https://www.w3.org/2006/vcard/ns#Address>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"https://www.w3.org/2006/vcard/ns#Address\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}>", list, " . ");
				if(this.Bibo_presentedAt.Vcard_address.IdVcard_hasCountryName != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{this.Bibo_presentedAt.Vcard_address.IdVcard_hasCountryName}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Vcard_address.IdVcard_hasRegion != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{this.Bibo_presentedAt.Vcard_address.IdVcard_hasRegion}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Vcard_address.Vcard_locality)}\"", list, " . ");
				}
			}
				if(this.Bibo_presentedAt.IdBibo_organizer != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/ontology/bibo/organizer", $"<{this.Bibo_presentedAt.IdBibo_organizer}>", list, " . ");
				}
				if(this.Bibo_presentedAt.IdVivo_geographicFocus != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#geographicFocus", $"<{this.Bibo_presentedAt.IdVivo_geographicFocus}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Roh_geographicFocusOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://w3id.org/roh/geographicFocusOther", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Roh_geographicFocusOther)}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Vivo_start != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#start", $"\"{this.Bibo_presentedAt.Vivo_start.Value.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Roh_withExternalAdmissionsCommittee != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://w3id.org/roh/withExternalAdmissionsCommittee", $"\"{this.Bibo_presentedAt.Roh_withExternalAdmissionsCommittee.ToString()}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Vivo_end != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#end", $"\"{this.Bibo_presentedAt.Vivo_end.Value.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Dc_title)}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.IdDc_type != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/dc/elements/1.1/type", $"<{this.Bibo_presentedAt.IdDc_type}>", list, " . ");
				}
			}
			if(this.Roh_hasKnowledgeArea != null)
			{
			foreach(var item0 in this.Roh_hasKnowledgeArea)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CategoryPath_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"<http://w3id.org/roh/CategoryPath>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CategoryPath_{ResourceID}_{item0.ArticleID}", "http://www.w3.org/2000/01/rdf-schema#label", $"\"http://w3id.org/roh/CategoryPath\"", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}{ResourceID}", "http://gnoss/hasEntidad", $"<{resourceAPI.GraphsUrl}items/CategoryPath_{ResourceID}_{item0.ArticleID}>", list, " . ");
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://w3id.org/roh/hasKnowledgeArea", $"<{resourceAPI.GraphsUrl}items/CategoryPath_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdsRoh_categoryNode != null)
				{
					foreach(var item2 in item0.IdsRoh_categoryNode)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/CategoryPath_{ResourceID}_{item0.ArticleID}", "http://w3id.org/roh/categoryNode", $"<{item2}>", list, " . ");
					}
				}
			}
			}
				if(this.IdRoh_supportType != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/supportType", $"<{this.IdRoh_supportType}>", list, " . ");
				}
				if(this.IdsVcard_hasLanguage != null)
				{
					foreach(var item2 in this.IdsVcard_hasLanguage)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "https://www.w3.org/2006/vcard/ns#hasLanguage", $"<{item2}>", list, " . ");
					}
				}
				if(this.Roh_legalDeposit != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/legalDeposit", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_legalDeposit)}\"", list, " . ");
				}
				if(this.Roh_publicationTitle != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/publicationTitle", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_publicationTitle)}\"", list, " . ");
				}
				if(this.Roh_authorsNumber != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/authorsNumber", $"{this.Roh_authorsNumber.Value.ToString()}", list, " . ");
				}
				if(this.Vivo_freeTextKeyword != null)
				{
					foreach(var item2 in this.Vivo_freeTextKeyword)
					{
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}", "http://vivoweb.org/ontology/core#freeTextKeyword", $"\"{GenerarTextoSinSaltoDeLinea(item2)}\"", list, " . ");
					}
				}
				if(this.Bibo_isbn != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/isbn", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_isbn)}\"", list, " . ");
				}
				if(this.Bibo_issue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/issue", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_issue)}\"", list, " . ");
				}
				if(this.Bibo_volume != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/volume", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_volume)}\"", list, " . ");
				}
				if(this.Bibo_editor != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/editor", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_editor)}\"", list, " . ");
				}
				if(this.Dct_issued != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/dc/terms/issued", $"\"{this.Dct_issued.Value.ToString("yyyyMMddHHmmss")}\"", list, " . ");
				}
				if(this.Bibo_issn != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/issn", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_issn)}\"", list, " . ");
				}
				if(this.Bibo_pageStart != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/pageStart", $"{this.Bibo_pageStart.Value.ToString()}", list, " . ");
				}
				if(this.Roh_congressProceedingsPublication != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/congressProceedingsPublication", $"\"{this.Roh_congressProceedingsPublication.ToString()}\"", list, " . ");
				}
				if(this.Roh_crisIdentifier != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/crisIdentifier", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_crisIdentifier)}\"", list, " . ");
				}
				if(this.Vcard_url != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "https://www.w3.org/2006/vcard/ns#url", $"\"{GenerarTextoSinSaltoDeLinea(this.Vcard_url)}\"", list, " . ");
				}
				if(this.Bibo_pageEnd != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/ontology/bibo/pageEnd", $"{this.Bibo_pageEnd.Value.ToString()}", list, " . ");
				}
				if(this.Roh_collection != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/collection", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_collection)}\"", list, " . ");
				}
				if(this.Roh_isRelevantPublication != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/isRelevantPublication", $"\"{this.Roh_isRelevantPublication.ToString()}\"", list, " . ");
				}
				if(this.Vivo_hasPublicationVenue != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://vivoweb.org/ontology/core#hasPublicationVenue", $"\"{GenerarTextoSinSaltoDeLinea(this.Vivo_hasPublicationVenue)}\"", list, " . ");
				}
				if(this.IdDc_type != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://purl.org/dc/elements/1.1/type", $"<{this.IdDc_type}>", list, " . ");
				}
				if(this.Roh_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/Document_{ResourceID}_{ArticleID}",  "http://w3id.org/roh/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_title)}\"", list, " . ");
				}
			return list;
		}

		public override List<string> ToSearchGraphTriples(ResourceApi resourceAPI)
		{
			List<string> list = new List<string>();
			List<string> listaSearch = new List<string>();
			AgregarTags(list);
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", $"\"document\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/type", $"\"http://purl.org/ontology/bibo/Document\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechapublicacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hastipodoc", "\"5\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasfechamodificacion", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnumeroVisitas", "0", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasprivacidadCom", "\"publico\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://xmlns.com/foaf/0.1/firstName", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_title)}\"", list, " . ");
			AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://gnoss/hasnombrecompleto", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_title)}\"", list, " . ");
			string search = string.Empty;
			if(this.Roh_dataAuthor != null)
			{
			foreach(var item0 in this.Roh_dataAuthor)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/dataAuthor", $"<{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdRoh_motivatedBy != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_motivatedBy;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/motivatedBy", $"<{itemRegex}>", list, " . ");
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
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/contributionGrade", $"<{itemRegex}>", list, " . ");
				}
					if (this.IdsRoh_project != null)
					{
						foreach (var item2 in this.IdsRoh_project)
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
							AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/project", $"<{itemRegex}>", list, " . ");
						}
					}
					if (item0.Roh_relevantResults != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/relevantResults", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_relevantResults).ToLower()}\"", list, " . ");
				}
				if(item0.Roh_correspondingAuthor != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/correspondingAuthor", $"\"{item0.Roh_correspondingAuthor.ToString().ToLower()}\"", list, " . ");
				}
				if(item0.Roh_relevantPublication != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/relevantPublication", $"\"{item0.Roh_relevantPublication.ToString().ToLower()}\"", list, " . ");
				}
				if(item0.Roh_motivatedByOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/motivatedByOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_motivatedByOther).ToLower()}\"", list, " . ");
				}
				if(item0.IdRdf_member != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRdf_member;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/dataauthor_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#member", $"<{itemRegex}>", list, " . ");
				}
			}
			}
			if(this.Bibo_identifier != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://purl.org/ontology/bibo/identifier", $"<{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Bibo_identifier.ArticleID}>", list, " . ");
				//if(this.Bibo_identifier.Foaf_topic != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://xmlns.com/foaf/0.1/topic", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Foaf_topic).ToLower()}\"", list, " . ");
				//}
				//if(this.Bibo_identifier.Dc_title != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Dc_title).ToLower()}\"", list, " . ");
				//}
				//if(this.Bibo_identifier.Foaf_primaryTopic != null)
				//{
				//	AgregarTripleALista($"{resourceAPI.GraphsUrl}items/document_{ResourceID}_{this.Bibo_identifier.ArticleID}",  "http://xmlns.com/foaf/0.1/primaryTopic", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_identifier.Foaf_primaryTopic).ToLower()}\"", list, " . ");
				//}
			}
			if(this.Bibo_authorList != null)
			{
			foreach(var item0 in this.Bibo_authorList)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://purl.org/ontology/bibo/authorList", $"<{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Foaf_nick != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}",  "http://xmlns.com/foaf/0.1/nick", $"\"{GenerarTextoSinSaltoDeLinea(item0.Foaf_nick).ToLower()}\"", list, " . ");
				}
				if(item0.IdRdf_member != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRdf_member;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#member", $"<{itemRegex}>", list, " . ");
				}
				if(item0.Rdf_comment != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/bfo_0000023_{ResourceID}_{item0.ArticleID}",  "http://www.w3.org/1999/02/22-rdf-syntax-ns#comment", $"{item0.Rdf_comment.ToString()}", list, " . ");
				}
			}
			}
			if(this.Roh_impactIndex != null)
			{
			foreach(var item0 in this.Roh_impactIndex)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/impactIndex", $"<{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdRoh_impactCategory != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_impactCategory;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactCategory", $"<{itemRegex}>", list, " . ");
				}
				if(item0.Roh_impactIndexInYear != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactIndexInYear", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_impactIndexInYear).ToLower()}\"", list, " . ");
				}
				if(item0.Roh_impactSourceOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactSourceOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_impactSourceOther).ToLower()}\"", list, " . ");
				}
				if(item0.Roh_journalNumberInCat != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/journalNumberInCat", $"{item0.Roh_journalNumberInCat.Value.ToString()}", list, " . ");
				}
				if(item0.Roh_publicationPosition != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/publicationPosition", $"{item0.Roh_publicationPosition.Value.ToString()}", list, " . ");
				}
				if(item0.Roh_journalTop25 != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/journalTop25", $"\"{item0.Roh_journalTop25.ToString().ToLower()}\"", list, " . ");
				}
				if(item0.IdRoh_impactSource != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_impactSource;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/impactindex_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/impactSource", $"<{itemRegex}>", list, " . ");
				}
			}
			}
			if(this.Vcard_address != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_address.ArticleID}>", list, " . ");
				if(this.Vcard_address.IdVcard_hasCountryName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Vcard_address.IdVcard_hasCountryName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{itemRegex}>", list, " . ");
				}
				if(this.Vcard_address.IdVcard_hasRegion != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Vcard_address.IdVcard_hasRegion;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{itemRegex}>", list, " . ");
				}
				if(this.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Vcard_address.Vcard_locality).ToLower()}\"", list, " . ");
				}
			}
			if(this.Roh_hasMetric != null)
			{
			foreach(var item0 in this.Roh_hasMetric)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/hasMetric", $"<{resourceAPI.GraphsUrl}items/publicationmetric_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.Roh_metricNameOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/publicationmetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/metricNameOther", $"\"{GenerarTextoSinSaltoDeLinea(item0.Roh_metricNameOther).ToLower()}\"", list, " . ");
				}
				if(item0.IdRoh_metricName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = item0.IdRoh_metricName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/publicationmetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/metricName", $"<{itemRegex}>", list, " . ");
				}
				if(item0.Roh_citationCount != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/publicationmetric_{ResourceID}_{item0.ArticleID}",  "http://w3id.org/roh/citationCount", $"{item0.Roh_citationCount.ToString()}", list, " . ");
				}
			}
			}
			if(this.Bibo_presentedAt != null)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://purl.org/ontology/bibo/presentedAt", $"<{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}>", list, " . ");
			if(this.Bibo_presentedAt.Vcard_address != null)
			{
				AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}", "https://www.w3.org/2006/vcard/ns#address", $"<{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}>", list, " . ");
				if(this.Bibo_presentedAt.Vcard_address.IdVcard_hasCountryName != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Bibo_presentedAt.Vcard_address.IdVcard_hasCountryName;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasCountryName", $"<{itemRegex}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Vcard_address.IdVcard_hasRegion != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Bibo_presentedAt.Vcard_address.IdVcard_hasRegion;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#hasRegion", $"<{itemRegex}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Vcard_address.Vcard_locality != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/address_{ResourceID}_{this.Bibo_presentedAt.Vcard_address.ArticleID}",  "https://www.w3.org/2006/vcard/ns#locality", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Vcard_address.Vcard_locality).ToLower()}\"", list, " . ");
				}
			}
				if(this.Bibo_presentedAt.IdBibo_organizer != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Bibo_presentedAt.IdBibo_organizer;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/ontology/bibo/organizer", $"<{itemRegex}>", list, " . ");
				}
				if(this.Bibo_presentedAt.IdVivo_geographicFocus != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Bibo_presentedAt.IdVivo_geographicFocus;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#geographicFocus", $"<{itemRegex}>", list, " . ");
				}
				if(this.Bibo_presentedAt.Roh_geographicFocusOther != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://w3id.org/roh/geographicFocusOther", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Roh_geographicFocusOther).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Vivo_start != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#start", $"{this.Bibo_presentedAt.Vivo_start.Value.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Bibo_presentedAt.Roh_withExternalAdmissionsCommittee != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://w3id.org/roh/withExternalAdmissionsCommittee", $"\"{this.Bibo_presentedAt.Roh_withExternalAdmissionsCommittee.ToString().ToLower()}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.Vivo_end != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://vivoweb.org/ontology/core#end", $"{this.Bibo_presentedAt.Vivo_end.Value.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Bibo_presentedAt.Dc_title != null)
				{
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/dc/elements/1.1/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_presentedAt.Dc_title).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_presentedAt.IdDc_type != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.Bibo_presentedAt.IdDc_type;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"{resourceAPI.GraphsUrl}items/event_{ResourceID}_{this.Bibo_presentedAt.ArticleID}",  "http://purl.org/dc/elements/1.1/type", $"<{itemRegex}>", list, " . ");
				}
			}
			if(this.Roh_hasKnowledgeArea != null)
			{
			foreach(var item0 in this.Roh_hasKnowledgeArea)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://w3id.org/roh/hasKnowledgeArea", $"<{resourceAPI.GraphsUrl}items/categorypath_{ResourceID}_{item0.ArticleID}>", list, " . ");
				if(item0.IdsRoh_categoryNode != null)
				{
					foreach(var item2 in item0.IdsRoh_categoryNode)
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
						AgregarTripleALista($"{resourceAPI.GraphsUrl}items/categorypath_{ResourceID}_{item0.ArticleID}", "http://w3id.org/roh/categoryNode", $"<{itemRegex}>", list, " . ");
					}
				}
			}
			}
				if(this.IdRoh_supportType != null)
				{
					Regex regex = new Regex(@"\/items\/.+_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}");
					string itemRegex = this.IdRoh_supportType;
					if (regex.IsMatch(itemRegex))
					{
						itemRegex = $"http://gnoss/{resourceAPI.GetShortGuid(itemRegex).ToString().ToUpper()}";
					}
					else
					{
						itemRegex = itemRegex.ToLower();
					}
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/supportType", $"<{itemRegex}>", list, " . ");
				}
				if(this.IdsVcard_hasLanguage != null)
				{
					foreach(var item2 in this.IdsVcard_hasLanguage)
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
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "https://www.w3.org/2006/vcard/ns#hasLanguage", $"<{itemRegex}>", list, " . ");
					}
				}
				if(this.Roh_legalDeposit != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/legalDeposit", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_legalDeposit).ToLower()}\"", list, " . ");
				}
				if(this.Roh_publicationTitle != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/publicationTitle", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_publicationTitle).ToLower()}\"", list, " . ");
				}
				if(this.Roh_authorsNumber != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/authorsNumber", $"{this.Roh_authorsNumber.Value.ToString()}", list, " . ");
				}
				if(this.Vivo_freeTextKeyword != null)
				{
					foreach(var item2 in this.Vivo_freeTextKeyword)
					{
						AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://vivoweb.org/ontology/core#freeTextKeyword", $"\"{GenerarTextoSinSaltoDeLinea(item2).ToLower()}\"", list, " . ");
					}
				}
				if(this.Bibo_isbn != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/isbn", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_isbn).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_issue != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/issue", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_issue).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_volume != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/volume", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_volume).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_editor != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/editor", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_editor).ToLower()}\"", list, " . ");
				}
				if(this.Dct_issued != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/dc/terms/issued", $"{this.Dct_issued.Value.ToString("yyyyMMddHHmmss")}", list, " . ");
				}
				if(this.Bibo_issn != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/issn", $"\"{GenerarTextoSinSaltoDeLinea(this.Bibo_issn).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_pageStart != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/pageStart", $"{this.Bibo_pageStart.Value.ToString()}", list, " . ");
				}
				if(this.Roh_congressProceedingsPublication != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/congressProceedingsPublication", $"\"{this.Roh_congressProceedingsPublication.ToString().ToLower()}\"", list, " . ");
				}
				if(this.Roh_crisIdentifier != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/crisIdentifier", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_crisIdentifier).ToLower()}\"", list, " . ");
				}
				if(this.Vcard_url != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "https://www.w3.org/2006/vcard/ns#url", $"\"{GenerarTextoSinSaltoDeLinea(this.Vcard_url).ToLower()}\"", list, " . ");
				}
				if(this.Bibo_pageEnd != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/ontology/bibo/pageEnd", $"{this.Bibo_pageEnd.Value.ToString()}", list, " . ");
				}
				if(this.Roh_collection != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/collection", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_collection).ToLower()}\"", list, " . ");
				}
				if(this.Roh_isRelevantPublication != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/isRelevantPublication", $"\"{this.Roh_isRelevantPublication.ToString().ToLower()}\"", list, " . ");
				}
				if(this.Vivo_hasPublicationVenue != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://vivoweb.org/ontology/core#hasPublicationVenue", $"\"{GenerarTextoSinSaltoDeLinea(this.Vivo_hasPublicationVenue).ToLower()}\"", list, " . ");
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
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://purl.org/dc/elements/1.1/type", $"<{itemRegex}>", list, " . ");
				}
				if(this.Roh_title != null)
				{
					AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}",  "http://w3id.org/roh/title", $"\"{GenerarTextoSinSaltoDeLinea(this.Roh_title).ToLower()}\"", list, " . ");
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
			string titulo = $"{this.Roh_title.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
			string descripcion = $"{this.Roh_title.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\"", "\"\"").Replace("'", "''").Replace("|", "#PIPE#")}";
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
			return $"{resourceAPI.GraphsUrl}items/DocumentOntology_{ResourceID}_{ArticleID}";
		}

		private string GenerarTextoSinSaltoDeLinea(string pTexto)
		{
			return pTexto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"");
		}

		internal void AddResourceTitle(ComplexOntologyResource resource)
		{
			resource.Title = this.Roh_title;
		}

		internal void AddResourceDescription(ComplexOntologyResource resource)
		{
			resource.Description = this.Roh_title;
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
