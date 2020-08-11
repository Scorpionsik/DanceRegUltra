using DanceRegUltra.Models;

namespace DanceRegUltra.Interfaces
{
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
