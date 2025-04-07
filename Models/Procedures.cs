public class Proceduret
{
    public int ProcedureID { get; set; }
    public int ProcessID { get; set; }
    public string ProcedureName { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime DueDate { get; set; }

    public Processst Process { get; set; }
}