namespace DanceRegUltra.Interfaces
{
    public enum UpdateStatus
    {
        Default,
        Add,
        Edit,
        Delete,
        Move
    }

    public delegate void UpdateMember(int eventId, int memberId, string dataColumn, object currentData = null, UpdateStatus status = UpdateStatus.Default, object replaceData = null);

    public interface IMember
    {
        event UpdateMember Event_UpdateMember;

        /// <summary>
        /// Id танцора или группы в бд
        /// </summary>
        int EventId { get; }

        /// <summary>
        /// Id танцора или группы в бд
        /// </summary>
        int MemberId { get; }

        /// <summary>
        /// Номер участника в событии
        /// </summary>
        int MemberNum { get; set; }
    }
}
