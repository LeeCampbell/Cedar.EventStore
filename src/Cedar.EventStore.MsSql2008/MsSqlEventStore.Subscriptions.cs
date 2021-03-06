﻿namespace Cedar.EventStore
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cedar.EventStore.Subscriptions;

    public sealed partial class MsSqlEventStore
    {
        protected override async Task<IStreamSubscription> SubscribeToStreamInternal(
            string streamId,
            int startVersion,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken)
        {
            var subscription = new StreamSubscription(
                streamId,
                startVersion,
                this,
                GetStoreObservable,
                streamEventReceived,
                subscriptionDropped);

            await subscription.Start(cancellationToken);

            return subscription;
        }

        protected override async Task<IAllStreamSubscription> SubscribeToAllInternal(
            long? fromCheckpoint,
            StreamEventReceived streamEventReceived,
            SubscriptionDropped subscriptionDropped,
            string name,
            CancellationToken cancellationToken)
        {
            var subscription = new AllStreamSubscription(
                fromCheckpoint,
                this,
                GetStoreObservable,
                streamEventReceived,
                subscriptionDropped, 
                name);

            await subscription.Start(cancellationToken);

            return subscription;
        }
    }
}