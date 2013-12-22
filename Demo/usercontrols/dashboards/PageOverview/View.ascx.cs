using System;
using System.Globalization;
using System.Linq;
using Examine;
using Examine.SearchCriteria;
using Newtonsoft.Json;
using umbraco.BusinessLogic;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Demo.usercontrols.dashboards.PageOverview
{
    public partial class View : System.Web.UI.UserControl
    {        
        protected IContentTypeService ContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        protected string Payload;

        protected void Page_Load(object sender, EventArgs e)
        {
            var pageContentTypes = ContentTypeService
                .GetAllContentTypes()
                .Where(x => x.Alias.EndsWith("page"))
                .ToList();
            
            var me = User.GetCurrent();

            var searcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];

            var query = searcher
                .CreateSearchCriteria()
                .GroupedOr(new[] {"nodeTypeAlias"}, pageContentTypes.Select(x => x.Alias).ToArray())
                .And()
                .GroupedOr(new[] {"__PathParts"}, me.StartNodeId.ToString())
                .Not()
                .GroupedOr(new[] {"__PathParts"}, "-20");

            var results = searcher.Search(query.Compile());

            var culture = new CultureInfo("da-DK");

            Payload = JsonConvert.SerializeObject(results.Select(x =>
            {
                var fields = x.Fields;

                var creatorId = int.Parse(fields["creatorID"]);
                var writerIds = fields["__AllWriterIds"].Split(' ').Select(int.Parse);
                var updated = DateTime.ParseExact(fields["updateDate"], "yyyyMMddHHmmssfff", culture);

                return new
                {
                    MyPage = me.Id == creatorId || writerIds.Contains(me.Id),
                    PageId = x.Id,
                    PageName = fields["nodeName"],
                    PageType = pageContentTypes.First(y => y.Alias == fields["nodeTypeAlias"]).Name,
                    Updated = updated.ToString("dd-MM-yyyy HH:mm"),
                    WriterName = fields["writerName"],
                    CreatorName = fields["creatorName"],
                    Published = fields["__HasPublishedVersion"] == "1"
                };
            }));
        }
    }
}