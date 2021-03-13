using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class InteractiveService : IDisposable
    {
        public BaseSocketClient Discord { get; }

        private readonly Dictionary<ulong, IReactionCallback> _callbacks;
        private TimeSpan _defaultTimeout;

        // helpers to allow DI containers to resolve without a custom factory
        public InteractiveService(DiscordSocketClient discord, InteractiveServiceConfig config = null)
            : this((BaseSocketClient)discord, config) { }

        public InteractiveService(DiscordShardedClient discord, InteractiveServiceConfig config = null)
            : this((BaseSocketClient)discord, config) { }

        public InteractiveService(BaseSocketClient discord, InteractiveServiceConfig config = null)
        {
            Discord = discord;
            Discord.ReactionAdded += HandleReactionAsync;

            config = config ?? new InteractiveServiceConfig();
            _defaultTimeout = config.DefaultTimeout;

            _callbacks = new Dictionary<ulong, IReactionCallback>();
        }

        public Task<SocketMessage> NextMessageAsync(SocketCommandContext context,
            bool fromSourceUser = true,
            bool inSourceChannel = true,
            TimeSpan? timeout = null,
            CancellationToken token = default(CancellationToken))
        {
            Criteria<SocketMessage> criterion = new Criteria<SocketMessage>();
            if (fromSourceUser)
            {
                criterion.AddCriterion(new EnsureSourceUserCriterion());
            }

            if (inSourceChannel)
            {
                criterion.AddCriterion(new EnsureSourceChannelCriterion());
            }

            return NextMessageAsync(context, criterion, timeout, token);
        }

        public async Task<SocketMessage> NextMessageAsync(SocketCommandContext context,
            ICriterion<SocketMessage> criterion,
            TimeSpan? timeout = null,
            CancellationToken token = default(CancellationToken))
        {
            timeout = timeout ?? _defaultTimeout;

            TaskCompletionSource<SocketMessage> eventTrigger = new TaskCompletionSource<SocketMessage>();
            TaskCompletionSource<bool> cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true));

            async Task Handler(SocketMessage message)
            {
                bool result = await criterion.JudgeAsync(context, message).ConfigureAwait(false);
                if (result)
                {
                    eventTrigger.SetResult(message);
                }
            }

            context.Client.MessageReceived += Handler;

            Task<SocketMessage> trigger = eventTrigger.Task;
            Task<bool> cancel = cancelTrigger.Task;
            Task delay = Task.Delay(timeout.Value);
            Task task = await Task.WhenAny(trigger, delay, cancel).ConfigureAwait(false);

            context.Client.MessageReceived -= Handler;

            if (task == trigger)
            {
                return await trigger.ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        public async Task<IUserMessage> ReplyAndDeleteAsync(SocketCommandContext context,
            string content, bool isTTS = false,
            Embed embed = null,
            TimeSpan? timeout = null,
            RequestOptions options = null)
        {
            timeout = timeout ?? _defaultTimeout;
            Rest.RestUserMessage message = await context.Channel.SendMessageAsync(content, isTTS, embed, options).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value)
                .ContinueWith(_ => message.DeleteAsync().ConfigureAwait(false))
                .ConfigureAwait(false);
            return message;
        }

        public async Task<IUserMessage> SendPaginatedMessageAsync(SocketCommandContext context,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            PaginatedMessageCallback callback = new PaginatedMessageCallback(this, context, pager, criterion);
            await callback.DisplayAsync().ConfigureAwait(false);
            return callback.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback)
        {
            _callbacks[message.Id] = callback;
        }

        public void RemoveReactionCallback(IMessage message)
        {
            RemoveReactionCallback(message.Id);
        }

        public void RemoveReactionCallback(ulong id)
        {
            _callbacks.Remove(id);
        }

        public void ClearReactionCallbacks()
        {
            _callbacks.Clear();
        }

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message,
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (reaction.UserId == Discord.CurrentUser.Id)
            {
                return;
            }

            if (!(_callbacks.TryGetValue(message.Id, out IReactionCallback callback)))
            {
                return;
            }

            if (!(await callback.Criterion.JudgeAsync(callback.Context, reaction).ConfigureAwait(false)))
            {
                return;
            }

            switch (callback.RunMode)
            {
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                        {
                            RemoveReactionCallback(message.Id);
                        }
                    });
                    break;

                default:
                    if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                    {
                        RemoveReactionCallback(message.Id);
                    }

                    break;
            }
        }

        public void Dispose()
        {
            Discord.ReactionAdded -= HandleReactionAsync;
        }
    }
}