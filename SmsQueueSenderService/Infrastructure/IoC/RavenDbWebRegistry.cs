using Activetrail.SmsQueueSenderService.Infrastructure.Db;
//using Raven.Client;
//using Raven.Client.Document;
//using Raven.Client.Indexes;
using StructureMap;
using System.ComponentModel.Composition.Hosting;

namespace Activetrail.SmsQueueSenderService.Infrastructure
{
    public sealed class RavenDbWebRegistry : Registry
    {
        public RavenDbWebRegistry()
        {
            // register RavenDB document store
            //ForSingletonOf<IDocumentStore>().Use(ConfigureDocumentStore());
        }

        //private IDocumentStore ConfigureDocumentStore()
        //{
        //    var documentStore = new DocumentStore
        //    {
        //        ConnectionStringName = "RavenDb",
        //        DefaultDatabase = "messages"
        //    };
        //    documentStore.Initialize();

        //    //IndexCreation.CreateIndexes(typeof(SMSMessageQueueInfo_ByStatus).Assembly, documentStore);
        //    var catalog = new AssemblyCatalog(typeof(Program).Assembly);
        //    var container = new CompositionContainer(catalog);
        //    IndexCreation.CreateIndexes(container, documentStore.DatabaseCommands, documentStore.Conventions);


        //    return documentStore;
        //}
    }
}
