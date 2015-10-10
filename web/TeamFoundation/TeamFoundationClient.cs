namespace Skeleton.TeamFoundation {
    using Cards;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi.Patch;
    using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
    using State;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class TeamFoundationClient : ICardsClient {
        readonly ISessionProvider _provider;

        public TeamFoundationClient(ISessionProvider provider) {
            _provider = provider;
        }

        public async Task<IEnumerable<string>> GetProjectNames() {
            var projects = await ProjectsClient.GetProjects();
            return projects.Select(p => p.Name).ToList();
        }

        public async Task UpdateCards(IEnumerable<Card> cards) {
            foreach (var card in cards)
                await Update(card);
        }
        public async Task<IEnumerable<Card>> GetCards(string projectName) {
            return await Query(
                string.Format(@"SELECT * FROM WorkItemLinks WHERE 
                                [Source].[System.TeamProject] = '{0}' AND
                                [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND
                                [Target].[System.State] NOT IN('Done', 'Closed', 'Resolved') AND
                                [Target].[System.WorkItemType] IN('Feature', 'User Story', 'Product Backlog Item') AND 
                                [Source].[System.WorkItemType] IN('Feature', 'User Story', 'Product Backlog Item')
                                ORDER BY [Microsoft.VSTS.Common.BacklogPriority], [Source].[System.WorkItemType]
                                mode(recursive)", projectName));
        }

        private ProjectHttpClient _projectsClient;
        protected virtual ProjectHttpClient ProjectsClient {
            get {
                if (_projectsClient == null)
                    _projectsClient = new ProjectHttpClient(
                        new Uri(_provider.Session.Url),
                        Credential());
                return _projectsClient;
            }
        }

        private WorkItemTrackingHttpClient _workItemsClient;
        protected virtual WorkItemTrackingHttpClient WorkItemsClient {
            get {
                if (_workItemsClient == null)
                    _workItemsClient = new WorkItemTrackingHttpClient(
                      new Uri(_provider.Session.Url),
                      Credential());
                return _workItemsClient;
            }
        }

        private WindowsCredential Credential() {
            return new WindowsCredential(
                new NetworkCredential(
                    _provider.Session.Username,
                    _provider.Session.Password));
        }

        private async Task Update(Card card) {
            var updates = Updates(FieldNames.Priority, card.Priority);
            await WorkItemsClient.UpdateWorkItemAsync(updates, card.Id);
        }

        private static JsonPatchDocument Updates(string field, object value) {
            var updates = new JsonPatchDocument();
            updates.Add(Update(field, value));
            return updates;
        }

        private static JsonPatchOperation Update(string field, object value) {
            return new JsonPatchOperation {
                Path = "/fields/" + field,
                Operation = Operation.Replace,
                Value = value
            };
        }



        private async Task<IEnumerable<Card>> Query(string wiql) {
            var result = await WorkItemsClient.QueryByWiqlAsync(new Wiql { Query = wiql });
            var workItems = await GetWorkItems(result);
            var cards = workItems.Select(AsCard).ToList();
            foreach (var relation in result.WorkItemRelations) {
                var story = cards.FirstOrDefault(c => c.Id == relation.Target?.Id);
                if (story != null)
                    story.FeatureId = relation.Source?.Id;
            }
            return cards;
        }

        private async Task<List<WorkItem>> GetWorkItems(WorkItemQueryResult result) {
            return await WorkItemsClient.GetWorkItemsAsync(Ids(result), null, null, WorkItemExpand.Fields);
        }

        private static IEnumerable<int> Ids(WorkItemQueryResult result) {
            return result.WorkItemRelations.Where(r => r.Target != null).Select(r => r.Target.Id)
                .Union(result.WorkItemRelations.Where(r => r.Source != null).Select(r => r.Source.Id)).Take(100);
        }

        private static Func<WorkItem, Card> AsCard =
             i => new Card {
                 Id = i.Id.GetValueOrDefault(0),
                 Title = (string)i.Fields["System.Title"],
                 Type = (string)i.Fields["System.WorkItemType"],
                 Priority = Priority(i)
             };

        private static double? Priority(WorkItem i) {
            if (!i.Fields.Keys.Contains(FieldNames.Priority))
                return null;

            return (double)i.Fields[FieldNames.Priority];
        }
    }
}