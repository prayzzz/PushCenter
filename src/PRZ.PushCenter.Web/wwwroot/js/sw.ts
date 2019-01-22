declare var self: ServiceWorkerGlobalScope;

function onPush(this: ServiceWorkerGlobalScope, event: PushEvent) {
    const pushMessage: PushMessage = event.data.json();

    event.waitUntil(
        self.registration.showNotification(pushMessage.title, {
            body: pushMessage.message,
            icon: pushMessage.iconUrl
        })
    )
}

self.addEventListener('push', onPush);

interface PushMessage {
    title: string;
    message: string;
    iconUrl: string;
}