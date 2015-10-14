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

    public class TeamFoundationClient : ICardsClient, IProjectsClient {
        readonly ISessionProvider _provider;
        readonly string[] _workItemTypes = { "Feature", "User Story", "Product Backlog Item" };

        public TeamFoundationClient(ISessionProvider provider) {
            _provider = provider;
        }

        public async Task<IEnumerable<string>> GetProjectNames() {
            var projects = await ProjectsClient.GetProjects();
            return projects.Select(p => p.Name).ToList();
        }

        public async Task<Dictionary<string, string>> GetProjectPriorityFieldNames(IEnumerable<string> projectNames) {
            var priorityFields = new Dictionary<string, string>();
            foreach (var name in projectNames)
                priorityFields.Add(name.ToLower(), await GetPriorityFieldName(name));
            return priorityFields;
        }

        private async Task<string> GetPriorityFieldName(string projectName) {
            var types = await WorkItemsClient.GetWorkItemTypesAsync(projectName);
            var xmlForms = types.Where(t => _workItemTypes.Contains(t.Name)).Select(t => t.XmlForm);
            var xmlForm = string.Join(string.Empty, xmlForms);
            return GetPriorityFieldNameFromXml(xmlForm);
        }

        private static string GetPriorityFieldNameFromXml(string xml) {
            if (xml.Contains(FieldNames.StackRank))
                return FieldNames.StackRank;

            return FieldNames.BacklogPriority;
        }

        public async Task UpdateCards(IEnumerable<Card> cards) {
            foreach (var card in cards)
                await Update(card);
        }

        private async Task Update(Card card) {
            var doc = CreateUpdateDocument(card);
            await WorkItemsClient.UpdateWorkItemAsync(doc, card.Id);
        }

        private JsonPatchDocument CreateUpdateDocument(Card card) {
            return new JsonPatchDocument {
                new JsonPatchOperation {
                    Path = "/fields/" + PriorityFieldNameFor(card.Project),
                    Operation = GetOperation(card),
                    Value = card.Priority ?? 0
                }
            };
        }

        private string PriorityFieldNameFor(string projectName) {
            return _provider.Session.ProjectPriorityFieldNames[projectName.ToLower()];
        }

        public async Task<IEnumerable<Card>> GetCards(string projectName) {
            return await Query(
                string.Format(@"SELECT * FROM WorkItemLinks WHERE 
                                [Source].[System.TeamProject] = '{0}' AND
                                [System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward' AND
                                [Target].[System.State] NOT IN('Done', 'Closed', 'Resolved') AND
                                [Target].[System.WorkItemType] IN('Feature', 'User Story', 'Product Backlog Item') AND 
                                [Source].[System.WorkItemType] IN('Feature', 'User Story', 'Product Backlog Item')
                                ORDER BY [Source].[System.WorkItemType], [{1}]
                                mode(recursive)", projectName, PriorityFieldNameFor(projectName)));
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


        private static Operation GetOperation(Card card) {
            return card.OriginalPriority == null
                ? Operation.Add
                : Operation.Replace;
        }


        private async Task<IEnumerable<Card>> Query(string wiql) {
            var result = await WorkItemsClient.QueryByWiqlAsync(new Wiql { Query = wiql });
            var workItems = await GetWorkItems(result);
            var cards = workItems.Select(AsCard).ToList();
            AssignFeatures(result.WorkItemRelations, cards);
            return cards;
        }

        private static void AssignFeatures(IEnumerable<WorkItemLink> links, List<Card> cards) {
            foreach (var link in links)
                AssignFeature(link, cards);

            FlattenFeatures(cards);
        }

        private static void FlattenFeatures(IEnumerable<Card> cards) {
            foreach (var card in cards.Where(c => c.FeatureId != null))
                card.FeatureId = GetParentId(cards, card);
        }

        private static int GetParentId(IEnumerable<Card> cards, Card card) {
            var parent = cards.FirstOrDefault(c => c.Id == card.FeatureId);
            if (parent.FeatureId == null)
                return parent.Id;
            return GetParentId(cards, parent);
        }

        private static void AssignFeature(WorkItemLink link, IEnumerable<Card> cards) {
            var card = cards.FirstOrDefault(c => c.Id == link.Target?.Id);
            if (card != null)
                card.FeatureId = link.Source?.Id;
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
                 Title = (string)i.Fields[FieldNames.Title],
                 Type = (string)i.Fields[FieldNames.WorkItemType],
                 Priority = PriorityValue(i),
                 OriginalPriority = PriorityValue(i),
                 Project = (string)i.Fields[FieldNames.TeamProject]
             };

        private static string PriorityFieldName(WorkItem i) {
            if (HasField(i, FieldNames.StackRank))
                return FieldNames.StackRank;
            if (HasField(i, FieldNames.StackRank))
                return FieldNames.BacklogPriority;
            return null;
        }

        private static double? PriorityValue(WorkItem i) {
            var field = PriorityFieldName(i);
            if (HasField(i, field))
                return (double)i.Fields[field];

            return null;
        }

        private static bool HasField(WorkItem item, string fieldName) {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            return item.Fields.Keys.Contains(fieldName);
        }

    }
}