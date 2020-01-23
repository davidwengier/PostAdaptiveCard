using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MessageCardModel.Actions;

namespace PostAdaptiveCard
{
    internal class ActionConverter : JsonConverter<IEnumerable<IAction>>
    {
        public override IEnumerable<IAction> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<IAction> value, JsonSerializerOptions options)
        {
            foreach (IAction action in value)
            {
                switch (action.Type)
                {
                    case ActionType.ActionCard:
                    {
                        JsonSerializer.Serialize(writer, (ActionCardAction)action, options);
                        break;
                    }
                    case ActionType.HttpPost:
                    {
                        JsonSerializer.Serialize(writer, (HttpPostAction)action, options);
                        break;
                    }
                    case ActionType.OpenUri:
                    {
                        JsonSerializer.Serialize(writer, (OpenUriAction)action, options);
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException($"No idea what a {action.Type} action is");
                    }
                }
            }
        }
    }
}
