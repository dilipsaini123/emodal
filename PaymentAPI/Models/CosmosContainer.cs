using Newtonsoft.Json;
using System;

public class CosmosContainer
{
    [JsonProperty("id")]
    public string id { get; set; }  
    
    [JsonProperty("ContainerId")]
    public string ContainerId { get; set; }

    [JsonProperty("TradeType")]
    public string TradeType { get; set; }

    [JsonProperty("Status")]
    public string Status { get; set; }

    [JsonProperty("Holds")]
    public string Holds { get; set; }

    [JsonProperty("PregateTicket")]
    public string PregateTicket { get; set; }

    [JsonProperty("EModalPregate")]
    public string EModalPregate { get; set; }

    [JsonProperty("GateStatus")]
    public string GateStatus { get; set; }

    [JsonProperty("Origin")]
    public string Origin { get; set; }

    [JsonProperty("Destination")]
    public string Destination { get; set; }

    [JsonProperty("CurrentLocation")]
    public string CurrentLocation { get; set; }

    [JsonProperty("line")]
    public string Line { get; set; }

    [JsonProperty("VesselName")]
    public string VesselName { get; set; }

    [JsonProperty("VesselCode")]
    public string VesselCode { get; set; }

    [JsonProperty("Voyage")]
    public string Voyage { get; set; } // Corrected property name from 'Vogage' to 'Voyage'

    [JsonProperty("SizeType")]
    public string SizeType { get; set; }

    [JsonProperty("AdditionalFees")]
    public decimal AdditionalFees { get; set; }

    [JsonProperty("DemurrageFees")]
    public decimal DemurrageFees { get; set; }

    [JsonProperty("Date")]
    public DateOnly Date { get; set; }

    // Optional: If you want to ignore metadata fields
    [JsonIgnore]
    public string ContainerDetails { get; set; }

    [JsonIgnore]
    public string _rid { get; set; }

    [JsonIgnore]
    public string _self { get; set; }

    [JsonIgnore]
    public string _etag { get; set; }

    [JsonIgnore]
    public string _attachments { get; set; }

    [JsonIgnore]
    public int _ts { get; set; }
}
