
    public class CNT
    {
        public string id { get; set; }
        public string ContainerId { get; set; }
        public string TradeType { get; set; }
        public string  Status { get; set; }
        public string  Holds { get; set; }
        public string  PregateTicket { get; set; }
        public string EModalPregate { get; set; }   
        public string GateStatus { get; set; }  
        public String Origin { get; set; }  
        public String Destination { get; set; } 
        public string CurrentLocation { get; set; } 
        public string line { get; set; }
        public string VesselName { get; set; }
        public string  VesselCode { get; set; }
        public string Voyage { get; set; }  
        public string SizeType { get; set; }    
        public int AdditionalFees { get; set; }
        public int DemurrageFees { get; set; }
        public DateOnly Date { get; set; }
        public SegmentDetails ContainerDetails { get; set; } = new SegmentDetails();
    }

