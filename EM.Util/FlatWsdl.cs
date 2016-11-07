using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml.Schema;
using System.ServiceModel.Configuration;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;


//http://my-tech-talk.blogspot.com/2008/07/adding-flatwsdl-to-wcf-webservice.html

namespace EM.Util
{
    public class FlatWsdl : BehaviorExtensionElement, IWsdlExportExtension, IEndpointBehavior, IServiceBehavior
    {
        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            DoExport(exporter);
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            DoExport(exporter);
        }

        private void DoExport(WsdlExporter exporter)
        {
            XmlSchemaSet schemaSet = exporter.GeneratedXmlSchemas;

            foreach (ServiceDescription wsdl in exporter.GeneratedWsdlDocuments)
            {
                List<XmlSchema> importsList = new List<XmlSchema>();

                foreach (XmlSchema schema in wsdl.Types.Schemas)
                {
                    AddImportedSchemas(schema, schemaSet, importsList);
                }

                if (importsList.Count > 0)
                {
                    wsdl.Types.Schemas.Clear();

                    foreach (XmlSchema schema in importsList)
                    {
                        RemoveXsdImports(schema);
                        wsdl.Types.Schemas.Add(schema);
                    }
                }
            }
        }

        private void AddImportedSchemas(XmlSchema schema, XmlSchemaSet schemaSet, List<XmlSchema> importsList)
        {
            foreach (XmlSchemaImport import in schema.Includes)
            {
                ICollection realSchemas =
                    schemaSet.Schemas(import.Namespace);

                foreach (XmlSchema ixsd in realSchemas)
                {
                    if (!importsList.Contains(ixsd))
                    {
                        importsList.Add(ixsd);
                        AddImportedSchemas(ixsd, schemaSet, importsList);
                    }
                }
            }
        }

        private void RemoveXsdImports(XmlSchema schema)
        {
            for (int i = 0; i < schema.Includes.Count; i++)
            {
                if (schema.Includes[i] is XmlSchemaImport)
                    schema.Includes.RemoveAt(i--);
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public override System.Type BehaviorType
        {
            get { return typeof(FlatWsdl); }
        }

        protected override object CreateBehavior()
        {
            return new FlatWsdl();
        }

        public void AddBindingParameters(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            //throw new System.NotImplementedException();
        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            //throw new System.NotImplementedException();
        }

        public void Validate(System.ServiceModel.Description.ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            //throw new System.NotImplementedException();
        }
    }
}