using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class EnsureSourceChannelCriterion : ICriterion<IMessage>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, IMessage parameter)
        {
            bool ok = sourceContext.Channel.Id == parameter.Channel.Id;
            return Task.FromResult(ok);
        }
    }
}