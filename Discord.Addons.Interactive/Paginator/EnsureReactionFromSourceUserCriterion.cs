using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    internal class EnsureReactionFromSourceUserCriterion : ICriterion<SocketReaction>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketReaction parameter)
        {
            bool ok = parameter.UserId == sourceContext.User.Id;
            return Task.FromResult(ok);
        }
    }
}