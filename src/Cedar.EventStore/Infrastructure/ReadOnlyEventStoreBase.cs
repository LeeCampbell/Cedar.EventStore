namespace Cedar.EventStore.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cedar.EventStore.Streams;
    using Cedar.EventStore.Subscriptions;
    using EnsureThat;

    public abstract class ReadOnlyEventStoreBase : IReadOnlyEventStore
    {
        private bool _isDisposed;

        public Task<AllEventsPage> ReadAllForwards(
            long fromCheckpointInclusive,
            int maxCount,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(fromCheckpointInclusive, nameof(fromCheckpointInclusive)).IsGte(0);
            Ensure.That(maxCount, nameof(maxCount)).IsGte(1);

            CheckIfDisposed();

            return ReadAllForwardsInternal(fromCheckpointInclusive, maxCount, cancellationToken);
        }

        public Task<AllEventsPage> ReadAllBackwards(
            long fromCheckpointInclusive,
            int maxCount,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(fromCheckpointInclusive, nameof(fromCheckpointInclusive)).IsGte(-1);
            Ensure.That(maxCount, nameof(maxCount)).IsGte(1);

            CheckIfDisposed();

            return ReadAllBackwardsInternal(fromCheckpointInclusive, maxCount, cancellationToken);
        }

        public Task<StreamEventsPage> ReadStreamForwards(
            string streamId,
            int fromVersionInclusive,
            int maxCount,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(streamId, nameof(streamId)).IsNotNullOrWhiteSpace();
            Ensure.That(fromVersionInclusive, nameof(fromVersionInclusive)).IsGte(0);
            Ensure.That(maxCount, nameof(maxCount)).IsGte(1);

            CheckIfDisposed();

            return ReadStreamForwardsInternal(streamId, fromVersionInclusive, maxCount, cancellationToken);
        }

        public Task<StreamEventsPage> ReadStreamBackwards(
            string streamId,
            int fromVersionInclusive,
            int maxCount,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(streamId, nameof(streamId)).IsNotNullOrWhiteSpace();
            Ensure.That(fromVersionInclusive, nameof(fromVersionInclusive)).IsGte(-1);
            Ensure.That(maxCount, nameof(maxCount)).IsGte(1);

            CheckIfDisposed();

            return ReadStreamBackwardsInternal(streamId, fromVersionInclusive, maxCount, cancellationToken);
        }

        public Task<IStreamSubscription> SubscribeToStream(
            string streamId,
            int fromVersionExclusive,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped = null,
            string name = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(streamId, nameof(streamId)).IsNotNullOrWhiteSpace();
            Ensure.That(streamEventReceived, nameof(streamEventReceived)).IsNotNull();

            CheckIfDisposed();

            return SubscribeToStreamInternal(streamId,
                fromVersionExclusive,
                streamEventReceived,
                subscriptionDropped,
                name,
                cancellationToken);
        }

        public Task<IAllStreamSubscription> SubscribeToAll(
            long? fromCheckpointExclusive,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped = null,
            string name = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.That(streamEventReceived, nameof(streamEventReceived)).IsNotNull();

            CheckIfDisposed();

            return SubscribeToAllInternal(fromCheckpointExclusive,
                streamEventReceived,
                subscriptionDropped,
                name,
                cancellationToken);
        }

        public Task<long> ReadHeadCheckpoint(CancellationToken cancellationToken)
        {
            CheckIfDisposed();

            return ReadHeadCheckpointInternal(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        protected abstract Task<AllEventsPage> ReadAllForwardsInternal(
            long fromCheckpointExlusive,
            int maxCount,
            CancellationToken cancellationToken);

        protected abstract Task<AllEventsPage> ReadAllBackwardsInternal(
            long fromCheckpointExclusive,
            int maxCount,
            CancellationToken cancellationToken);

        protected abstract Task<StreamEventsPage> ReadStreamForwardsInternal(
            string streamId,
            int start,
            int count,
            CancellationToken cancellationToken);

        protected abstract Task<StreamEventsPage> ReadStreamBackwardsInternal(
            string streamId,
            int fromVersionInclusive,
            int count,
            CancellationToken cancellationToken);

        protected abstract Task<IStreamSubscription> SubscribeToStreamInternal(
            string streamId,
            int startVersion,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken);

        protected abstract Task<long> ReadHeadCheckpointInternal(CancellationToken cancellationToken);

        protected abstract Task<IAllStreamSubscription> SubscribeToAllInternal(
            long? fromCheckpoint,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
        {}

        protected void CheckIfDisposed()
        {
            if(_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        ~ReadOnlyEventStoreBase()
        {
            Dispose(false);
        }
    }
}