declare var self: ServiceWorkerGlobalScope;
declare var clients: Clients;

function onPush(this: ServiceWorkerGlobalScope, event: PushEvent) {
    const pushMessage: PushMessage = event.data.json();

    event.waitUntil(
        self.registration.showNotification(pushMessage.title, {
            body: pushMessage.message,
            icon: pushMessage.iconUrl,
            data: pushMessage
        })
    )
}

function onNotificationClick(this: ServiceWorkerGlobalScope, event: NotificationEvent) {
    console.log(event);
    
    event.notification.close();

    const pushMessage = <PushMessage>event.notification.data;
    if (pushMessage.link) {
        event.waitUntil(clients.openWindow(pushMessage.link));
    }
}

self.addEventListener('push', onPush);
self.addEventListener('notificationclick', onNotificationClick);

interface PushMessage {
    title: string;
    message: string;
    iconUrl: string;
    link: string;
}