using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class EmptyCriterion<T> : ICriterion<T>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter)
        {
            return Task.FromResult(true);
        }
    }
}