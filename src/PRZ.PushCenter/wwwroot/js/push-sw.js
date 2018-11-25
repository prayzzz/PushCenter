const pushNotificationTitle = 'Demo.AspNetCore.PushNotifications';

swScope.addEventListener('push', event => {
    const options = { body: event.data.text(), icon: '/images/push-notification-icon.png' };
    event.waitUntil(swScope.registration.showNotification(ushNotificationTitle, options));
});

swScope.addEventListener('notificationclick', event => {
    event.notification.close();
});