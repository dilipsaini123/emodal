
    public class GSSegment
    {
        public string FunctionalIdentifierCode { get; set; }
        public string FunctionalSenderCode { get; set; }
        public string FunctionalReceiverCode { get; set; }
        public DateTime EventTime { get; set; }
        public string GroupControlNumber { get; set; }
        public string ResponsibleAgencyCode { get; set; }
        public string VersionReleaseIndustryIdentifierCode { get; set; }
    }
    public class GESegment
    {
        public string NumberOfTransactionSetsIncluded { get; set; }
        public string GroupControlNumber { get; set; }
    }

