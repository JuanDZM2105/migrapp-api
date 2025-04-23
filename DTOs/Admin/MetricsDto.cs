namespace migrapp_api.DTOs.Admin
{
    public class MetricsDto
    {
        public int? TotalUsers { get; set; }
        public int? TotalAdmins { get; set; }
        public int? TotalLawyers { get; set; }
        public int? TotalAuditors { get; set; }
        public int? TotalReaders { get; set; }
        public int? TotalActiveUsers { get; set; }
        public int? TotalEliminatedAccountStatus { get; set; }
        public int? TotalActiveAccountStatus { get; set; }
        public int? TotalBlockedAccountStatus { get; set; }
        public int? TotalOpenLegalProcesses { get; set; }
        public int? TotalCompletedLegalProcesses { get; set; }


        public int? AssignedCasesCount { get; set; }
        public int? NewCases { get; set; }
        public int? InProcessCases { get; set; }
        public int? EndedCases { get; set; }






    }
}
