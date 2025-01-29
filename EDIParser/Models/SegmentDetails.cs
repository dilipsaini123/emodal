
    public class SegmentDetails
    {
        
        public List<StSegment> STSegments { get; set; } = new List<StSegment>();
        public B4Segment B4Segments { get; set; } 
        public List<N9Segment> N9Segments { get; set; } = new List<N9Segment>();
        public List<Q2Segment> Q2Segments { get; set; } = new List<Q2Segment>();
        public List<SgSegment> SGSegments { get; set; } = new List<SgSegment>();

        public R4Segment R4Segments { get; set; } 
        
        public List<SeSegment> SESegments { get; set; } = new List<SeSegment>();

    }
    
   

    public class StSegment
    {
        public string TransactionSetIdentifierCode { get; set; }
        public string TransactionSetControlNumber { get; set; }
    }
    public class SeSegment
    {
        public string NumberofIncludedSegments { get; set; }
        public string TransactionSetControlNumber { get; set; }
    }

    public class SgSegment{
        public string ShipmentStatusCode { get; set; }
        public string Date { get; set; }
        
    }

    public class Q2Segment{
        public string VesselCode { get; set; }
        public string Voyage { get; set; }
        public string VesselName { get; set; }
    }
    public class B4Segment
    {
        public string SpecialHandlingCode { get; set; }
        public string ShipmentStatusCode { get; set; }
        public string Date { get; set; }
        public string EquipmentStatusCode { get; set; }
        public string SizeType { get; set; }
        public string ContainerNumber { get; set; }
    }

    public class N9Segment
    {
        public string ReferenceIdentificationQualifier { get; set; }
        public string ReferenceIdentification { get; set; }
    }

    public class R4Segment
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        
    }

   

