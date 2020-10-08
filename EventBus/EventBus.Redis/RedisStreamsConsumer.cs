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
            _ = streams ?? throw new ArgumentNullException(nameof(streams));

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            _readerTask = Task.Factory.StartNew(() => FetchItems(streams.ToArray()), _cts.Token,
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

        private void FetchItems(string[] streams)
        {
            ProcessPending(streams);

            _cts.Token.ThrowIfCancellationRequested();

            ProcessLive(streams);
        }

        private void ProcessPending(string[] streams)
        {
            //https://redis.io/commands/xreadgroup#usage-example
            //ID 0 - read pending items
            var pendingPositions = streams.Select(s => new StreamPosition(s, "0")).ToArray();

            bool hasPending;

            //process pending entries list items
            do
            {
                var pendingItems = _db.StreamReadGroup(pendingPositions, _groupName, _consumerName, _batchPerGroupSize);
                hasPending = pendingItems.SelectMany(s => s.Entries).Any();
                LoopOverStreams(pendingItems);

                _cts.Token.ThrowIfCancellationRequested();
            } while (hasPending);
        }

        private void ProcessLive(string[] streams)
        {
            var livePositions = streams.Select(s => new StreamPosition(s, ">")).ToArray();

            while (!_cts.Token.IsCancellationRequested)
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

                _cts.Token.ThrowIfCancellationRequested();
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
