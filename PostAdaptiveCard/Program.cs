using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using MessageCardModel;
using MessageCardModel.Actions;
using MessageCardModel.Actions.OpenUri;

namespace PostAdaptiveCard
{
    internal class Program
    {
        private static void Main(string webhookUri, string eventPath)
        {
            if (string.IsNullOrWhiteSpace(eventPath))
            {
                eventPath = Environment.GetEnvironmentVariable("GITHUB_EVENT_PATH");
            }

            using FileStream fs = File.OpenRead(eventPath);
            var doc = JsonDocument.Parse(fs);

            string eventName = doc.RootElement.GetProperty("action").GetString();

            MessageCard card;
            if (doc.RootElement.TryGetProperty("issue", out JsonElement issue))
            {
                card = GetIssueCard(eventName, issue);
            }
            else if (doc.RootElement.TryGetProperty("pull_request", out JsonElement pullRequest))
            {
                card = GetPullRequestCard(eventName, pullRequest);
            }
            else
            {
                throw new InvalidOperationException("Can not support this type of event.");
            }

            string converted = card.ToJson();

            Console.WriteLine(JsonSerializer.Serialize(JsonDocument.Parse(converted), new JsonSerializerOptions { WriteIndented = true }));

            if (!string.IsNullOrWhiteSpace(webhookUri))
            {
                using var client = new HttpClient();
                using var content = new StringContent(converted, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = client.PostAsync(webhookUri, content).Result;
                response.EnsureSuccessStatusCode();
            }

            static MessageCard GetPullRequestCard(string eventName, JsonElement pullRequest)
            {
                // pullRequest is one of these: https://developer.github.com/v3/pulls/#response

                JsonElement user = pullRequest.GetProperty("user");
                return new MessageCard
                {

                    Title = $"PR #{pullRequest.GetProperty("number")} {eventName} - {pullRequest.GetProperty("title")}",
                    Summary = $"PR #{pullRequest.GetProperty("number")} {eventName} by { user.GetProperty("login")}, {pullRequest.GetProperty("title")}",
                    Sections = new List<Section>()
                    {
                        new Section
                        {
                            ActivityTitle = user.GetProperty("login").GetString(),
                            ActivityImage  = user.GetProperty("avatar_url").GetString(),
                            ActivitySubtitle = $"{pullRequest.GetProperty("head").GetProperty("label")} -> {pullRequest.GetProperty("base").GetProperty("label")}",
                            Text = pullRequest.GetProperty("body").GetString(),
                            Facts = IgnoreNulls(
                                GetLabelFact(pullRequest.GetProperty("labels")),
                                GetMilestoneFact(pullRequest.GetProperty("milestone"))
                            )
                        }
                    },
                    Actions = new List<OpenUriAction>
                    {
                        GetViewInGitHubAction(pullRequest)
                    }
                };
            }

            static MessageCard GetIssueCard(string eventName, JsonElement issue)
            {
                // issue is one of these: https://developer.github.com/v3/issues/#response

                JsonElement user = issue.GetProperty("user");
                return new MessageCard
                {
                    Title = $"Issue #{issue.GetProperty("number")} {eventName} - {issue.GetProperty("title").GetString()}",
                    Summary = $"Issue #{issue.GetProperty("number")} {eventName} by { user.GetProperty("login")}, {issue.GetProperty("title")}",
                    Sections = new List<Section>()
                    {
                        new Section
                        {
                            ActivityTitle = user.GetProperty("login").GetString(),
                            ActivityImage  = user.GetProperty("avatar_url").GetString(),
                            Text = issue.GetProperty("body").GetString(),
                            Facts = IgnoreNulls(
                                GetLabelFact(issue.GetProperty("labels")),
                                GetMilestoneFact(issue.GetProperty("milestone"))
                            )
                        }
                    },
                    Actions = new List<OpenUriAction>
                    {
                        GetViewInGitHubAction(issue)
                    }
                };
            }
        }

        private static OpenUriAction GetViewInGitHubAction(JsonElement element)
        {
            return new OpenUriAction
            {
                Type = ActionType.OpenUri,
                Name = "View on GitHub",
                Targets = new List<Target>
                {
                    new Target
                    {
                        OS = TargetOs.Default,
                        Uri = element.GetProperty("html_url").GetString()
                    }
                }
            };
        }

        private static IEnumerable<T> IgnoreNulls<T>(params T[] items)
        {
            var result = new List<T>(items.Where(i => i != null));

            if (result.Count == 0)
            {
                return null;
            }

            return result;
        }

        private static Fact GetLabelFact(JsonElement labels)
        {
            if (labels.GetArrayLength() == 0)
            {
                return null;
            }

            return new Fact
            {
                Name = "Labels",
                Value = string.Join(", ", labels.EnumerateArray().Select(l => l.GetProperty("name").GetString()))
            };
        }

        private static Fact GetMilestoneFact(JsonElement milestone)
        {
            if (milestone.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return new Fact
            {
                Name = "Milestone",
                Value = milestone.GetProperty("title").GetString()
            };
        }
    }
}
