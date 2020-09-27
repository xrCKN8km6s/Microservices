using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace EventBus.Redis
{
    public class RedisStreamsConsumer : IRedisStreamsConsumer
    {
        private readonly IDatabase _db;
        private readonly string _groupName;
        private readonly string _consumerName;
        private readonly int _batchPerGroupSize;

        private readonly CancellationTokenSource _cts;
        private Task _readerTask;

        private readonly TimeSpan _sleepDuration = TimeSpan.FromMilliseconds(1000);

        private Func<string, StreamEntry, Task> _handler;

        public RedisStreamsConsumer(IDatabase db, string consumerGroupName, string consumerName, int batchPerGroupSize)
        {
            _db = db;
            _groupName = consumerGroupName;
            _consumerName = consumerName;
            _batchPerGroupSize = batchPerGroupSize;
            _cts = new CancellationTokenSource();
        }

        public void Start(IReadOnlyCollection<string> streams, Func<string, StreamEntry, Task> handler)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            _readerTask = Task.Factory.StartNew(() => FetchItems(streams.ToArray(), _cts.Token), _cts.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach |
                TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
        }

        public void Stop()
        {
            _cts.Cancel();
            try
            {
                //gracefully waiting
                _readerTask.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                //cancellation token was cancelled, its fine
            }
        }

        private void FetchItems(string[] streams, CancellationToken token)
        {
            var pendingPositions = streams.Select(s => new StreamPosition(s, "0")).ToArray();
            var livePositions = streams.Select(s => new StreamPosition(s, ">")).ToArray();

            bool hasPending;

            // process PEL items
            do
            {
                var pendingItems = _db.StreamReadGroup(pendingPositions, _groupName, _consumerName, _batchPerGroupSize);

                hasPending = pendingItems.SelectMany(s => s.Entries).Any();

                LoopOverStreams(pendingItems);

            } while (hasPending);

            token.ThrowIfCancellationRequested();

            //actual live loop
            while (!token.IsCancellationRequested)
            {
                var items = _db.StreamReadGroup(livePositions, _groupName, _consumerName, _batchPerGroupSize);

                if (items.Length > 0)
                {
                    LoopOverStreams(items);
                }
                else
                {
                    //StackExchange.Redis doesn't support blocking API, sleeping for now
                    Thread.Sleep(_sleepDuration);
                }

                token.ThrowIfCancellationRequested();
            }
        }

        private void LoopOverStreams(RedisStream[] streams)
        {
            foreach (var stream in streams)
            {
                foreach (var entry in stream.Entries)
                {
                    try
                    {
                        _handler(stream.Key, entry).Wait();
                        _db.StreamAcknowledge(stream.Key, _groupName, entry.Id);
                    }
                    catch (AggregateException)
                    {
                        //log
                    }

                    _cts.Token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
