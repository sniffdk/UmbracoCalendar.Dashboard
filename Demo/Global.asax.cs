using System;
using System.Linq;
using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Demo
{
    public class Global : UmbracoApplication
    {
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            RegisterIndexRules();
        }

        protected static void RegisterIndexRules()
        {
            var contentService = ApplicationContext.Current.Services.ContentService;
            var draftIndexer = ((LuceneIndexer)ExamineManager.Instance.IndexProviderCollection["InternalIndexer"]);

            draftIndexer.DocumentWriting += (sender, args) =>
            {
                var pathParts = string.Join(" ", args.Fields["path"].Split(','));
                var pathPartsField = new Field(
                    "__PathParts", 
                    pathParts, 
                    Field.Store.NO, 
                    Field.Index.ANALYZED_NO_NORMS, 
                    Field.TermVector.NO
                );

                args.Document.Add(pathPartsField);

                var hasPublishedVersion = contentService.HasPublishedVersion(args.NodeId);
                var hasPublishedVersionField = new Field(
                    "__HasPublishedVersion", 
                    hasPublishedVersion ? "1" : "0", 
                    Field.Store.YES, 
                    Field.Index.NOT_ANALYZED_NO_NORMS, 
                    Field.TermVector.NO
                );

                args.Document.Add(hasPublishedVersionField);

                var writerIds = contentService
                    .GetVersions(args.NodeId)
                    .Select(x => x.WriterId)
                    .Distinct();
                
                var writerIdsField = new Field(
                    "__AllWriterIds", 
                    string.Join(" ", writerIds), 
                    Field.Store.YES, 
                    Field.Index.ANALYZED_NO_NORMS, 
                    Field.TermVector.NO
                );

                args.Document.Add(writerIdsField);
            };

            ContentService.UnPublished += (sender, args) =>
            {
                foreach (var publishedEntity in args.PublishedEntities)
                {
                    draftIndexer.ReIndexNode(publishedEntity.ToXml(), "content");
                }
            };

            ContentService.Trashed += (sender, args) =>
            {
                /* The Trashed event is only triggered for the selected node. 
                   Get all descendants and re-index them as well. */
                var descendants = sender.GetDescendants(args.Entity);
                foreach (var descendant in descendants)
                {
                    draftIndexer.ReIndexNode(descendant.ToXml(), "content");
                }
            };

            content.AfterUpdateDocumentCache += (sender, args) =>
            {
                /* The AfterUpdateDocumentCache is part of the old api, so we are given a Document. 
                   Get the IContent equivalent instead. */
                var contentItem = ApplicationContext.Current.Services.ContentService.GetById(sender.Id);
                draftIndexer.ReIndexNode(contentItem.ToXml(), "content");
            };
        }
    }
}