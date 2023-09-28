using System;


namespace DatabaseApp_Paul_Zaldea.CardDataFiles
{
    public interface iCardData
    {
        int RefNr { get; set; }
        int CRC { get; set; }
        string Certificate { get; set; }
        long CardSerNr { get; set; }
        int Rev { get; set; }
        string CasinoIssued { get; set; }
        string SiteIssued { get; set; }
        int IpAddrIssued { get; set; }
        DateTime TimeIssued { get; set; }
        short? TypeLevel { get; set; }
        short? Currency { get; set; }
        short? Language { get; set; }
        int? Flags { get; set; }
        long PlayerId { get; set; }
        long DateOfBirth { get; set; }
        int ValidDays { get; set; }
        long? AutoloadLimit { get; set; }
        Int32? AccessRights { get; set; }
        int? LockReason { get; set; }
        int? Pin { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Gender { get; set; }
        string NickName { get; set; }
        Guid SrmId { get; set; }
        short? CardLocked { get; set; }
        long TransNr { get; set; }
        string CasinoIdUsed { get; set; }
        string SiteUsed { get; set; }
        int? IpAddrUsed { get; set; }
        DateTime TimeUsed { get; set; }
        short? EndReason { get; set; }
        DateTime? LastUsedByPlayer { get; set; }
        DateTime? LastCashTransaction { get; set; }
        string LastVisitCasino { get; set; }
        DateTime? LastVisitUtcTime { get; set; }
        long BalCCC { get; set; }
        long BalNCC { get; set; }
        long BalPTS { get; set; }
        long Deposit { get; set; }
        long InBalCCC { get; set; }
        long InBalNCC { get; set; }
        long InBalPTS { get; set; }
        long InDeposit { get; set; }
        int DaysSinceLastUse { get; set; }
        long DailyKioskCashout { get; set; }
        long DailyExchange { get; set; }
        long DailyTotalIn { get; set; }
        long DailyTotalWin { get; set; }
        long DailyGames { get; set; }
        long DailyBasePoints { get; set; }
        long DailyBonusPoints { get; set; }
        long LifeToDateTotalIn { get; set; }
        long LifeToDateTotalWin { get; set; }
        long LifeToDateGames { get; set; }
        long LifeToDateBasePoints { get; set; }
        long LifeToDateBonusPoints { get; set; }
        string SessionLockedCasinoId { get; set; }
        string SessionLockedSiteId { get; set; }
        int? SessionLockedIpAddr { get; set; }
    }
}
