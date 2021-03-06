﻿namespace Cedar.EventStore
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cedar.EventStore.Streams;

    public interface IEventStore : IReadOnlyEventStore
    {
        Task AppendToStream(
            string streamId,
            int expectedVersion,
            NewStreamEvent[] events,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteStream(
            string streamId,
            int expectedVersion = ExpectedVersion.Any,
            CancellationToken cancellationToken = default(CancellationToken));      
    }
}