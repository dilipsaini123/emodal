using log4net;
using log4net.Config;
using Newtonsoft.Json;
public class EdiProcessorService
{
    private readonly ILog _log;
    private readonly string _sourceFolderPath;
    private readonly string _processedFolderPath;
    public List<string> N901CheckListDamrange = new List<string>{
        "4I","4I1","4I2","4I3","4I4","4I5","4I6","4I7","4I8",
        "4I9"

    };
     public List<string> N901CheckListAdditional = new List<string>{
        "22","4N","4LE","BBC","DVF","GEN","MSL","GC","IGF"

    };
    public EdiProcessorService(string sourceFolderPath, string processedFolderPath, ILog log)
    {
        _sourceFolderPath = sourceFolderPath;
        _processedFolderPath = processedFolderPath;
        _log = log;
    }

    public async Task ProcessEdiFilesAsync()
    {
        // Ensure the "Processed" folder exists
        if (!Directory.Exists(_processedFolderPath))
        {
            Directory.CreateDirectory(_processedFolderPath);
        }

        string[] filePaths = Directory.GetFiles(_sourceFolderPath, "*.txt");
        foreach (var filePath in filePaths)
        {
            _log.Info($"Processing EDI file: {filePath}");

            // Initialize necessary containers
            var ediJson = new Dictionary<string, object>();
            var containers = new List<CNT>();
            CNT currentContainer = null;

            // Read and parse the EDI file
            var ediLines = ReadEdiFile(filePath);

            _log.Info("Successfully read the EDI file. Processing segments...");
            ProcessEdiSegments(ediLines, ediJson, containers, ref currentContainer);

            _log.Info("JSON output generated successfully.");
            ediJson["Containers"] = containers;
            string jsonOutput = JsonConvert.SerializeObject(containers, Formatting.Indented);
            Console.WriteLine(jsonOutput);

            // Upload to Cosmos DB
            await CosmosDbHelper.UploadToCosmosDbAsync(containers);

            // Optionally move the processed file
            // string destinationPath = Path.Combine(_processedFolderPath, Path.GetFileName(filePath));
            // File.Move(filePath, destinationPath);
            // _log.Info($"Successfully moved processed file: {filePath} to {destinationPath}");
        }
    }

