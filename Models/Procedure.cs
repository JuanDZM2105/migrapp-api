using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace migrapp_api.Models
{
  public class Procedure
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime? DueDate { get; set; }

    public int LegalProcessId { get; set; }

    [JsonIgnore]
    public LegalProcess LegalProcess { get; set; }

        public ICollection<ProcedureDocument> ProcedureDocuments { get; set; } = new List<ProcedureDocument>();
  }
}

