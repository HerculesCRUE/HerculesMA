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
using Feature = FeatureOntology.Feature;

namespace CurriculumvitaeOntology
{
	public class PersonalData : GnossOCBase
	{

		public PersonalData() : base() { } 

		public PersonalData(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propSchema_nationality = pSemCmsModel.GetPropertyByPath("http://www.schema.org/nationality");
			if(propSchema_nationality != null && propSchema_nationality.PropertyValues.Count > 0)
			{
				this.Schema_nationality = new Feature(propSchema_nationality.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propVcard_address = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#address");
			if(propVcard_address != null && propVcard_address.PropertyValues.Count > 0)
			{
				this.Vcard_address = new Address(propVcard_address.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_hasFax = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasFax");
			if(propRoh_hasFax != null && propRoh_hasFax.PropertyValues.Count > 0)
			{
				this.Roh_hasFax = new TelephoneType(propRoh_hasFax.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vivo_researcherId = new List<Document>();
			SemanticPropertyModel propVivo_researcherId = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#researcherId");
			if(propVivo_researcherId != null && propVivo_researcherId.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVivo_researcherId.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Document vivo_researcherId = new Document(propValue.RelatedEntity,idiomaUsuario);
						this.Vivo_researcherId.Add(vivo_researcherId);
					}
				}
			}
			SemanticPropertyModel propRoh_birthplace = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/birthplace");
			if(propRoh_birthplace != null && propRoh_birthplace.PropertyValues.Count > 0)
			{
				this.Roh_birthplace = new Address(propRoh_birthplace.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vcard_hasTelephone = new List<TelephoneType>();
			SemanticPropertyModel propVcard_hasTelephone = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasTelephone");
			if(propVcard_hasTelephone != null && propVcard_hasTelephone.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propVcard_hasTelephone.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						TelephoneType vcard_hasTelephone = new TelephoneType(propValue.RelatedEntity,idiomaUsuario);
						this.Vcard_hasTelephone.Add(vcard_hasTelephone);
					}
				}
			}
			SemanticPropertyModel propFoaf_openid = pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/openid");
			if(propFoaf_openid != null && propFoaf_openid.PropertyValues.Count > 0)
			{
				this.Foaf_openid = new Document(propFoaf_openid.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_hasMobilePhone = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasMobilePhone");
			if(propRoh_hasMobilePhone != null && propRoh_hasMobilePhone.PropertyValues.Count > 0)
			{
				this.Roh_hasMobilePhone = new TelephoneType(propRoh_hasMobilePhone.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Foaf_homepage = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/homepage"));
			this.Vcard_birth_date= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#birth-date"));
			this.Foaf_gender = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/gender"));
			this.Foaf_img = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/img"));
			this.Vcard_email = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#email"));
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Nacionalidad")]
		[RDFProperty("http://www.schema.org/nationality")]
		public  Feature Schema_nationality  { get; set;} 
		public string IdSchema_nationality  { get; set;} 

		[LABEL(LanguageEnum.es,"Dirección de contacto")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#address")]
		public  Address Vcard_address { get; set;}

		[LABEL(LanguageEnum.es,"Fax")]
		[RDFProperty("http://w3id.org/roh/hasFax")]
		public  TelephoneType Roh_hasFax { get; set;}

		[LABEL(LanguageEnum.es,"Identificador autor")]
		[RDFProperty("http://vivoweb.org/ontology/core#researcherId")]
		public  List<Document> Vivo_researcherId { get; set;}

		[LABEL(LanguageEnum.es,"Lugar de nacimiento")]
		[RDFProperty("http://w3id.org/roh/birthplace")]
		public  Address Roh_birthplace { get; set;}

		[LABEL(LanguageEnum.es,"Teléfono fijo")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#hasTelephone")]
		public  List<TelephoneType> Vcard_hasTelephone { get; set;}

		[LABEL(LanguageEnum.es,"Documento identificativo")]
		[RDFProperty("http://xmlns.com/foaf/0.1/openid")]
		public  Document Foaf_openid { get; set;}

		[LABEL(LanguageEnum.es,"Teléfono móvil")]
		[RDFProperty("http://w3id.org/roh/hasMobilePhone")]
		public  TelephoneType Roh_hasMobilePhone { get; set;}

		[LABEL(LanguageEnum.es,"Página web personal")]
		[RDFProperty("http://xmlns.com/foaf/0.1/homepage")]
		public  string Foaf_homepage { get; set;}

		[LABEL(LanguageEnum.es,"Fecha de nacimiento")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#birth-date")]
		public  DateTime? Vcard_birth_date { get; set;}

		[LABEL(LanguageEnum.es,"Sexo")]
		[RDFProperty("http://xmlns.com/foaf/0.1/gender")]
		public  string Foaf_gender { get; set;}

		[LABEL(LanguageEnum.es,"Imagen")]
		[RDFProperty("http://xmlns.com/foaf/0.1/img")]
		public  string Foaf_img { get; set;}

		[LABEL(LanguageEnum.es,"Email")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#email")]
		public  string Vcard_email { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("schema:nationality", this.IdSchema_nationality));
			propList.Add(new StringOntologyProperty("foaf:homepage", this.Foaf_homepage));
			if (this.Vcard_birth_date.HasValue){
				propList.Add(new DateOntologyProperty("vcard:birth_date", this.Vcard_birth_date.Value));
				}
			propList.Add(new StringOntologyProperty("foaf:gender", this.Foaf_gender));
			propList.Add(new StringOntologyProperty("foaf:img", this.Foaf_img));
			propList.Add(new StringOntologyProperty("vcard:email", this.Vcard_email));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
			if(Vcard_address!=null){
				Vcard_address.GetProperties();
				Vcard_address.GetEntities();
				OntologyEntity entityVcard_address = new OntologyEntity("https://www.w3.org/2006/vcard/ns#Address", "https://www.w3.org/2006/vcard/ns#Address", "vcard:address", Vcard_address.propList, Vcard_address.entList);
				entList.Add(entityVcard_address);
			}
			if(Roh_hasFax!=null){
				Roh_hasFax.GetProperties();
				Roh_hasFax.GetEntities();
				OntologyEntity entityRoh_hasFax = new OntologyEntity("https://www.w3.org/2006/vcard/ns#TelephoneType", "https://www.w3.org/2006/vcard/ns#TelephoneType", "roh:hasFax", Roh_hasFax.propList, Roh_hasFax.entList);
				entList.Add(entityRoh_hasFax);
			}
			if(Vivo_researcherId!=null){
				foreach(Document prop in Vivo_researcherId){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityDocument = new OntologyEntity("http://xmlns.com/foaf/0.1/Document", "http://xmlns.com/foaf/0.1/Document", "vivo:researcherId", prop.propList, prop.entList);
				entList.Add(entityDocument);
				prop.Entity= entityDocument;
				}
			}
			if(Roh_birthplace!=null){
				Roh_birthplace.GetProperties();
				Roh_birthplace.GetEntities();
				OntologyEntity entityRoh_birthplace = new OntologyEntity("https://www.w3.org/2006/vcard/ns#Address", "https://www.w3.org/2006/vcard/ns#Address", "roh:birthplace", Roh_birthplace.propList, Roh_birthplace.entList);
				entList.Add(entityRoh_birthplace);
			}
			if(Vcard_hasTelephone!=null){
				foreach(TelephoneType prop in Vcard_hasTelephone){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityTelephoneType = new OntologyEntity("https://www.w3.org/2006/vcard/ns#TelephoneType", "https://www.w3.org/2006/vcard/ns#TelephoneType", "vcard:hasTelephone", prop.propList, prop.entList);
				entList.Add(entityTelephoneType);
				prop.Entity= entityTelephoneType;
				}
			}
			if(Foaf_openid!=null){
				Foaf_openid.GetProperties();
				Foaf_openid.GetEntities();
				OntologyEntity entityFoaf_openid = new OntologyEntity("http://xmlns.com/foaf/0.1/Document", "http://xmlns.com/foaf/0.1/Document", "foaf:openid", Foaf_openid.propList, Foaf_openid.entList);
				entList.Add(entityFoaf_openid);
			}
			if(Roh_hasMobilePhone!=null){
				Roh_hasMobilePhone.GetProperties();
				Roh_hasMobilePhone.GetEntities();
				OntologyEntity entityRoh_hasMobilePhone = new OntologyEntity("https://www.w3.org/2006/vcard/ns#TelephoneType", "https://www.w3.org/2006/vcard/ns#TelephoneType", "roh:hasMobilePhone", Roh_hasMobilePhone.propList, Roh_hasMobilePhone.entList);
				entList.Add(entityRoh_hasMobilePhone);
			}
		} 







		internal override void AddImages(ComplexOntologyResource pResource)
		{
			base.AddImages(pResource);
			List<ImageAction> actionListimg = new List<ImageAction>();
			actionListimg.Add(new ImageAction(0,234, ImageTransformationType.Crop, 100));
			pResource.AttachImage(this.Foaf_img, actionListimg,"foaf:img", true, this.GetExtension(this.Foaf_img), this.Entity);
			this.Vcard_address.AddImages(pResource);
			this.Roh_hasFax.AddImages(pResource);
			if(Vivo_researcherId!=null){
				foreach (Document prop in Vivo_researcherId)
			{
				prop.AddImages(pResource);
				}
			}
			this.Roh_birthplace.AddImages(pResource);
			if(Vcard_hasTelephone!=null){
				foreach (TelephoneType prop in Vcard_hasTelephone)
			{
				prop.AddImages(pResource);
				}
			}
			this.Foaf_openid.AddImages(pResource);
			this.Roh_hasMobilePhone.AddImages(pResource);
		}

	}
}