    private List<string> ReadEdiFile(string filePath)
    {
        var ediLines = new List<string>();

        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                ediLines.Add(line);
            }
        }

        return ediLines;
    }

    private void ProcessEdiSegments(
        List<string> ediLines,
        Dictionary<string, object> ediJson,
        List<CNT> containers,
        ref CNT currentContainer)
    {
        foreach (string segment in ediLines)
        {
            var elements = segment.Split('*').Select(e => e.Trim()).ToArray();
            var segmentId = elements[0];

            switch (segmentId)
            {
                case "ISA":
                    ProcessIsaSegment(elements, ediJson);
                    break;
                case "GS":
                    ProcessGsSegment(elements, ediJson);
                    break;
                case "GE":
                    ProcessGeSegment(elements, ediJson);
                    break;
                case "ST":
                    currentContainer = new CNT { ContainerDetails = new SegmentDetails() };
                    ProcessStSegment(elements, ref currentContainer);
                    break;
                case "B4":
                    ProcessB4Segment(elements, ref currentContainer);
                    break;
                case "N9":
                    ProcessN9Segment(elements, ref currentContainer);
                    break;
                case "R4":
                    ProcessR4Segment(elements, ref currentContainer);
                    break;
                case "SE":
                    ProcessSeSegment(elements, ref currentContainer, containers);
                    break;
                case "Q2":
                    ProcessQ2Segment(elements, ref currentContainer);
                    break;
                case "SG":
                    ProcessSgSegment(elements, ref currentContainer);
                    break;
                case "IEA":
                    ProcessIeaSegment(elements);
                    break;
            }
        }
    }

    private void ProcessIsaSegment(string[] elements, Dictionary<string, object> ediJson)
    {
        if (elements.Length >= 16)
        {
            var isaSegment = new IsaSegment
            {
                AuthorizationInformationQualifier = elements[1],
                SecurityInformationQualifier = elements[2],
                InterchangeSenderIdQualifier = elements[4],
                InterchangeSenderId = elements[5],
                InterchangeReceiverIdQualifier = elements[6],
                InterchangeReceiverId = elements[7],
                EventTime = DateTime.TryParse($"{elements[9]}T{elements[10]}", out var eventTime) ? eventTime : DateTime.MinValue,
                InterchangeControlStandardIdentifier = elements[11],
                InterchangeControlVersionNumber = elements[12],
                InterchangeControlNumber = elements[13],
                AcknowledgementRequested = elements[14],
                TestIndicator = elements[15]
            };

            ediJson["ISA"] = isaSegment;
        }
    }

    private void ProcessGsSegment(string[] elements, Dictionary<string, object> ediJson)
    {
        var gsSegment = new GSSegment
        {
            FunctionalIdentifierCode = elements[1],
            FunctionalSenderCode = elements[2],
            FunctionalReceiverCode = elements[3],
            EventTime = DateTime.TryParse($"{elements[4]}T{elements[5]}", out var gsEventTime) ? gsEventTime : DateTime.MinValue,
            GroupControlNumber = elements[6],
            ResponsibleAgencyCode = elements[7],
            VersionReleaseIndustryIdentifierCode = elements[8]
        };

        ediJson["GS"] = gsSegment;
    }

    private void ProcessGeSegment(string[] elements, Dictionary<string, object> ediJson)
    {
        var geSegment = new GESegment
        {
            NumberOfTransactionSetsIncluded = elements[1],
            GroupControlNumber = elements[2]
        };
        ediJson["GE"] = geSegment;
    }

    private void ProcessStSegment(string[] elements, ref CNT currentContainer)
    {
        var stSegment = new StSegment
        {
            TransactionSetIdentifierCode = elements[1],
            TransactionSetControlNumber = elements[2]
        };
        currentContainer.ContainerDetails.STSegments.Add(stSegment);
    }

    private void ProcessB4Segment(string[] elements, ref CNT currentContainer)
    {
        if (currentContainer != null && elements.Length >= 8)
        {
            currentContainer.TradeType = elements[1];
            currentContainer.Status = elements[3];
            currentContainer.ContainerId = $"{elements[7]}{elements[8]}";
            string dateString = elements[4];

            DateOnly dateOnly = DateOnly.ParseExact(dateString, "yyyyMMdd");
            currentContainer.Date = dateOnly;
            currentContainer.SizeType = elements[10];

            var b4Segment = new B4Segment
            {
                SpecialHandlingCode = elements[1],
                ShipmentStatusCode = elements[3],
                EquipmentStatusCode = elements[9],
                SizeType = elements[10],
                Date = elements[4],
                ContainerNumber = $"{elements[7]}{elements[8]}"
            };
            currentContainer.ContainerDetails.B4Segments = b4Segment;
        }
    }

    private void ProcessN9Segment(string[] elements, ref CNT currentContainer)
    {
        if (currentContainer != null && elements.Length >= 3)
        {
            if (elements[1] == "SCA")
            {
                currentContainer.line = elements[2];
            }
            if(N901CheckListAdditional.Contains(elements[1]))
            {
                currentContainer.AdditionalFees+=Convert.ToInt32(elements[2]);
            }
            else if(N901CheckListDamrange.Contains(elements[1])){
                currentContainer.DemurrageFees+=Convert.ToInt32(elements[2]);
            }
            var n9Segment = new N9Segment
            {
                ReferenceIdentificationQualifier = elements[1],
                ReferenceIdentification = elements[2]
            };
            currentContainer.ContainerDetails.N9Segments.Add(n9Segment);
        }
    }

    private void ProcessR4Segment(string[] elements, ref CNT currentContainer)
    {
        if (currentContainer != null)
        {
            if (currentContainer.ContainerDetails.R4Segments == null)
                currentContainer.ContainerDetails.R4Segments = new R4Segment();

            if (elements[1] == "L")
            {
                currentContainer.Origin = elements[4];
                currentContainer.ContainerDetails.R4Segments.Origin = elements[4];
            }
            if (elements[1] == "D")
            {
                currentContainer.Destination = elements[4];
                currentContainer.ContainerDetails.R4Segments.Destination = elements[4];
            }
        }
    }

    private void ProcessSeSegment(string[] elements, ref CNT currentContainer, List<CNT> containers)
    {
        if (currentContainer != null)
        {
            var seSegment = new SeSegment
            {
                NumberofIncludedSegments = elements[1],
                TransactionSetControlNumber = elements[2]
            };
            currentContainer.ContainerDetails.SESegments.Add(seSegment);
            containers.Add(currentContainer);
            currentContainer = null;
        }
    }

    private void ProcessQ2Segment(string[] elements, ref CNT currentContainer)
    {
        if (currentContainer != null)
        {
            currentContainer.VesselName = elements[13];
            currentContainer.VesselCode = elements[1];
            currentContainer.Voyage = elements[9];

            var q2Segment = new Q2Segment
            {
                VesselCode = elements[1],
                Voyage = elements[9],
                VesselName = elements[13]
            };
            currentContainer.ContainerDetails.Q2Segments.Add(q2Segment);
        }
    }

    private void ProcessSgSegment(string[] elements, ref CNT currentContainer)
    {
        if (currentContainer != null)
        {
            var sgSegment = new SgSegment
            {
                ShipmentStatusCode = elements[1],
                Date = elements[2]
            };
            currentContainer.ContainerDetails.SGSegments.Add(sgSegment);
        }
    }

    private void ProcessIeaSegment(string[] elements)
    {
        var iea = new IEA
        {
            NumberOfFunctionalGroups = elements[1],
            ControlNumber = elements[2]
        };
    }
}
