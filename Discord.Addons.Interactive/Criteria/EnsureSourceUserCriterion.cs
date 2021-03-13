using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class EnsureSourceUserCriterion : ICriterion<IMessage>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, IMessage parameter)
        {
            bool ok = sourceContext.User.Id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}