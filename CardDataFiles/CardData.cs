using DatabaseApp_Paul_Zaldea.CardDataFiles;
using System;


namespace DatabaseApp_Paul_Zaldea
{
    public class CardData : iCardData
    {
        public int RefNr { get; set; }
        public int CRC { get; set; }
        public string Certificate { get; set; }
        public long CardSerNr { get; set; }
        public int Rev { get; set; }
        public string CasinoIssued { get; set; }
        public string SiteIssued { get; set; }
        public int IpAddrIssued { get; set; }
        public DateTime TimeIssued { get; set; }
        public short? TypeLevel { get; set; }
        public short? Currency { get; set; }
        public short? Language { get; set; }
        public int? Flags { get; set; }
        public long PlayerId { get; set; }
        public long DateOfBirth { get; set; }
        public int ValidDays { get; set; }
        public long? AutoloadLimit { get; set; }
        public Int32? AccessRights { get; set; }
        public int? LockReason { get; set; }
        public int? Pin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NickName { get; set; }
        public Guid SrmId { get; set; }
        public short? CardLocked { get; set; }
        public long TransNr { get; set; }
        public string CasinoIdUsed { get; set; }
        public string SiteUsed { get; set; }
        public int? IpAddrUsed { get; set; }
        public DateTime TimeUsed { get; set; }
        public short? EndReason { get; set; }
        public DateTime? LastUsedByPlayer { get; set; }
        public DateTime? LastCashTransaction { get; set; }
        public string LastVisitCasino { get; set; }
        public DateTime? LastVisitUtcTime { get; set; }
        public long BalCCC { get; set; }
        public long BalNCC { get; set; }
        public long BalPTS { get; set; }
        public long Deposit { get; set; }
        public long InBalCCC { get; set; }
        public long InBalNCC { get; set; }
        public long InBalPTS { get; set; }
        public long InDeposit { get; set; }
        public int DaysSinceLastUse { get; set; }
        public long DailyKioskCashout { get; set; }
        public long DailyExchange { get; set; }
        public long DailyTotalIn { get; set; }
        public long DailyTotalWin { get; set; }
        public long DailyGames { get; set; }
        public long DailyBasePoints { get; set; }
        public long DailyBonusPoints { get; set; }
        public long LifeToDateTotalIn { get; set; }
        public long LifeToDateTotalWin { get; set; }
        public long LifeToDateGames { get; set; }
        public long LifeToDateBasePoints { get; set; }
        public long LifeToDateBonusPoints { get; set; }
        public string SessionLockedCasinoId { get; set; }
        public string SessionLockedSiteId { get; set; }
        public int? SessionLockedIpAddr { get; set; }
    }
}
