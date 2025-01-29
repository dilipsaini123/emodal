

    public class IsaSegment
    {
        public string AuthorizationInformationQualifier { get; set; }
        public string SecurityInformationQualifier { get; set; }
        public string InterchangeSenderIdQualifier { get; set; }
        public string InterchangeSenderId { get; set; }
        public string InterchangeReceiverIdQualifier { get; set; }
        public string InterchangeReceiverId { get; set; }
        public string InterchangeControlStandardIdentifier { get; set; }
        public string InterchangeControlVersionNumber { get; set; }
        public string InterchangeControlNumber { get; set; }
        public string AcknowledgementRequested { get; set; }
        public string TestIndicator { get; set; }
        
        public DateTime EventTime { get; set; }
    }
    

