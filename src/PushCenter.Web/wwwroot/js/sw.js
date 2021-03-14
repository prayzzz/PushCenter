"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.default = null;
function onPush(event) {
    const data = event.data;
    if (data == null) {
        return;
    }
    const pushMessage = data.json();
    event.waitUntil(self.registration.showNotification(pushMessage.title, {
        body: pushMessage.message,
        icon: pushMessage.iconUrl,
        data: pushMessage
    }));
}
function onNotificationClick(event) {
    console.log(event);
    event.notification.close();
    const pushMessage = event.notification.data;
    if (pushMessage.link) {
        event.waitUntil(clients.openWindow(pushMessage.link));
    }
}
self.addEventListener('push', onPush);
self.addEventListener('notificationclick', onNotificationClick);
//# sourceMappingURL=sw.js.map