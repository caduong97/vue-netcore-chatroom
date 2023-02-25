using System;
using System.Text.Json.Serialization;

namespace vue_netcore_chatroom.Models
{
    [Serializable]
    public class HubResponse<T>
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public Guid Id;

        [JsonPropertyName("data")]
        [JsonInclude]
        public T Data;

        public HubResponse(T data)
        {
            Id = Guid.NewGuid();
            Data = data;
        }
    }
}

